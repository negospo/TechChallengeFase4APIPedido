using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTOs.Imput
{
    public class Pedido
    {
        /// <summary>
        /// Id do cliente. Deixar null caso queira que o pedido seja feito em modo anônimo
        /// </summary>
        [JsonIgnore]
        public int? ClienteId { get; set; }
        /// <summary>
        /// Observação do cliente
        /// </summary>
        public string ClienteObservacao { get; set; } = string.Empty;

        /// <summary>
        /// Itens do pedido
        /// </summary>
        [Required]
        public IEnumerable<DTOs.Imput.PedidoItem> Itens { get; set; }

        /// <summary>
        /// Dados do pagamento
        /// </summary>
        [Required]
        public DTOs.Imput.Pagamento Pagamento { get; set; }
    }
}
