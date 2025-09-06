using Intellenum;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// An enum to use together with the File status property to determine the current file status.
/// </summary>
[Intellenum]
public sealed partial class FilePondStatus 
{
    public static readonly FilePondStatus Empty = new(0);
    public static readonly FilePondStatus Idle = new(1);
    public static readonly FilePondStatus Error = new(2);
    public static readonly FilePondStatus Busy = new(3);
    public static readonly FilePondStatus Ready = new(4);
}
