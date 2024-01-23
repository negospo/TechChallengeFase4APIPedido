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
            string cacheKey = Redis.Keys.Product.Get(id);
            if (SkySoftware.Library.Redis.Connection.KeyExists(cacheKey,throwOnError:false))
                return SkySoftware.Library.Redis.Connection.GetObject<Produto>(cacheKey, throwOnError: false);

            string query = "select * from produto where excluido = false and id = @id";
            var result =  Database.Connection().QueryFirstOrDefault<Produto>(query, new { id = id });

            SkySoftware.Library.Redis.Connection.SetObject(cacheKey, result, TimeSpan.FromMinutes(1),throwOnError:false);
            return result;
        }

        public IEnumerable<Produto> List()
        {
            string cacheKey = Redis.Keys.Product.List;
            if (SkySoftware.Library.Redis.Connection.KeyExists(cacheKey, throwOnError: false))
                return SkySoftware.Library.Redis.Connection.GetObject<IEnumerable<Produto>>(cacheKey, throwOnError: false);

            string query = "select * from produto where excluido = false";
            var result =  Database.Connection().Query<Produto>(query);

            SkySoftware.Library.Redis.Connection.SetObject(cacheKey, result, TimeSpan.FromMinutes(1), throwOnError: false);
            return result;
        }

        public IEnumerable<Produto> ListByCategory(ProdutoCategoria categoria)
        {
            string cacheKey = Redis.Keys.Product.ListByCategory(categoria);
            if (SkySoftware.Library.Redis.Connection.KeyExists(cacheKey, throwOnError: false))
                return SkySoftware.Library.Redis.Connection.GetObject<IEnumerable<Produto>>(cacheKey, throwOnError: false);

            string query = "select * from produto where excluido = false and produto_categoria_id = @produto_categoria_id";
            var result = Database.Connection().Query<Produto>(query, new
            {
                produto_categoria_id = categoria
            });

            SkySoftware.Library.Redis.Connection.SetObject(cacheKey, result, TimeSpan.FromMinutes(1), throwOnError: false);
            return result;
        }

        public IEnumerable<Produto> ListByIds(List<int> ids)
        {
            string cacheKey = Redis.Keys.Product.ListByIds(ids);
            if (SkySoftware.Library.Redis.Connection.KeyExists(cacheKey, throwOnError: false))
                return SkySoftware.Library.Redis.Connection.GetObject<IEnumerable<Produto>>(cacheKey, throwOnError: false);


            string query = "select * from produto where excluido = false and id = ANY(@ids)";
            var result = Database.Connection().Query<Produto>(query, new
            {
                ids = ids
            });

            SkySoftware.Library.Redis.Connection.SetObject(cacheKey, result, TimeSpan.FromMinutes(1), throwOnError: false);
            return result;
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
