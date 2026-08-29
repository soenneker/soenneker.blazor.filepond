using System.Threading.Tasks;
using Soenneker.Blazor.FilePond.Options;
using Soenneker.Blazor.FilePond.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Soenneker.Blazor.Utils.EventListeningInterop.Abstract;
using System;
using Soenneker.Blazor.FilePond.Enums;
using Soenneker.Blazor.FilePond.Constants;

namespace Soenneker.Blazor.FilePond.Abstract;

/// <summary>
/// A Blazor interop library for the file upload library FilePond.
/// </summary>
public interface IFilePondInterop : IEventListeningInterop, IAsyncDisposable
{
    /// <summary>
    /// Initializes the file pond so it is ready for use.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the file pond is ready for use.</returns>
    ValueTask Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a FilePond instance for the specified HTML element with optional configuration options.
    /// </summary>
    /// <param name="elementId">The unique identifier for the HTML element, used to associate the FilePond instance with the element.</param>
    /// <param name="options">(Optional) Configuration options for customizing the behavior of the FilePond instance.</param>
    /// <param name="useBlazorServerProcess">Whether FilePond should install the Blazor-driven server.process bridge.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that completes when the create operation is complete.</returns>
    ValueTask Create(string elementId, FilePondOptions? options = null, bool useBlazorServerProcess = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Overrides multiple options at once.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="options">An object containing the options to override.</param>
    /// <param name="useBlazorServerProcess">Whether FilePond should install the Blazor-driven server.process bridge.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the options has been stored.</returns>
    ValueTask SetOptions(string elementId, FilePondOptions options, bool useBlazorServerProcess = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a file to FilePond.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="uriOrBase64EncodedData">The file source, either a URI or base64-encoded data.</param>
    /// <param name="addFileOptions">Additional options for the added file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the file addition is complete.</returns>
    ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? addFileOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a file to FilePond from a stream.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="stream">Stream for the add file operation.</param>
    /// <param name="options">Additional options for the added file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the file addition is complete.</returns>
    ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a limbo file to FilePond. A limbo file is a placeholder file that doesn't contain actual file data.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="filename">The name of the file to be added as a limbo file.</param>
    /// <param name="options">Additional options for the added file, including MimeType.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the limbo file addition is complete.</returns>
    ValueTask AddLimboFile(string elementId, string filename, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple files to FilePond.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="uris">An array or a FileList containing file sources.</param>
    /// <param name="addFileOptions">Additional options for the added files.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the files addition is complete.</returns>
    ValueTask AddFiles(string elementId, List<string> uris, FilePondAddFileOptions? addFileOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a file with the specified query associated with the specified element.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="query">CSS media-query expression to evaluate against the current viewport.</param>
    /// <param name="options">Options for removing the file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the file removal is complete.</returns>
    ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all files or files matching the query.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="query">CSS media-query expression to evaluate against the current viewport.</param>
    /// <param name="options">Options for removing the files.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the files removal is complete.</returns>
    ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts processing the file matching the given query.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="query">CSS media-query expression to evaluate against the current viewport.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes after the file pond has started.</returns>
    ValueTask ProcessFile(string elementId, object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts processing all files or files matching the query.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="query">CSS media-query expression to evaluate against the current viewport.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes after the file pond has started.</returns>
    ValueTask ProcessFiles(string elementId, object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts preparing the file matching the given query, returns a Promise.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The query to match the file to be prepared.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A Promise resolved with the file item and the output file { file, output }.</returns>
    ValueTask<object> PrepareFile(string elementId, object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts preparing all output files or files matching the query, returns a Promise.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The query to match files to be prepared.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A Promise resolved with an array of file prepare output objects { file, output }.</returns>
    ValueTask<object[]> PrepareFiles(string elementId, object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the file with the specified query with the specified FilePond element.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The fileId of the file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A <see cref="ValueTask{FilePondFileItem}"/> representing the asynchronous operation of obtaining the file with the specified fileId. If no such file exists, the result may be <c>null</c>.</returns>
    ValueTask<FilePondFileItem?> GetFile(string elementId, object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all files associated with the specified FilePond element.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A list representing the asynchronous operation of obtaining a list of all files. If no files exist, the result may be an empty list.</returns>
    ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens the browse file dialog. Note that this only works if the user initiated the callstack that ends up calling the browse method.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the browse operation is complete.</returns>
    ValueTask Browse(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sorts files in the list using the supplied compare function.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="compareFunctionName">Name of the compare function to target.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the sort operation is complete.</returns>
    ValueTask Sort(string elementId, string compareFunctionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves the file to a new index in the files array.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="query">CSS media-query expression to evaluate against the current viewport.</param>
    /// <param name="index">Zero-based position of the target item.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the move file operation is complete.</returns>
    ValueTask MoveFile(string elementId, object query, int index, CancellationToken cancellationToken = default);

    /// <summary>
    /// Destroys this FilePond instance.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the destroy operation is complete.</returns>
    ValueTask Destroy(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates observer.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the observer creation is complete.</returns>
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the value produced by enable Plugins.
    /// </summary>
    /// <param name="useCdn">Whether cdn.</param>
    /// <param name="filePondPluginTypes">file Pond Plugin Types to process.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the enable plugins operation is complete.</returns>
    ValueTask EnablePlugins(bool useCdn, List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the value produced by enable Other Plugins.
    /// </summary>
    /// <param name="useCdn">Whether cdn.</param>
    /// <param name="filePondOtherPlugins">file Pond Other Plugins to process.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the enable other plugins operation is complete.</returns>
    ValueTask EnableOtherPlugins(bool useCdn, List<string> filePondOtherPlugins, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the file with the specified identifier within the given FilePond instance.
    /// This method returns the transformed file data when plugins like ImageResize or ImageTransform are used, falling back to the original file if no transformation is available.
    /// The stream should be disposed after use.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element associated with the FilePond instance.</param>
    /// <param name="query">The unique identifier of the file for which to retrieve the stream.</param>
    /// <param name="maxAllowedSize">(Optional) The maximum allowed size of the stream in bytes. Defaults to 2MB.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task whose result is the requested stream.</returns>
    ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the original (untransformed) file with the specified identifier within the given FilePond instance.
    /// This method explicitly returns the original file data, bypassing any transformations applied by plugins.
    /// The stream should be disposed after use.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element associated with the FilePond instance.</param>
    /// <param name="query">The unique identifier of the file for which to retrieve the original stream.</param>
    /// <param name="maxAllowedSize">(Optional) The maximum allowed size of the stream in bytes. Defaults to 2MB.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task whose result is the requested stream.</returns>
    ValueTask<Stream?> GetOriginalStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of <see cref="Stream"/> objects for all files within the given FilePond instance. The streams should be disposed after use.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element associated with the FilePond instance.</param>
    /// <param name="maxAllowedSize">(Optional) The maximum allowed size. Defaults to 2MB.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task whose result is the collection returned by get All Streams.</returns>
    ValueTask<List<Stream>> GetAllStreams(string elementId, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves streams For Files.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element associated with the FilePond instance.</param>
    /// <param name="fileIds">The list of file identifiers for which to retrieve streams.</param>
    /// <param name="maxAllowedSize">(Optional) The maximum allowed size. Defaults to 2MB.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task whose result is the collection returned by get Streams For Files.</returns>
    ValueTask<List<Stream>> GetStreamsForFiles(string elementId, List<string> fileIds, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the validation state of the FilePond component, showing or hiding error styling and messages.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="isValid">Whether the FilePond is in a valid state.</param>
    /// <param name="errorMessage">Optional error message to display.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the validation state has been stored.</returns>
    ValueTask SetValidationState(string elementId, bool isValid, string? errorMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of a specific file by ID, making it appear green.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="fileId">Identifier of the file to target.</param>
    /// <param name="isSuccess">Whether the file should be marked as successful.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the file success has been stored.</returns>
    ValueTask SetFileSuccess(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of a specific file by index, making it appear green.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="fileIndex">File Index for the set file success operation.</param>
    /// <param name="isSuccess">Whether the file should be marked as successful.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the file success has been stored.</returns>
    ValueTask SetFileSuccess(string elementId, int fileIndex, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of all files in the FilePond, making them appear green.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="isSuccess">Whether all files should be marked as successful.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the all files success has been stored.</returns>
    ValueTask SetAllFilesSuccess(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of a specific file by ID when the file is fully processed and ready.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="fileId">Identifier of the file to target.</param>
    /// <param name="isSuccess">Whether the file should be marked as successful.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the file success when ready has been stored.</returns>
    ValueTask SetFileSuccessWhenReady(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the success state of all files when they are fully processed and ready.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="isSuccess">Whether all files should be marked as successful.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the all files success when ready has been stored.</returns>
    ValueTask SetAllFilesSuccessWhenReady(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reports progress for an active Blazor-driven <c>server.process</c> callback.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="processId">The unique ID for the active process callback.</param>
    /// <param name="isLengthComputable">Whether the upload length can be computed.</param>
    /// <param name="loaded">Loaded for the report server process progress operation.</param>
    /// <param name="total">Total for the report server process progress operation.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    /// <returns>A task that completes when the report server process progress operation is complete.</returns>
    ValueTask ReportServerProcessProgress(string elementId, string processId, bool isLengthComputable, long loaded, long total,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a Blazor-driven <c>server.process</c> handler for the specified FilePond element.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="handler">The handler that should process uploads for this FilePond instance.</param>
    /// <param name="cancellationToken">A token associated with the owning component lifecycle.</param>
    void RegisterServerProcessHandler(string elementId, Func<FilePondServerProcessRequest, CancellationToken, ValueTask<string>> handler,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the Blazor-driven <c>server.process</c> handler for the specified FilePond element and cancels active uploads.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    void UnregisterServerProcessHandler(string elementId);
}
