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
using Soenneker.Blazor.FilePond.Constants;
using Soenneker.Extensions.String;

namespace Soenneker.Blazor.FilePond;

/// <inheritdoc cref="IFilePondInterop"/>
public class FilePondInterop : EventListeningInterop, IFilePondInterop
{
    private readonly ILogger<FilePondInterop> _logger;
    private readonly List<FilePondPluginType> _enabledPlugins = [];
    private readonly List<string> _enabledOtherPlugins = [];
    private readonly IResourceLoader _resourceLoader;

    private readonly AsyncSingleton<object> _interopInitializer;
    private readonly AsyncSingleton<object> _styleInitializer;
    private readonly AsyncSingleton<object> _scriptInitializer;

    public FilePondInterop(IJSRuntime jSRuntime, ILogger<FilePondInterop> logger, IResourceLoader resourceLoader) : base(jSRuntime)
    {
        _logger = logger;
        _resourceLoader = resourceLoader;

        _interopInitializer = new AsyncSingleton<object>(async (token, _) =>
        {
            await resourceLoader.ImportModuleAndWaitUntilAvailable("Soenneker.Blazor.FilePond/filepondinterop.js", nameof(FilePondInterop), 100, token).NoSync();

            return new object();
        });

        _styleInitializer = new AsyncSingleton<object>(async (token, _) =>
        {
            (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(null);

            await _resourceLoader.LoadStyle(style.uri!, style.integrity, cancellationToken: token).NoSync();
            return new object();
        });

        _scriptInitializer = new AsyncSingleton<object>(async (token, _) =>
        {
            (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(null);

            await _resourceLoader.LoadScriptAndWaitForVariable(script.uri!, "FilePond", script.integrity, cancellationToken: token).NoSync();
            return new object();
        });
    }

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        _ = await _interopInitializer.Get(cancellationToken, 0);
    }

    public async ValueTask Create(string elementId, FilePondOptions? options = null, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Get(cancellationToken, 0).NoSync();
        await _styleInitializer.Get(cancellationToken, 0).NoSync();
        await _scriptInitializer.Get(cancellationToken, 0).NoSync();

        string? json = null;

        if (options != null)
            json = JsonUtil.Serialize(options);

        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.create", cancellationToken, elementId, json);
    }

    public ValueTask SetOptions(string elementId, FilePondOptions options, CancellationToken cancellationToken = default)
    {
        string json = JsonUtil.Serialize(options)!;

        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setOptions", cancellationToken, elementId, json);
    }

    public ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.addFile", cancellationToken, elementId, uriOrBase64EncodedData, options);
    }

    public ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var streamRef = new DotNetStreamReference(stream, leaveOpen: true);
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.addFileFromStream", cancellationToken, elementId, streamRef, options);
    }

    public ValueTask AddFiles(string elementId, List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.addFiles", cancellationToken, elementId, uriOrBase64EncodedData, options);
    }

    public ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        var str = "".ToLowerInvariantFast();
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.removeFile", cancellationToken, elementId, query, options);
    }

    public ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.removeFiles", cancellationToken, elementId, query);
    }

    public ValueTask ProcessFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.processFile", cancellationToken, elementId, query);
    }

    public ValueTask ProcessFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.processFiles", cancellationToken, elementId, query);
    }

    public ValueTask<object> PrepareFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<object>($"{nameof(FilePondInterop)}.prepareFile", cancellationToken, elementId, query);
    }

    public ValueTask<object[]> PrepareFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeAsync<object[]>($"{nameof(FilePondInterop)}.prepareFiles", cancellationToken, elementId, query);
    }

    public async ValueTask<FilePondFileItem?> GetFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        string str = await JsRuntime.InvokeAsync<string>($"{nameof(FilePondInterop)}.getFile", cancellationToken, elementId, query).NoSync();
        var result = JsonUtil.Deserialize<FilePondFileItem>(str);
        return result;
    }

    public async ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default)
    {
        string str = await JsRuntime.InvokeAsync<string>($"{nameof(FilePondInterop)}.getFiles", cancellationToken, elementId).NoSync();
        var result = JsonUtil.Deserialize<List<FilePondFileItem>>(str);
        return result;
    }

    public ValueTask Browse(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.browse", cancellationToken, elementId);
    }

    public ValueTask Sort(string elementId, string compareFunctionName, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.sort", cancellationToken, elementId, compareFunctionName);
    }

    public ValueTask MoveFile(string elementId, object query, int index, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.moveFile", cancellationToken, elementId, query, index);
    }

    public ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.destroy", cancellationToken, elementId);
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.createObserver", cancellationToken, elementId);
    }

    public async ValueTask EnablePlugins(List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Get(cancellationToken, 0).NoSync();

        List<FilePondPluginType> resultList = filePondPluginTypes.Except(_enabledPlugins).ToList();

        _enabledPlugins.AddRange(resultList);

        foreach (FilePondPluginType plugin in resultList)
        {
            (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(plugin);

            if (style.uri != null)
                await _resourceLoader.LoadStyle(style.uri, style.integrity, cancellationToken: cancellationToken).NoSync();

            (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(plugin);

            if (script.uri != null)
                await _resourceLoader.LoadScript(script.uri, script.integrity, cancellationToken: cancellationToken).NoSync();
        }

        // FilePond.js is added after the plugins in all the examples...
        await _scriptInitializer.Get(cancellationToken, 0);

        if (resultList.Count > 0)
        {
            List<string> strings = resultList.Select(c => c.Name.ToString()).ToList();
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.enablePlugins", cancellationToken, strings);
        }
    }

    public async ValueTask EnableOtherPlugins(List<string> filePondOtherPlugins, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Get(cancellationToken, 0).NoSync();

        List<string> resultList = filePondOtherPlugins.Except(_enabledOtherPlugins).ToList();

        _enabledOtherPlugins.AddRange(resultList);

        await _scriptInitializer.Get(cancellationToken, 0);

        if (resultList.Count > 0)
        {
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.enableOtherPlugins", cancellationToken, resultList);
        }
    }

    public async ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default)
    {
        try
        {
            IJSStreamReference blob = await JsRuntime.InvokeAsync<IJSStreamReference>($"{nameof(FilePondInterop)}.getFileAsBlob", cancellationToken, elementId, query).NoSync();

            Stream stream = await blob.OpenReadStreamAsync(maxAllowedSize, cancellationToken);

            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get the stream for file");
            return null;
        }
    }

    public async ValueTask<List<Stream>> GetAllStreams(string elementId, long maxAllowedSize = FilePondConstants.DefaultMaximumSize, CancellationToken cancellationToken = default)
    {
        List<FilePondFileItem>? files = await GetFiles(elementId, cancellationToken).NoSync();

        var streams = new List<Stream>();

        if (files.IsNullOrEmpty())
            return streams;

        foreach (FilePondFileItem file in files)
        {
            Stream? stream = await GetStreamForFile(elementId, file.Id, maxAllowedSize, cancellationToken).NoSync();

            if (stream != null)
                streams.Add(stream);
        }

        return streams;
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return _resourceLoader.DisposeModule("Soenneker.Blazor.FilePond/filepondinterop.js");
    }
}