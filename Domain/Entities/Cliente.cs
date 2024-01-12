namespace Domain.Entities
{
    public class Cliente
    {
        public Cliente(int? id, string nome, string email, string cpf)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Cpf = cpf;

            this.Validate();
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(this.Nome))
                throw new CustomExceptions.BadRequestException("Nome não pode ser vazio");

            if (!Validator.Helper.ValidateEmail(this.Email))
                throw new CustomExceptions.BadRequestException("Email inválido");

            if (!Validator.Helper.ValidateCPF(this.Cpf))
                throw new CustomExceptions.BadRequestException("CPF inválido");
        }

        public Cliente() { }

        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
    }
}
