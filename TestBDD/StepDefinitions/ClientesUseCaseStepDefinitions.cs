using Application.Implementations;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Moq;
using System;
using TechTalk.SpecFlow;

namespace TestBDD.StepDefinitions
{
    [Binding]
    public class ClientesUseCaseStepDefinitions
    {
        readonly Mock<IClienteRepository> _mockClienteRepository;
        readonly IClienteUseCase _clienteUseCase;

        private List<Domain.Entities.Cliente> databaseMock = new List<Domain.Entities.Cliente>();
        private int _clienteId;
        private string _clienteCpf;

        private Application.DTOs.Output.Cliente _getResult;
        private IEnumerable<Application.DTOs.Output.Cliente> _listResult;
        private Application.DTOs.Imput.ClienteInsert _novoCliente;
        private Application.DTOs.Imput.ClienteUpdate _atualizacaoCliente;
        private Domain.Entities.Cliente _cliente;
        private Action _act;

        public ClientesUseCaseStepDefinitions()
        {
            _mockClienteRepository = new Mock<IClienteRepository>();
            _clienteUseCase = new ClienteUseCase(_mockClienteRepository.Object);

            databaseMock = new List<Domain.Entities.Cliente>
            {
                new Domain.Entities.Cliente(){ Id = 1, Nome = "MV", Cpf = "408.158.700-00", Email = "mv@mvmail.com"},
                new Domain.Entities.Cliente(){ Id = 2, Nome = "CS", Cpf = "463.425.450-65", Email = "cs@csmail.com"}
            };
        }

        [Given(@"Eu tenho um ID de cliente existente")]
        public void GivenEuTenhoUmIDDeClienteExistente()
        {
            _clienteId = 1;

            _mockClienteRepository.Setup(r => r.Get(_clienteId))
                .Returns(databaseMock.First(f => f.Id == _clienteId));

            _mockClienteRepository.Setup(r => r.Delete(_clienteId))
                .Returns(true);
        }

        [When(@"Eu solicito os detalhes do cliente com o ID fornecido")]
        public void WhenEuSolicitoOsDetalhesDoClienteComOIDFornecido()
        {
            _getResult = _clienteUseCase.Get(_clienteId);
        }

        [Then(@"Os detalhes do cliente são retornados com sucesso")]
        public void ThenOsDetalhesDoClienteSaoRetornadosComSucesso()
        {
            _getResult.Should().NotBeNull();
            _getResult.Should().BeEquivalentTo(databaseMock.First(p => p.Id == _clienteId));
        }

        [Given(@"Eu tenho um ID de cliente inexistente")]
        public void GivenEuTenhoUmIDDeClienteInexistente()
        {
            _clienteId = 3;
            _mockClienteRepository.Setup(r => r.Get(_clienteId))
                .Returns((Domain.Entities.Cliente)null);

            _mockClienteRepository.Setup(r => r.Delete(_clienteId))
                .Returns(false);
        }

        [When(@"Eu tento obter os detalhes do cliente com o ID fornecido")]
        public void WhenEuTentoObterOsDetalhesDoClienteComOIDFornecido()
        {
            _act = () => _clienteUseCase.Get(_clienteId);
        }

        [Then(@"Uma exceção de Cliente não encontrado é lançada")]
        public void ThenUmaExcecaoDeClienteNaoEncontradoELancada()
        {
            _act.Should().Throw<Application.CustomExceptions.NotFoundException>()
                .WithMessage("Cliente não encontrado");
        }

        [Given(@"Eu tenho um CPF de cliente existente")]
        public void GivenEuTenhoUmCPFDeClienteExistente()
        {
            _clienteCpf = "408.158.700-00";

            _mockClienteRepository.Setup(r => r.GetByCpf(_clienteCpf))
                .Returns(databaseMock.First(f => f.Cpf == _clienteCpf));
        }

        [When(@"Eu solicito os detalhes do cliente com o CPF fornecido")]
        public void WhenEuSolicitoOsDetalhesDoClienteComOCPFFornecido()
        {
            _getResult = _clienteUseCase.GetByCpf(_clienteCpf);
        }

        [Then(@"Os detalhes do cliente são retornados com sucesso por meio do CPF")]
        public void ThenOsDetalhesDoClienteSaoRetornadosComSucessoPorMeioDoCPF()
        {
            _getResult.Should().NotBeNull();
            _getResult.Should().BeEquivalentTo(databaseMock.First(p => p.Cpf == _clienteCpf));
        }


        [Given(@"Eu tenho um CPF de cliente inexistente")]
        public void GivenEuTenhoUmCPFDeClienteInexistente()
        {
            _clienteCpf = "069.228.100-23";
            _mockClienteRepository.Setup(r => r.GetByCpf(_clienteCpf))
                .Returns((Domain.Entities.Cliente)null);
        }

