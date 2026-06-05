using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Dtos;

/// <summary>
/// Represents the file pond file rename info record.
/// </summary>
public sealed record FilePondFileRenameInfo
{
    /// <summary>
    /// Gets or sets base name.
    /// </summary>
    [JsonPropertyName("basename")]
    public string? BaseName { get; set; }

    /// <summary>
    /// Gets or sets extension.
    /// </summary>
    [JsonPropertyName("extension")]
    public string? Extension { get; set; }

    /// <summary>
    /// Gets or sets name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
