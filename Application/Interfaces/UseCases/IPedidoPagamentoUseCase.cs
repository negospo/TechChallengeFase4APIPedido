namespace Application.Interfaces.UseCases
{
    public interface IPedidoPagamentoUseCase
    {
        public DTOs.External.PagamentoService.PedidoPagamentoResponse Get(int pedidoId);
        public IEnumerable<DTOs.External.PagamentoService.PedidoPagamentoResponse> List(IEnumerable<int> pedidoIds);
        public bool Save(DTOs.External.PagamentoService.PedidoPagamentoRequest pedido);
    }
}
