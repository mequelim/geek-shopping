using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Repository.Interfaces;
using GeekShopping.ProductAPI.Shared.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    /// <summary>
    /// Controller for managing product operations in the application, such as retrieving, creating, updating, and deleting products.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository productRepository) : ControllerBase
    {
        private readonly IProductRepository _productRepository = productRepository
                                                                 ?? throw new ArgumentNullException(nameof(productRepository));

        //? GET:
        /// <summary>
        /// Handles the retrieval of all product records from the database.
        /// </summary>
        /// <returns>A list of <c>ProductDto</c> objects representing all products in the system.</returns>
        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            IEnumerable<ProductDto> products = await _productRepository.FindAllProducts();

            return Ok(products);
        }

        /// <summary>
        /// Retrieves the details of a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to retrieve.</param>
        /// <returns>An <see cref="ActionResult"/> containing the product details as a <see cref="ProductDto"/>.</returns>
        [Authorize]
        [HttpGet("GetProductById/{id:guid}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            ProductDto product = await _productRepository.FindProductsById(id);

            Console.WriteLine($">>> Version no DTO: '{product.Version}'");

            return Ok(product);
        }

        //? POST:
        /// <summary>
        /// Creates a new product record in the system.
        /// </summary>
        /// <param name="productDto">An object containing the details of the product to create.</param>
        /// <returns>An <see cref="ActionResult"/> containing the created product as a <see cref="ProductDto"/>.</returns>
        [Authorize]
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDto productDto)
        {
            ProductDto product = await _productRepository.CreateProducts(productDto);

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        //? PUT:
        /// <summary>
        /// Updates an existing product in the system with the specified data.
        /// </summary>
        /// <param name="productDto">The data transfer object containing the updated product information.</param>
        /// <returns>Returns the updated product as a data transfer object.</returns>
        [Authorize]
        [HttpPut("UpdateProduct/{id:guid}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct([FromBody] ProductDto productDto)
        {
            ProductDto product = await _productRepository.UpdateProducts(productDto);

            return Ok(product);
        }

        //? DELETE:
        /// <summary>
        /// Deletes a product identified by its unique ID and specific version from the repository.
        /// Requires the user to be authenticated and have administrative privileges.
        /// </summary>
        /// <param name="id">The unique identifier of the product to be deleted.</param>
        /// <param name="version">The version of the product to be deleted.</param>
        /// <returns>Returns a boolean value indicating whether the product was successfully deleted.</returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("DeleteProduct/{id:guid}")]
        public async Task<ActionResult<bool>> DeleteProducts(Guid id, [FromQuery] uint version)
        {
            Console.WriteLine($">>> Auth header: {Request.Headers.Authorization}");
            Console.WriteLine($">>> Is Authenticated? {User.Identity?.IsAuthenticated}");
            Console.WriteLine($">>> Claims: {string.Join(", ", User.Claims.Select((claim) => $"{claim.Type}={claim.Value}"))}");
            Console.WriteLine($">>> Is admin? {User.IsInRole("Admin")}");

            bool result = await _productRepository.DeleteProducts(id, version);

            if(!result) return NotFound();

            return Ok(result);
        }
    }
}