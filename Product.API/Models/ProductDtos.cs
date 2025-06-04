using System.ComponentModel.DataAnnotations;

namespace Product.API.Models
{
    public record CreateProductDto
    {
        [Required]
        public string Name { get; init; } = null!;

        public string? Description { get; init; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; init; }

        [Required]
        public string SKU { get; init; } = null!;
    }

    public record UpdateProductDto
    {
        [Required]
        public string Name { get; init; } = null!;

        public string? Description { get; init; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; init; }

        [Required]
        public string SKU { get; init; } = null!;
    }

    public record ProductResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string SKU { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
