using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options;

/// <summary>
/// Represents additional options for adding a file to FilePond.
/// </summary>
public class FilePondAddFileOptions
{
    /// <summary>
    /// Sets the index at which the file should be added.
    /// </summary>
    /// <remarks>
    /// The index determines the position of the added file in the list of files.
    /// If not specified, the file is added at the end of the list.
    /// </remarks>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>
    /// If enabled, does not trigger AddFile event after adding this particular file.
    /// </summary>
    [JsonIgnore]
    public bool SilentAdd { get; set; }

    // TODO: Document other available options for adding a file.
}