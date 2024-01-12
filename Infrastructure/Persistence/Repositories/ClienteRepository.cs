using Dapper;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class ClienteRepository : Application.Interfaces.Repositories.IClienteRepository
    {
        public bool Delete(int id)
        {
            string query = "update cliente set excluido = true where id = @id";
            int affected = Database.Connection().Execute(query, new { id = id });
            return (affected > 0);
        }

        public Cliente Get(int id)
        {
            string query = "select * from cliente where excluido = false and id = @id";
            return Database.Connection().QueryFirstOrDefault<Cliente>(query, new { id = id });
        }

        public Cliente GetByCpf(string cpf)
        {
            string query = "select * from cliente where excluido = false and cpf = @cpf";
            return Database.Connection().QueryFirstOrDefault<Cliente>(query, new { cpf = cpf });
        }

        public Cliente GetByEmail(string email)
        {
            string query = "select * from cliente where excluido = false and email = @email";
            return Database.Connection().QueryFirstOrDefault<Cliente>(query, new { email = email });
        }

        public int Insert(Cliente cliente)
        {
            string query = "insert into cliente (nome,email,cpf) values (@nome,@email,@cpf) returning id";
            int id = Database.Connection().ExecuteScalar<int>(query, new
            {
                nome = cliente.Nome.Trim(),
                email = cliente.Email.Trim(),
                cpf = cliente.Cpf.Trim()
            });

            return id;
        }

        public IEnumerable<Cliente> List()
        {
            string query = "select * from cliente where excluido = false";
            return Database.Connection().Query<Cliente>(query);
        }

        public bool Update(Cliente cliente)
        {
            string query = "update cliente set nome = @nome,email = @email, cpf = @cpf where id = @id";
            int affected = Database.Connection().Execute(query, new
            {
                id = cliente.Id,
                nome = cliente.Nome.Trim(),
                email = cliente.Email.Trim(),
                cpf = cliente.Cpf.Trim()
            });

            return affected > 0;
        }
    }
}
