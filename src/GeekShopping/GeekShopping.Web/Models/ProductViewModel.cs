using GeekShopping.Web.Shared.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GeekShopping.Web.Models
{
    public class ProductViewModel
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }

        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        public required decimal Price { get; init; }

        public decimal? Discount { get; init; }
        public string? Description { get; init; }
        public required string Category { get; init; }
        public required string ImageUrl { get; init; }
        public uint Version { get; init; }

        [Range(1, 100)]
        public int Count { get; init; } = 1;

        // Methods:
        public string SubstringName()
        {
            if(Name.Length < 24) return Name;

            return $"{Name[..21]}...";
        }

        public string SubstringDescription()
        {
            if(Description?.Length < 355) return Description;

            return $"{Description?[..352]}...";
        }
    }
}