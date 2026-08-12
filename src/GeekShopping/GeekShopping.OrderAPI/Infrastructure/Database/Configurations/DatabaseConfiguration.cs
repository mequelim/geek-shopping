using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GeekShopping.OrderAPI.Infrastructure.Database.Configurations
{
    /// <summary>
    /// Provides a set of utilities and global configurations for database schema conventions, including entity and property naming, foreign key constraint naming, index naming, and GUID primary key
    /// column configuration.
    /// </summary>
    public static class DatabaseConfiguration
    {
        /// <summary>
        /// Converts the specified string to snake_case format.
        /// </summary>
        /// <param name="value">The input string to convert. Can be null or empty.</param>
        /// <returns>A new string in snake_case format. Returns the original value if it is null or empty.</returns>
        private static string ToSnakeCase(string value)
        {
            if(string.IsNullOrEmpty(value)) return value;

            List<char> result = [];

            for(int i = 0; i < value.Length; i++)
            {
                char character = value[i];

                if(char.IsUpper(character))
                {
                    if(i > 0) result.Add('_');
                    result.Add(char.ToLower(character));
                }
                else
                {
                    result.Add(character);
                }
            }

            return new string(result.ToArray());
        }

        /// <summary>
        /// Applies global naming and type conventions to all entities in the model builder, including table names, column names, foreign key constraint names, index names, and GUID primary key columns.
        /// </summary>
        /// <remarks>
        /// This method standardizes entity and property naming to snake_case and configures GUID primary keys to use the 'uuid' column type with value generation on adding.
        /// It is intended to be called during model configuration to ensure consistent database schema conventions across the application.
        /// </remarks>
        /// <param name="builder">The model builder to which the global conventions are applied.</param>
        public static void ApplyGlobalConventions(this ModelBuilder builder)
        {
            foreach(IMutableEntityType entity in builder.Model.GetEntityTypes())
            {
                //* Table name:
                entity.SetTableName(ToSnakeCase(entity.GetTableName()!));

                //* Columns names:
                foreach(IMutableProperty property in entity.GetProperties())
                {
                    StoreObjectIdentifier storeObject = StoreObjectIdentifier.Table(entity.GetTableName()!);
                    property.SetColumnName(ToSnakeCase(property.GetColumnName(storeObject)!));
                }

                //* Foreign keys names:
                foreach(IMutableForeignKey fk in entity.GetForeignKeys())
                {
                    if(fk.GetConstraintName() is not null) fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()!));
                }

                //* Index names:
                foreach(IMutableIndex index in entity.GetIndexes())
                {
                    if(index.GetDatabaseName() is not null) index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()!));
                }

                //* Guid PK (uuid):
                foreach(IMutableProperty property in entity.GetProperties())
                {
                    if((property.ClrType == typeof(Guid)) && (property.IsPrimaryKey()))
                    {
                        property.SetColumnType("uuid");
                        property.ValueGenerated = ValueGenerated.OnAdd;
                    }
                }
            }

        }
    }
}