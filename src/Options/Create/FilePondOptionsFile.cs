using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options.Create;

public class FilePondOptionsFile
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}