using Soenneker.Gen.EnumValues;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// This enum contains the names for the different file origins.
/// </summary>
[EnumValue]
public sealed partial class FilePondFileOrigin
{
    // Prevent JSON source generation from assuming an implicit public constructor.
    private FilePondFileOrigin() => throw new System.NotSupportedException("Use a declared enum value.");

    /// <summary>
    /// Represents a file item input by the user.
    /// </summary>
    public static readonly FilePondFileOrigin Input = new(1);

    /// <summary>
    /// Represents a file item restored from the server as a temporary file in the LIMBO state.
    /// </summary>
    public static readonly FilePondFileOrigin Limbo = new(2);

    /// <summary>
    /// Represents a file item that is a local server file, i.e., a file already uploaded and confirmed
    /// but not located in the server's temporary uploads folder.
    /// </summary>
    public static readonly FilePondFileOrigin Local = new(3);
}
