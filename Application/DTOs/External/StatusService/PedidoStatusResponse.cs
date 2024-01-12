using System.Text.Json.Serialization;

namespace Application.DTOs.External.StatusService
{
    public class PedidoStatusResponse
    {
        public int PedidoId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<Enums.PedidoStatus>))]
        public Enums.PedidoStatus Status { get; set; }
    }
}
