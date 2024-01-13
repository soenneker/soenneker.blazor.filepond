using Ardalis.SmartEnum;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// This enum contains the names for the different file origins.
/// </summary>
public sealed class FilePondFileOrigin : SmartEnum<FilePondFileOrigin>
{
    /// <summary>
    /// Represents a file item input by the user.
    /// </summary>
    public static readonly FilePondFileOrigin Input = new(nameof(Input), 1);

    /// <summary>
    /// Represents a file item restored from the server as a temporary file in the LIMBO state.
    /// </summary>
    public static readonly FilePondFileOrigin Limbo = new(nameof(Limbo), 2);

    /// <summary>
    /// Represents a file item that is a local server file, i.e., a file already uploaded and confirmed
    /// but not located in the server's temporary uploads folder.
    /// </summary>
    public static readonly FilePondFileOrigin Local = new(nameof(Local), 3);

    private FilePondFileOrigin(string name, int value) : base(name, value)
    {
    }
}