using GeekShopping.OrderAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekShopping.OrderAPI.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Configures the database table mapping and entity properties for the OrderHeader entity.
    /// </summary>
    /// <remarks>
    /// Maps the <c>OrderHeader</c> entity to the "cart_header" table in the database and defines the column mappings, keys, and constraints for the entity properties.
    /// Includes configurations for primary key, required fields, concurrency tokens, and default values for audit fields.
    /// </remarks>
    /// <example>Applied as an Entity Configuration in the Entity Framework's <c>ModelBuilder</c>.</example>
    public class OrderHeadersTable : IEntityTypeConfiguration<OrderHeader>
    {
        /// <summary>
        /// Configures the entity mappings and properties for the CartHeader entity in the database.
        /// </summary>
        /// <param name="builder">
        /// Provides a configuration object to set up the schema details for the CartHeader entity, including defining table name, primary key, column names, constraints, and default values.
        /// </param>
        public void Configure(EntityTypeBuilder<OrderHeader> builder)
        {
            builder.ToTable("order_header");

            //* Id (GUID):
            builder.HasKey((orderHeader) => orderHeader.Id);  // Primary Key (PK).

            builder
                .Property((orderHeader) => orderHeader.Id)
                .HasColumnName("order_header_id")
                .IsRequired();

            //* Other fields:
            builder
                .Property((orderHeader) => orderHeader.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.CouponCode)
                .HasColumnName("coupon_code")
                .HasMaxLength(50)
                .IsRequired(false);

            builder
                .Property((orderHeader) => orderHeader.Version)
                .HasColumnName("version")
                .IsConcurrencyToken(); // or IsRowVersion();

            builder
                .Property((orderHeader) => orderHeader.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(90)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(90)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.Email)
                .HasColumnName("email")
                .HasMaxLength(90)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.CardNumber)
                .HasColumnName("card_number")
                .HasMaxLength(19)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.ExpiryMonthYear)
                .HasColumnName("expired_month_year")
                .HasMaxLength(5)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.Cvv)
                .HasColumnName("cvv")
                .HasMaxLength(3)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.OrderTotalItems)
                .HasColumnName("total_items")
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.PurchaseAmount)
                .HasColumnName("purchase_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.DiscountTotal)
                .HasColumnName("discount_total")
                .HasPrecision(18, 2)
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.IsPaid)
                .HasColumnName("is_paid")
                .HasDefaultValue(false)
                .IsRequired();

            //* Date:
            builder
                .Property((orderHeader) => orderHeader.PurchaseDate)
                .HasColumnName("purchase_date")
                .HasDefaultValueSql("NOW()")
                .IsRequired();

            builder
                .Property((orderHeader) => orderHeader.PurchaseTime)
                .HasColumnName("order_time")
                .HasColumnType("timestamp with time zone") // for PostgreSQL; or "datetime" for MySQL/SQL Server.
                .HasDefaultValueSql("NOW()")
                .IsRequired();
        }
    }
}