namespace Application.Interfaces.UseCases
{
    public interface IProdutoUseCase
    {
        public DTOs.Output.Produto Get(int id);
        public IEnumerable<DTOs.Output.Produto> List();
        public IEnumerable<DTOs.Output.Produto> ListByCategory(Enums.ProdutoCategoria categoria);
        public bool Delete(int id);
        public DTOs.Output.Produto Insert(DTOs.Imput.ProdutoInsert produto);
        public DTOs.Output.Produto Update(DTOs.Imput.ProdutoUpdate produto);
    }
}
