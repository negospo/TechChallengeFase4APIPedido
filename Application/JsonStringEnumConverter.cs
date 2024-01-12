﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application
{
    public class JsonStringEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            if (Enum.TryParse<T>(value, ignoreCase: true, out T result))
            {
                return result;
            }

            throw new JsonException($"Value '{value}' is not valid for enum type {typeof(T)}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
