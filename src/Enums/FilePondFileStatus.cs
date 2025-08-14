using Intellenum;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// An enum to use together with the FilePond status property to determine the current filepond instance status. <para/>
/// </summary>
[Intellenum]
public sealed partial class FilePondFileStatus
{
    /// <summary>
    /// Represents the initialization status of a FilePond file. <para/>
    /// This status is set when a file is initially added to FilePond but has not started processing or loading.
    /// </summary>
    public static readonly FilePondFileStatus Init = new(0);

    /// <summary>
    /// Represents the idle status of a FilePond file. <para/>
    /// In the idle state, a file is ready for user interaction. This state occurs after the initialization and before any processing or loading starts.
    /// </summary>
    public static readonly FilePondFileStatus Idle = new(1);

    /// <summary>
    /// Represents the processing queued status of a FilePond file. <para/>
    /// This status is set when a file is in the queue to be processed but has not yet started processing.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingQueued = new(2);

    /// <summary>
    /// Represents the processing status of a FilePond file. <para/>
    /// This status indicates that the file is currently being processed, such as during image editing, filtering, or other transformations.
    /// </summary>
    public static readonly FilePondFileStatus Processing = new(3);

    /// <summary>
    /// Represents the processing complete status of a FilePond file. <para/>
    /// This status is set when the processing of a file is successfully completed, and the processed file is ready for further actions or uploading.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingComplete = new(4);

    /// <summary>
    /// Represents the processing error status of a FilePond file. <para/>
    /// This status is set when an error occurs during the processing of a file, and the file cannot be processed successfully.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingError = new(5);

    /// <summary>
    /// Represents the processing revert error status of a FilePond file. <para/>
    /// This status is set when an error occurs while reverting the processing changes of a file.
    /// </summary>
    public static readonly FilePondFileStatus ProcessingRevertError = new(6);

    /// <summary>
    /// Represents the loading status of a FilePond file. <para/>
    /// This status is set when a file is in the process of being loaded, such as when retrieving an existing file from a server.
    /// </summary>
    public static readonly FilePondFileStatus Loading = new(7);

    /// <summary>
    /// Represents the load error status of a FilePond file. <para/>
    /// This status is set when an error occurs while loading a file, and the file cannot be loaded successfully.
    /// </summary>
    public static readonly FilePondFileStatus LoadError = new(8);
}