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
    ValueTask Initialize(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a FilePond instance for the specified HTML element with optional configuration options.
    /// </summary>
    /// <param name="elementId">The unique identifier for the HTML element, used to associate the FilePond instance with the element.</param>
    /// <param name="options">(Optional) Configuration options for customizing the behavior of the FilePond instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    ValueTask Create(string elementId, FilePondOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Overrides multiple options at once.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="options">An object containing the options to override.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask SetOptions(string elementId, FilePondOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a file to FilePond.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="uriOrBase64EncodedData">The file source, either a URI or base64-encoded data.</param>
    /// <param name="addFileOptions">Additional options for the added file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? addFileOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a file to FilePond from a stream.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="stream">The stream containing the file data.</param>
    /// <param name="options">Additional options for the added file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple files to FilePond.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="uris">An array or a FileList containing file sources.</param>
    /// <param name="addFileOptions">Additional options for the added files.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask AddFiles(string elementId, List<string> uris, FilePondAddFileOptions? addFileOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a file with the specified query associated with the specified element.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The ID of the file to remove.</param>
    /// <param name="options">Options for removing the file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all files or files matching the query.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The query to match files to be removed.</param>
    /// <param name="options">Options for removing the files.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts processing the file matching the given query.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The query to match the file to be processed.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask ProcessFile(string elementId, object? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts processing all files or files matching the query.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The query to match files to be processed.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
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
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask Browse(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sorts files in the list using the supplied compare function.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="compareFunctionName">The compare function used for sorting.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask Sort(string elementId, string compareFunctionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves the file to a new index in the files array.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="query">The query to match the file to be moved.</param>
    /// <param name="index">The new index for the file.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask MoveFile(string elementId, object query, int index, CancellationToken cancellationToken = default);

    /// <summary>
    /// Destroys this FilePond instance.
    /// </summary>
    /// <param name="elementId">The ID of the FilePond element.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask Destroy(string elementId, CancellationToken cancellationToken = default);

    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    ValueTask EnablePlugins(List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default);

    ValueTask EnableOtherPlugins(List<string> filePondOtherPlugins, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a <see cref="Stream"/> for the file with the specified identifier within the given FilePond instance. The stream should be disposed after use.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element associated with the FilePond instance.</param>
    /// <param name="query">The unique identifier of the file for which to retrieve the stream.</param>
    /// <param name="maxAllowedSize">(Optional) The maximum allowed size of the stream in bytes. Defaults to 2MB.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of <see cref="Stream"/> objects for all files within the given FilePond instance. The streams should be disposed after use.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element associated with the FilePond instance.</param>
    /// <param name="maxAllowedSize">(Optional) The maximum allowed size. Defaults to 2MB.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the asynchronous operation to complete.</param>
    ValueTask<List<Stream>> GetAllStreams(string elementId, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default);
}