using Application.DTOs.Output;
using Dapper;

namespace Infrastructure.Persistence.Repositories
{
    public class PedidoRepository : Application.Interfaces.Repositories.IPedidoRepository
    {
        public Application.DTOs.Output.Pedido Get(int id)
        {
            string queryOrder = @"select * from pedido where id = @id";
            string queryOrderItem = @"select a.pedido_id,a.produto_id,a.preco_unitario,a.quantidade,b.nome as nome_produto from pedido_item a
            inner join produto b on b.id = a.produto_id  
            where pedido_id = @produto_id";

            var order = Database.Connection().QueryFirstOrDefault<Application.DTOs.Output.Pedido>(queryOrder, new { id = id });
            if (order == null)
                return order;

            var orderItems = Database.Connection().Query<Application.DTOs.Output.PedidoItem>(queryOrderItem, new { pedido_id = order.Id });
            order.Itens = orderItems;

            return order;
        }
        public IEnumerable<Application.DTOs.Output.Pedido> List()
        {
            //string queryOrder = "select a.*,b.nome as pedido_status  from pedido a inner join pedido_status b on b.id = a.pedido_status_id where pedido_status_id <> 4 order by pedido_status_id desc, data";
            string queryOrder = "select * from pedido";


            string queryOrderItem = @"select a.pedido_id,a.produto_id,a.preco_unitario,a.quantidade,b.nome as nome_produto from pedido_item a
            inner join produto b on b.id = a.produto_id  
            where pedido_id = any(@ids)";

            var orders = Database.Connection().Query<Application.DTOs.Output.Pedido>(queryOrder);
            var orderItems = Database.Connection().Query<Application.DTOs.Output.PedidoItem>(queryOrderItem, new
            {
                ids = orders.Select(s => s.Id).ToList()
            });


            orders.ToList().ForEach(order =>
            {
                order.Itens = orderItems.Where(w => w.PedidoId == order.Id);
            });

            return orders;
        }
        public IEnumerable<Pedido> ListByIds(IEnumerable<int> pedidoIds)
        {
            string queryOrder = "select * from pedido where id = any(@ids)";

            string queryOrderItem = @"select a.pedido_id,a.produto_id,a.preco_unitario,a.quantidade,b.nome as nome_produto from pedido_item a
            inner join produto b on b.id = a.produto_id  
            where pedido_id = any(@ids)";

            var orders = Database.Connection().Query<Application.DTOs.Output.Pedido>(queryOrder, new { ids = pedidoIds.ToList() });
            var orderItems = Database.Connection().Query<Application.DTOs.Output.PedidoItem>(queryOrderItem, new
            {
                ids = orders.Select(s => s.Id).ToList()
            });


            orders.ToList().ForEach(order =>
            {
                order.Itens = orderItems.Where(w => w.PedidoId == order.Id);
            });

            return orders;
        }
        public int Order(Domain.Entities.Pedido pedido)
        {
            string queryOrder = @"insert into pedido 
                                (cliente_id,anonimo,anonimo_identificador,valor,cliente_observacao,data) 
                                values 
                                (@cliente_id,@anonimo,@anonimo_identificador,@valor,@cliente_observacao,now() AT TIME ZONE 'America/Sao_Paulo') RETURNING id";

            string queryOrderItem = @"insert into pedido_item 
                                    (pedido_id,produto_id,preco_unitario,quantidade) 
                                    values 
                                    (@pedido_id,@produto_id,@preco_unitario,@quantidade)";


            using (var connection = Database.Connection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    //Salva o pedido
                    int id = transaction.Connection.ExecuteScalar<int>(queryOrder, new
                    {
                        cliente_id = pedido.ClienteId,
                        anonimo = pedido.Anonimo,
                        anonimo_identificador = pedido.AnonimoIdentificador,
                        valor = pedido.Valor,
                        cliente_observacao = pedido.ClienteObservacao
                    });
                    //Cria a lista de itens do pedido
                    var orderItems = pedido.Itens.Select(item => new
                    {
                        pedido_id = id,
                        produto_id = item.ProdutoId,
                        preco_unitario = item.PrecoUnitario,
                        quantidade = item.Quantidade
                    }).ToList();
                    //Salva os itens do pedido
                    transaction.Connection.Execute(queryOrderItem, orderItems);
                    transaction.Commit();
                    connection.Close();
                    return id;
                }
            }
        }
    }
}
