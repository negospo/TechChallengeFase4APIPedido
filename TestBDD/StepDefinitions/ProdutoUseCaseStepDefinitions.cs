using Application.Implementations;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Moq;

namespace TestBDD.StepDefinitions
{
    [Binding]
    public class ProdutoUseCaseStepDefinitions
    {
        readonly Mock<IProdutoRepository> _mockProdutoRepository;
        readonly IProdutoUseCase _produtoUseCase;

        private List<Domain.Entities.Produto> databaseMock = new List<Domain.Entities.Produto>();
        private int _produtoId;

        private Application.DTOs.Output.Produto _getResult;
        private IEnumerable<Application.DTOs.Output.Produto> _listResult;
        private Application.DTOs.Imput.ProdutoInsert _novoProduto;
        private Application.DTOs.Imput.ProdutoUpdate _atualizacaoProduto;
        private Action _act;

        public ProdutoUseCaseStepDefinitions()
        {
            _mockProdutoRepository = new Mock<IProdutoRepository>();
            _produtoUseCase = new ProdutoUseCase(_mockProdutoRepository.Object);

            databaseMock = new List<Domain.Entities.Produto>
            {
                new Domain.Entities.Produto(){ Id = 1, Nome = "Coca", Descricao = "Refrigerante", ProdutoCategoriaId=Domain.Enums.ProdutoCategoria.Bebida, Preco=4, Imagem="123"},
                new Domain.Entities.Produto(){ Id = 2, Nome = "Burguer", Descricao = "Hamburguer", ProdutoCategoriaId=Domain.Enums.ProdutoCategoria.Lanche, Preco=20, Imagem="456"}
            };

        }

        [Given(@"Existe um produto com o ID especificado")]
        public void GivenExisteUmProdutoComOIDEspecificado()
        {
            _produtoId = 1;
            
            _mockProdutoRepository.Setup(r => r.Get(_produtoId))
                .Returns(databaseMock.First(f => f.Id == _produtoId));

            _mockProdutoRepository.Setup(r => r.Delete(_produtoId))
                .Returns(true);
        }

        [When(@"Eu solicito o produto com o ID específico")]
        public void WhenEuSolicitoOProdutoComOIDEspecifico()
        {
            _getResult = _produtoUseCase.Get(_produtoId);
        }

        [Then(@"O produto correspondente é retornado")]
        public void ThenOProdutoCorrespondenteERetornado()
        {
            _getResult.Should().NotBeNull();
            _getResult.Should().BeEquivalentTo(databaseMock.First(p => p.Id == _produtoId));
        }

        [Given(@"Eu tenho um ID de produto inexistente")]
        public void GivenEuTenhoUmIDDeProdutoInexistente()
        {
            _produtoId = 3;
            _mockProdutoRepository.Setup(r => r.Get(_produtoId))
                .Returns((Domain.Entities.Produto)null);

            _mockProdutoRepository.Setup(r => r.Delete(_produtoId))
                .Returns(false);
        }

        [When(@"Eu tento obter os detalhes do produto com o ID fornecido")]
        public void WhenEuTentoObterOsDetalhesDoProdutoComOIDFornecido()
        {
            _act = () => _produtoUseCase.Get(_produtoId);
        }

        [Then(@"Uma exceção de Produto não encontrado é lançada")]
        public void ThenUmaExcecaoDeProdutoNaoEncontradoELancada()
        {
            _act.Should().Throw<Application.CustomExceptions.NotFoundException>()
                .WithMessage("Produto não encontrado");
        }


        [Given(@"Existem produtos cadastrados no sistema")]
        public void GivenExistemProdutosCadastradosNoSistema()
        {
            _mockProdutoRepository.Setup(r => r.List())
                .Returns(databaseMock);
        }

        [When(@"Eu solicito a lista de todos os produtos")]
        public void WhenEuSolicitoAListaDeTodosOsProdutos()
        {
            _listResult = _produtoUseCase.List();
        }

        [Then(@"Todos os produtos são retornados")]
        public void ThenTodosOsProdutosSaoRetornados()
        {
            _listResult.Should().NotBeEmpty()
                .And.HaveCount(2);
        }

        [Given(@"Cada produto possui uma categoria atribuída")]
        public void GivenCadaProdutoPossuiUmaCategoriaAtribuida()
        {
            _mockProdutoRepository.Setup(r => r.ListByCategory(
                Application.Enums.ProdutoCategoria.Bebida))
                .Returns(databaseMock.Where(
                    p => (int)p.ProdutoCategoriaId == (int)Application.Enums.ProdutoCategoria.Bebida)
                    .ToList());
        }

        [When(@"Eu solicito a lista de produtos de uma categoria específica")]
        public void WhenEuSolicitoAListaDeProdutosDeUmaCategoriaEspecifica()
        {
            _listResult = _produtoUseCase.ListByCategory(Application.Enums.ProdutoCategoria.Bebida);
        }

        [Then(@"Apenas os produtos da categoria especificada são retornados")]
        public void ThenApenasOsProdutosDaCategoriaEspecificadaSaoRetornados()
        {
            _listResult.Should().NotBeEmpty()
                .And.HaveCount(1);
        }

        [When(@"Eu solicito a exclusão do produto com o ID específico")]
        public void WhenEuSolicitoAExclusaoDoProdutoComOIDEspecifico()
        {
            _produtoUseCase.Delete(_produtoId);
        }

