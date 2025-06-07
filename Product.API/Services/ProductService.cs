using Microsoft.EntityFrameworkCore;
using Product.API.Caching;
using Product.API.Common;
using Product.API.Data;
using Product.API.Models;

namespace Product.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;
        private readonly ICacheService _cacheService;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQuery query)
        {
            try
            {
                _logger.LogInformation("Fetching products with query: {@Query}", query);

                var productsQuery = _context.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    string searchTerm = query.Search.ToLower();
                    productsQuery = productsQuery.Where(p =>
                        p.Name.ToLower().Contains(searchTerm) ||
                        p.SKU.ToLower().Contains(searchTerm));
                }

                int totalCount = await productsQuery.CountAsync();
                int skip = (query.Page - 1) * query.Limit;

                // --- FIX: Add OrderBy clause for predictable paging ---
                // This ensures the order of products is the same every time you query.
                var items = await productsQuery
                    .OrderBy(p => p.CreatedAt) // Ordering by creation date
                    .Skip(skip)
                    .Take(query.Limit)
                    .Select(p => new ProductResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        SKU = p.SKU,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    }).ToListAsync();

                return new PagedResult<ProductResponseDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = query.Page,
                    Limit = query.Limit
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products with query: {@Query}", query);
                throw;
            }
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            try
            {
                string cacheKey = $"product:{id}";
                var cachedProduct = await _cacheService.GetDataAsync<ProductResponseDto>(cacheKey);

                if (cachedProduct is not null)
                {
                    _logger.LogInformation("Cache hit for product ID: {ProductId}", id);
                    return cachedProduct;
                }

                _logger.LogInformation("Cache miss for product ID: {ProductId}. Fetching from DB.", id);
                var product = await _context.Products.FindAsync(id);
                if (product is null) return null;

                var productDto = MapToDto(product);
                await _cacheService.SetDataAsync(cacheKey, productDto, absoluteExpireTime: TimeSpan.FromMinutes(5));

                return productDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product by ID: {ProductId}", id);
                throw; // Let global middleware handle the exception
            }
        }
        public async Task<ProductResponseDto> CreateAsync(CreateProductDto dto)
        {
            try
            {
                var product = new Products
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    SKU = dto.SKU,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created: {@Product}", product);

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {@Dto}", dto);
                throw;
            }
        }

        public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", id);

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for update: {ProductId}", id);
                    return null;
                }

                // ... update properties ...
                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.SKU = dto.SKU;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // BEST PRACTICE: Invalidate the cache for this item
                string cacheKey = $"product:{id}";
                await _cacheService.RemoveDataAsync(cacheKey);
                _logger.LogInformation("Cache invalidated for product ID: {ProductId}", id);

                return MapToDto(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                throw;
            }
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for delete: {ProductId}", id);
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                // BEST PRACTICE: Invalidate the cache for this item
                string cacheKey = $"product:{id}";
                await _cacheService.RemoveDataAsync(cacheKey);
                _logger.LogInformation("Cache invalidated for product ID: {ProductId}", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                throw;
            }
        }

        private static ProductResponseDto MapToDto(Products p)
        {
            return new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                SKU = p.SKU,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
