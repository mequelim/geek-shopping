using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeekShopping.Email.Shared.Converters
{
    /// <summary>
    /// A custom JSON converter for serializing and deserializing <see cref="TimeOnly"/> objects.
    /// </summary>
    /// <remarks>
    /// This converter handles JSON serialization and deserialization for <see cref="TimeOnly"/> values, using a specific time format.
    /// The format ensures a consistent representation of time values in JSON documents.
    /// </remarks>
    /// <seealso cref="System.Text.Json.Serialization.JsonConverter{TimeOnly}" />
    /// <example>This converter can be registered in the <see cref="JsonSerializerOptions.Converters"/> collection to enable support for <see cref="TimeOnly"/> in JSON processing.</example>
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string TimeFormat = "HH:mm:ss";

        // Methods:
        /// <summary>
        /// Reads and converts the JSON string representation of a time value to a <see cref="TimeOnly"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read the JSON data from.</param>
        /// <param name="typeToConvert">The <see cref="Type"/> of the object being converted.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> used during deserialization.</param>
        /// <returns>A <see cref="TimeOnly"/> object that represents the time value from the JSON data.</returns>
        /// <exception cref="JsonException">Thrown when the JSON contains a time value that does not match the expected format.</exception>
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            return (!TimeOnly.TryParseExact(value, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly timeOnly))
                ? throw new JsonException($"Invalid time format. Expected \"{TimeFormat}\", got \"{value}\".")
                : timeOnly;
        }

        /// <summary>
        /// Writes a <see cref="TimeOnly"/> object as its JSON string representation using the format "HH:mm:ss".
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON data to.</param>
        /// <param name="value">The <see cref="TimeOnly"/> value to be converted to JSON.</param>
        /// <param name="options">The <see cref="JsonSerializerOptions"/> used during serialization.</param>
        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(TimeFormat));
        }
    }
}