        [Then(@"O produto é removido do sistema")]
        public void ThenOProdutoERemovidoDoSistema()
        {
            _mockProdutoRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Once());
        }

        [When(@"Eu tento excluir o produto com o ID fornecido")]
        public void WhenEuTentoExcluirOProdutoComOIDFornecido()
        {
           _act = () => _produtoUseCase.Delete(_produtoId);
        }

        [Given(@"Eu tenho informações válidas para um novo produto")]
        public void GivenEuTenhoInformacoesValidasParaUmNovoProduto()
        {
            _produtoId = 3;
            _novoProduto = new Application.DTOs.Imput.ProdutoInsert
            {
                Nome = "Fanta",
                Descricao = "Bebida",
                Preco = 4,
                Imagem = "123",
                ProdutoCategoriaId = Application.Enums.ProdutoCategoria.Bebida
            };

            databaseMock.Add(
                new Domain.Entities.Produto
                {
                    Id = 3,
                    Nome = "Fanta",
                    Descricao = "Bebida",
                    Preco = 4,
                    Imagem = "123",
                    ProdutoCategoriaId = Domain.Enums.ProdutoCategoria.Bebida
                }
             );

            _mockProdutoRepository.Setup(r => r.Get(_produtoId))
                .Returns(databaseMock.First(f => f.Id == _produtoId));

            _mockProdutoRepository.Setup(r => r.Insert(
                It.IsAny<Domain.Entities.Produto>()))
                .Returns(3);
        }

        [When(@"Eu solicito a inserção do novo produto")]
        public void WhenEuSolicitoAInsercaoDoNovoProduto()
        {
             _getResult = _produtoUseCase.Insert(_novoProduto);
        }

        [Then(@"O novo produto é cadastrado no sistema")]
        public void ThenONovoProdutoECadastradoNoSistema()
        {
            _getResult.Should().NotBeNull();
            _getResult.Id.Should().Be(3);
            _getResult.Should().BeOfType<Application.DTOs.Output.Produto>();
        }

        [Given(@"Eu tenho informações atualizadas para o produto")]
        public void GivenEuTenhoInformacoesAtualizadasParaOProduto()
        {
            _produtoId = 2;
            _atualizacaoProduto = new Application.DTOs.Imput.ProdutoUpdate
            {
                Id = _produtoId,
                Nome = "Fanta",
                Descricao = "Bebida",
                Preco = 4,
                Imagem = "123",
                ProdutoCategoriaId = Application.Enums.ProdutoCategoria.Bebida
            };

            _mockProdutoRepository.Setup(r => r.Get(_produtoId))
                .Returns(databaseMock.First(f => f.Id == _produtoId));

            _mockProdutoRepository.Setup(r => r.Update(
                It.IsAny<Domain.Entities.Produto>()))
                .Returns(true);
        }

        [When(@"Eu solicito a atualização do produto com o ID especifico")]
        public void WhenEuSolicitoAAtualizacaoDoProdutoComOIDEspecifico()
        {
            _getResult = _produtoUseCase.Update(_atualizacaoProduto);
        }

        [Then(@"O produto é atualizado com as novas informações")]
        public void ThenOProdutoEAtualizadoComAsNovasInformacoes()
        {
            _getResult.Should().NotBeNull();
            _getResult.Id.Should().Be(2);
            _getResult.Should().BeOfType<Application.DTOs.Output.Produto>();
        }

        [Given(@"Eu tenho informações de atualização de produto sem ID")]
        public void GivenEuTenhoInformacoesDeAtualizacaoDeProdutoSemID()
        {
            _atualizacaoProduto = new Application.DTOs.Imput.ProdutoUpdate
            {
                Id = null,
                Nome = "Fanta",
                Descricao = "Bebida",
                Preco = 4,
                Imagem = "123",
                ProdutoCategoriaId = Application.Enums.ProdutoCategoria.Bebida
            };
        }

        [When(@"Eu tento atualizar o produto")]
        public void WhenEuTentoAtualizarOProduto()
        {
            _act = () => _produtoUseCase.Update(_atualizacaoProduto);
        }

        [Then(@"Uma exceção de Conflito é lançada indicando que o ID está vazio")]
        public void ThenUmaExcecaoDeConflitoELancadaIndicandoQueOIDEstaVazio()
        {
            _act.Should().Throw<Application.CustomExceptions.ConflictException>()
                .WithMessage("Id do produto está vazio");
        }

        [Given(@"Eu tenho um ID de produto inexistente e informações de atualização")]
        public void GivenEuTenhoUmIDDeProdutoInexistenteEInformacoesDeAtualizacao()
        {
            _atualizacaoProduto = new Application.DTOs.Imput.ProdutoUpdate
            {
                Id = 4,
                Nome = "Fanta",
                Descricao = "Bebida",
                Preco = 4,
                Imagem = "123",
                ProdutoCategoriaId = Application.Enums.ProdutoCategoria.Bebida
            };

            _mockProdutoRepository.Setup(r => r.Update(
                It.IsAny<Domain.Entities.Produto>()))
                .Returns(false);
        }

        [When(@"Eu tento atualizar o produto com as novas informações")]
        public void WhenEuTentoAtualizarOProdutoComAsNovasInformacoes()
        {
            _act = () => _produtoUseCase.Update(_atualizacaoProduto);
        }
    }
}