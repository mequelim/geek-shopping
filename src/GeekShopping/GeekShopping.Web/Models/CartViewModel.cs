using GeekShopping.Web.Shared.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Models
{
    public class CartViewModel
    {
        public required CartHeaderViewModel CartHeader { get; init; }
        public required IEnumerable<CartDetailsViewModel> CartDetails { get; init; }

        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        public double PurchaseAmount { get; init; }
    }
}