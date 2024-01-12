using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.Imput
{
    public class ProdutoInsert
    {
        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(500)]
        public string Descricao { get; set; }
        [Required]
        public Enums.ProdutoCategoria? ProdutoCategoriaId { get; set; }
        [Required]
        [Range(0.1,double.MaxValue)]
        public decimal Preco { get; set; }

        [MaxLength(1000)]
        public string Imagem { get; set; }
    }
}
