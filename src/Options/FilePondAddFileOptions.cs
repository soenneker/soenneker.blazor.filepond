using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options;

/// <summary>
/// Represents additional options for adding a file to FilePond.
/// </summary>
public sealed class FilePondAddFileOptions
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

    /// <summary>
    /// Gets or sets a value indicating whether to show file size information for this specific file.
    /// When set to false, the file size will be hidden by applying CSS display: none to the file's .filepond--file-info-sub element.
    /// </summary>
    [JsonIgnore]
    public bool ShowFileSize { get; set; } = true;

    // TODO: Document other available options for adding a file.
}