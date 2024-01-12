namespace Application.DTOs.External.PagamentoService
{
    public class PagamentoRequest
    {
        public Enums.TipoPagamento TipoPagamento { get; set; }

        public string Nome { get; set; }

        public string TokenCartao { get; set; }

        public decimal Valor { get; set; }
    }
}
