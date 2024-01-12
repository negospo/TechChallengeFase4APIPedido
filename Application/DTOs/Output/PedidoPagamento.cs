namespace Application.DTOs.Output
{
    public class PedidoPagamento
    {
        public int PedidoId { get; set; }

        public Enums.PagamentoStatus? StatusPagamento { get; set; }
    }
}
