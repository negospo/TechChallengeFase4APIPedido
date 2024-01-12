using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Imput
{
    public class PedidoStatusUpdate
    {
        [Required]
        public Enums.PedidoStatus? Status { get; set; }
    }
}
