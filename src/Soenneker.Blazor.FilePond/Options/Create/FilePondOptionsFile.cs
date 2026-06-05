using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options.Create;

/// <summary>
/// Represents the file pond options file.
/// </summary>
public sealed class FilePondOptionsFile
{
    /// <summary>
    /// Gets or sets name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets size.
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; set; }

    /// <summary>
    /// Gets or sets type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}