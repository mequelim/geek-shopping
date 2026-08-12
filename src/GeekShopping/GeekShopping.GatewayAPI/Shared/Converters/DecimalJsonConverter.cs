using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeekShopping.GatewayAPI.Shared.Converters
{
    /// <summary>
    /// A custom JSON converter for handling the serialization and deserialization of decimal values.
    /// This converter ensures proper parsing and formatting of decimal data, particularly handling cases where the decimal value might include culture-specific formatting.
    /// </summary>
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        /// <summary>
        /// Reads and converts the JSON to a decimal value.
        /// </summary>
        /// <param name="reader">The reader to retrieve the JSON data from.</param>
        /// <param name="typeToConvert">The type of the object to convert.</param>
        /// <param name="options">Optional JSON serialization options.</param>
        /// <return>The converted decimal value.</return>
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType.Equals(JsonTokenType.Number)) return reader.GetDecimal();

            string? value = reader.GetString();

            if(string.IsNullOrEmpty(value)) return 0;

            value = value.Replace(",", ".");

            return decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Writes a decimal value to JSON.
        /// </summary>
        /// <param name="writer">The writer to write the JSON data to.</param>
        /// <param name="value">The decimal value to be serialized.</param>
        /// <param name="options">Optional JSON serialization options.</param>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
    }
}