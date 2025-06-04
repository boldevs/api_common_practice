using FluentAssertions;
using Product.API.Common;
using Product.API.Models;
using System.Net;
using System.Net.Http.Json;

namespace Testing.IntegrationTesting.Endpoints
{
    public class ProductEndpointsTests : IClassFixture<TestApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductEndpointsTests(TestApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostProduct_ShouldReturnCreated()
        {
            var dto = new CreateProductDto
            {
                Name = "Integration Test Product",
                Description = "Created from test",
                Price = 29.99m,
                SKU = "INT-001"
            };

            var response = await _client.PostAsJsonAsync("/api/v1/products", dto);
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.Created, content);
            var result = await response.Content.ReadFromJsonAsync<ProductResponseDto>();

            result.Should().NotBeNull();
            result!.Name.Should().Be(dto.Name);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct()
        {
            var createDto = new CreateProductDto
            {
                Name = "Test Get Product",
                Description = "To be retrieved",
                Price = 10.99m,
                SKU = "GET-001"
            };

            var postResponse = await _client.PostAsJsonAsync("/api/v1/products", createDto);
            var created = await postResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            var getResponse = await _client.GetAsync($"/api/v1/products/{created!.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var retrieved = await getResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be(createDto.Name);
        }

        [Fact]
        public async Task PutProduct_ShouldUpdateProduct()
        {
            // Arrange: Create a product first
            var createDto = new CreateProductDto
            {
                Name = "Product to Update",
                Description = "Initial",
                Price = 9.99m,
                SKU = "PUT-001"
            };

            var postResponse = await _client.PostAsJsonAsync("/api/v1/products", createDto);
            var created = await postResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            // Act: Update it
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Product",
                Description = "Updated Desc",
                Price = 19.99m,
                SKU = "PUT-001-U"
            };

            var putResponse = await _client.PutAsJsonAsync($"/api/v1/products/{created!.Id}", updateDto);

            // Assert
            putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = await putResponse.Content.ReadFromJsonAsync<ProductResponseDto>();
            updated.Should().NotBeNull();
            updated!.Name.Should().Be(updateDto.Name);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnNoContent()
        {
            // Arrange: Create a product to delete
            var createDto = new CreateProductDto
            {
                Name = "Product to Delete",
                Description = "Bye-bye",
                Price = 15.00m,
                SKU = "DEL-001"
            };

            var postResponse = await _client.PostAsJsonAsync("/api/v1/products", createDto);
            var created = await postResponse.Content.ReadFromJsonAsync<ProductResponseDto>();

            // Act: Delete it
            var deleteResponse = await _client.DeleteAsync($"/api/v1/products/{created!.Id}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Optional: Try to GET it afterward to confirm it's gone
            var getResponse = await _client.GetAsync($"/api/v1/products/{created.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAll_ShouldReturnPaginatedAndFilteredResults()
        {
            // Arrange: seed multiple products
            var products = new List<CreateProductDto>
            {
                new() { Name = "Apple Watch", Description = "Wearable", Price = 199.99m, SKU = "APL-001" },
                new() { Name = "Apple iPhone", Description = "Phone", Price = 999.99m, SKU = "APL-002" },
                new() { Name = "Samsung Galaxy", Description = "Phone", Price = 899.99m, SKU = "SMS-001" },
                new() { Name = "Google Pixel", Description = "Phone", Price = 799.99m, SKU = "GOO-001" }
            };

            foreach (var p in products)
                await _client.PostAsJsonAsync("/api/v1/products", p);

            var query = "?page=1&limit=2&search=apple";

            // Act
            var response = await _client.GetAsync($"/api/v1/products{query}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductResponseDto>>();

            result.Should().NotBeNull();
            result!.Items.Should().HaveCount(2);
            result.Items.All(p => p.Name.ToLower().Contains("apple")).Should().BeTrue();
            result.TotalCount.Should().Be(2);
            result.Page.Should().Be(1);
            result.Limit.Should().Be(2);
        }


    }
}
