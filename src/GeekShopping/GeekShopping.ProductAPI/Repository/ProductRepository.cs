using AutoMapper;
using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Infrastructure.Database;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Repository.Interfaces;
using GeekShopping.ProductAPI.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository
{
    /// <summary>
    /// Represents a repository for managing and retrieving product data.
    /// Provides methods for accessing, adding, updating, and deleting product entities from the underlying data source.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _databaseContext;
        private readonly IMapper _mapper;

        // Constructor:
        /// <summary>
        /// Provides methods for managing product data in the database, including retrieval, creation, update, and deletion operations.
        /// </summary>
        public ProductRepository(AppDbContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        // Methods:
        /// <summary>
        /// Retrieves all products available in the database.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="ProductDto"/> objects representing all the products.</returns>
        public async Task<IEnumerable<ProductDto>> FindAllProducts()
        {
            List<Product> products = await _databaseContext.Products.ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        /// <summary>
        /// Retrieves a single product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to retrieve.</param>
        /// <returns>A <see cref="ProductDto"/> object representing the product if found, or null if no product exists with the given identifier.</returns>
        public async Task<ProductDto> FindProductsById(Guid id)
        {
            Product? product = await _databaseContext.Products
                .Where((prod) => prod.Id.Equals(id))
                .FirstOrDefaultAsync();

            return _mapper.Map<ProductDto>(product);
        }

        /// <summary>
        /// Creates a new product in the database.
        /// </summary>
        /// <param name="productDto">An object containing the details of the product to be created.</param>
        /// <returns>A <see cref="ProductDto"/> object representing the newly created product.</returns>
        public async Task<ProductDto> CreateProducts(ProductDto productDto)
        {
            Product product = _mapper.Map<Product>(productDto);

            _databaseContext.Products.Add(product);
            await _databaseContext.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        /// <summary>
        /// Updates an existing product in the repository with the provided data.
        /// </summary>
        /// <param name="productDto">The data transfer object containing the updated product information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated product data.</returns>
        /// <exception cref="ConflictException">Thrown when the product cannot be updated due to a concurrency conflict.</exception>
        public async Task<ProductDto> UpdateProducts(ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);

                _databaseContext.Products.Update(product);
                _databaseContext
                    .Entry(product)
                    .Property((prod) => prod.Version)
                    .OriginalValue = productDto.Version;

                await _databaseContext.SaveChangesAsync();

                return productDto;
            }
            catch(DbUpdateConcurrencyException exception)
            {
                await Console.Error
                    .WriteLineAsync($"A concurrent update occurred: error message -> {exception.Message}; source -> {exception.Source}; stack trace -> {exception.StackTrace}.");

                throw new ConflictException(
                    "The changes could not be saved... the product was been modified by another user!",
                    nameof(Product)
                );
            }
        }

        /// <summary>
        /// Deletes a product from the database by its identifier and version, ensuring concurrency control.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <param name="version">The version of the product to ensure it has not been modified by another user.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the deletion was successful.
        /// </returns>
        /// <exception cref="ConflictException">Thrown when the product has been modified by another user, causing a concurrency conflict.</exception>
        public async Task<bool> DeleteProducts(Guid id, uint version)
        {
            try
            {
                Product? product = await _databaseContext.Products
                    .Where((prod) => prod.Id.Equals(id))
                    .FirstOrDefaultAsync();

                if(product is null) return false;

                _databaseContext.Products.Remove(product);
                _databaseContext.Entry(product).Property(p => p.Version).OriginalValue = version;

                await _databaseContext.SaveChangesAsync();

                return true;
            }
            catch(DbUpdateConcurrencyException)
            {
                throw new ConflictException(
                    "The product has been modified by another user! Please refresh before deleting.",
                    nameof(Product)
                );
            }
            catch(Exception exception)
            {
                await Console.Error.WriteLineAsync($"Error deleting product with ID {id}: {exception.Message}");
                throw;
            }
        }
    }
}