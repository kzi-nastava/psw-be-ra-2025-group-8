using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.API.Controllers.Tourist
{
    //[Authorize(Policy = "shoppingCartPolicy")]
    [Route("api/tours/cart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITourService _tourService;

        public ShoppingCartController(IShoppingCartService shoppingCartService, ITourService tourService)
        {
            _shoppingCartService = shoppingCartService;
            _tourService = tourService;
        }

        [HttpGet()]
        public ActionResult<ShoppingCartDto> GetCart([FromQuery] long userId)
        {
            try
            {
                var cart = _shoppingCartService.GetCart(userId);
                return Ok(cart);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("new")]
        public ActionResult NewCart([FromQuery] long userId)
        {
            try
            {
                var cart = _shoppingCartService.CreateCart(userId);
                return Ok("New cart created.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("items")]
        public IActionResult AddItem([FromQuery] long userId,[FromQuery] long tourId)
        {
            try
            {
                var cart = _shoppingCartService.GetCart(userId);
                var tour = _tourService.GetById(tourId);

                if (tour == null) return NotFound("Tour not found.");
                var dto = new OrderItemDto
                {
                    TourId = tourId
                };
                _shoppingCartService.AddItem(userId, dto);
                return Ok("Item added to cart.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("remove")]
        public IActionResult RemoveItem([FromQuery] long userId, [FromQuery] long tourId)
        {
            try
            {
                var cart = _shoppingCartService.GetCart(userId);
                _shoppingCartService.RemoveItem(userId, tourId);
                return Ok("Item removed from cart.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("clear")]
        public IActionResult ClearCart([FromQuery] long userId)
        {
            try
            {
                var cart = _shoppingCartService.GetCart(userId);
                _shoppingCartService.ClearCart(userId);
                return Ok("Cart cleared.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("delete")]
        public IActionResult DeleteCart([FromQuery] long userId)
        {
            try
            {
                _shoppingCartService.DeleteCart(userId);
                return Ok("Cart deleted.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("purchase/item")]
        public IActionResult PurchaseItem([FromQuery] long userId, [FromQuery] long tourId)
        {
            try
            {
                _shoppingCartService.PurchaseItem(userId, tourId);
                return Ok("Item purchased successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("purchase/all")]
        public IActionResult PurchaseAllItems([FromQuery] long userId)
        {
            try
            {
                _shoppingCartService.PurchaseAllItems(userId);
                return Ok("All items purchased successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
