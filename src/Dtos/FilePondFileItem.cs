using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Soenneker.Blazor.FilePond.Enums;

namespace Soenneker.Blazor.FilePond.Dtos;

/// <summary>
/// Represents a file item used in the FilePond library.
/// </summary>
public record FilePondFileItem
{
    /// <summary>
    /// Gets or sets the id of the file.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the server id of the file.
    /// </summary>
    [JsonPropertyName("serverId")]
    public string? ServerId { get; set; }

    /// <summary>
    /// Gets or sets the origin of the file, either input (added by user), limbo (temporary server file), or local (existing server file).
    /// </summary>
    [JsonPropertyName("origin")]
    [JsonConverter(typeof(SmartEnumValueConverter<FilePondFileOrigin, int>))]
    public FilePondFileOrigin? Origin { get; set; }


    /// <summary>
    /// Gets or sets the current status of the file. Use the FilePond.FileStatus enum to determine the status.
    /// </summary>
    [JsonPropertyName("status")]
    [JsonConverter(typeof(SmartEnumValueConverter<FilePondFileStatus, int>))]
    public FilePondFileStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets the file extension.
    /// </summary>
    [JsonPropertyName("fileExtension")]
    public string? FileExtension { get; set; }

    /// <summary>
    /// Gets or sets the size of the file.
    /// </summary>
    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the full name of the file.
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>
    /// Gets or sets the name of the file without extension.
    /// </summary>
    [JsonPropertyName("filenameWithoutExtension")]
    public string? FilenameWithoutExtension { get; set; }
}