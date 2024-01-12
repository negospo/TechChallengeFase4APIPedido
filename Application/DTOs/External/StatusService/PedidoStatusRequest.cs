namespace Application.DTOs.External.StatusService
{
    public class PedidoStatusRequest
    {
        public int PedidoId { get; set; }

        public Enums.PedidoStatus Status { get; set; }
    }
}
