namespace Application.Implementations
{
    public class ClienteUseCase : Interfaces.UseCases.IClienteUseCase
    {
        private readonly Interfaces.Repositories.IClienteRepository _clienteRepository;

        public ClienteUseCase(Interfaces.Repositories.IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public bool Delete(int id)
        {
            var result = this._clienteRepository.Delete(id);
            if(!result)
                throw new CustomExceptions.NotFoundException("Cliente não encontrado");

            return result;
        }

        public DTOs.Output.Cliente Get(int id)
        {
            var result = this._clienteRepository.Get(id);
            if (result == null)
                throw new CustomExceptions.NotFoundException("Cliente não encontrado");

            return new DTOs.Output.Cliente
            {
                Id = result.Id.Value,
                Cpf = result.Cpf,
                Email = result.Email,
                Nome = result.Nome
            };
        }

        public DTOs.Output.Cliente GetByCpf(string cpf)
        {
            var result =  this._clienteRepository.GetByCpf(cpf);
            if (result == null)
                throw new CustomExceptions.NotFoundException("Cliente não encontrado");

            return new DTOs.Output.Cliente
            {
                Id = result.Id.Value,
                Cpf = result.Cpf,
                Email = result.Email,
                Nome = result.Nome
            };
        }

        public IEnumerable<DTOs.Output.Cliente> List()
        {
            return this._clienteRepository.List().Select(s => new DTOs.Output.Cliente
            {
                Id = s.Id.Value,
                Cpf = s.Cpf,
                Email = s.Email,
                Nome = s.Nome
            });
        }

        public DTOs.Output.Cliente Insert(DTOs.Imput.ClienteInsert cliente)
        {
            var existsByCpf = this._clienteRepository.GetByCpf(cliente.Cpf);
            if (existsByCpf != null)
                throw new CustomExceptions.ConflictException("Ja existe outro usuário com esse cpf");

            var existsByEmail = this._clienteRepository.GetByEmail(cliente.Email);
            if (existsByEmail != null)
                throw new CustomExceptions.ConflictException("Ja existe outro usuário com esse email");
            
           
            var entity = new Domain.Entities.Cliente(null, cliente.Nome, cliente.Email, cliente.Cpf);
            
            var id = this._clienteRepository.Insert(entity);
            return this.Get(id);
        }

        public DTOs.Output.Cliente Update(DTOs.Imput.ClienteUpdate cliente)
        {
            if (!cliente.Id.HasValue)
                throw new CustomExceptions.ConflictException("Id do cliente está vazio");

            var existsByCpf = this._clienteRepository.GetByCpf(cliente.Cpf);
            if (existsByCpf != null && existsByCpf.Id != cliente.Id)
                throw new CustomExceptions.ConflictException("Ja existe outro usuário com esse cpf");

            var existsByEmail = this._clienteRepository.GetByEmail(cliente.Email);
            if (existsByEmail != null && existsByEmail.Id != cliente.Id)
                throw new CustomExceptions.ConflictException("Ja existe outro usuário com esse email");
            
            var entity = new Domain.Entities.Cliente(cliente.Id, cliente.Nome, cliente.Email, cliente.Cpf);

            var sucess = this._clienteRepository.Update(entity);
            if(!sucess)
                throw new CustomExceptions.NotFoundException("Cliente não encontrado");

            return this.Get(cliente.Id.Value);
        }
    }
}
