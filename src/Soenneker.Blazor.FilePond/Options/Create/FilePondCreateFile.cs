using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options.Create;

/// <summary>
/// Represents the file pond create file.
/// </summary>
public sealed class FilePondCreateFile
{
    /// <summary>
    /// Gets or sets source.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets options.
    /// </summary>
    [JsonPropertyName("options")]
    public FilePondCreateFileOptions? Options { get; set; }
}
