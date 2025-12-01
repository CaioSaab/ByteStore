using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class Client
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Client(string nome, string email, string passwordHash)
        {
            this.Id = Guid.NewGuid();
            this.Nome = nome;
            this.Email = email;
            this.PasswordHash = passwordHash;
        }
        private Client() { }
    }
}