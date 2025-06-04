using System.ComponentModel;

namespace Product.API.Models
{
    public class ProductQuery
    {
        public string? Search { get; set; }
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        [DefaultValue(10)]
        public int Limit { get; set; } = 10;
    }
}
