using Soenneker.Blazor.FilePond.Dtos;
using Soenneker.Blazor.FilePond.Enums;
using Soenneker.Blazor.FilePond.Options;
using Soenneker.Blazor.FilePond.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Soenneker.Blazor.FilePond;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, ReadCommentHandling = JsonCommentHandling.Skip, UseStringEnumConverter = true, Converters = new[] { typeof(FilePondFileOriginMetadataConverter), typeof(FilePondFileStatusMetadataConverter), typeof(FilePondPluginTypeMetadataConverter), typeof(FilePondStatusMetadataConverter) })]
[JsonSerializable(typeof(FilePondFileItem))]
[JsonSerializable(typeof(FilePondOptions))]
[JsonSerializable(typeof(List<FilePondFileItem>))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(System.Text.Json.JsonElement))]
[JsonSerializable(typeof(System.Collections.Generic.Dictionary<string, object?>))]
[JsonSerializable(typeof(System.Collections.Generic.List<object?>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(object[]))]
internal partial class LibraryJsonContext : JsonSerializerContext
{
    internal static JsonTypeInfo<T> Get<T>() =>
        (JsonTypeInfo<T>)(Default.GetTypeInfo(typeof(T)) ?? throw new NotSupportedException($"No generated JSON metadata for {typeof(T)}."));

    internal static JsonSerializerOptions WithContext(JsonSerializerContext? additionalContext)
    {
        JsonSerializerOptions defaults = Get<object>().Options;
        if (additionalContext is null)
            return defaults;
        var options = new JsonSerializerOptions(defaults)
        {
            TypeInfoResolver = JsonTypeInfoResolver.Combine(defaults.TypeInfoResolver!, additionalContext)
        };
        options.MakeReadOnly();
        return options;
    }
}

internal sealed class FilePondFileOriginMetadataConverter : JsonConverter<Soenneker.Blazor.FilePond.Enums.FilePondFileOrigin>
{
    public override Soenneker.Blazor.FilePond.Enums.FilePondFileOrigin Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int rawValue) && Soenneker.Blazor.FilePond.Enums.FilePondFileOrigin.TryFromValue(rawValue, out var value) ? value : throw new JsonException("Unknown FilePondFileOrigin value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Blazor.FilePond.Enums.FilePondFileOrigin value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}

internal sealed class FilePondFileStatusMetadataConverter : JsonConverter<Soenneker.Blazor.FilePond.Enums.FilePondFileStatus>
{
    public override Soenneker.Blazor.FilePond.Enums.FilePondFileStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int rawValue) && Soenneker.Blazor.FilePond.Enums.FilePondFileStatus.TryFromValue(rawValue, out var value) ? value : throw new JsonException("Unknown FilePondFileStatus value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Blazor.FilePond.Enums.FilePondFileStatus value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}

internal sealed class FilePondPluginTypeMetadataConverter : JsonConverter<Soenneker.Blazor.FilePond.Enums.FilePondPluginType>
{
    public override Soenneker.Blazor.FilePond.Enums.FilePondPluginType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.String && Soenneker.Blazor.FilePond.Enums.FilePondPluginType.TryFromValue(reader.GetString(), out var value) ? value : throw new JsonException("Unknown FilePondPluginType value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Blazor.FilePond.Enums.FilePondPluginType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

internal sealed class FilePondStatusMetadataConverter : JsonConverter<Soenneker.Blazor.FilePond.Enums.FilePondStatus>
{
    public override Soenneker.Blazor.FilePond.Enums.FilePondStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int rawValue) && Soenneker.Blazor.FilePond.Enums.FilePondStatus.TryFromValue(rawValue, out var value) ? value : throw new JsonException("Unknown FilePondStatus value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Blazor.FilePond.Enums.FilePondStatus value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.Value);
}
