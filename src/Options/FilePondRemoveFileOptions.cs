using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options;

/// <summary>
/// Represents additional options for removing a file from FilePond.
/// </summary>
public class FilePondRemoveFileOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the file should be reverted after removal.
    /// </summary>
    /// <remarks>
    /// If set to <c>true</c>, the removal of the file will be followed by a reversion process.
    /// Reverting a file means restoring it to its original state before any processing.
    /// </remarks>
    [JsonPropertyName("revert")]
    public bool? Revert { get; set; }

    // TODO: Document other available options for removing a file.
}