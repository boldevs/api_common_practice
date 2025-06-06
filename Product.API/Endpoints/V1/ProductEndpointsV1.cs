using Product.API.Helpers;
using Product.API.Models;
using Product.API.Services;
using System.ComponentModel.DataAnnotations;

namespace Product.API.Endpoints.V1
{
    public static class ProductEndpointsV1
    {
        public static void MapProductV1(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/v1/products").WithTags("Products v1");

            group.MapGet("/", async (IProductService service, [AsParameters] ProductQuery query) =>
            {
                var result = await service.GetAllAsync(query);
                return Results.Ok(result);
            });

            group.MapGet("/{id:guid}", async (IProductService service, Guid id, HttpContext http) =>
            {
                var product = await service.GetByIdAsync(id);
                return product is null
                    ? ErrorResults.NotFound($"No product found with ID '{id}'", http)
                    : Results.Ok(product);
            });

            group.MapPost("/", async (IProductService service, CreateProductDto dto, HttpContext http) =>
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(dto);

                if (!Validator.TryValidateObject(dto, context, validationResults, true))
                {
                    return Results.ValidationProblem(validationResults.ToDictionary(
                        r => r.MemberNames.FirstOrDefault() ?? "",
                        r => new[] { r.ErrorMessage ?? "Invalid" }
                    ));
                }

                var created = await service.CreateAsync(dto);
                return Results.Created($"/api/v1/products/{created.Id}", created);
            });

            group.MapPut("/{id:guid}", async (IProductService service, Guid id, UpdateProductDto dto, HttpContext http) =>
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(dto);

                if (!Validator.TryValidateObject(dto, context, validationResults, true))
                {
                    return Results.ValidationProblem(validationResults.ToDictionary(
                        r => r.MemberNames.FirstOrDefault() ?? "",
                        r => new[] { r.ErrorMessage ?? "Invalid" }
                    ));
                }

                var updated = await service.UpdateAsync(id, dto);
                return updated is null
                    ? ErrorResults.NotFound($"No product found with ID '{id}'", http)
                    : Results.Ok(updated);
            });

            group.MapDelete("/{id:guid}", async (IProductService service, Guid id, HttpContext http) =>
            {
                var deleted = await service.DeleteAsync(id);
                return deleted
                    ? Results.NoContent()
                    : ErrorResults.NotFound($"No product found with ID '{id}'", http);
            });
        }
    }
}

