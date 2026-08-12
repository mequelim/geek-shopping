using GeekShopping.CartAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekShopping.CartAPI.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Provides the configuration for the <see cref="CartDetail"/> entity in the database.
    /// This class defines the mapping of the <see cref="CartDetail"/> entity to the "card_details" table in the database.
    /// </summary>
    public class CartDetailsTable : IEntityTypeConfiguration<CartDetail>
    {
        /// <summary>
        /// Configures the entity framework mappings for the <see cref="CartDetail"/> entity,
        /// defining the table name, primary key, foreign keys, and additional properties.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{CardDetails}"/> used to configure the entity properties and relationships for the database.</param>
        public void Configure(EntityTypeBuilder<CartDetail> builder)
        {
            builder.ToTable("cart_details");

            //* Id (GUID):
            builder.HasKey((cardDetails) => cardDetails.Id);  // Primary Key (PK).

            builder
                .Property((cardDetails) => cardDetails.Id)
                .HasColumnName("cart_id")
                .IsRequired();

            //* Foreign Key (FK):
            builder
                .Property((cartDetails) => cartDetails.CartHeaderId)
                .HasColumnName("cart_header_id")
                .IsRequired();

            builder
                .HasOne((cartDetails) => cartDetails.CartHeader)
                .WithMany()
                .HasForeignKey((cartDetails) => cartDetails.CartHeaderId)
                .OnDelete(DeleteBehavior.Cascade);

            //* Other fields:
            builder
                .Property((cartDetails) => cartDetails.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder
                .HasOne((cartDetails) => cartDetails.Product)
                .WithMany()
                .HasForeignKey(cartDetails => cartDetails.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Property((cartDetails) => cartDetails.Count)
                .HasColumnName("count")
                .IsRequired()
                .HasDefaultValue(1);

            builder
                .ToTable((table) => table.HasCheckConstraint("CK_cart_details_count_positive", "count >= 1"));

            builder
                .Property((cartDetails) => cartDetails.Version)
                .HasColumnName("version")
                .IsConcurrencyToken(); // or IsRowVersion();

            //* Dates:
            builder
                .Property<DateTime>("created_at")
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            builder
                .Property<DateTime>("updated_at")
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()");
        }
    }
}