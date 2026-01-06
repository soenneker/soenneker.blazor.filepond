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
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.EventListeningInterop;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Blazor.FilePond.Utils;
using Soenneker.Blazor.FilePond.Enums;
using Soenneker.Blazor.FilePond.Constants;

namespace Soenneker.Blazor.FilePond;

/// <inheritdoc cref="IFilePondInterop"/>
public sealed class FilePondInterop : EventListeningInterop, IFilePondInterop
{
    private readonly ILogger<FilePondInterop> _logger;
    private readonly List<FilePondPluginType> _enabledPlugins = [];
    private readonly List<string> _enabledOtherPlugins = [];
    private readonly IResourceLoader _resourceLoader;

    private readonly AsyncInitializer _interopInitializer;
    private readonly AsyncInitializer<bool> _styleInitializer;
    private readonly AsyncInitializer<bool> _scriptInitializer;
    private readonly AsyncInitializer _interopStyleInitializer;

    private const string _module = "Soenneker.Blazor.FilePond/js/filepondinterop.js";

    public FilePondInterop(IJSRuntime jSRuntime, ILogger<FilePondInterop> logger, IResourceLoader resourceLoader) : base(jSRuntime)
    {
        _logger = logger;
        _resourceLoader = resourceLoader;

        _interopInitializer = new AsyncInitializer(InitializeInterop);
        _styleInitializer = new AsyncInitializer<bool>(InitializeStyle);
        _scriptInitializer = new AsyncInitializer<bool>(InitializeScript);
        _interopStyleInitializer = new AsyncInitializer(InitializeInteropStyle);
    }

