using System;
using System.Collections.Generic;
using Soenneker.Blazor.FilePond.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Soenneker.Blazor.FilePond.Dtos;
using Soenneker.Blazor.FilePond.Options;
using Soenneker.Utils.Json;
using System.IO;
using System.Linq;
using Soenneker.Extensions.Enumerable;
using System.Threading;
using Soenneker.Blazor.Utils.EventListeningInterop;

namespace Soenneker.Blazor.FilePond;

/// <inheritdoc cref="IFilePondInterop"/>
public class FilePondInterop : EventListeningInterop, IFilePondInterop
{
    private readonly ILogger<FilePondInterop> _logger;

    private readonly List<string> _enabledPlugins = [];

    public FilePondInterop(IJSRuntime jSRuntime, ILogger<FilePondInterop> logger) : base(jSRuntime)
    {
        _logger = logger;
    }

    public ValueTask Create(ElementReference elementReference, string elementId, FilePondOptions? options = null, CancellationToken cancellationToken = default)
    {
        string? json = null;

        if (options != null)
            json = JsonUtil.Serialize(options);

        return JsRuntime.InvokeVoidAsync("filepondinterop.create", cancellationToken, elementReference, elementId, json);
    }

    public ValueTask SetOptions(string elementId, FilePondOptions options, CancellationToken cancellationToken = default)
    {
        string json = JsonUtil.Serialize(options)!;

        return JsRuntime.InvokeVoidAsync("filepondinterop.setOptions", cancellationToken, elementId, json);
    }

    public ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.addFile", cancellationToken, elementId, uriOrBase64EncodedData, options);
    }

    public ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var streamRef = new DotNetStreamReference(stream, leaveOpen: false);
        return JsRuntime.InvokeVoidAsync("filepondinterop.addFileFromStream", cancellationToken, elementId, streamRef, options);
    }

    public ValueTask AddFiles(string elementId, List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.addFiles", cancellationToken, elementId, uriOrBase64EncodedData, options);
    }

    public ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.removeFile", cancellationToken, elementId, query, options);
    }

    public ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.removeFiles", cancellationToken, elementId, query);
    }

    public ValueTask ProcessFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.processFile", cancellationToken, elementId, query);
    }

    public ValueTask ProcessFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.processFiles", cancellationToken, elementId, query);
    }

    public ValueTask<object> PrepareFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<object>("filepondinterop.prepareFile", cancellationToken, elementId, query);
    }

    public ValueTask<object[]> PrepareFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<object[]>("filepondinterop.prepareFiles", cancellationToken, elementId, query);
    }

    public async ValueTask<FilePondFileItem?> GetFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        var str = await JsRuntime.InvokeAsync<string>("filepondinterop.getFile", cancellationToken, elementId, query);
        var result = JsonUtil.Deserialize<FilePondFileItem>(str);
        return result;
    }

    public async ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default)
    {
        var str = await JsRuntime.InvokeAsync<string>("filepondinterop.getFiles", cancellationToken, elementId);
        var result = JsonUtil.Deserialize<List<FilePondFileItem>>(str);
        return result;
    }

    public ValueTask Browse(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.browse", cancellationToken, elementId);
    }

    public ValueTask Sort(string elementId, string compareFunctionName, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.sort", cancellationToken, elementId, compareFunctionName);
    }

    public ValueTask MoveFile(string elementId, object query, int index, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.moveFile", cancellationToken, elementId, query, index);
    }

    public ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("filepondinterop.destroy", cancellationToken, elementId);
    }

    public ValueTask EnablePlugins(List<string> filePondPluginTypes, CancellationToken cancellationToken = default)
    {
        List<string> resultList = filePondPluginTypes.Except(_enabledPlugins).ToList();

        _enabledPlugins.AddRange(resultList);

        if (resultList.Any())
            return JsRuntime.InvokeVoidAsync("filepondinterop.enablePlugins", cancellationToken, resultList);

        return ValueTask.CompletedTask;
    }
    
    public async ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        try
        {
            var blob = await JsRuntime.InvokeAsync<IJSStreamReference>("filepondinterop.getFileAsBlob", cancellationToken, elementId, query);

            Stream stream = await blob.OpenReadStreamAsync(maxAllowedSize, cancellationToken);

            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get the stream for file");
            return null;
        }
    }

    public async ValueTask<List<Stream>> GetAllStreams(string elementId, long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        List<FilePondFileItem>? files = await GetFiles(elementId, cancellationToken);

        var streams = new List<Stream>();

        if (files.IsNullOrEmpty())
            return streams;

        foreach (FilePondFileItem file in files)
        {
            Stream? stream = await GetStreamForFile(elementId, file.Id, maxAllowedSize, cancellationToken);

            if (stream != null)
                streams.Add(stream);
        }

        return streams;
    }
}