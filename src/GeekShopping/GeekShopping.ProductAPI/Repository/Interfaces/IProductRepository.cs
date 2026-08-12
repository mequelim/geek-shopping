using GeekShopping.ProductAPI.Data.DTOs;

namespace GeekShopping.ProductAPI.Repository.Interfaces
{
    /// <summary>
    /// Represents the contract for performing CRUD operations on product data.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retrieves all products available in the database.
        /// </summary>
        /// <returns>An enumerable collection of <c>ProductDto</c> objects representing all the products.</returns>
        Task<IEnumerable<ProductDto>> FindAllProducts();

        /// <summary>
        /// Retrieves a single product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to retrieve.</param>
        /// <returns>A <see cref="ProductDto"/> object representing the product if found, or null if no product exists with the given identifier.</returns>
        Task<ProductDto> FindProductsById(Guid id);

        /// <summary>
        /// Creates a new product in the database.
        /// </summary>
        /// <param name="productDto">An object containing the details of the product to be created.</param>
        /// <returns>A <c>ProductDto</c> object representing the newly created product.</returns>
        Task<ProductDto> CreateProducts(ProductDto productDto);

        /// <summary>
        /// Updates an existing product in the database with new data.
        /// </summary>
        /// <param name="productDto">The <c>ProductDto</c> containing updated details of the product to be modified.</param>
        /// <returns>A <c>ProductDto</c> object reflecting the updates made to the product in the database.</returns>
        Task<ProductDto> UpdateProducts(ProductDto productDto);

        /// <summary>
        /// Deletes a product from the database based on its unique identifier and version.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <param name="version">The version of the product being deleted for concurrency control.</param>
        /// <returns>A boolean value indicating whether the deletion was successful.</returns>
        Task<bool> DeleteProducts(Guid id, uint version);
    }
}