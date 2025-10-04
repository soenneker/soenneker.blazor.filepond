using Microsoft.AspNetCore.Components;
using Soenneker.Blazor.FilePond.Dtos;
using Soenneker.Blazor.FilePond.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Quark;

namespace Soenneker.Blazor.FilePond.Abstract;

/// <summary>
/// Represents a FilePond component in Blazor.
/// </summary>
public interface IFilePond : ICoreCancellableComponent
{
    /// <summary>
    /// Gets or sets the options for the FilePond component.
    /// </summary>
    FilePondOptions? Options { get; set; }

    /// <summary>
    /// Event occurs when the FilePond instance has been created and is ready.
    /// </summary>
    EventCallback OnInit { get; set; }

    /// <summary>
    /// Event occurs when the FilePond instance throws a warning. 
    /// Optionally receives file if the error is related to a file object.
    /// </summary>
    EventCallback<(FilePondError? Error, FilePondFileItem File, string Status)> OnWarning { get; set; }

    /// <summary>
    /// Event occurs when the FilePond instance throws an error. 
    /// Optionally receives file if the error is related to a file object.
    /// </summary>
    EventCallback<(FilePondError? Error, FilePondFileItem File, string Status)> OnError { get; set; }

    /// <summary>
    /// Event occurs when a file item has been created.
    /// </summary>
    EventCallback<FilePondFileItem> OnInitFile { get; set; }

    /// <summary>
    /// Event occurs when starting the file load process.
    /// </summary>
    EventCallback<FilePondFileItem> OnAddFileStart { get; set; }

    /// <summary>
    /// Event occurs during progress while loading a file.
    /// </summary>
    EventCallback<(FilePondFileItem File, int Progress)> OnAddFileProgress { get; set; }

    /// <summary>
    /// Event occurs if no error during the file load process.
    /// </summary>
    EventCallback<(FilePondError?, FilePondFileItem File)> OnAddFile { get; set; }

    /// <summary>
    /// Event occurs when starting the file processing.
    /// </summary>
    EventCallback<FilePondFileItem> OnProcessFileStart { get; set; }

    /// <summary>
    /// Event occurs during progress while processing a file.
    /// </summary>
    EventCallback<(FilePondFileItem File, int Progress)> OnProcessFileProgress { get; set; }

    /// <summary>
    /// Event occurs when the processing of a file is aborted.
    /// </summary>
    EventCallback<FilePondFileItem> OnProcessFileAbort { get; set; }

    /// <summary>
    /// Event occurs when the processing of a file is reverted.
    /// </summary>
    EventCallback<FilePondFileItem> OnProcessFileRevert { get; set; }

    /// <summary>
    /// Event occurs if no error during the file processing.
    /// </summary>
    EventCallback<(FilePondError? Error, FilePondFileItem File)> OnProcessFile { get; set; }

    /// <summary>
    /// Event occurs when all files in the list have been processed.
    /// </summary>
    EventCallback OnProcessFiles { get; set; }

    /// <summary>
    /// Event occurs when a file has been removed.
    /// </summary>
    EventCallback<(FilePondError? Error, FilePondFileItem File)> OnRemoveFile { get; set; }

    /// <summary>
    /// Event occurs when a file has been transformed by the transform plugin or another plugin subscribing to the prepare_output filter.
    /// It receives the file item and the output data.
    /// </summary>
    EventCallback<(FilePondFileItem File, object Output)> OnPrepareFile { get; set; }

    /// <summary>
    /// Event occurs when a file has been added or removed, receives a list of file items.
    /// </summary>
    EventCallback<List<FilePondFileItem>> OnUpdateFiles { get; set; }

    /// <summary>
    /// Event occurs when a file is clicked or tapped.
    /// </summary>
    EventCallback<FilePondFileItem> OnActivateFile { get; set; }

    /// <summary>
    /// Event occurs when the files list has been reordered, receives the current list of files (reordered) plus file origin and target index.
    /// </summary>
    EventCallback<(List<FilePondFileItem> Files, int Origin, int Target)> OnReorderFiles { get; set; }

    EventCallback<FilePondFileItem> OnServerLoad { get; set; }

    EventCallback<FilePondFileItem> OnServerError { get; set; }

    EventCallback<FilePondFileItem> OnServerData { get; set; }

    /// <summary>
    /// FilePond is about to allow this item to be dropped, it can be a URL or a File object. Return true or false depending on if you want to allow the item to be dropped.
    /// </summary>
    Func<FilePondFileItem, ValueTask<bool>>? OnBeforeDropFile { get; set; }

