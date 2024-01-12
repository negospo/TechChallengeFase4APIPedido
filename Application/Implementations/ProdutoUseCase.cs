using Application.DTOs.Output;
using Application.Enums;

namespace Application.Implementations
{
    public class ProdutoUseCase : Interfaces.UseCases.IProdutoUseCase
    {
        private readonly Interfaces.Repositories.IProdutoRepository _produtoRepository;

        public ProdutoUseCase(Interfaces.Repositories.IProdutoRepository produtoRepository)
        {
            this._produtoRepository = produtoRepository;
        }

        public bool Delete(int id)
        {
            var result = this._produtoRepository.Delete(id);
            if(!result)
                throw new CustomExceptions.NotFoundException("Produto não encontrado");

            return result;
        }

        public DTOs.Output.Produto Get(int id)
        {
            var result = this._produtoRepository.Get(id);
            if (result == null)
                throw new CustomExceptions.NotFoundException("Produto não encontrado");

            return new DTOs.Output.Produto
            {
                Id = result.Id.Value,
                Descricao = result.Descricao,
                Imagem = result.Imagem,
                Nome = result.Nome,
                Preco = result.Preco,
                ProdutoCategoriaId = (Enums.ProdutoCategoria)result.ProdutoCategoriaId
            };
        }

        public IEnumerable<DTOs.Output.Produto> List()
        {
            return this._produtoRepository.List().Select(s => new DTOs.Output.Produto
            {
                Id = s.Id.Value,
                Descricao = s.Descricao,
                Imagem = s.Imagem,
                Nome = s.Nome,
                Preco = s.Preco,
                ProdutoCategoriaId = (Enums.ProdutoCategoria)s.ProdutoCategoriaId
            });
        }

        public DTOs.Output.Produto Insert(DTOs.Imput.ProdutoInsert produto)
        {
            var entity = new Domain.Entities.Produto(null, produto.Nome, produto.Descricao, (Domain.Enums.ProdutoCategoria)produto.ProdutoCategoriaId, produto.Preco, produto.Imagem);
            var id = this._produtoRepository.Insert(entity);
            return this.Get(id);
        }

        public DTOs.Output.Produto Update(DTOs.Imput.ProdutoUpdate produto)
        {
            if (!produto.Id.HasValue)
                throw new CustomExceptions.ConflictException("Id do produto está vazio");

            var entity = new Domain.Entities.Produto(produto.Id, produto.Nome, produto.Descricao, (Domain.Enums.ProdutoCategoria)produto.ProdutoCategoriaId, produto.Preco, produto.Imagem);
            var sucess = this._produtoRepository.Update(entity);
            if(!sucess)
                throw new CustomExceptions.NotFoundException("Produto não encontrado");

            return this.Get(produto.Id.Value);
        }

        public IEnumerable<Produto> ListByCategory(ProdutoCategoria categoria)
        {
            return this._produtoRepository.ListByCategory(categoria).Select(s => new DTOs.Output.Produto
            {
                Id = s.Id.Value,
                Descricao = s.Descricao,
                Imagem = s.Imagem,
                Nome = s.Nome,
                Preco = s.Preco,
                ProdutoCategoriaId = (Enums.ProdutoCategoria)s.ProdutoCategoriaId
            });
        }
    }
}
