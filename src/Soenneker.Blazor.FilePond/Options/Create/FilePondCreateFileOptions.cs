using Soenneker.Blazor.FilePond.Enums;
using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options.Create;

/// <summary>
/// Represents the file pond create file options.
/// </summary>
public sealed class FilePondCreateFileOptions
{
    /// <summary>
    /// Set type to 'local' to indicate an already uploaded file
    /// </summary>
    [JsonPropertyName("type")]
    public FilePondFileOrigin? Type { get; set; }

    /// <summary>
    /// Gets or sets file.
    /// </summary>
    [JsonPropertyName("file")]
    public FilePondOptionsFile? File { get; set; }
}