    private ValueTask InitializeInterop(CancellationToken token)
    {
        return _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, nameof(FilePondInterop), 100, token);
    }

    private ValueTask InitializeStyle(bool useCdn, CancellationToken token)
    {
        (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(useCdn);
        return _resourceLoader.LoadStyle(style.uri!, style.integrity, cancellationToken: token);
    }

    private ValueTask InitializeScript(bool useCdn, CancellationToken token)
    {
        (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(useCdn);
        return _resourceLoader.LoadScriptAndWaitForVariable(script.uri!, "FilePond", script.integrity, cancellationToken: token);
    }

    private ValueTask InitializeInteropStyle(CancellationToken token)
    {
        return _resourceLoader.LoadStyle("_content/Soenneker.Blazor.FilePond/css/filepondinterop.css", cancellationToken: token);
    }

    public ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        return _interopInitializer.Init(cancellationToken);
    }

    public async ValueTask Create(string elementId, FilePondOptions? options = null, CancellationToken cancellationToken = default)
    {
        bool useCdn = options?.UseCdn ?? true;

        await _interopInitializer.Init(cancellationToken);
        await _styleInitializer.Init(useCdn, cancellationToken);

        await _scriptInitializer.Init(useCdn, cancellationToken);
        await _interopStyleInitializer.Init(cancellationToken);

        string? json = null;

        if (options != null)
            json = JsonUtil.Serialize(options);

        await JsRuntime.InvokeVoidAsync("FilePondInterop.create", cancellationToken, elementId, json);

        // Handle global ShowFileSize option
        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", cancellationToken, elementId, false);
        }
    }

    public async ValueTask SetOptions(string elementId, FilePondOptions options, CancellationToken cancellationToken = default)
    {
        string json = JsonUtil.Serialize(options)!;

        await JsRuntime.InvokeVoidAsync("FilePondInterop.setOptions", cancellationToken, elementId, json);

        await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", cancellationToken, elementId, options.ShowFileSize);
    }

    public async ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await JsRuntime.InvokeVoidAsync("FilePondInterop.addFile", cancellationToken, elementId, uriOrBase64EncodedData, options);

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", cancellationToken, elementId, false);
        }
    }

    public async ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var streamRef = new DotNetStreamReference(stream, leaveOpen: true);
        await JsRuntime.InvokeVoidAsync("FilePondInterop.addFileFromStream", cancellationToken, elementId, streamRef, options);

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", cancellationToken, elementId, false);
        }
    }

    public async ValueTask AddLimboFile(string elementId, string filename, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await JsRuntime.InvokeVoidAsync("FilePondInterop.addLimboFile", cancellationToken, elementId, filename, options);

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", cancellationToken, elementId, false);
        }
    }

    public async ValueTask AddFiles(string elementId, List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await JsRuntime.InvokeVoidAsync("FilePondInterop.addFiles", cancellationToken, elementId, uriOrBase64EncodedData, options);

        if (options is {ShowFileSize: false})
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", cancellationToken, elementId, false);
        }
    }

    public ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.removeFile", cancellationToken, elementId, query, options);
    }

    public ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
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
        return JsonUtil.Deserialize<FilePondFileItem>(str);
    }

    public async ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default)
    {
        var str = await JsRuntime.InvokeAsync<string>("FilePondInterop.getFiles", cancellationToken, elementId);
        return JsonUtil.Deserialize<List<FilePondFileItem>>(str);
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

    public async ValueTask EnablePlugins(bool useCdn, List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Init(cancellationToken);

        List<FilePondPluginType> resultList = filePondPluginTypes.Except(_enabledPlugins).ToList();

        _enabledPlugins.AddRange(resultList);

        foreach (FilePondPluginType plugin in resultList)
        {
            (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(useCdn, plugin);

            if (style.uri != null)
                await _resourceLoader.LoadStyle(style.uri, style.integrity, cancellationToken: cancellationToken);

            (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(useCdn, plugin);

            if (script.uri != null)
                await _resourceLoader.LoadScript(script.uri, script.integrity, cancellationToken: cancellationToken);
        }

        // FilePond.js is added after the plugins in all the examples...
        await _scriptInitializer.Init(useCdn, cancellationToken);

        if (resultList.Count > 0)
        {
            List<string> strings = resultList.Select(c => c.Name.ToString()).ToList();
            await JsRuntime.InvokeVoidAsync("FilePondInterop.enablePlugins", cancellationToken, strings);
        }
    }

    public async ValueTask EnableOtherPlugins(bool useCdn, List<string> filePondOtherPlugins, CancellationToken cancellationToken = default)
    {
        await _interopInitializer.Init(cancellationToken);

        List<string> resultList = filePondOtherPlugins.Except(_enabledOtherPlugins).ToList();

        _enabledOtherPlugins.AddRange(resultList);

        await _scriptInitializer.Init(useCdn, cancellationToken);

        if (resultList.Count > 0)
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.enableOtherPlugins", cancellationToken, resultList);
        }
    }

    public async ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // First check if the file has content (non-zero size)
            var hasContent = await JsRuntime.InvokeAsync<bool>("FilePondInterop.hasFileContent", cancellationToken, elementId, query);

            if (!hasContent)
            {
                _logger.LogWarning("File has no content (zero length), cannot create stream");
                return null;
            }

            var blob = await JsRuntime
                                            .InvokeAsync<IJSStreamReference>("FilePondInterop.getFileAsBlob", cancellationToken, elementId, query)
                                            ;

            return await blob.OpenReadStreamAsync(maxAllowedSize, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get the stream for file");
            return null;
        }
    }

    public async ValueTask<Stream?> GetOriginalStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // First check if the file has content (non-zero size)
            var hasContent = await JsRuntime.InvokeAsync<bool>("FilePondInterop.hasFileContent", cancellationToken, elementId, query);

            if (!hasContent)
            {
                _logger.LogWarning("File has no content (zero length), cannot create stream");
                return null;
            }

            var blob = await JsRuntime
                                            .InvokeAsync<IJSStreamReference>("FilePondInterop.getOriginalFileAsBlob", cancellationToken, elementId, query)
                                            ;

            return await blob.OpenReadStreamAsync(maxAllowedSize, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get the original stream for file");
            return null;
        }
    }

    public async ValueTask<List<Stream>> GetAllStreams(string elementId, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        List<FilePondFileItem>? files = await GetFiles(elementId, cancellationToken);

        if (files.IsNullOrEmpty())
            return new List<Stream>();

        // Use the new GetStreamsForFiles method to avoid concurrency issues
        var fileIds = files.Select(f => f.Id).ToList();
        return await GetStreamsForFiles(elementId, fileIds, maxAllowedSize, cancellationToken);
    }

    public async ValueTask<List<Stream>> GetStreamsForFiles(string elementId, List<string> fileIds, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (fileIds.IsNullOrEmpty())
            {
                _logger.LogWarning("GetStreamsForFiles called with empty fileIds list");
                return new List<Stream>();
            }

            var streams = new List<Stream>(fileIds.Count);

            // Sequentially fetch each file as an IJSStreamReference and open a .NET stream
            foreach (string fileId in fileIds)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    // Skip zero-length files to avoid interop exceptions
                    var hasContent = await JsRuntime.InvokeAsync<bool>("FilePondInterop.hasFileContent", cancellationToken, elementId, fileId);
                    if (!hasContent)
                    {
                        _logger.LogWarning("File {FileId} has no content (zero length), skipping", fileId);
                        continue;
                    }

                    var jsStream = await JsRuntime.InvokeAsync<IJSStreamReference>("FilePondInterop.getFileAsBlob", cancellationToken, elementId, fileId);
                    Stream stream = await jsStream.OpenReadStreamAsync(maxAllowedSize, cancellationToken);
                    streams.Add(stream);
                }
                catch (JSException jsex)
                {
                    _logger.LogError(jsex, "JS interop failed while getting stream for file {FileId}", fileId);
                }
                catch (OperationCanceledException)
                {
                    // Respect cancellation
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to open stream for file {FileId}", fileId);
                }
            }

            return streams;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get streams for files");
            return new List<Stream>();
        }
    }

    public ValueTask SetValidationState(string elementId, bool isValid, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.setValidationState", cancellationToken, elementId, isValid, errorMessage);
    }

    public ValueTask SetFileSuccess(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSuccess", cancellationToken, elementId, fileId, isSuccess);
    }

    public ValueTask SetFileSuccess(string elementId, int fileIndex, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSuccess", cancellationToken, elementId, fileIndex, isSuccess);
    }

    public ValueTask SetAllFilesSuccess(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.setAllFilesSuccess", cancellationToken, elementId, isSuccess);
    }

    public ValueTask SetFileSuccessWhenReady(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSuccessWhenReady", cancellationToken, elementId, fileId, isSuccess);
    }

    public ValueTask SetAllFilesSuccessWhenReady(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        return JsRuntime.InvokeVoidAsync("FilePondInterop.setAllFilesSuccessWhenReady", cancellationToken, elementId, isSuccess);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _interopInitializer.DisposeAsync();
        await _styleInitializer.DisposeAsync();
        await _scriptInitializer.DisposeAsync();
        await _interopStyleInitializer.DisposeAsync();
    }
}
