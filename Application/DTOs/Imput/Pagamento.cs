using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Imput
{
    public class Pagamento
    {
        [Required]
        public Enums.TipoPagamento TipoPagamento { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(50)]
        public string TokenCartao { get; set; }
    }
}
