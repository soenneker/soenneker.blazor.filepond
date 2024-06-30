using System;
using System.Collections.Generic;
using Soenneker.Blazor.FilePond.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Soenneker.Blazor.FilePond.Dtos;
using Soenneker.Blazor.FilePond.Options;
using Soenneker.Utils.Json;
using System.IO;
using System.Linq;
using Soenneker.Extensions.Enumerable;
using System.Threading;
using Soenneker.Blazor.Utils.EventListeningInterop;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Blazor.FilePond.Utils;
using Soenneker.Blazor.FilePond.Enums;
using Soenneker.Extensions.ValueTask;

namespace Soenneker.Blazor.FilePond;

/// <inheritdoc cref="IFilePondInterop"/>
public class FilePondInterop : EventListeningInterop, IFilePondInterop
{
    private readonly ILogger<FilePondInterop> _logger;
    private readonly List<FilePondPluginType> _enabledPlugins = [];
    private readonly IResourceLoader _resourceLoader;

    private readonly AsyncSingleton<object> _interopInitializer;
    private readonly AsyncSingleton<object> _styleInitializer;
    private readonly AsyncSingleton<object> _scriptInitializer;

    public FilePondInterop(IJSRuntime jSRuntime, ILogger<FilePondInterop> logger, IResourceLoader resourceLoader) : base(jSRuntime)
    {
        _logger = logger;
        _resourceLoader = resourceLoader;

        _interopInitializer = new AsyncSingleton<object>(async objects =>
        {
            var cancellationToken = (CancellationToken) objects[0];
            await resourceLoader.ImportModuleAndWaitUntilAvailable("Soenneker.Blazor.FilePond/filepondinterop.js", "FilePondInterop", 100, cancellationToken).NoSync();

            return new object();
        });

        _styleInitializer = new AsyncSingleton<object>(async objects =>
        {
            var cancellationToken = (CancellationToken)objects[0];

            (string uri, string integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(null);

            await _resourceLoader.LoadStyle(style.uri, style.integrity, cancellationToken).NoSync();
            return new object();
        });

        _scriptInitializer = new AsyncSingleton<object>(async objects =>
        {
            var cancellationToken = (CancellationToken)objects[0];

            (string uri, string integrity) script = FilePondUtil.GetUriAndIntegrityForScript(null);

            await _resourceLoader.LoadScriptAndWaitForVariable(script.uri, "FilePond", script.integrity, cancellationToken);
            return new object();
        });
    }

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        _ = await _interopInitializer.Get(cancellationToken);
    }

    public async ValueTask Create(string elementId, FilePondOptions? options = null, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Get(cancellationToken);
        await _styleInitializer.Get(cancellationToken);
        await _scriptInitializer.Get(cancellationToken);

        string? json = null;

        if (options != null)
            json = JsonUtil.Serialize(options);

        await JsRuntime.InvokeVoidAsync("FilePondInterop.create", cancellationToken, elementId, json);
    }

    public ValueTask SetOptions(string elementId, FilePondOptions options, CancellationToken cancellationToken = default)
    {
        string json = JsonUtil.Serialize(options)!;

        return JsRuntime.InvokeVoidAsync("FilePondInterop.setOptions", cancellationToken, elementId, json);
    }

    public ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.addFile", cancellationToken, elementId, uriOrBase64EncodedData, options);
    }

    public ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var streamRef = new DotNetStreamReference(stream, leaveOpen: true);
        return JsRuntime.InvokeVoidAsync("FilePondInterop.addFileFromStream", cancellationToken, elementId, streamRef, options);
    }

    public ValueTask AddFiles(string elementId, List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.addFiles", cancellationToken, elementId, uriOrBase64EncodedData, options);
    }

    public ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.removeFile", cancellationToken, elementId, query, options);
    }

    public ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.removeFiles", cancellationToken, elementId, query);
    }

    public ValueTask ProcessFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.processFile", cancellationToken, elementId, query);
    }

    public ValueTask ProcessFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.processFiles", cancellationToken, elementId, query);
    }

    public ValueTask<object> PrepareFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<object>("FilePondInterop.prepareFile", cancellationToken, elementId, query);
    }

    public ValueTask<object[]> PrepareFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<object[]>("FilePondInterop.prepareFiles", cancellationToken, elementId, query);
    }

    public async ValueTask<FilePondFileItem?> GetFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        var str = await JsRuntime.InvokeAsync<string>("FilePondInterop.getFile", cancellationToken, elementId, query);
        var result = JsonUtil.Deserialize<FilePondFileItem>(str);
        return result;
    }

    public async ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default)
    {
        var str = await JsRuntime.InvokeAsync<string>("FilePondInterop.getFiles", cancellationToken, elementId);
        var result = JsonUtil.Deserialize<List<FilePondFileItem>>(str);
        return result;
    }

    public ValueTask Browse(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.browse", cancellationToken, elementId);
    }

    public ValueTask Sort(string elementId, string compareFunctionName, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.sort", cancellationToken, elementId, compareFunctionName);
    }

    public ValueTask MoveFile(string elementId, object query, int index, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.moveFile", cancellationToken, elementId, query, index);
    }

    public ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.destroy", cancellationToken, elementId);
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.createObserver", cancellationToken, elementId);
    }

    public async ValueTask EnablePlugins(List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Get(cancellationToken).NoSync();

        List<FilePondPluginType> resultList = filePondPluginTypes.Except(_enabledPlugins).ToList();

        _enabledPlugins.AddRange(resultList);

        foreach (FilePondPluginType plugin in resultList)
        {
            (string uri, string integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(plugin);

            await _resourceLoader.LoadStyle(style.uri, style.integrity, cancellationToken).NoSync();

            (string uri, string integrity) script = FilePondUtil.GetUriAndIntegrityForScript(plugin);

            await _resourceLoader.LoadScript(script.uri, script.integrity, cancellationToken).NoSync();
        }

        await _scriptInitializer.Get(cancellationToken);

        if (resultList.Any())
        {
            List<string> strings = resultList.Select(c => c.Name.ToString()).ToList();
            await JsRuntime.InvokeVoidAsync("FilePondInterop.enablePlugins", cancellationToken, strings);
        }
    }

    public async ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        try
        {
            var blob = await JsRuntime.InvokeAsync<IJSStreamReference>("FilePondInterop.getFileAsBlob", cancellationToken, elementId, query);

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

    public ValueTask DisposeAsync()
    {
        return _resourceLoader.DisposeModule("Soenneker.Blazor.FilePond/filepondinterop.js");
    }
}