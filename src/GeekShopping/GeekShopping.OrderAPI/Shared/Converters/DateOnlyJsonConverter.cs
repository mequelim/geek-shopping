using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeekShopping.OrderAPI.Shared.Converters
{
    /// <summary>
    /// A custom JSON converter for handling serialization and deserialization of <see cref="DateOnly"/> values.
    /// </summary>
    /// <remarks>
    /// This converter allows <see cref="DateOnly"/> to be serialized and deserialized as a JSON string, using a specific date format.
    /// It ensures seamless integration of <see cref="DateOnly"/> with the <c>System.Text.Json</c> library while adhering to specified formatting rules.
    /// </remarks>
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        // Methods:
        /// <summary>
        /// Reads and converts the JSON string representation of a date to a <see cref="DateOnly"/> value, using the specified date format.
        /// </summary>
        /// <remarks>
        /// The method expects the JSON string to match the required date format exactly.
        /// If the format does not match, a JsonException is thrown.
        /// </remarks>
        /// <param name="reader">
        /// The reader to read the JSON value from.
        /// The reader must be positioned at a JSON string representing a date.
        /// </param>
        /// <param name="typeToConvert">
        /// The type of the object to convert.
        /// This parameter is ignored for this converter.
        /// </param>
        /// <param name="options">
        /// The serialization options to use.
        /// This method does not use this parameter.
        /// </param>
        /// <returns>A <see cref="DateOnly"/> value parsed from the JSON string.</returns>
        /// <exception cref="JsonException">Thrown if the JSON value is not in the expected date format or cannot be parsed as a <see cref="DateOnly"/> value.</exception>
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            return (!DateOnly.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateOnly))
                ? throw new JsonException($"Invalid date format: expected \"{DateFormat}\", got \"{value}\"")
                : dateOnly;
        }

        /// <summary>
        /// Writes a <see cref="DateOnly"/> object as a JSON string using a specific date format.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="DateOnly"/> object to convert to a JSON string.</param>
        /// <param name="options">The serializer options to use.</param>
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
    }
}