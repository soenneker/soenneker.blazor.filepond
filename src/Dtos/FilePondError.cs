using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Dtos;

public record FilePondError
{
    /// <summary>
    /// The main error information.
    /// </summary>
    [JsonPropertyName("main")]
    public string? Main { get; set; }

    /// <summary>
    /// The sub error information.
    /// </summary>
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }
}