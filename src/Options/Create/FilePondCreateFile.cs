using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options.Create;

public class FilePondCreateFile
{
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("options")]
    public FilePondCreateFileOptions? Options { get; set; }
}