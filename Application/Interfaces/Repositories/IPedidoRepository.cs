using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IPedidoRepository
    {
        public IEnumerable<DTOs.Output.Pedido> List();
        public IEnumerable<DTOs.Output.Pedido> ListByIds(IEnumerable<int> pedidoIds);
        public DTOs.Output.Pedido Get(int id);
        public int Order(Pedido pedido);
    }
}
