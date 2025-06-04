using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Product.API.Caching; // <-- IMPORTANT
using Product.API.Data;
using Product.API.Models;
using Product.API.Services;
using Product.API.Common;

namespace Testing.UnitTesting.Services;

public class ProductServiceTests
{
    // // Re-use mocks and context across tests by making them class members
    // private readonly ApplicationDbContext _context;
    // private readonly Mock<ILogger<ProductService>> _mockLogger;
    // private readonly Mock<ICacheService> _mockCacheService;
    // private readonly ProductService _service;

    // // Use a constructor for common test setup (Arrange phase)
    // public ProductServiceTests()
    // {
    //     var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    //         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test run
    //         .Options;

    //     _context = new ApplicationDbContext(options);
    //     _mockLogger = new Mock<ILogger<ProductService>>();
    //     _mockCacheService = new Mock<ICacheService>();

    //     // The service is now created once with all its dependencies
    //     _service = new ProductService(_context, _mockLogger.Object, _mockCacheService.Object);
    // }

    // [Fact]
    // public async Task GetByIdAsync_ShouldReturnProductFromDb_WhenCacheMiss()
    // {
    //     // Arrange
    //     var product = new Products { Id = Guid.NewGuid(), Name = "Test Product" };
    //     _context.Products.Add(product);
    //     await _context.SaveChangesAsync();

    //     string cacheKey = $"product:{product.Id}";

    //     // Simulate a cache miss
    //     _mockCacheService.Setup(c => c.GetDataAsync<ProductResponseDto>(cacheKey))
    //         .ReturnsAsync((ProductResponseDto)null);

    //     // Act
    //     var result = await _service.GetByIdAsync(product.Id);

    //     // Assert
    //     result.Should().NotBeNull();
    //     result!.Id.Should().Be(product.Id);

    //     // Verify that the service tried to set the cache after fetching from DB
    //     _mockCacheService.Verify(c => c.SetDataAsync(
    //         cacheKey,
    //         It.IsAny<ProductResponseDto>(),
    //         It.IsAny<TimeSpan?>(),
    //         It.IsAny<TimeSpan?>()),
    //         Times.Once);
    // }

    // [Fact]
    // public async Task GetByIdAsync_ShouldReturnProductFromCache_WhenCacheHit()
    // {
    //     // Arrange
    //     var productId = Guid.NewGuid();
    //     var cacheKey = $"product:{productId}";
    //     var cachedProduct = new ProductResponseDto { Id = productId, Name = "Cached Product" };

    //     // Simulate a cache hit
    //     _mockCacheService.Setup(c => c.GetDataAsync<ProductResponseDto>(cacheKey))
    //         .ReturnsAsync(cachedProduct);

    //     // Act
    //     var result = await _service.GetByIdAsync(productId);

    //     // Assert
    //     result.Should().NotBeNull();
    //     result!.Id.Should().Be(productId);
    //     result.Name.Should().Be("Cached Product");

    //     // Verify that the DB was NOT touched (this is a key test for caching)
    //     // (No direct verification needed, the test passes if no DB context error occurs without data)
    // }

    // [Fact]
    // public async Task UpdateAsync_ShouldUpdateProductAndInvalidateCache_WhenProductExists()
    // {
    //     // Arrange
    //     var product = new Products { Id = Guid.NewGuid(), Name = "Original" };
    //     _context.Products.Add(product);
    //     await _context.SaveChangesAsync();

    //     var updateDto = new UpdateProductDto { Name = "Updated" };
    //     var cacheKey = $"product:{product.Id}";

    //     // Act
    //     var result = await _service.UpdateAsync(product.Id, updateDto);

    //     // Assert
    //     result.Should().NotBeNull();
    //     result!.Name.Should().Be("Updated");

    //     // Verify that the cache for this item was removed (invalidated)
    //     _mockCacheService.Verify(c => c.RemoveDataAsync(cacheKey), Times.Once);
    // }
    
    // [Fact]
    // public async Task DeleteAsync_ShouldDeleteProductAndInvalidateCache_WhenProductExists()
    // {
    //     // Arrange
    //     var product = new Products { Id = Guid.NewGuid(), Name = "To Be Deleted" };
    //     _context.Products.Add(product);
    //     await _context.SaveChangesAsync();
    //     var cacheKey = $"product:{product.Id}";

    //     // Act
    //     var result = await _service.DeleteAsync(product.Id);
        
    //     // Assert
    //     result.Should().BeTrue();
    //     var deletedProduct = await _context.Products.FindAsync(product.Id);
    //     deletedProduct.Should().BeNull();
        
    //     // Verify that the cache for this item was removed
    //     _mockCacheService.Verify(c => c.RemoveDataAsync(cacheKey), Times.Once);
    // }

    // // --- Other tests require minimal changes beyond the constructor setup ---

    // [Fact]
    // public async Task CreateAsync_ShouldCreateProduct()
    // {
    //     // Arrange
    //     var createDto = new CreateProductDto { Name = "Test Product" };

    //     // Act
    //     var result = await _service.CreateAsync(createDto);

    //     // Assert
    //     result.Should().NotBeNull();
    //     result.Name.Should().Be(createDto.Name);
    //     var productInDb = await _context.Products.FindAsync(result.Id);
    //     productInDb.Should().NotBeNull();
    // }

    // [Fact]
    // public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    // {
    //     // Arrange
    //     var nonExistentId = Guid.NewGuid();

    //     // Act
    //     var result = await _service.DeleteAsync(nonExistentId);

    //     // Assert
    //     result.Should().BeFalse();
    // }
    
    // // Note: GetAllAsync caching was not implemented, so the test remains the same.
    // [Fact]
    // public async Task GetAllAsync_ShouldReturnFilteredAndPagedResults()
    // {
    //     // Arrange
    //     _context.Products.AddRange(
    //         new Products { Id = Guid.NewGuid(), Name = "Apple Watch" },
    //         new Products { Id = Guid.NewGuid(), Name = "Apple iPhone" },
    //         new Products { Id = Guid.NewGuid(), Name = "Samsung TV" }
    //     );
    //     await _context.SaveChangesAsync();

    //     var query = new ProductQuery { Page = 1, Limit = 2, Search = "apple" };

    //     // Act
    //     var result = await _service.GetAllAsync(query);

    //     // Assert
    //     result.Should().NotBeNull();
    //     result.Items.Should().HaveCount(2);
    //     result.TotalCount.Should().Be(2);
    // }
}