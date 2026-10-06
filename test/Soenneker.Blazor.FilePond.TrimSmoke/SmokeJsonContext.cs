using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(SmokePayload))]
internal partial class SmokeJsonContext : JsonSerializerContext;
