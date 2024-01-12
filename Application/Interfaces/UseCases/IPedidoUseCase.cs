namespace Application.Interfaces.UseCases
{
    public interface IPedidoUseCase
    {
        public IEnumerable<DTOs.Output.Pedido> List();
        public IEnumerable<DTOs.Output.Pedido> ListByStatus(Enums.PedidoStatus status);
        public DTOs.Output.Pedido Get(int id);
        public DTOs.Output.PedidoIdentificador Order(DTOs.Imput.Pedido pedido);
        public bool UpdateOrderStatus(int id, Enums.PedidoStatus status);
        public Application.DTOs.Output.PedidoPagamento GetPaymentStatus (int idPedido);
    }
}