        [When(@"Eu tento obter os detalhes do cliente com o CPF fornecido")]
        public void WhenEuTentoObterOsDetalhesDoClienteComOCPFFornecido()
        {
            _act = () => _clienteUseCase.GetByCpf(_clienteCpf);
        }

        [Given(@"Existem clientes registrados")]
        public void GivenExistemClientesRegistrados()
        {
            _mockClienteRepository.Setup(r => r.List())
                .Returns(databaseMock);
        }

        [When(@"Eu solicito a lista de todos os clientes")]
        public void WhenEuSolicitoAListaDeTodosOsClientes()
        {
            _listResult = _clienteUseCase.List();
        }

        [Then(@"A lista de clientes é retornada com sucesso")]
        public void ThenAListaDeClientesERetornadaComSucesso()
        {
            _listResult.Should().NotBeEmpty()
                .And.HaveCount(2);
        }

        [When(@"Eu solicito a exclusão do cliente com o ID fornecido")]
        public void WhenEuSolicitoAExclusaoDoClienteComOIDFornecido()
        {
            _clienteUseCase.Delete(_clienteId);
        }

        [Then(@"O cliente é excluído com sucesso")]
        public void ThenOClienteEExcluidoComSucesso()
        {
            _mockClienteRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once());
        }

        [When(@"Eu tento excluir o cliente com o ID fornecido")]
        public void WhenEuTentoExcluirOClienteComOIDFornecido()
        {
            _act = () => _clienteUseCase.Delete(_clienteId);
        }

        [Given(@"Eu tenho informações de um novo cliente")]
        public void GivenEuTenhoInformacoesDeUmNovoCliente()
        {
            _clienteId = 3;
            _novoCliente = new Application.DTOs.Imput.ClienteInsert
            {
                Nome = "Silva",
                Cpf = "069.228.100-23",
                Email = "silva@email.com"
            };

            databaseMock.Add(
                new Domain.Entities.Cliente
                {
                    Id = 3,
                    Nome = "Silva",
                    Cpf = "069.228.100-23",
                    Email = "silva@email.com"
                }
             );

            _mockClienteRepository.Setup(r => r.Get(_clienteId))
                .Returns(databaseMock.First(f => f.Id == _clienteId));

            _mockClienteRepository.Setup(r => r.Insert(
                It.IsAny<Domain.Entities.Cliente>()))
                .Returns(3);
        }

        [When(@"Eu solicito a inserção do novo cliente")]
        public void WhenEuSolicitoAInsercaoDoNovoCliente()
        {
            _getResult = _clienteUseCase.Insert(_novoCliente);
        }

        [Then(@"O cliente é inserido com sucesso")]
        public void ThenOClienteEInseridoComSucesso()
        {
            _getResult.Should().NotBeNull();
            _getResult.Id.Should().Be(3);
            _getResult.Should().BeOfType<Application.DTOs.Output.Cliente>();
        }

        [Given(@"Eu tenho informações de um cliente com CPF já existente")]
        public void GivenEuTenhoInformacoesDeUmClienteComCPFJaExistente()
        {
            _novoCliente = new Application.DTOs.Imput.ClienteInsert
            {
                Nome = "MV",
                Cpf = "408.158.700-00",
                Email = "mv@mvmail.com"
            };
            _cliente = new Domain.Entities.Cliente()
            {
                Id = 1,
                Nome = "MV",
                Cpf = "408.158.700-00",
                Email = "mv@mvmail.com"
            };

            _mockClienteRepository.Setup(r => r.GetByCpf(It.IsAny<string>()))
                .Returns(_cliente);
        }

        [When(@"Eu tento inserir o cliente duplicado")]
        public void WhenEuTentoInserirOClienteDuplicado()
        {
            _act = () => _clienteUseCase.Insert(_novoCliente);
        }

        [Then(@"Uma exceção de Conflito é lançada indicando CPF duplicado")]
        public void ThenUmaExcecaoDeConflitoELancadaIndicandoCPFDuplicado()
        {
            _act.Should().Throw<Application.CustomExceptions.ConflictException>()
                .WithMessage("Ja existe outro usuário com esse cpf");
        }

        [Given(@"Eu tenho informações de um cliente com email já existente")]
        public void GivenEuTentoInserirUmClienteComEmailJaExistente()
        {
            _novoCliente = new Application.DTOs.Imput.ClienteInsert
            {
                Nome = "MV",
                Cpf = "408.158.700-00",
                Email = "mv@mvmail.com"
            };
            _cliente = new Domain.Entities.Cliente()
            {
                Id = 1,
                Nome = "MV",
                Cpf = "408.158.700-00",
                Email = "mv@mvmail.com"
            };

            _mockClienteRepository.Setup(r => r.GetByEmail(It.IsAny<string>()))
                .Returns(_cliente);

        }

