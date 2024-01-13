using Ardalis.SmartEnum;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// An enum to use together with the File status property to determine the current file status.
/// </summary>
public sealed class FilePondStatus : SmartEnum<FilePondStatus>
{
    public static readonly FilePondStatus Empty = new(nameof(Empty), 0);
    public static readonly FilePondStatus Idle = new(nameof(Idle), 1);
    public static readonly FilePondStatus Error = new(nameof(Error), 2);
    public static readonly FilePondStatus Busy = new(nameof(Busy), 3);
    public static readonly FilePondStatus Ready = new(nameof(Ready), 4);

    private FilePondStatus(string name, int value) : base(name, value)
    {
    }
}