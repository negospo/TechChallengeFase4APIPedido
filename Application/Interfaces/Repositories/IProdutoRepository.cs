namespace Application.Interfaces.Repositories
{
    public interface IProdutoRepository
    {
        public Domain.Entities.Produto Get(int id);
        public IEnumerable<Domain.Entities.Produto> List();
        public bool Delete(int id);
        public int Insert(Domain.Entities.Produto produto);
        public bool Update(Domain.Entities.Produto produto);
        public IEnumerable<Domain.Entities.Produto> ListByCategory(Enums.ProdutoCategoria categoria);
        public IEnumerable<Domain.Entities.Produto> ListByIds(List<int> ids);
    }
}
