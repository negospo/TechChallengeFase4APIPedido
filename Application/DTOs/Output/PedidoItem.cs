using System.Text.Json.Serialization;

namespace Application.DTOs.Output
{
    public class PedidoItem
    {
        [JsonIgnore]
        public int PedidoId { get; set; }
        [JsonIgnore]
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
