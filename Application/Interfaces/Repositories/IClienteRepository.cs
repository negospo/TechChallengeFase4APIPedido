namespace Application.Interfaces.Repositories
{
    public interface IClienteRepository
    {
        public Domain.Entities.Cliente Get(int id);
        public IEnumerable<Domain.Entities.Cliente> List();
        public bool Delete(int id);
        public int Insert(Domain.Entities.Cliente cliente);
        public bool Update(Domain.Entities.Cliente cliente);
        public Domain.Entities.Cliente GetByCpf(string cpf);
        public Domain.Entities.Cliente GetByEmail(string email);
    }
}
