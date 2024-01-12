using Domain.Enums;

namespace Domain.Entities
{
    public class Produto
    {
        public Produto(int? id, string nome, string descricao, ProdutoCategoria produtoCategoriaId, decimal preco, string imagem)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            ProdutoCategoriaId = produtoCategoriaId;
            Preco = preco;
            Imagem = imagem;

            this.Validate();
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(this.Nome))
                throw new CustomExceptions.BadRequestException("Nome não pode ser vazio");

            if (this.Preco == 0)
                throw new CustomExceptions.BadRequestException("Preço deve ser maior do que zero");
        }

        public Produto() { }

        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public Enums.ProdutoCategoria ProdutoCategoriaId { get; set; }
        public decimal Preco { get; set; }
        public string Imagem { get; set; }


    }
}
