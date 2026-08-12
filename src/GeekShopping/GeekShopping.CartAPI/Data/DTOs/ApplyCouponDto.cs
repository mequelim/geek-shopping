namespace GeekShopping.CartAPI.Data.DTOs
{
    /// <summary>
    /// Data transfer object used to apply a coupon to a user's cart.
    /// This DTO contains the necessary information to identify the user and the coupon they wish to apply.
    /// </summary>
    public class ApplyCouponDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// Represents the user associated with the coupon being applied.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the code of the coupon to be applied.
        /// Represents the discount or promotional code associated with the user's cart.
        /// </summary>
        public string? CouponCode { get; set; }
    }
}