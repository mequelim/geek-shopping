namespace GeekShopping.Web.Models
{
    public class CartHeaderViewModel
    {
        public Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public string? CouponCode { get; init; }
        public decimal PurchaseAmount { get; set; }
        public decimal DiscountTotal { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateTime { get; init; } = DateTime.Now;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryMonthYear { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public uint Version { get; init; }
    }
}