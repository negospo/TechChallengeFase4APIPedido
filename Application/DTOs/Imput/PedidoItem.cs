using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Imput
{
    public class PedidoItem
    {
        [Required]
        public int? ProdutoId { get; set; }

        [Required]
        [Range(1,100)]
        public int? Quantidade { get; set; }
    }
}
