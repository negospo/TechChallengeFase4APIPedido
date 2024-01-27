using API.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("Pedido")]
    public class PedidoController : ControllerBase
    {
        private readonly Application.Interfaces.UseCases.IPedidoUseCase _pedidoUseCase;

        public PedidoController(Application.Interfaces.UseCases.IPedidoUseCase pedidoUseCase)
        {
            this._pedidoUseCase = pedidoUseCase;
        }

        /// <summary>
        /// Retorna status 200 se o usuário estiver autenticado
        /// </summary>
        /// <response code="200" >Autenticado</response>
        /// <response code="401" >Não autorizado</response>
        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public ActionResult Authenticated()
        {
            int? authenticatedUserId = this.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier") == null ? null : (int?)Convert.ToInt32(this.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));

            return Ok($"Autenticado como: {this.User.Identity.Name}");
        }


        /// <summary>
        /// Lista todos os pedidos ordenados em Pronto > Em Preparação > Recebido e Data do Pedido. Pedidos Finalizados não são exibidos neste end-point
        /// </summary>
        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(IEnumerable<Application.DTOs.Output.Pedido>), 200)]
        public ActionResult<IEnumerable<Application.DTOs.Output.Pedido>> List()
        {
            return Ok(_pedidoUseCase.List());
        }

        /// <summary>
        /// Lista todos os pedidos de um status
        /// </summary>
        /// <param name="status">Status do pedido</param>
        [HttpGet]
        [Route("listbystatus")]
        [ProducesResponseType(typeof(IEnumerable<Application.DTOs.Output.Pedido>), 200)]
        public ActionResult<IEnumerable<Application.DTOs.Output.Pedido>> ListByStatus(Application.Enums.PedidoStatus status)
        {
            return Ok(_pedidoUseCase.ListByStatus(status));
        }

        /// <summary>
        /// Cria um novo pedido. Deixe o ClienteId null ou 0 para fazer o pedido em modo anônimo.
        /// </summary>
        /// <param name="pedido">Dados do pedido</param>
        /// <response code="400" >Dados de cliente ou produtos inválidos</response>
        /// <response code="401" >Não autorizado</response>
        [HttpPost]
        [Route("order")]
        [CustonValidateModel]
        [ProducesResponseType(typeof(Validation.CustonValidationResultModel), 400)]
        [Authorize]
        public ActionResult<Application.DTOs.Output.PedidoIdentificador> CreateOrder(Application.DTOs.Imput.Pedido pedido)
        {
            try
            {
                //Pega o id do cliente
                if (!string.IsNullOrEmpty(this.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")))
                    pedido.ClienteId = Convert.ToInt32(this.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));

                var sucess = _pedidoUseCase.Order(pedido);
                return Ok(sucess);
            }
            catch (Application.CustomExceptions.BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Altera o status de um pedido
        /// </summary>
        /// <param name="id">Id do pedido</param>
        /// <param name="status">Status do pedido</param>
        /// <response code="404" >Pedido não encontrado</response>
        [HttpPost]
        [Route("{id}/status/update")]
        public ActionResult<bool> UpdateOrderStatus(int id, Application.Enums.PedidoStatus status)
        {
            try
            {
                return _pedidoUseCase.UpdateOrderStatus(id, status);
            }
            catch (Application.CustomExceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>
        /// Retorna o status de pagamento de um pedido
        /// </summary>
        /// <param name="pedidoId">Id do pedido</param>
        /// <response code="404" >Pedido não encontrado</response>  
        [HttpGet]
        [Route("GetPaymentStatus")]
        [ProducesResponseType(typeof(Application.DTOs.Output.PedidoPagamento), 200)]
        public ActionResult<Application.DTOs.Output.PedidoPagamento> GetPaymentStatus(int pedidoId)
        {
            try
            {
                return Ok(_pedidoUseCase.GetPaymentStatus(pedidoId));
            }
            catch (Application.CustomExceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
