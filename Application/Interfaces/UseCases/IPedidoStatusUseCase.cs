namespace Application.Interfaces.UseCases
{
    public interface IPedidoStatusUseCase
    {
        public DTOs.External.StatusService.PedidoStatusResponse Get(int pedidoId);
        public IEnumerable<DTOs.External.StatusService.PedidoStatusResponse> List(IEnumerable<int> pedidoIds);
        public IEnumerable<DTOs.External.StatusService.PedidoStatusResponse> ListByStatus(Enums.PedidoStatus status);
        public bool Save(DTOs.External.StatusService.PedidoStatusRequest pedido);
        public bool Update(int pedidoId, Enums.PedidoStatus status);
    }
}
