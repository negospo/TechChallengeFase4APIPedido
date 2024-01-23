using API.Validation;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("Produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly Application.Interfaces.UseCases.IProdutoUseCase _produtoUseCase;

        public ProdutoController(Application.Interfaces.UseCases.IProdutoUseCase produtoUseCase)
        {
            this._produtoUseCase = produtoUseCase;
        }


        [HttpGet]
        [Route("redis/test")]
        [ProducesResponseType(typeof(bool), 200)]
        public ActionResult<bool> Get()
        {
            try
            {
                SkySoftware.Library.Redis.Connection.KeyExists("teste");
                return Ok(true);
            }
            catch (Application.CustomExceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>
        /// Exclui um produto pelo seu id
        /// </summary>
        /// <response code="404" >Produto não encontrado</response>
        [HttpDelete]
        [Route("{id}")]
        public ActionResult<bool> Delete(int id)
        {
            try
            {
                return Ok(this._produtoUseCase.Delete(id));
            }
            catch (Application.CustomExceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }



        /// <summary>
        /// Retorna um produto pelo seu id
        /// </summary>
        /// <param name="id">Id do produto</param>
        /// <response code="404" >Produto não encontrado</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Output.Produto), 200)]
        public ActionResult<Application.DTOs.Output.Produto> Get(int id)
        {
            try
            {
                return Ok(this._produtoUseCase.Get(id));
            }
            catch (Application.CustomExceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>
        /// Lista todos os produtos
        /// </summary>
        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(IEnumerable<Application.DTOs.Output.Produto>), 200)]
        public ActionResult<IEnumerable<Application.DTOs.Output.Produto>> List()
        {
            return Ok(this._produtoUseCase.List());
        }


        /// <summary>
        /// Lista todos os produtos de uma categoria
        /// </summary>
        [HttpGet]
        [Route("listByCategory")]
        [ProducesResponseType(typeof(IEnumerable<Application.DTOs.Output.Produto>), 200)]
        public ActionResult<IEnumerable<Application.DTOs.Output.Produto>> ListByCategory(Application.Enums.ProdutoCategoria categoria)
        {
            return Ok(this._produtoUseCase.ListByCategory(categoria));
        }


        /// <summary>
        /// Cria um novo produto
        /// </summary>
        [HttpPost]
        [Route("create")]
        [CustonValidateModel]
        [ProducesResponseType(typeof(Validation.CustonValidationResultModel), 400)]
        [ProducesResponseType(typeof(Application.DTOs.Output.Produto), 200)]
        public ActionResult<Application.DTOs.Output.Produto> Create(Application.DTOs.Imput.ProdutoInsert produto)
        {
            try
            {
                var result = _produtoUseCase.Insert(produto);
                return Ok(result);
            }
            catch (Application.CustomExceptions.ConflictException ex) 
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Altera um produto
        /// </summary>
        /// <response code="404" >Produto não encontrado</response>      
        [HttpPost]
        [Route("update")]
        [CustonValidateModel]
        [ProducesResponseType(typeof(Validation.CustonValidationResultModel), 400)]
        [ProducesResponseType(typeof(Application.DTOs.Output.Produto), 200)]
        public ActionResult<Application.DTOs.Output.Produto> Update(Application.DTOs.Imput.ProdutoUpdate cliente)
        {
            try
            {
                var result = _produtoUseCase.Update(cliente);
                return Ok(result);
            }
            catch (Application.CustomExceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Application.CustomExceptions.NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
