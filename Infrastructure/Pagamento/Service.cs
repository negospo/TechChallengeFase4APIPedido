using Application.DTOs.External.PagamentoService;
using Infrastructure.Persistence;
using RestSharp;

namespace Infrastructure.Pagamento
{
    public class Service : Application.Interfaces.UseCases.IPedidoPagamentoUseCase
    {
        public PedidoPagamentoResponse Get(int pedidoId)
        {
            var client = new RestClient(Settings.APIPagamentoPath);
            var request = new RestRequest($"Pedido/{pedidoId}", Method.Get);
            var response = client.Execute<Application.DTOs.External.PagamentoService.PedidoPagamentoResponse>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public IEnumerable<PedidoPagamentoResponse> List(IEnumerable<int> pedidoIds)
        {
            var client = new RestClient(Settings.APIPagamentoPath);
            var request = new RestRequest("Pedido/list", Method.Post);
            request.AddJsonBody(pedidoIds);
            var response = client.Execute<List<Application.DTOs.External.PagamentoService.PedidoPagamentoResponse>>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

        public bool Save(PedidoPagamentoRequest pedido)
        {
            var client = new RestClient(Settings.APIPagamentoPath);
            var request = new RestRequest("Pedido/save", Method.Post);
            request.AddJsonBody(pedido);
            var response = client.Execute<bool>(request);
            if (!response.IsSuccessful)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }
    }
}
