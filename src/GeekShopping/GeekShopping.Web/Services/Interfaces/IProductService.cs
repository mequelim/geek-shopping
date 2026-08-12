using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProducts();
        Task<ProductViewModel> GetProductById(Guid id, string? token);
        Task<ProductViewModel> CreateProduct(ProductViewModel productViewModel, string? token);
        Task<ProductViewModel> UpdateProduct(ProductViewModel productViewModel, string? token);
        Task<bool> DeleteProduct(Guid id, uint version, string? token);
    }
}