using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;

namespace Application.Implementations
{
    public class PedidoUseCase : Interfaces.UseCases.IPedidoUseCase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPedidoStatusUseCase _pedidoStatusUseCase;
        private readonly IPedidoPagamentoUseCase _pedidoPagamentoUseCase;



        public PedidoUseCase(IPedidoRepository pedidoRepository,IClienteRepository clienteRepository, IProdutoRepository produtoRepository, IPedidoStatusUseCase pedidoStatusUseCase, IPedidoPagamentoUseCase pedidoPagamentoUseCase)
        {
            this._pedidoRepository = pedidoRepository;
            this._clienteRepository = clienteRepository;
            this._produtoRepository = produtoRepository;
            this._pedidoStatusUseCase = pedidoStatusUseCase;
            this._pedidoPagamentoUseCase = pedidoPagamentoUseCase;
        }

        public DTOs.Output.Pedido Get(int id)
        {
            var order = this._pedidoRepository.Get(id);
            if (order == null)
                throw new CustomExceptions.NotFoundException("Pedido não encontrado");

            //Busca os dados no service de Status
            var orderStatus = this._pedidoStatusUseCase.Get(id);
            order.PedidoStatus = orderStatus.Status.ToString();
            //Busca dados no service de Pagamento
            var orderPayment = this._pedidoPagamentoUseCase.Get(id);
            order.Pagamento = new DTOs.Output.Pagamento
            {
                TipoPagamento = orderPayment.TipoPagamento,
                StatusPagamento = orderPayment.PagamentoStatus
            };

            return order;
        }

        public IEnumerable<DTOs.Output.Pedido> List()
        {
            var orders = this._pedidoRepository.List().ToList();
           
            //Busca os dados no service de Status
            var ordersStatus = this._pedidoStatusUseCase.List(orders.Select(s => s.Id));
            orders.ForEach(order =>
            {
                var orderStatus = ordersStatus.FirstOrDefault(f => f.PedidoId == order.Id);
                if (orderStatus != null)
                    order.PedidoStatus = orderStatus.Status.ToString();
            });
           
            //Busca dados no service de Pagamento
            var ordersPayment = this._pedidoPagamentoUseCase.List(orders.Select(s => s.Id));
            orders.ForEach(order =>
            {
                var orderPayment = ordersPayment.FirstOrDefault(f => f.PedidoId == order.Id);
                if (orderPayment != null)
                {
                    order.Pagamento = new DTOs.Output.Pagamento
                    {
                        TipoPagamento = orderPayment.TipoPagamento,
                        StatusPagamento = orderPayment.PagamentoStatus
                    };
                }
            });

            return orders;
        }

        public IEnumerable<DTOs.Output.Pedido> ListByStatus(PedidoStatus status)
        {
            //Busca os dados no service de Status
            var ordersStatus = this._pedidoStatusUseCase.ListByStatus(status);
            if(ordersStatus.Count() == 0)
                return new List<DTOs.Output.Pedido>();  

            var orders = this._pedidoRepository.ListByIds(ordersStatus.Select(s => s.PedidoId)).ToList();
            orders.ForEach(order =>
            {
                var orderStatus = ordersStatus.FirstOrDefault(f => f.PedidoId == order.Id);
                if (orderStatus != null)
                    order.PedidoStatus = orderStatus.Status.ToString();
            });

            //Busca dados no service de Pagamento
            var ordersPayment = this._pedidoPagamentoUseCase.List(orders.Select(s => s.Id));
            orders.ForEach(order =>
            {
                var orderPayment = ordersPayment.FirstOrDefault(f => f.PedidoId == order.Id);
                if (orderPayment != null)
                {
                    order.Pagamento = new DTOs.Output.Pagamento
                    {
                        TipoPagamento = orderPayment.TipoPagamento,
                        StatusPagamento = orderPayment.PagamentoStatus
                    };
                }
            });

            return orders;
        }

        public Application.DTOs.Output.PedidoPagamento GetPaymentStatus(int pedidoId)
        {
            //Busca dados do pagamento no service de pagamento
            var item = this._pedidoPagamentoUseCase.Get(pedidoId);
            return new DTOs.Output.PedidoPagamento
            {
                PedidoId = item.PedidoId,
                StatusPagamento = item.PagamentoStatus
            };
        }

        public DTOs.Output.PedidoIdentificador Order(DTOs.Imput.Pedido pedido)
        {
            if (pedido.ClienteId.HasValue && pedido.ClienteId.Value > 0)
            {
                var customerExists = _clienteRepository.Get(pedido.ClienteId.Value);
                if (customerExists == null)
                    throw new CustomExceptions.BadRequestException($"Cliente inválido");
            }

            var selectProducts = _produtoRepository.ListByIds(pedido.Itens.Select(s => s.ProdutoId.Value).ToList());
            var prodNotFound = pedido.Itens.Where(s => !selectProducts.Any(a => a.Id == s.ProdutoId)).ToList();
            if (prodNotFound.Count > 0)
            {
                throw new CustomExceptions.BadRequestException($"Produtos inválidos - Ids:[{string.Join(",", prodNotFound.Select(s => s.ProdutoId))}]");
            }

          
            //Busca os produtos do pedido para poder pegar os valores unitarios
            var products = _produtoRepository.List().Where(w => pedido.Itens.Select(s => s.ProdutoId).Any(a => a == w.Id)).ToList();
            //Cria a lista de itens para o request
            var itemsRequest = pedido.Itens.Select(s => 
                new Domain.Entities.PedidoItem(s.ProdutoId.Value, s.Quantidade.Value, products.FirstOrDefault(f => f.Id == s.ProdutoId).Preco));
            //Soma o total do pedido
            decimal totalValue = itemsRequest.Select(s => s.PrecoUnitario * s.Quantidade).Sum();


            //Cria o objeto de request
            var pedidoEntity = new Domain.Entities.Pedido(
                pedido.ClienteId,              
                Domain.Enums.PedidoStatus.Recebido,
                totalValue,
                pedido.ClienteObservacao,
                itemsRequest,
                new Domain.Entities.Pagamento(
                    (Domain.Enums.TipoPagamento)pedido.Pagamento.TipoPagamento,
                    pedido.Pagamento.Nome,
                    pedido.Pagamento.TokenCartao,
                    totalValue
                    ));

            //Salva no repositorio
            var identifier = _pedidoRepository.Order(pedidoEntity);

            //Atualiza dados no service de Status
            this._pedidoStatusUseCase.Save(new DTOs.External.StatusService.PedidoStatusRequest
            {
                PedidoId = identifier,
                Status = (PedidoStatus)pedidoEntity.PedidoStatus
            });

            //Atualiza dados no service de pagamento
            var paymentResult = this._pedidoPagamentoUseCase.Save(new DTOs.External.PagamentoService.PedidoPagamentoRequest
            {
                PedidoId = identifier,
                Pagamento = new DTOs.External.PagamentoService.PagamentoRequest
                {
                    Nome = pedidoEntity.Pagamento.Nome,
                    TipoPagamento = (TipoPagamento)pedidoEntity.Pagamento.TipoPagamentoId,
                    TokenCartao = pedidoEntity.Pagamento.TokenCartao,
                    Valor = pedidoEntity.Valor
                }
            });

            return new DTOs.Output.PedidoIdentificador { Id = identifier }; 
        }

        public bool UpdateOrderStatus(int pedidoId, PedidoStatus status)
        {
            //Atualiza os dados no service de Status
            return this._pedidoStatusUseCase.Update(pedidoId, status);
        }
    }
}
