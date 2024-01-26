using Application.Implementations;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace TestBDD.StepDefinitions
{
    [Binding]
    public class PedidoUseCaseStepDefinitions
    {
        readonly Mock<IPedidoRepository> _mockPedidoRepository;
        readonly Mock<IClienteRepository> _mockClienteRepository;
        readonly Mock<IProdutoRepository> _mockProdutoRepository;

        readonly Mock<IPedidoStatusUseCase> _mockPedidoStatusUseCase;
        readonly Mock<IPedidoPagamentoUseCase> _mockPedidoPagamentoUseCase;

        readonly IPedidoUseCase _pedidoUseCase;

        private List<Application.DTOs.Output.Pedido> _databaseMock = new List<Application.DTOs.Output.Pedido>();
        private List<Application.DTOs.External.StatusService.PedidoStatusResponse> _pedidoStatusServiceDatabaseMock = new List<Application.DTOs.External.StatusService.PedidoStatusResponse>();
        private List<Application.DTOs.External.PagamentoService.PedidoPagamentoResponse> _pedidoPagamentoServiceDatabaseMock = new List<Application.DTOs.External.PagamentoService.PedidoPagamentoResponse>();
       
        private int _pedidoId;
        private bool _updateOrderStatusResult;

        private Application.DTOs.Output.Pedido _getResult;
        private IEnumerable<Application.DTOs.Output.Pedido> _listResult;
        private Application.DTOs.Imput.Pedido _novoPedido;
        private Application.DTOs.Output.PedidoIdentificador _identificadorPedido;

        private Application.DTOs.Output.PedidoPagamento _pagamentoResult;
        private Action _act;

        public PedidoUseCaseStepDefinitions()
        {
            _mockProdutoRepository = new Mock<IProdutoRepository>();
            _mockProdutoRepository = new Mock<IProdutoRepository>();
            _mockPedidoRepository = new Mock<IPedidoRepository>();
            _mockClienteRepository = new Mock<IClienteRepository>();
            _mockPedidoStatusUseCase = new Mock<IPedidoStatusUseCase>();
            _mockPedidoPagamentoUseCase = new Mock<IPedidoPagamentoUseCase>();

            _pedidoUseCase = new PedidoUseCase(
                _mockPedidoRepository.Object,
                _mockClienteRepository.Object,
                _mockProdutoRepository.Object,
                _mockPedidoStatusUseCase.Object,
                _mockPedidoPagamentoUseCase.Object
            );

            _databaseMock = new List<Application.DTOs.Output.Pedido> 
            {
                new Application.DTOs.Output.Pedido 
                {       Id = 1,
                        Data = DateTime.Now,
                        ClienteId = 1,
                        PedidoStatus = Application.Enums.PedidoStatus.Recebido.ToString(),
                        Valor = 40,
                        ClienteObservacao = "Observação do cliente 1",
                        Itens = new List<Application.DTOs.Output.PedidoItem>
                        {
                            new Application.DTOs.Output.PedidoItem
                            {
                                PedidoId = 1,
                                ProdutoId = 1,
                                NomeProduto = "Coca",
                                PrecoUnitario = 5,
                                Quantidade = 2,
                            },
                            new Application.DTOs.Output.PedidoItem
                            {
                                PedidoId = 1,
                                ProdutoId = 2,
                                NomeProduto = "Burguer",
                                PrecoUnitario = 30,
                                Quantidade = 1,
                            },
                        },
                        Pagamento = new Application.DTOs.Output.Pagamento
                        {
                            StatusPagamento = Application.Enums.PagamentoStatus.Aprovado,
                            TipoPagamento = Application.Enums.TipoPagamento.Debito
                        }
                },          
                new Application.DTOs.Output.Pedido 
                {       Id = 2,
                        Data = DateTime.Now,
                        ClienteId = 2,
                        PedidoStatus = Application.Enums.PedidoStatus.EmPreparacao.ToString(),
                        Valor = 80,
                        ClienteObservacao = "Observação do cliente 2",
                        Itens = new List<Application.DTOs.Output.PedidoItem>
                        {
                            new Application.DTOs.Output.PedidoItem
                            {
                                PedidoId = 2,
                                ProdutoId = 1,
                                NomeProduto = "Coca",
                                PrecoUnitario = 5,
                                Quantidade = 4,
                            },
                            new Application.DTOs.Output.PedidoItem
                            {
                                PedidoId = 2,
                                ProdutoId = 2,
                                NomeProduto = "Burguer",
                                PrecoUnitario = 30,
                                Quantidade = 2,
                            },
                        },
                        Pagamento = new Application.DTOs.Output.Pagamento
                        {
                            StatusPagamento = Application.Enums.PagamentoStatus.Aprovado,
                            TipoPagamento = Application.Enums.TipoPagamento.Credito
                        }
                },
            };
           
            _pedidoStatusServiceDatabaseMock = new List<Application.DTOs.External.StatusService.PedidoStatusResponse>
            {
                new Application.DTOs.External.StatusService.PedidoStatusResponse
                {
                    PedidoId = 1,
                    Status = Application.Enums.PedidoStatus.Recebido
                },
                new Application.DTOs.External.StatusService.PedidoStatusResponse
                {
                    PedidoId = 2,
                    Status = Application.Enums.PedidoStatus.EmPreparacao
                },
            };

            _pedidoPagamentoServiceDatabaseMock = new List<Application.DTOs.External.PagamentoService.PedidoPagamentoResponse>
            {
                new Application.DTOs.External.PagamentoService.PedidoPagamentoResponse
                {
                    PedidoId = 1,
                    CodigoTransacao = "123",
                    Valor = 40,
                    PagamentoStatus = Application.Enums.PagamentoStatus.Aprovado,
                    TipoPagamento = Application.Enums.TipoPagamento.Debito
                },
                new Application.DTOs.External.PagamentoService.PedidoPagamentoResponse
                {
                    PedidoId = 2,
                    CodigoTransacao = "123",
                    Valor = 80,
                    PagamentoStatus = Application.Enums.PagamentoStatus.Aprovado,
                    TipoPagamento = Application.Enums.TipoPagamento.Credito
                },
            };
        }

        [Given(@"Que exista um pedido com o ID específico")]
        public void GivenQueExistaUmPedidoComOIDEspecifico()
        {
            _pedidoId = 1;

            _mockPedidoRepository.Setup(r => r.Get(_pedidoId))
                .Returns(_databaseMock.First(p => p.Id == _pedidoId));
            
            _mockPedidoStatusUseCase.Setup(s => s.Get(It.IsAny<int>()))
                .Returns(_pedidoStatusServiceDatabaseMock.First(p => p.PedidoId == _pedidoId));
            
            _mockPedidoPagamentoUseCase.Setup(p => p.Get(It.IsAny<int>()))
                .Returns(_pedidoPagamentoServiceDatabaseMock.First(p => p.PedidoId == _pedidoId));
        }

        [When(@"Eu solicito o pedido pelo ID")]
        public void WhenEuSolicitoOPedidoPeloID()
        {
           _getResult = _pedidoUseCase.Get(_pedidoId);
        }

        [Then(@"Devo receber o pedido correspondente")]
        public void ThenDevoReceberOPedidoCorrespondente()
        {
            _getResult.Should().NotBeNull();
            _getResult.Should().BeOfType<Application.DTOs.Output.Pedido>();
            _getResult.Should().BeEquivalentTo(_databaseMock.First(p => p.Id == _pedidoId));
        }

        [Given(@"Que existem pedidos no sistema")]
        public void GivenQueExistemPedidosNoSistema()
        {
            _mockPedidoRepository.Setup(r => r.List())
                .Returns(_databaseMock);

            _mockPedidoStatusUseCase.Setup(s => s.List(_databaseMock.Select(s => s.Id)))
                .Returns(_pedidoStatusServiceDatabaseMock);

            _mockPedidoPagamentoUseCase.Setup(p => p.List(_databaseMock.Select(s => s.Id)))
                .Returns(_pedidoPagamentoServiceDatabaseMock);
           
        }

        [When(@"Eu solicito a lista de todos os pedidos")]
        public void WhenEuSolicitoAListaDeTodosOsPedidos()
        {
            _listResult = _pedidoUseCase.List();
        }

        [Then(@"Devo receber uma lista contendo todos os pedidos")]
        public void ThenDevoReceberUmaListaContendoTodosOsPedidos()
        {
            _listResult.Should().NotBeEmpty()
                .And.HaveCount(2);
        }

        [Given(@"Que existem pedidos com diferentes status")]
        public void GivenQueExistemPedidosComDiferentesStatus()
        {
            var pedidoStatus = Application.Enums.PedidoStatus.Recebido;

            _mockPedidoStatusUseCase.Setup(s => s.ListByStatus(pedidoStatus))
             .Returns(_pedidoStatusServiceDatabaseMock.Where(p => p.Status == pedidoStatus).ToList());

            _mockPedidoRepository.Setup(r => r.ListByIds(It.IsAny<IEnumerable<int>>()))
                .Returns(_databaseMock.Where(p => p.PedidoStatus == pedidoStatus.ToString()).ToList());
    
            _mockPedidoPagamentoUseCase.Setup(p => p.List(It.IsAny<IEnumerable<int>>()))
                .Returns(_pedidoPagamentoServiceDatabaseMock.Where(p => p.PedidoId == 1).ToList());
        }

        [When(@"Eu solicito a lista de pedidos filtrados por um status específico")]
        public void WhenEuSolicitoAListaDePedidosFiltradosPorUmStatusEspecifico()
        {
            var pedidoStatus = Application.Enums.PedidoStatus.Recebido;
            _listResult = _pedidoUseCase.ListByStatus(pedidoStatus);
        }

        [Then(@"Devo receber uma lista contendo apenas os pedidos com o status especificado")]
        public void ThenDevoReceberUmaListaContendoApenasOsPedidosComOStatusEspecificado()
        {
            _listResult.Should().NotBeEmpty()
                .And.HaveCount(1);
        }

        [Given(@"Que tenho os detalhes de um novo pedido")]
        public void GivenQueTenhoOsDetalhesDeUmNovoPedido()
        {
            var clientId = 1;

            _novoPedido = new Application.DTOs.Imput.Pedido
            {
                ClienteId = clientId,
                ClienteObservacao = "Observação do Cliente",
                Itens = new List<Application.DTOs.Imput.PedidoItem>
                {
                    new Application.DTOs.Imput.PedidoItem
                    {
                        ProdutoId = 1,
                        Quantidade = 1
                    }
                },
                Pagamento = new Application.DTOs.Imput.Pagamento
                {
                    Nome = "Cliente 1",
                    TipoPagamento = Application.Enums.TipoPagamento.Debito,
                    TokenCartao = "123"
                }
            };

            var listaDeProdutos = new List<Domain.Entities.Produto>
            {
                new Domain.Entities.Produto
                {
                    Id = 1,
                    Nome = "Coca",
                    Descricao = "Refrigerante",
                    Preco = 4,
                    Imagem = "123",
                    ProdutoCategoriaId = Domain.Enums.ProdutoCategoria.Bebida
                }
            };

            _mockClienteRepository.Setup(r => r.Get(It.IsAny<int>()))
                .Returns(new Mock<Domain.Entities.Cliente>().Object);
            
            _mockPedidoRepository.Setup(r => r.Order(It.IsAny<Domain.Entities.Pedido>()))
                .Returns(123);

            _mockProdutoRepository.Setup(r => r.ListByIds(It.IsAny<List<int>>()))
                .Returns(listaDeProdutos); 
            _mockProdutoRepository.Setup(r => r.List()).Returns(listaDeProdutos);
            
            _mockPedidoStatusUseCase.Setup(p => p.Save(It.IsAny<Application.DTOs.External.StatusService.PedidoStatusRequest>()))
                .Returns(true);           
            _mockPedidoPagamentoUseCase.Setup(p => p.Save(It.IsAny<Application.DTOs.External.PagamentoService.PedidoPagamentoRequest>()))
                .Returns(true);
        }

        [When(@"Eu crio um novo pedido")]
        public void WhenEuCrioUmNovoPedido()
        {
            _identificadorPedido = _pedidoUseCase.Order(_novoPedido);
        }

        [Then(@"Deve ser criado um novo pedido com sucesso")]
        public void ThenDeveSerCriadoUmNovoPedidoComSucesso()
        {
            _identificadorPedido.Should().NotBeNull();
            _identificadorPedido.Should().BeOfType<Application.DTOs.Output.PedidoIdentificador>();
            _identificadorPedido.Id.Should().Be(123);

            _mockPedidoStatusUseCase.Verify(s => s.Save(It.IsAny<Application.DTOs.External.StatusService.PedidoStatusRequest>()),
                Times.Once());        
            _mockPedidoPagamentoUseCase.Verify(s => s.Save(It.IsAny<Application.DTOs.External.PagamentoService.PedidoPagamentoRequest>()),
                Times.Once());
        }

        [Given(@"Que existe um pedido com um status específico")]
        public void GivenQueExisteUmPedidoComUmStatusEspecifico()
        {
            _pedidoId = 1;

            _mockPedidoPagamentoUseCase.Setup(p => p.Get(It.IsAny<int>()))
                .Returns(_pedidoPagamentoServiceDatabaseMock.First(p => p.PedidoId == _pedidoId));

            _mockPedidoStatusUseCase.Setup(p => p.Update(It.IsAny<int>(), It.IsAny<Application.Enums.PedidoStatus>()))
               .Returns(true);
        }

        [When(@"Eu solicito o status desse pedido")]
        public void WhenEuSolicitoOStatusDessePedido()
        {
            _pagamentoResult = _pedidoUseCase.GetPaymentStatus(_pedidoId);
        }

        [Then(@"Devo receber o status do pedido correspondente")]
        public void ThenDevoReceberOStatusDoPedidoCorrespondente()
        {
            var item = _pedidoPagamentoServiceDatabaseMock.First(p => p.PedidoId == _pedidoId);
            _pagamentoResult.Should().BeOfType<Application.DTOs.Output.PedidoPagamento>();

            _pagamentoResult.PedidoId.Should().Be(item.PedidoId);
            _pagamentoResult.StatusPagamento.Should().Be(item.PagamentoStatus);
        }

        [When(@"Eu atualizo o status desse pedido")]
        public void WhenEuAtualizoOStatusDessePedido()
        {
            _updateOrderStatusResult = _pedidoUseCase.UpdateOrderStatus(_pedidoId, Application.Enums.PedidoStatus.Recebido);
        }

        [Then(@"O status do pedido deve ser atualizado com sucesso")]
        public void ThenOStatusDoPedidoDeveSerAtualizadoComSucesso()
        {
            _updateOrderStatusResult.Should().BeTrue();
            _mockPedidoStatusUseCase.Verify(s => s.Update(It.IsAny<int>(), It.IsAny<Application.Enums.PedidoStatus>()),
                Times.Once());
        }
    }
}
