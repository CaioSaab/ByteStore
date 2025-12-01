using ByteStoreAPI.Entity;

public enum OrderStatus
{
    Preparando = 0,
    Enviado = 1
}

public class Sale
{
    public Guid Id { get; set; }
    public Guid CompradorId { get; set; }
    public Client Comprador { get; set; }
    public decimal PrecoTotal { get; set; }
    public DateTime DataDaCompra { get; set; }
    public List<SaleItem> Itens { get; set; }
    public string PaymentMethod { get; set; }
    public OrderStatus Status { get; set; }

    public Sale(Guid compradorId, decimal precoTotal)
    {
        Id = Guid.NewGuid();
        CompradorId = compradorId;
        PrecoTotal = precoTotal;
        DataDaCompra = DateTime.UtcNow;
        Itens = new List<SaleItem>();
        PaymentMethod = "";
        Status = OrderStatus.Preparando;
    }
}