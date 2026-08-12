using GeekShopping.CartAPI.Data.DTOs;
using GeekShopping.CartAPI.Data.Messages;
using GeekShopping.CartAPI.RabbitMQ.Sender.Interface;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling shopping cart-related operations.
    /// Provides endpoints for managing user shopping carts, including adding items, updating carts, applying coupons, and managing the checkout process.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartRepository cartRepository, ICouponRepository couponRepository, IRabbitMqMessageSender rabbitMqMessageSender) : ControllerBase
    {
        private readonly ICartRepository _cartRepository = cartRepository
                                                           ?? throw new ArgumentNullException(nameof(cartRepository));
        private readonly ICouponRepository _couponRepository = couponRepository
                                                               ?? throw new ArgumentNullException(nameof(couponRepository));
        private readonly IRabbitMqMessageSender _rabbitMqMessageSender = rabbitMqMessageSender
                                                                         ?? throw new ArgumentNullException(nameof(rabbitMqMessageSender));

        // Methods:
        //? GET:
        /// <summary>
        /// Retrieves the shopping cart associated with the specified user identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart is to be retrieved.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a <see cref="GeekShopping.CartAPI.Data.DTOs.CartDto"/> object that represents the user's shopping cart, or an
        /// appropriate HTTP status code if the operation fails.
        /// </returns>
        [HttpGet("FindCartByUserId/{userId:guid}")]
        public async Task<ActionResult<CartDto>> FindCartByUserId(Guid userId) => Ok(await _cartRepository.FindCartByUserId(userId));

        //? POST:
        /// <summary>
        /// Adds items to a user's shopping cart or updates the cart if it already exists.
        /// </summary>
        /// <param name="cartDto">The data transfer object representing the shopping cart with item details and header information.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="GeekShopping.CartAPI.Data.DTOs.CartDto"/> object representing the
        /// updated or newly created shopping cart.
        /// </returns>
        [HttpPost("AddItemToCart")]
        public async Task<ActionResult<CartDto>> AddItemToCart(CartDto cartDto) => Ok(await _cartRepository.SaveOrUpdateCart(cartDto));

        /// <summary>
        /// Applies a coupon to the user's shopping cart.
        /// </summary>
        /// <param name="couponDto">An object containing the user identifier and the coupon code to be applied to the cart.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a boolean value indicating whether the coupon was successfully applied, or an appropriate HTTP status code if
        /// the operation fails.
        /// </returns>
        [HttpPost("ApplyCoupon")]
        public async Task<ActionResult<CartDto>> ApplyCoupon(ApplyCouponDto couponDto)
        {
            bool status = await _cartRepository.ApplyCoupon(couponDto.UserId, couponDto.CouponCode);

            if(!status) return NotFound();

            return Ok(status);
        }

        /// <summary>
        /// Processes the checkout operation for the provided checkout header information.
        /// </summary>
        /// <param name="checkoutHeaderDto">
        /// An instance of <see cref="Data.Messages.CheckoutHeaderDto"/> containing the details of the checkout process, including user ID, coupon code,
        /// and cart details.
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the updated <see cref="Data.Messages.CheckoutHeaderDto"/> after processing the checkout, or an
        /// appropriate HTTP status code if the operation fails.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if the coupon associated with the provided coupon code is not found.</exception>
        /// <remarks>StatusCode 412 (Precondition Failed) -> the client indicated, in its headers, preconditions that the server does not meet.</remarks>
        [HttpPost("Checkout")]
        public async Task<ActionResult<CheckoutHeaderDto>> Checkout(CheckoutHeaderDto checkoutHeaderDto)
        {
            string token = Request.Headers["Authorization"]!;

            if((checkoutHeaderDto?.UserId == Guid.Empty) || (checkoutHeaderDto?.UserId is null)) return BadRequest();

            CartDto cart = await _cartRepository.FindCartByUserId(checkoutHeaderDto.UserId);

            if(!string.IsNullOrEmpty(checkoutHeaderDto.CouponCode))
            {
                CouponDto? couponDto = await _couponRepository.GetCouponByCouponCode(checkoutHeaderDto.CouponCode, token);

                if(couponDto is null) throw new ArgumentNullException(nameof(couponDto));
                if(!couponDto.DiscountAmount.Equals(couponDto.DiscountAmount)) return StatusCode(412);  //? Precondition failed.
            }

            checkoutHeaderDto.CartDetails = cart.CartDetails;
            checkoutHeaderDto.DateTime = DateTime.Now;

           
            await _rabbitMqMessageSender.SendMessage(checkoutHeaderDto, "checkoutQueue");  //? Implements RabbitMQ.
            await _cartRepository.ClearCart(checkoutHeaderDto.UserId);

            return Ok(checkoutHeaderDto);
        }

        //? PATCH:
        /// <summary>
        /// Updates the shopping cart with the provided cart data and returns the updated cart information.
        /// </summary>
        /// <param name="cartDto">The data transfer object representing the shopping cart, including header and item details.</param>
        /// <returns>An asynchronous operation that returns an <see cref="ActionResult{CartDto}"/> containing the updated shopping cart data.</returns>
        [HttpPatch("UpdateCart")]
        public async Task<ActionResult<CartDto>> UpdateCart(CartDto cartDto) => Ok(await _cartRepository.SaveOrUpdateCart(cartDto));

        //? DELETE:
        /// <summary>
        /// Removes an item from the shopping cart based on the specified cart details identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the cart item to be removed.</param>
        /// <returns>An action result indicating the outcome of the remove operation.</returns>
        [HttpDelete("RemoveFromCart/{id:guid}")]
        public async Task<ActionResult> RemoveFromCart(Guid id) => Ok(await _cartRepository.RemoveFromCart(id));

        /// <summary>
        /// Removes the applied coupon for the shopping cart associated with the specified user identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose applied coupon is to be removed.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> indicating the success or failure of the operation.
        /// Returns a boolean value wrapped in an HTTP response: true if the coupon is successfully removed, otherwise an appropriate HTTP status code if the operation fails.
        /// </returns>
        [HttpDelete("RemoveCoupon/{userId:guid}")]
        public async Task<ActionResult<CartDto>> RemoveCoupon(Guid userId)
        {
            bool status = await _cartRepository.RemoveCoupon(userId);

            if(!status) return NotFound();

            return Ok(status);
        }
    }
}