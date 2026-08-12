using GeekShopping.OrderAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekShopping.OrderAPI.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Represents the configuration for the Orders table in the database, including schema definitions, primary key configuration, and property mappings for the entity.
    /// </summary>
    public class OrderDetailsTable : IEntityTypeConfiguration<OrderDetail>
    {
        /// <summary>
        /// Configures the database schema for the <see cref="OrderDetail"/> table, including table name, primary key, and properties for the entity.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("order_details");

            //* Id (GUID):
            builder.HasKey((orderDetails) => orderDetails.Id);  // Primary Key (PK).

            builder
                .Property((orderDetails) => orderDetails.Id)
                .HasColumnName("order_id")
                .IsRequired();

            //* Foreing Key (FK):
            builder
                .HasOne((cardDetail) => cardDetail.OrderHeader)
                .WithMany((orderHeader) => orderHeader.OrderDetails)
                .HasForeignKey((orderDetails) => orderDetails.OrderHeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property((orderDetails) => orderDetails.OrderHeaderId)
                .HasColumnName("order_header_id");

            //* Other fields:
            builder
                .Property((orderDetails) => orderDetails.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder
                .Property((orderDetails) => orderDetails.Count)
                .HasColumnName("count")
                .IsRequired()
                .HasDefaultValue(1);

            builder
                .ToTable((table) => table.HasCheckConstraint("CK_cart_details_count_positive", "count >= 1"));

            builder
                .Property((orderDetails) => orderDetails.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(255)
                .IsRequired();

            builder
                .Property((orderDetails) => orderDetails.Price)
                .HasColumnName("order_value")
                .HasPrecision(18, 2)
                .IsRequired();

            builder
                .Property((cartDetails) => cartDetails.Version)
                .HasColumnName("version")
                .IsConcurrencyToken(); // or IsRowVersion();

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