using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeekShopping.Web.Controllers
{
    public class HomeController(IProductService productService, ICartService cartService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductViewModel> products = await productService.GetAllProducts();

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");
            ProductViewModel viewModel = await productService.GetProductById(id, token);

            return View(viewModel);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductViewModel model)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");
            string? userIdClaim = User.Claims
                .FirstOrDefault((u) => u.Type.Equals("sub"))
                ?.Value;

            if((userIdClaim is null) || (!Guid.TryParse(userIdClaim, out Guid userId))) return Unauthorized();

            // Fetching full product data to ensure all required fields are populated...
            ProductViewModel product = await productService.GetProductById(model.Id, token);

            CartViewModel cartViewModel = new()
            {
                CartHeader = new CartHeaderViewModel { UserId = userId },
                CartDetails =
                [
                    new CartDetailsViewModel
                    {
                        Count = model.Count,
                        ProductId = product.Id,
                        Product = new ProductViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Discount = product.Discount,
                            Description = product.Description,
                            Category = product.Category,
                            ImageUrl = product.ImageUrl
                        }
                    }
                ]
            };

            await cartService.AddItemToCart(cartViewModel, token!);

            return RedirectToAction(nameof(Index));  // PRG: Post-Redirect-Get.
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        [Authorize]
        public IActionResult Login() => RedirectToAction(nameof(Index));

        public IActionResult Logout() => SignOut("Cookies", "oidc");
    }
}