using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class Resposta
    {
        [Key]
        public Guid Id { get; set; }
        public Vendor Conta { get; set; }
        public Guid PerguntaId { get; set; }
        public Pergunta Pergunta { get; set; }
        public string Conteudo { get; set; }
        public DateTime CriadaEm { get; set; }

        public Resposta(Vendor conta, DateTime criadaEm, string conteudo, Pergunta pergunta)
        {
            this.Id = Guid.NewGuid();
            this.Conta = conta;
            this.CriadaEm = criadaEm;
            this.Conteudo = conteudo;
            this.Pergunta = pergunta;
            this.PerguntaId = pergunta.Id;
        }
        private Resposta() { }
    }
}
