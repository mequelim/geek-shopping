namespace GeekShopping.Web.Models
{
    public class CouponViewModel
    {
        public Guid Id { get; init; }
        public required string CouponCode { get; init; }
        public decimal DiscountAmount { get; init; }
    }
}