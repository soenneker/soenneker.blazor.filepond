using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options;

/// <summary>
/// To be used with the <see cref="FilePondOptions.Server"/> option.
/// </summary>
public class FilePondServerEndpointOptions
{
    /// <summary>
    /// Gets or sets the URL, which is the path to the endpoint. Required if the server option is used.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    /// <summary>
    /// Gets or sets the path to the endpoint.
    /// </summary>
    [JsonPropertyName("path")]
    public string? PathToEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the request method to use.
    /// </summary>
    [JsonPropertyName("method")]
    public string? RequestMethod { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to toggle the XMLHttpRequest withCredentials on or off.
    /// </summary>
    [JsonPropertyName("withCredentials")]
    public bool WithCredentials { get; set; }

    /// <summary>
    /// Gets or sets an object containing additional headers to send, or a function that returns a header object.
    /// </summary>
    [JsonPropertyName("headers")]
    public object? AdditionalHeaders { get; set; }

    /// <summary>
    /// Gets or sets the timeout for this action.
    /// </summary>
    [JsonPropertyName("timeout")]
    public int? Timeout { get; set; }
}