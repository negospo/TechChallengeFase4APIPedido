using Application.Enums;
using Dapper;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class ProdutoRepository : Application.Interfaces.Repositories.IProdutoRepository
    {
        public bool Delete(int id)
        {
            string query = "update produto set excluido = true where id = @id";
            int affected = Database.Connection().Execute(query, new { id = id });
            return (affected > 0);
        }

        public Produto Get(int id)
        {
            string query = "select * from produto where excluido = false and id = @id";
            return Database.Connection().QueryFirstOrDefault<Produto>(query, new { id = id });
        }

        public IEnumerable<Produto> List()
        {
            string query = "select * from produto where excluido = false";
            return Database.Connection().Query<Produto>(query);
        }

        public IEnumerable<Produto> ListByCategory(ProdutoCategoria categoria)
        {
            string query = "select * from produto where excluido = false and produto_categoria_id = @produto_categoria_id";
            return Database.Connection().Query<Produto>(query, new
            {
                produto_categoria_id = categoria
            });
        }

        public IEnumerable<Produto> ListByIds(List<int> ids)
        {
            string query = "select * from produto where excluido = false and id = ANY(@ids)";
            return Database.Connection().Query<Produto>(query, new
            {
                ids = ids
            });
        }

        public int Insert(Produto produto)
        {
            string query = "insert into produto (nome,descricao,produto_categoria_id,preco,imagem) values (@nome,@descricao,@produto_categoria_id,@preco,@imagem) returning id";
            int id = Database.Connection().ExecuteScalar<int>(query, new
            {
                nome = produto.Nome.Trim(),
                descricao = produto.Descricao.Trim(),
                produto_categoria_id = produto.ProdutoCategoriaId,
                preco = produto.Preco,
                imagem = produto.Imagem
            });

            return id;
        }

        public bool Update(Produto produto)
        {
            string query = "update produto set nome = @nome,descricao=@descricao, produto_categoria_id = @produto_categoria_id, preco = @preco,imagem = @imagem where id = @id";
            int affected = Database.Connection().Execute(query, new
            {
                id = produto.Id,
                nome = produto.Nome.Trim(),
                descricao = produto.Descricao.Trim(),
                produto_categoria_id = produto.ProdutoCategoriaId,
                preco = produto.Preco,
                imagem = produto.Imagem
            });

            return (affected > 0);
        }
    }
}
