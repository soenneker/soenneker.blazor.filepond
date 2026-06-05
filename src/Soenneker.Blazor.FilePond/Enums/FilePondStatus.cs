using Soenneker.Gen.EnumValues;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// An enum to use together with the File status property to determine the current file status.
/// </summary>
[EnumValue]
public sealed partial class FilePondStatus 
{
    /// <summary>
    /// The empty.
    /// </summary>
    public static readonly FilePondStatus Empty = new(0);
    /// <summary>
    /// The idle.
    /// </summary>
    public static readonly FilePondStatus Idle = new(1);
    /// <summary>
    /// The error.
    /// </summary>
    public static readonly FilePondStatus Error = new(2);
    /// <summary>
    /// The busy.
    /// </summary>
    public static readonly FilePondStatus Busy = new(3);
    /// <summary>
    /// The ready.
    /// </summary>
    public static readonly FilePondStatus Ready = new(4);
}
