using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class Vendor
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<Product> Produtos { get; set; }

        public Vendor(string nome, string email, string passwordHash)
        {
            this.Id = Guid.NewGuid();
            this.Nome = nome;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.Produtos = new List<Product>();
        }
        private Vendor() { }
    }
}