using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options;

/// <summary>
/// To be used with the <see cref="FilePondOptions.Server"/> option.
/// </summary>
public class FilePondServerOptions
{
    /// <summary>
    /// Gets or sets the URL, which is the path to the endpoint. Required if the server option is used.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    /// <summary>
    /// Gets or sets the FilePond server endpoint options for the "process" operation.
    /// </summary>
    [JsonPropertyName("process")]
    public FilePondServerEndpointOptions? Process { get; set; }

    /// <summary>
    /// Gets or sets the FilePond server endpoint options for the "revert" operation.
    /// </summary>
    [JsonPropertyName("revert")]
    public FilePondServerEndpointOptions? Revert { get; set; }

    /// <summary>
    /// Gets or sets the FilePond server endpoint options for the "restore" operation.
    /// </summary>
    [JsonPropertyName("restore")]
    public FilePondServerEndpointOptions? Restore { get; set; }

    /// <summary>
    /// Gets or sets the FilePond server endpoint options for the "load" operation.
    /// </summary>
    [JsonPropertyName("load")]
    public FilePondServerEndpointOptions? Load { get; set; }

    /// <summary>
    /// Gets or sets the FilePond server endpoint options for the "fetch" operation.
    /// </summary>
    [JsonPropertyName("fetch")]
    public FilePondServerEndpointOptions? Fetch { get; set; }
}