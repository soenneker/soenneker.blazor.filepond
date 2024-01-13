using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Dtos;

public record FilePondFileRenameInfo
{
    [JsonPropertyName("basename")]
    public string? BaseName { get; set; }

    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}