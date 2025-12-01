using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class SaleItem
    {
        [Key]
        public Guid Id { get; set; }

        //Relacionamento com a Venda (Pai)
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; }
        public Guid ProductVariationId { get; set; }
        public ProductVariation ProductVariation { get; set; }

        //Dados da Venda
        public int QuantidadeComprada { get; set; }
        public decimal PrecoUnitario { get; set; }
        public SaleItem(Guid saleId, Guid productVariationId, int quantidadeComprada, decimal precoUnitario)
        {
            this.Id = Guid.NewGuid();
            this.SaleId = saleId;
            this.ProductVariationId = productVariationId;
            this.QuantidadeComprada = quantidadeComprada;
            this.PrecoUnitario = precoUnitario;
        }
        private SaleItem() { }
    }
}