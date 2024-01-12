namespace Application.DTOs.Output
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int? ClienteId { get; set; }
        public string ClienteNome { get; set; }
        public bool Anonimo { get; set; }
        public string? AnonimoIdentificador { get; set; }
        public string PedidoStatus { get; set; }
        public decimal Valor { get; set; }
        public string ClienteObservacao { get; set; }
        public IEnumerable<DTOs.Output.PedidoItem> Itens { get; set; }
        public Output.Pagamento Pagamento { get; set; }
    }
}
