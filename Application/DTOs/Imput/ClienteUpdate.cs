using Application.DTOs.Validation;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Imput
{
    public class ClienteUpdate
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [CPFAttribute]
        public string Cpf { get; set; }
    }
}
