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

namespace Soenneker.Blazor.FilePond;

/// <inheritdoc cref="IFilePondInterop"/>
public sealed class FilePondInterop : EventListeningInterop, IFilePondInterop
{
    private readonly ILogger<FilePondInterop> _logger;
    private readonly List<FilePondPluginType> _enabledPlugins = [];
    private readonly List<string> _enabledOtherPlugins = [];
    private readonly IResourceLoader _resourceLoader;

    private readonly AsyncSingleton _interopInitializer;
    private readonly AsyncSingleton _styleInitializer;
    private readonly AsyncSingleton _scriptInitializer;
    private readonly AsyncSingleton _interopStyleInitializer;

    private const string _module = "Soenneker.Blazor.FilePond/js/filepondinterop.js";

    public FilePondInterop(IJSRuntime jSRuntime, ILogger<FilePondInterop> logger, IResourceLoader resourceLoader) : base(jSRuntime)
    {
        _logger = logger;
        _resourceLoader = resourceLoader;

        _interopInitializer = new AsyncSingleton(async (token, _) =>
        {
            await resourceLoader.ImportModuleAndWaitUntilAvailable(_module, nameof(FilePondInterop), 100, token).NoSync();
            return new object();
        });

        _styleInitializer = new AsyncSingleton(async (token, obj) =>
        {
            var useCdn = true;

            if (obj.Length > 0)
                useCdn = (bool) obj[0];

            (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(useCdn);

            await _resourceLoader.LoadStyle(style.uri!, style.integrity, cancellationToken: token).NoSync();
            return new object();
        });

        _scriptInitializer = new AsyncSingleton(async (token, obj) =>
        {
            var useCdn = true;

            if (obj.Length > 0)
                useCdn = (bool) obj[0];

            (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(useCdn);

            await _resourceLoader.LoadScriptAndWaitForVariable(script.uri!, "FilePond", script.integrity, cancellationToken: token).NoSync();
            return new object();
        });

        _interopStyleInitializer = new AsyncSingleton(async (token, _) =>
        {
            await _resourceLoader.LoadStyle("_content/Soenneker.Blazor.FilePond/css/filepondinterop.css", cancellationToken: token).NoSync();
            return new object();
        });
    }

    public ValueTask Initialize(bool useCdn, CancellationToken cancellationToken = default)
    {
        return _interopInitializer.Init(cancellationToken, useCdn);
    }

    public async ValueTask Create(string elementId, FilePondOptions? options = null, CancellationToken cancellationToken = default)
    {
        bool useCdn = options?.UseCdn ?? true;

        await _interopInitializer.Init(cancellationToken).NoSync();
        await _styleInitializer.Init(cancellationToken, useCdn).NoSync();

        await _scriptInitializer.Init(cancellationToken, useCdn).NoSync();
        await _interopStyleInitializer.Init(cancellationToken).NoSync();

        string? json = null;

        if (options != null)
            json = JsonUtil.Serialize(options);

        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.create", cancellationToken, elementId, json).NoSync();

        // Handle global ShowFileSize option
        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSizeVisibility", cancellationToken, elementId, false).NoSync();
        }
    }

    public async ValueTask SetOptions(string elementId, FilePondOptions options, CancellationToken cancellationToken = default)
    {
        string json = JsonUtil.Serialize(options)!;

        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setOptions", cancellationToken, elementId, json).NoSync();

        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSizeVisibility", cancellationToken, elementId, options.ShowFileSize).NoSync();
    }

    public async ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.addFile", cancellationToken, elementId, uriOrBase64EncodedData, options).NoSync();

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSizeVisibility", cancellationToken, elementId, false).NoSync();
        }
    }

    public async ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var streamRef = new DotNetStreamReference(stream, leaveOpen: true);
        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.addFileFromStream", cancellationToken, elementId, streamRef, options).NoSync();

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSizeVisibility", cancellationToken, elementId, false).NoSync();
        }
    }

    public async ValueTask AddLimboFile(string elementId, string filename, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.addLimboFile", cancellationToken, elementId, filename, options).NoSync();

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSizeVisibility", cancellationToken, elementId, false).NoSync();
        }
    }

    public async ValueTask AddFiles(string elementId, List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.addFiles", cancellationToken, elementId, uriOrBase64EncodedData, options).NoSync();

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSizeVisibility", cancellationToken, elementId, false).NoSync();
        }
    }

    public ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.removeFile", cancellationToken, elementId, query, options);
    }

    public ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
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
        return JsonUtil.Deserialize<FilePondFileItem>(str);
    }

    public async ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default)
    {
        string str = await JsRuntime.InvokeAsync<string>($"{nameof(FilePondInterop)}.getFiles", cancellationToken, elementId).NoSync();
        return JsonUtil.Deserialize<List<FilePondFileItem>>(str);
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

    public async ValueTask EnablePlugins(bool useCdn, List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Init(cancellationToken, 0).NoSync();

        List<FilePondPluginType> resultList = filePondPluginTypes.Except(_enabledPlugins).ToList();

        _enabledPlugins.AddRange(resultList);

        foreach (FilePondPluginType plugin in resultList)
        {
            (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(useCdn, plugin);

            if (style.uri != null)
                await _resourceLoader.LoadStyle(style.uri, style.integrity, cancellationToken: cancellationToken).NoSync();

            (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(useCdn, plugin);

            if (script.uri != null)
                await _resourceLoader.LoadScript(script.uri, script.integrity, cancellationToken: cancellationToken).NoSync();
        }

        // FilePond.js is added after the plugins in all the examples...
        await _scriptInitializer.Init(cancellationToken, useCdn).NoSync();

        if (resultList.Count > 0)
        {
            List<string> strings = resultList.Select(c => c.Name.ToString()).ToList();
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.enablePlugins", cancellationToken, strings).NoSync();
        }
    }

    public async ValueTask EnableOtherPlugins(bool useCdn, List<string> filePondOtherPlugins, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Init(cancellationToken, 0).NoSync();

        List<string> resultList = filePondOtherPlugins.Except(_enabledOtherPlugins).ToList();

        _enabledOtherPlugins.AddRange(resultList);

        await _scriptInitializer.Init(cancellationToken, useCdn).NoSync();

        if (resultList.Count > 0)
        {
            await JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.enableOtherPlugins", cancellationToken, resultList).NoSync();
        }
    }

    public async ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // First check if the file has content (non-zero size)
            bool hasContent = await JsRuntime.InvokeAsync<bool>($"{nameof(FilePondInterop)}.hasFileContent", cancellationToken, elementId, query).NoSync();

            if (!hasContent)
            {
                _logger.LogWarning("File has no content (zero length), cannot create stream");
                return null;
            }

            IJSStreamReference blob = await JsRuntime
                                            .InvokeAsync<IJSStreamReference>($"{nameof(FilePondInterop)}.getFileAsBlob", cancellationToken, elementId, query)
                                            .NoSync();

            return await blob.OpenReadStreamAsync(maxAllowedSize, cancellationToken).NoSync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get the stream for file");
            return null;
        }
    }

    public async ValueTask<List<Stream>> GetAllStreams(string elementId, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        List<FilePondFileItem>? files = await GetFiles(elementId, cancellationToken).NoSync();

        var streams = new List<Stream>();

        if (files.IsNullOrEmpty())
            return streams;

        for (var i = 0; i < files.Count; i++)
        {
            FilePondFileItem file = files[i];
            Stream? stream = await GetStreamForFile(elementId, file.Id, maxAllowedSize, cancellationToken).NoSync();

            if (stream != null)
                streams.Add(stream);
        }

        return streams;
    }

    public ValueTask SetValidationState(string elementId, bool isValid, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setValidationState", cancellationToken, elementId, isValid, errorMessage);
    }

    public ValueTask SetFileSuccess(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSuccess", cancellationToken, elementId, fileId, isSuccess);
    }

    public ValueTask SetFileSuccess(string elementId, int fileIndex, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSuccess", cancellationToken, elementId, fileIndex, isSuccess);
    }

    public ValueTask SetAllFilesSuccess(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setAllFilesSuccess", cancellationToken, elementId, isSuccess);
    }

    public ValueTask SetFileSuccessWhenReady(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync($"{nameof(FilePondInterop)}.setFileSuccessWhenReady", cancellationToken, elementId, fileId, isSuccess);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module).NoSync();
        await _interopInitializer.DisposeAsync().NoSync();
        await _styleInitializer.DisposeAsync().NoSync();
        await _scriptInitializer.DisposeAsync().NoSync();
        await _interopStyleInitializer.DisposeAsync().NoSync();
    }
}