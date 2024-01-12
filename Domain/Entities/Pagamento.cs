namespace Domain.Entities
{
    public class Pagamento
    {
        public Pagamento(Enums.TipoPagamento tipoPagamentoId, string nome, string tokenCartao, decimal valor)
        {
            TipoPagamentoId = tipoPagamentoId;
            Nome = nome;
            TokenCartao = tokenCartao;
            Valor = valor;

            this.Validate();
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(this.Nome))
                throw new CustomExceptions.BadRequestException("Nome não pode ser vazio");

            if (string.IsNullOrEmpty(this.Nome))
                throw new CustomExceptions.BadRequestException("Token do cartão vazio");

            if (this.Valor == 0)
                throw new CustomExceptions.BadRequestException("O Valor deve ser maior do que zero");
        }

        public Enums.TipoPagamento TipoPagamentoId { get; set; }

        public string Nome { get; set; }

        public string TokenCartao { get; set; }

        public decimal Valor { get; set; }

        public Enums.PagamentoStatus? PagamentoStatus { get; set; }

        public string CodigoTransacao { get; set; }

        public void AtualizaCodigoTransacao(string codigo)
        { 
            this.CodigoTransacao = codigo;
        }
        public void AtualizaStatusPagamento(Enums.PagamentoStatus status)
        {
            this.PagamentoStatus = status;
        }

    }
}
