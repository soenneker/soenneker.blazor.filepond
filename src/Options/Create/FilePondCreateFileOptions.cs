using Soenneker.Blazor.FilePond.Enums;
using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options.Create;

public class FilePondCreateFileOptions
{
    /// <summary>
    /// Set type to 'local' to indicate an already uploaded file
    /// </summary>
    [JsonPropertyName("type")]
    public FilePondFileOrigin? Type { get; set; }

    [JsonPropertyName("file")]
    public FilePondOptionsFile? File { get; set; }
}