using GeekShopping.ProductAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeekShopping.ProductAPI.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Represents the configuration for the "products" table in the database.
    /// Defines the table name, primary key, and column mappings, including constraints and default values.
    /// </summary>
    public class ProductsTable : IEntityTypeConfiguration<Product>
    {
        /// <summary>
        /// Configures the "products" table in the database by defining its schema, properties, and constraints.
        /// </summary>
        /// <param name="builder">An instance of <see cref="EntityTypeBuilder{Product}"/> used to configure the "products" table.</param>
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            //* Id (GUID):
            builder.HasKey((product) => product.Id);  // Primary Key (PK).

            builder
                .Property((product) => product.Id)
                .HasColumnName("product_id")
                .IsRequired();

            //* Other fields:
            builder
                .Property((product) => product.Name)
                .HasColumnName("product")
                .HasMaxLength(300)
                .IsRequired();

            builder
                .Property((product) => product.Price)
                .HasColumnName("price")
                .HasPrecision(18, 2)
                .IsRequired();

            builder
                .Property((product) => product.Discount)
                .HasColumnName("discount")
                .HasConversion(
                    (value) => (value.Equals(null)) ? null : (int?)(value * 100),
                    (value) => (value.Equals(null)) ? null : (value / 100m)
                )
                .HasPrecision(7, 2)
                .HasDefaultValue(0m)
                .IsRequired(false);

            builder
                .Property((product) => product.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired(false);

            builder
                .Property((product) => product.Category)
                .HasColumnName("category")
                .HasMaxLength(50)
                .IsRequired();

            builder
                .Property((product) => product.ImageUrl)
                .HasColumnName("image_url")
                .HasMaxLength(300)
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