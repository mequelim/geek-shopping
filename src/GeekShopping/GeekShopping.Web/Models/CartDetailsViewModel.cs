namespace GeekShopping.Web.Models
{
    public class CartDetailsViewModel
    {
        public Guid Id { get; init; }
        public Guid CartHeaderId { get; init; }
        public CartHeaderViewModel? CartHeader { get; init; }
        public Guid ProductId { get; init; }
        public required ProductViewModel Product { get; init; }
        public int Count { get; init; }
        public uint Version { get; init; }
    }
}