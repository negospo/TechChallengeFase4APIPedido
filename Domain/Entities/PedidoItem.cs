namespace Domain.Entities
{
    public class PedidoItem
    {
        public PedidoItem(int produtoId, int quantidade, decimal precoUnitario)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;

            this.Validate();
        }

        public void Validate()
        {
            if (this.Quantidade == 0 || this.Quantidade > 100)
                throw new CustomExceptions.BadRequestException("Quantidade deve ser entre 1 e 100");

            if (this.PrecoUnitario == 0)
                throw new CustomExceptions.BadRequestException("Preço unitário deve ser maior do que zero");
        }


        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
