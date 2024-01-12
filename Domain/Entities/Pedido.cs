using Domain.Enums;

namespace Domain.Entities
{
    public class Pedido
    {
        public Pedido(int? clienteId, PedidoStatus pedidoStatusId, decimal valor, string clienteObservacao, IEnumerable<PedidoItem> itens, Pagamento dadosPagamento)
        {
            ClienteId = clienteId;
            Anonimo = !clienteId.HasValue;
            AnonimoIdentificador = (!ClienteId.HasValue || ClienteId.Value == 0) ? Guid.NewGuid().ToString() : ""; ;
            PedidoStatus = pedidoStatusId;
            Valor = valor;
            ClienteObservacao = clienteObservacao;
            Itens = itens;
            Pagamento = dadosPagamento;
            Data = DateTime.Now;

            this.Validate();
        }

        void Validate()
        {
            if (this.Valor == 0)
                throw new CustomExceptions.BadRequestException("Valor não pode zer zero");

            if(Itens.Count() == 0)
                throw new CustomExceptions.BadRequestException("Nenhum item selecionado");

            if (this.Pagamento == null)
                throw new CustomExceptions.BadRequestException("Dados de pagamento inválidos");

            if(this.Itens.Any(a => a.Quantidade == 0))
                throw new CustomExceptions.BadRequestException("Quantidade de itens inválida");
        }

        public int? Id { get; set; }
        public DateTime Data { get; set; }
        public int? ClienteId { get; set; }
        public bool Anonimo { get; set; }
        public string AnonimoIdentificador { get; set; }
        public Enums.PedidoStatus PedidoStatus { get; set; }
        public decimal Valor { get; set; }
        public string ClienteObservacao { get; set; }
        public IEnumerable<PedidoItem> Itens { get; set; }
        public Pagamento Pagamento { get; set; }
    }
}
