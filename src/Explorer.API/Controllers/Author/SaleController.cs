using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.API.Controllers.Author
{
    [Route("api/author/sales")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpPost]
        public ActionResult<SaleDto> Create([FromBody] CreateSaleDto dto, [FromQuery] long authorId)
        {
            try
            {
                var result = _saleService.Create(dto, authorId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{saleId}")]
        public ActionResult<SaleDto> Update(long saleId, [FromBody] UpdateSaleDto dto, [FromQuery] long authorId)
        {
            try
            {
                var result = _saleService.Update(saleId, dto, authorId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{saleId}")]
        public ActionResult Delete(long saleId, [FromQuery] long authorId)
        {
            try
            {
                _saleService.Delete(saleId, authorId);
                return Ok("Sale deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{saleId}")]
        public ActionResult<SaleDto> Get(long saleId)
        {
            try
            {
                var result = _saleService.Get(saleId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("by-author")]
        public ActionResult<List<SaleDto>> GetByAuthor([FromQuery] long authorId)
        {
            var result = _saleService.GetByAuthor(authorId);
            return Ok(result);
        }

        [HttpGet("active")]
        public ActionResult<List<SaleDto>> GetActiveSales()
        {
            var result = _saleService.GetActiveSales();
            return Ok(result);
        }

        [HttpGet("paged")]
        public ActionResult<PagedResult<SaleDto>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _saleService.GetPaged(page, pageSize);
            return Ok(result);
        }
    }
}
