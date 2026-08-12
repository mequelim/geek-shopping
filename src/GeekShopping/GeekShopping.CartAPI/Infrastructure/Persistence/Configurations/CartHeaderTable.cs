using GeekShopping.CartAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekShopping.CartAPI.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the entity framework mappings and properties for the <see cref="CartHeader"/> entity.
    /// </summary>
    public class CartHeaderTable : IEntityTypeConfiguration<CartHeader>
    {
        /// <summary>
        /// Configures the entity mappings and properties for the CartHeader entity in the database.
        /// </summary>
        /// <param name="builder">
        /// Provides a configuration object to set up the schema details for the CartHeader entity, including defining table name, primary key, column names, constraints, and default values.
        /// </param>
        public void Configure(EntityTypeBuilder<CartHeader> builder)
        {
            builder.ToTable("cart_header");

            //* Id (GUID):
            builder.HasKey((cartHeader) => cartHeader.Id);  // Primary Key (PK).

            builder
                .Property((cartHeader) => cartHeader.Id)
                .HasColumnName("cart_id")
                .IsRequired();

            //* Other fields:
            builder
                .Property((cartHeader) => cartHeader.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder
                .Property((cartHeader) => cartHeader.CouponCode)
                .HasColumnName("coupon_code")
                .HasMaxLength(50)
                .IsRequired(false);

            builder
                .Property((cartHeader) => cartHeader.Version)
                .HasColumnName("version")
                .IsConcurrencyToken(); // or IsRowVersion();

            //* Dates:
            builder
                .Property<DateTime>("created_at")
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder
                .Property<DateTime>("updated_at")
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        }
    }
}