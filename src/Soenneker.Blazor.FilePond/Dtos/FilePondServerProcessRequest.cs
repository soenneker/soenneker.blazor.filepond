using System;
using System.Text.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.FilePond.Dtos;

/// <summary>
/// Represents a Blazor-driven FilePond server.process request.
/// </summary>
public sealed class FilePondServerProcessRequest
{
    private readonly Func<long?, CancellationToken, ValueTask<Stream?>> _getStreamFunc;
    private readonly Func<bool, long, long, CancellationToken, ValueTask> _reportProgressFunc;

    /// <summary>
    /// The unique ID for the active FilePond process callback.
    /// </summary>
    public string ProcessId { get; }

    /// <summary>
    /// The FilePond field name associated with the upload.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// The FilePond file item being processed.
    /// </summary>
    public FilePondFileItem File { get; }

    /// <summary>
    /// Optional metadata supplied by FilePond for the upload.
    /// </summary>
    public JsonElement? Metadata { get; }

    internal FilePondServerProcessRequest(string processId, string fieldName, FilePondFileItem file, JsonElement? metadata,
        Func<long?, CancellationToken, ValueTask<Stream?>> getStreamFunc, Func<bool, long, long, CancellationToken, ValueTask> reportProgressFunc)
    {
        ProcessId = processId;
        FieldName = fieldName;
        File = file;
        Metadata = metadata;
        _getStreamFunc = getStreamFunc;
        _reportProgressFunc = reportProgressFunc;
    }

    /// <summary>
    /// Retrieves stream.
    /// </summary>
    /// <param name="maxAllowedSize">Max Allowed Size for the get stream operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested stream.</returns>
    public ValueTask<Stream?> GetStream(long? maxAllowedSize = null, CancellationToken cancellationToken = default)
    {
        return _getStreamFunc(maxAllowedSize, cancellationToken);
    }

    /// <summary>
    /// Reports upload progress back to FilePond so the built-in progress UI can update.
    /// </summary>
    /// <param name="isLengthComputable">Whether length computable.</param>
    /// <param name="loaded">Loaded for the report progress operation.</param>
    /// <param name="total">Total for the report progress operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the report progress operation is complete.</returns>
    public ValueTask ReportProgress(bool isLengthComputable, long loaded, long total, CancellationToken cancellationToken = default)
    {
        return _reportProgressFunc(isLengthComputable, loaded, total, cancellationToken);
    }
}
