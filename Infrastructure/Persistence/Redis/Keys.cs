using Application.Enums;

namespace Infrastructure.Persistence.Redis
{
    public abstract class Keys
    {
        public class Product
        {
            public static string Get(int id) => $"APIPedido:Cache:Product:{id}";
            public static string List => $"APIPedido:Cache:Product:List";
            public static string ListByIds(IEnumerable<int> ids) => $"APIPedido:Cache:Product:ListByIds:{String.Join("-",ids)}";
            public static string ListByCategory(ProdutoCategoria category) => $"APIPedido:Cache:Product:ListByCategory:{(int)category}";
        }
    }
}
