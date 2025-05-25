using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserService.Utils.JsonConverters
{
    public class StringToNullableGuidConverter : JsonConverter<Guid?>
    {
        public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                
                // Handle empty string or whitespace
                if (string.IsNullOrWhiteSpace(stringValue))
                    return null;
                
                // Try to parse as Guid
                if (Guid.TryParse(stringValue, out Guid result))
                    return result;
                
                // If parsing failed, return null
                return null;
            }

            throw new JsonException($"Unexpected token {reader.TokenType} when parsing nullable Guid.");
        }

        public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.Value.ToString());
        }
    }
} 