        [Then(@"Uma exceção de Conflito é lançada indicando email duplicado")]
        public void ThenUmaExcecaoDeConflitoELancadaIndicandoEmailDuplicado()
        {
            _act.Should().Throw<Application.CustomExceptions.ConflictException>()
                .WithMessage("Ja existe outro usuário com esse email");
        }

        [Given(@"Eu tenho informações de atualização de um cliente existente")]
        public void GivenEuTenhoInformacoesDeAtualizacaoDeUmClienteExistente()
        {
            _clienteId = 2;
            _atualizacaoCliente = new Application.DTOs.Imput.ClienteUpdate
            {
                Id = _clienteId,
                Nome = "Silva",
                Cpf = "069.228.100-23",
                Email = "silva@email.com"
            };

            _mockClienteRepository.Setup(r => r.Get(_clienteId))
                .Returns(databaseMock.First(f => f.Id == _clienteId));

            _mockClienteRepository.Setup(r => r.Update(
                It.IsAny<Domain.Entities.Cliente>()))
                .Returns(true);
        }

        [When(@"Eu solicito a atualização do cliente com as novas informações")]
        public void WhenEuSolicitoAAtualizacaoDoClienteComAsNovasInformacoes()
        {
            _getResult = _clienteUseCase.Update(_atualizacaoCliente);
        }

        [Then(@"Os detalhes do cliente são atualizados com sucesso")]
        public void ThenOsDetalhesDoClienteSaoAtualizadosComSucesso()
        {
            _getResult.Should().NotBeNull();
            _getResult.Id.Should().Be(2);
            _getResult.Should().BeOfType<Application.DTOs.Output.Cliente>();
        }

        [Given(@"Eu tenho informações de atualização de cliente sem ID")]
        public void GivenEuTenhoInformacoesDeAtualizacaoDeClienteSemID()
        {
            _atualizacaoCliente = new Application.DTOs.Imput.ClienteUpdate
            {
                Id = null,
                Nome = "Silva",
                Cpf = "069.228.100-23",
                Email = "silva@email.com"
            };
        }

        [When(@"Eu tento atualizar o cliente sem ID")]
        public void WhenEuTentoAtualizarOCliente()
        {
           _act = () => _clienteUseCase.Update(_atualizacaoCliente);
        }

        [Then(@"Uma exceção de Conflito é lançada indicando que o ID do cliente está vazio")]
        public void ThenUmaExcecaoDeConflitoELancadaIndicandoQueOIDDoClienteEstaVazio()
        {
            _act.Should().Throw<Application.CustomExceptions.ConflictException>()
                .WithMessage("Id do cliente está vazio");
        }

        [Given(@"Eu tenho informações de atualização de um cliente existente com CPF duplicado")]
        public void GivenEuTenhoInformacoesDeAtualizacaoDeUmClienteExistenteComCPFDuplicado()
        {
            _atualizacaoCliente = new Application.DTOs.Imput.ClienteUpdate
            {
                Id = 1,
                Nome = "MV2",
                Cpf = "408.158.700-00",
                Email = "mv2@mvmail.com"
            };
            _cliente = new Domain.Entities.Cliente()
            {
                Id = 2,
                Nome = "MV",
                Cpf = "408.158.700-00",
                Email = "mv@mvmail.com"
            };

            _mockClienteRepository.Setup(r => r.GetByCpf(It.IsAny<string>()))
                .Returns(_cliente);
        }

        [Given(@"Eu tenho informações de atualização de um cliente existente com email duplicado")]
        public void GivenEuTenhoInformacoesDeAtualizacaoDeUmClienteExistenteComEmailDuplicado()
        {
            _atualizacaoCliente = new Application.DTOs.Imput.ClienteUpdate
            {
                Id = 1,
                Nome = "MV2",
                Cpf = "069.228.100-23",
                Email = "mv2@mvmail.com"
            };
            _cliente = new Domain.Entities.Cliente()
            {
                Id = 2,
                Nome = "MV",
                Cpf = "069.228.100-23",
                Email = "mv@mvmail.com"
            };

            _mockClienteRepository.Setup(r => r.GetByEmail(It.IsAny<string>()))
                .Returns(_cliente);
        }

        [When(@"Eu tento atualizar um cliente com email ou CPF duplicado")]
        public void WhenEuTentoAtualizarUmClienteComEmailOuCPFDuplicado()
        {
            _act = () => _clienteUseCase.Update(_atualizacaoCliente);
        }


    }
}
