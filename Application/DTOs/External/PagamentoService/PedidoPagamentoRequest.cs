namespace Application.DTOs.External.PagamentoService
{
    public class PedidoPagamentoRequest
    {
        public int? PedidoId { get; set; }

        public PagamentoRequest Pagamento { get; set; }
    }
}
