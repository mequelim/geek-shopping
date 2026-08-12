using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class CartController(ICartService cartService, ICouponService couponService) : Controller
    {
        private readonly ICartService _cartService = cartService
                                                     ?? throw new ArgumentNullException(nameof(cartService));
        private readonly ICouponService _couponService = couponService
                                                         ?? throw new ArgumentNullException(nameof(couponService));

        private async Task<CartViewModel?> FindUserCart()
        {
            string? token = await HttpContext.GetTokenAsync("access_token");
            string? userIdClaim = User.Claims
                .FirstOrDefault((u) => u.Type.Equals("sub"))
                ?.Value;

            if((userIdClaim is null) || (!Guid.TryParse(userIdClaim, out Guid userId))) return null;

            CartViewModel response = await _cartService.FindCartByUserId(userId, token!);

            if(response?.CartHeader is null) return null;

            if(!string.IsNullOrEmpty(response.CartHeader.CouponCode))
            {
                CouponViewModel? coupon = null;

                try
                {
                    coupon = await _couponService.GetCoupon(response.CartHeader.CouponCode, token!);
                }
                catch(HttpRequestException exception)
                {
                    Console.WriteLine($"Exception: {exception.Message}");
                }

                if(coupon?.CouponCode is not null) response.CartHeader.DiscountTotal = coupon.DiscountAmount;
            }

            foreach(CartDetailsViewModel cartDetails in response.CartDetails)
            {
                response.CartHeader.PurchaseAmount += (cartDetails.Product.Price * cartDetails.Count);
            }

            response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountTotal;

            return response;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            CartViewModel? cart = await FindUserCart();

            if(cart?.CartHeader is null) return RedirectToAction("Index", "Home");

            return View(cart);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout() => View(await FindUserCart());

        [HttpPost]
        public async Task<IActionResult> Checkout(CartViewModel cartViewModel)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            CartViewModel? fullCart = await FindUserCart();

            if(fullCart?.CartHeader is null) return RedirectToAction("Index", "Home");

            fullCart.CartHeader.FirstName = cartViewModel.CartHeader.FirstName;
            fullCart.CartHeader.LastName = cartViewModel.CartHeader.LastName;
            fullCart.CartHeader.Phone = cartViewModel.CartHeader.Phone;
            fullCart.CartHeader.Email = cartViewModel.CartHeader.Email;
            fullCart.CartHeader.CardNumber = cartViewModel.CartHeader.CardNumber;
            fullCart.CartHeader.Cvv = cartViewModel.CartHeader.Cvv;
            fullCart.CartHeader.ExpiryMonthYear = cartViewModel.CartHeader.ExpiryMonthYear;

            object response = await _cartService.Checkout(fullCart.CartHeader, token!);

            if(response is string)
            {
                TempData["Error"] = response;
                return RedirectToAction(nameof(Checkout));
            }
            else
            {
                return RedirectToAction(nameof(Confirmation));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(CartViewModel cartViewModel) => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid id)
        {
            Console.WriteLine($">>> Remove call with id: {id}.");

            string? token = await HttpContext.GetTokenAsync("access_token");

            Console.WriteLine($">>> Token: {token?[..20]}...");

            await _cartService.DeleteItemFromCart(id, token!);

            return RedirectToAction(nameof(CartIndex));
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartViewModel cartViewModel)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            await _cartService.ApplyCoupon(cartViewModel, token!);

            return RedirectToAction(nameof(CartIndex)); // Post/Redirect/Get (PRG) pattern.
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartViewModel cartViewModel)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");

            await _cartService.RemoveCoupon(cartViewModel.CartHeader.UserId, token!);

            return RedirectToAction(nameof(CartIndex)); // Post/Redirect/Get (PRG) pattern.
        }
    }
}