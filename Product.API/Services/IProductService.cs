using Product.API.Common;
using Product.API.Models;

namespace Product.API.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQuery query);
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto dto);
        Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
