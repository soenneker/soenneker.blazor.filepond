using Ardalis.SmartEnum;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// An enum to use together with the FilePond status property to determine the current filepond instance status. <para/>
/// </summary>
public sealed class FilePondFileStatus : SmartEnum<FilePondFileStatus>
{
    /// <summary>
    /// Represents the initialization status of a FilePond file. <para/>
    /// This status is set when a file is initially added to FilePond but has not started processing or loading.
    /// </summary>
    public static readonly FilePondFileStatus Init = new(nameof(Init), 1);

    /// <summary>
    /// Represents the idle status of a FilePond file. <para/>
    /// In the idle state, a file is ready for user interaction. This state occurs after the initialization and before any processing or loading starts.
    /// </summary>
    public static readonly FilePondFileStatus Idle = new(nameof(Idle), 2);

    /// <summary>
    /// Represents the processing queued status of a FilePond file. <para/>
    /// This status is set when a file is in the queue to be processed but has not yet started processing.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingQueued = new(nameof(ProcessingQueued), 9);

    /// <summary>
    /// Represents the processing status of a FilePond file. <para/>
    /// This status indicates that the file is currently being processed, such as during image editing, filtering, or other transformations.
    /// </summary>
    public static readonly FilePondFileStatus Processing = new(nameof(Processing), 3);

    /// <summary>
    /// Represents the processing complete status of a FilePond file. <para/>
    /// This status is set when the processing of a file is successfully completed, and the processed file is ready for further actions or uploading.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingComplete = new(nameof(ProcessingComplete), 5);

    /// <summary>
    /// Represents the processing error status of a FilePond file. <para/>
    /// This status is set when an error occurs during the processing of a file, and the file cannot be processed successfully.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingError = new(nameof(ProcessingError), 6);

    /// <summary>
    /// Represents the processing revert error status of a FilePond file. <para/>
    /// This status is set when an error occurs while reverting the processing changes of a file.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingRevertError = new(nameof(ProcessingRevertError), 10);

    /// <summary>
    /// Represents the loading status of a FilePond file. <para/>
    /// This status is set when a file is in the process of being loaded, such as when retrieving an existing file from a server.
    /// </summary>
    public static readonly FilePondFileStatus Loading = new(nameof(Loading), 7);

    /// <summary>
    /// Represents the load error status of a FilePond file. <para/>
    /// This status is set when an error occurs while loading a file, and the file cannot be loaded successfully.
    /// </summary>
    public static readonly FilePondFileStatus LoadError = new(nameof(LoadError), 8);

    private FilePondFileStatus(string name, int value) : base(name, value)
    {
    }
}