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
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging() // Enable detailed logging
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<ProductService>>();
        _mockCacheService = new Mock<ICacheService>();

        _service = new ProductService(_context, _mockLogger.Object, _mockCacheService.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "TEST-001" // Ensure SKU is set
        };

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);

        var productInDb = await _context.Products.FindAsync(result.Id);
        productInDb.Should().NotBeNull();
        productInDb!.Name.Should().Be(createDto.Name);
        productInDb.SKU.Should().Be(createDto.SKU); // Validate SKU
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProductAndInvalidateCache_WhenProductExists()
    {
        // Arrange
        var product = new Products
        {
            Id = Guid.NewGuid(),
            Name = "To Be Deleted",
            SKU = "DEL-001" // Ensure SKU is set
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var cacheKey = $"product:{product.Id}";
        _mockCacheService.Setup(c => c.RemoveDataAsync(cacheKey)).Verifiable();

        // Act
        var result = await _service.DeleteAsync(product.Id);

        // Assert
        result.Should().BeTrue();

        var deletedProduct = await _context.Products.FindAsync(product.Id);
        deletedProduct.Should().BeNull();

        _mockCacheService.Verify(c => c.RemoveDataAsync(cacheKey), Times.Once);
    }
}