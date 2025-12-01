using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class Pergunta
    {
        [Key]
        public Guid Id { get; set; }
        public Client Conta { get; set; }
        public Product Produto { get; set; }
        public string Conteudo { get; set; }
        public DateTime CriadaEm { get; set; }
        public Resposta? Resposta { get; set; }

        public Pergunta(Client conta, DateTime criadaEm, string conteudo, Product produto)
        {
            this.Id = Guid.NewGuid();
            this.Conta = conta;
            this.CriadaEm = criadaEm;
            this.Conteudo = conteudo;
            this.Produto = produto;
        }
        private Pergunta() { }
    }
}
