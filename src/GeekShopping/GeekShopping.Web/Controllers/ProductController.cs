using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Shared.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class ProductController(IProductService productService) : Controller
    {
        private readonly IProductService _productService = productService
                                                           ?? throw new ArgumentNullException(nameof(productService));

        [HttpGet]
        public async Task<IActionResult> ProductIndex()
        {
            IEnumerable<ProductViewModel> products = await _productService.GetAllProducts();

            return View(products);
        }

        public Task<IActionResult> ProductCreate()
        {
            try
            {
                return Task.FromResult<IActionResult>(View());
            }
            catch(Exception exception)
            {
                return Task.FromException<IActionResult>(exception);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductViewModel viewModel)
        {
            if(!ModelState.IsValid) return View(viewModel);

            string? token = await HttpContext.GetTokenAsync("access_token");
            ProductViewModel? response = await _productService.CreateProduct(viewModel, token);

            if(response is null) return View(viewModel);

            return RedirectToAction(nameof(ProductIndex));
        }

        [Authorize]
        public async Task<IActionResult> ProductUpdate(Guid id)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");
            ProductViewModel? model = await _productService.GetProductById(id, token);

            if(model is null) return NotFound();

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductViewModel viewModel)
        {
            if(!ModelState.IsValid) return View(viewModel);

            string? token = await HttpContext.GetTokenAsync("access_token");
            ProductViewModel? response = await _productService.UpdateProduct(viewModel, token);

            if(response is null) return View(viewModel);
            return RedirectToAction(nameof(ProductIndex));
        }

        [Authorize]
        public async Task<IActionResult> ProductDelete(Guid id)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");
            string[] parts = token!.Split('.');
            string payload = parts[1];
            int pad = payload.Length % 4;

            if(pad > 0) payload += new string('=', 4 - pad);

            string json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));

            Console.WriteLine($">>> Payload's token: {json}");

            ProductViewModel? model = await _productService.GetProductById(id, token);

            if(model is null) return NotFound();

            return View(model);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductViewModel viewModel)
        {
            string? token = await HttpContext.GetTokenAsync("access_token");
            bool response = await _productService.DeleteProduct(viewModel.Id, viewModel.Version, token);

            if(!response) return View(viewModel);

            return RedirectToAction(nameof(ProductIndex));
        }
    }
}