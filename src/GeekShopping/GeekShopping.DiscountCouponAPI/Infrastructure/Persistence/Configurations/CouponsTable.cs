using GeekShopping.DiscountCouponAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekShopping.DiscountCouponAPI.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Represents the configuration for the "coupons" table in the database.
    /// Defines the schema, properties, and constraints for the <see cref="Coupon"/> entity.
    /// </summary>
    public class CouponsTable : IEntityTypeConfiguration<Coupon>
    {
        /// <summary>
        /// Configures the schema, properties, and constraints for the <see cref="Coupon"/> entity in the database.
        /// </summary>
        /// <param name="builder">An instance of <see cref="EntityTypeBuilder{TEntity}"/> used to configure the <see cref="Coupon"/> entity.</param>
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("coupons");

            //* Id (GUID):
            builder.HasKey((coupon) => coupon.Id);  // Primary Key (PK).

            builder
                .Property((coupon) => coupon.Id)
                .HasColumnName("coupon_id")
                .IsRequired();

            //* Other fields:
            builder
                .Property((coupon) => coupon.CouponCode)
                .HasColumnName("coupon_code")
                .HasMaxLength(10)
                .IsRequired();

            builder
                .HasIndex((coupon) => coupon.CouponCode)
                .IsUnique();

            builder
                .Property((coupon) => coupon.DiscountAmount)
                .HasColumnName("coupon_discount_amount")
                .HasPrecision(10, 2)
                .HasDefaultValue(0m)
                .IsRequired();

            //* Dates:
            builder.Property<DateTime>("created_at")
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder.Property<DateTime>("updated_at")
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        }
    }
}