    /// <summary>
    /// FilePond is about to add this file, return false to prevent adding it, or return a Promise and resolve with true or false.
    /// </summary>
    Func<FilePondFileItem, ValueTask<bool>>? OnBeforeAddFile { get; set; }

    /// <summary>
    /// FilePond is about to remove this file, return false to prevent removal, or return a Promise and resolve with true or false.
    /// </summary>
    Func<FilePondFileItem, ValueTask<bool>>? OnBeforeRemoveFile { get; set; }

    /// <summary>
    /// A function that receives an objecting containing file information like basename, extension and name.
    /// </summary>
    Func<FilePondFileRenameInfo, ValueTask<string>>? OnFileRename { get; set; }

    /// <summary>
    /// The list of <see cref="FilePondFileItem"/> objects representing files associated with the FilePond instance.
    /// </summary>
    /// <value>
    /// The list of files currently managed by the FilePond instance.
    /// </value>
    List<FilePondFileItem> Files { get; }

    /// <summary>
    /// Creates a FilePond instance with the specified options.
    /// </summary>
    /// <param name="options">Options for configuring the FilePond instance (optional).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Create(FilePondOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Overrides multiple options at once.
    /// </summary>
    /// <param name="options">An object containing the options to override.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetOptions(FilePondOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a file to FilePond.
    /// </summary>
    /// <param name="uriOrBase64EncodedData">The file elementReference.</param>
    /// <param name="silentAdd">If true, don't trigger AddFile event</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddFile(string uriOrBase64EncodedData, bool silentAdd, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a file to FilePond.
    /// </summary>
    /// <param name="uriOrBase64EncodedData">The file elementReference.</param>
    /// <param name="options">Additional options for the added file.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddFile(string uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask AddFile(Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a limbo file to FilePond. A limbo file is a placeholder file that doesn't contain actual file data.
    /// </summary>
    /// <param name="filename">The name of the file to be added as a limbo file.</param>
    /// <param name="options">Additional options for the added file, including MimeType.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddLimboFile(string filename, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple files to FilePond.
    /// </summary>
    /// <param name="uriOrBase64EncodedData">An array containing file sources.</param>
    /// <param name="options">Additional options for the added files.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask AddFiles(List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask RemoveFile(FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask RemoveFile(int index, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a file from FilePond at the specified index using the provided options.
    /// </summary>
    /// <param name="fileId">The ID of the file to be removed.</param>
    /// <param name="options">Additional options for the file removal (optional).</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    ValueTask RemoveFile(string fileId, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all files or files matching the query.
    /// </summary>
    /// <param name="query">The query to match files to be removed.</param>
    /// <param name="options"></param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask RemoveFiles(object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts processing the file matching the given query.
    /// </summary>
    /// <param name="query">The query to match the file to be processed.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ProcessFile(object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts processing all files or files matching the query.
    /// </summary>
    /// <param name="query">The query to match files to be processed.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ProcessFiles(object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts preparing the file matching the given query, returns a Promise.
    /// </summary>
    /// <param name="query">The query to match the file to be prepared.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, with the result being an object { file, output }.</returns>
    ValueTask<object> PrepareFile(object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts preparing all output files or files matching the query, returns a Promise.
    /// </summary>
    /// <param name="query">The query to match files to be prepared.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, with the result being an array of file prepare output objects { file, output }.</returns>
    ValueTask<object[]> PrepareFiles(object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first file.
    /// </summary>
    /// <param name="forceInterop">A flag indicating whether to force interoperation.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, with the result being a <see cref="FilePondFileItem"/> or null.</returns>
    ValueTask<FilePondFileItem?> GetFile(bool forceInterop = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the file at an index.
    /// </summary>
    /// <param name="index">The index of the file to retrieve.</param>
    /// <param name="forceInterop">A flag indicating whether to force interoperation.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, with the result being a <see cref="FilePondFileItem"/> or null.</returns>
    ValueTask<FilePondFileItem?> GetFile(int index, bool forceInterop = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the file with a given id.
    /// </summary>
    /// <param name="fileId">The ID of the file to retrieve.</param>
    /// <param name="forceInterop">A flag indicating whether to force interoperation.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, with the result being a <see cref="FilePondFileItem"/> or null.</returns>
    ValueTask<FilePondFileItem?> GetFile(string fileId, bool forceInterop = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all files.
    /// </summary>
    /// <param name="forceInterop">A flag indicating whether to force interoperation.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, with the result being a list of <see cref="FilePondFileItem"/> or null.</returns>
    ValueTask<List<FilePondFileItem>?> GetFiles(bool forceInterop = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens the browse file dialog. Note that this only works if the user initiated the call stack that ends up calling the browse method.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Browse(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sorts files in the list using the supplied compare function.
    /// </summary>
    /// <param name="compareFunctionName">The compare function used for sorting.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Sort(string compareFunctionName, CancellationToken cancellationToken = default);

    ValueTask MoveFile(object query, int index, CancellationToken cancellationToken = default);

    ValueTask Destroy(CancellationToken cancellationToken = default);

    /// <summary>
    /// Destroys the element and initializes a new one in its place.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    ValueTask ReInitialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the file with the specified identifier within the given FilePond instance.
    /// This method returns the transformed file data when plugins like ImageResize or ImageTransform are used, falling back to the original file if no transformation is available.
    /// </summary>
    /// <param name="maxAllowedSize">
    /// (Optional) The maximum allowed size of the stream in bytes. Defaults to options.MaxFileSize, otherwise defaults to 2MB.
    /// </param>
    /// <param name="cancellationToken">
    /// (Optional) A <see cref="CancellationToken"/> to observe while waiting for the asynchronous operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{Stream}"/> representing the asynchronous operation of obtaining a stream for the specified file.
    /// If the file does not exist or if an error occurs, the result may be <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Use this method to get a stream for a specific file within a FilePond instance. The stream can be used to
    /// perform additional operations, such as reading the file contents or processing the file in-memory.
    /// This method automatically returns transformed file data when plugins are used, making it the preferred method for most use cases.
    /// </remarks>
    ValueTask<Stream?> GetStreamForFile(long? maxAllowedSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the file at the specified index within the given FilePond instance.
    /// This method returns the transformed file data when plugins like ImageResize or ImageTransform are used, falling back to the original file if no transformation is available.
    /// </summary>
    /// <param name="index">
    /// The index of the file for which to retrieve the stream.
    /// </param>
    /// <param name="maxAllowedSize">
    /// (Optional) The maximum allowed size of the stream in bytes. Defaults to options.MaxFileSize, otherwise defaults to 2MB.
    /// </param>
    /// <param name="cancellationToken">
    /// (Optional) A <see cref="CancellationToken"/> to observe while waiting for the asynchronous operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{Stream}"/> representing the asynchronous operation of obtaining a stream for the specified file.
    /// If the file does not exist or if an error occurs, the result may be <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Use this method to get a stream for a specific file within a FilePond instance based on its index. 
    /// The stream can be used to perform additional operations, such as reading the file contents or processing the file in-memory.
    /// This method automatically returns transformed file data when plugins are used, making it the preferred method for most use cases.
    /// </remarks>
    ValueTask<Stream?> GetStreamForFile(int index, long? maxAllowedSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the file with the specified identifier within the given FilePond instance.
    /// This method returns the transformed file data when plugins like ImageResize or ImageTransform are used, falling back to the original file if no transformation is available.
    /// </summary>
    /// <param name="fileId">
    /// The unique identifier of the file for which to retrieve the stream.
    /// </param>
    /// <param name="maxAllowedSize">
    /// (Optional) The maximum allowed size of the stream in bytes. Defaults to options.MaxFileSize, otherwise defaults to 2MB.
    /// </param>
    /// <param name="cancellationToken">
    /// (Optional) A <see cref="CancellationToken"/> to observe while waiting for the asynchronous operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{Stream}"/> representing the asynchronous operation of obtaining a stream for the specified file.
    /// If the file does not exist or if an error occurs, the result may be <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Use this method to get a stream for a specific file within a FilePond instance. The stream can be used to
    /// perform additional operations, such as reading the file contents or processing the file in-memory.
    /// This method automatically returns transformed file data when plugins are used, making it the preferred method for most use cases.
    /// </remarks>
    ValueTask<Stream?> GetStreamForFile(string fileId, long? maxAllowedSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of streams, each representing data, with a total size not exceeding the specified maximum allowed size.
    /// </summary>
    /// <param name="maxAllowedSize">
    /// (Optional) The maximum allowed size of the stream in bytes. Defaults to options.MaxFileSize, otherwise defaults to 2MB.
    /// </param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation.
    /// The result is a list of <see cref="Stream"/> objects, each representing a portion of the data.
    /// </returns>
    /// <remarks>
    /// The method retrieves streams of data with a total size not exceeding the specified maximum allowed size.
    /// The data is segmented into multiple streams to meet the size constraint.
    /// </remarks>
    ValueTask<List<Stream>> GetAllStreams(long? maxAllowedSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the original (untransformed) file with the specified identifier within the given FilePond instance.
    /// This method explicitly returns the original file data, bypassing any transformations applied by plugins.
    /// </summary>
    /// <param name="maxAllowedSize">
    /// (Optional) The maximum allowed size of the stream in bytes. Defaults to options.MaxFileSize, otherwise defaults to 2MB.
    /// </param>
    /// <param name="cancellationToken">
    /// (Optional) A <see cref="CancellationToken"/> to observe while waiting for the asynchronous operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{Stream}"/> representing the asynchronous operation of obtaining a stream for the original file.
    /// If the file does not exist or if an error occurs, the result may be <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Use this method to get a stream for the original (untransformed) file within a FilePond instance. The stream can be used to
    /// perform additional operations, such as reading the original file contents or processing the file in-memory.
    /// This method explicitly bypasses any transformations applied by plugins like ImageResize or ImageTransform.
    /// </remarks>
    ValueTask<Stream?> GetOriginalStreamForFile(long? maxAllowedSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the original (untransformed) file at the specified index within the given FilePond instance.
    /// This method explicitly returns the original file data, bypassing any transformations applied by plugins.
    /// </summary>
    /// <param name="index">
    /// The index of the file for which to retrieve the original stream.
    /// </param>
    /// <param name="maxAllowedSize">
    /// (Optional) The maximum allowed size of the stream in bytes. Defaults to options.MaxFileSize, otherwise defaults to 2MB.
    /// </param>
    /// <param name="cancellationToken">
    /// (Optional) A <see cref="CancellationToken"/> to observe while waiting for the asynchronous operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{Stream}"/> representing the asynchronous operation of obtaining a stream for the original file.
    /// If the file does not exist or if an error occurs, the result may be <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Use this method to get a stream for the original (untransformed) file within a FilePond instance based on its index. 
    /// The stream can be used to perform additional operations, such as reading the original file contents or processing the file in-memory.
    /// This method explicitly bypasses any transformations applied by plugins like ImageResize or ImageTransform.
    /// </remarks>
    ValueTask<Stream?> GetOriginalStreamForFile(int index, long? maxAllowedSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the original (untransformed) file with the specified identifier within the given FilePond instance.
    /// This method explicitly returns the original file data, bypassing any transformations applied by plugins.
    /// </summary>
    /// <param name="fileId">
    /// The unique identifier of the file for which to retrieve the original stream.
    /// </param>
    /// <param name="maxAllowedSize">
    /// (Optional) The maximum allowed size of the stream in bytes. Defaults to options.MaxFileSize, otherwise defaults to 2MB.
    /// </param>
    /// <param name="cancellationToken">
    /// (Optional) A <see cref="CancellationToken"/> to observe while waiting for the asynchronous operation to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{Stream}"/> representing the asynchronous operation of obtaining a stream for the original file.
    /// If the file does not exist or if an error occurs, the result may be <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Use this method to get a stream for the original (untransformed) file within a FilePond instance. The stream can be used to
    /// perform additional operations, such as reading the original file contents or processing the file in-memory.
    /// This method explicitly bypasses any transformations applied by plugins like ImageResize or ImageTransform.
    /// </remarks>
    ValueTask<Stream?> GetOriginalStreamForFile(string fileId, long? maxAllowedSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the validation state of the FilePond component.
    /// </summary>
    /// <param name="isValid">Whether the component is valid.</param>
    /// <param name="errorMessage">Optional error message to display.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetValidationState(bool isValid, string? errorMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the validation state to neutral (not explicitly validated).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask ResetValidationState(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of a specific file.
    /// </summary>
    /// <param name="fileId">The ID of the file to set success state for.</param>
    /// <param name="isSuccess">Whether the file is successful.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetFileSuccess(string fileId, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of a file at a specific index.
    /// </summary>
    /// <param name="fileIndex">The index of the file to set success state for.</param>
    /// <param name="isSuccess">Whether the file is successful.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetFileSuccess(int fileIndex, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of all files.
    /// </summary>
    /// <param name="isSuccess">Whether all files are successful.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetAllFilesSuccess(bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of a file when it becomes ready.
    /// </summary>
    /// <param name="fileId">The ID of the file to set success state for when ready.</param>
    /// <param name="isSuccess">Whether the file is successful.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetFileSuccessWhenReady(string fileId, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of all files when they become ready.
    /// </summary>
    /// <param name="isSuccess">Whether all files are successful.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SetAllFilesSuccessWhenReady(bool isSuccess = true, CancellationToken cancellationToken = default);
}
