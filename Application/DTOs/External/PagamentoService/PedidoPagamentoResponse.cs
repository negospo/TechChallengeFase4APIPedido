using System.Text.Json.Serialization;

namespace Application.DTOs.External.PagamentoService
{
    public class PedidoPagamentoResponse
    {
        public int PedidoId { get; set; }

        public decimal Valor { get; set; }

        public string CodigoTransacao { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<Enums.TipoPagamento>))]
        public Enums.TipoPagamento TipoPagamento { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter<Enums.PagamentoStatus>))]
        public Enums.PagamentoStatus PagamentoStatus { get; set; }
    }
}
