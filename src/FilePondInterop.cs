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
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Utils.CancellationScopes;

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

    private readonly CancellationScope _cancellationScope = new();

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

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _interopInitializer.Init(linked);
    }

    public async ValueTask Create(string elementId, FilePondOptions? options = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            bool useCdn = options?.UseCdn ?? true;

            await _interopInitializer.Init(linked);
            await _styleInitializer.Init(useCdn, linked);

            await _scriptInitializer.Init(useCdn, linked);
            await _interopStyleInitializer.Init(linked);

            string? json = null;

            if (options != null)
                json = JsonUtil.Serialize(options);

            await JsRuntime.InvokeVoidAsync("FilePondInterop.create", linked, elementId, json);

            // Handle global ShowFileSize option
            if (options is {ShowFileSize: false})
            {
                await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask SetOptions(string elementId, FilePondOptions options, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);
        string json = JsonUtil.Serialize(options)!;

        using (source)
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setOptions", linked, elementId, json);

            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", linked, elementId, options.ShowFileSize);
        }
    }

    public async ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.addFile", linked, elementId, uriOrBase64EncodedData, options);

            if (options is {ShowFileSize: false})
            {
                await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);
        using var streamRef = new DotNetStreamReference(stream, leaveOpen: true);

        using (source)
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.addFileFromStream", linked, elementId, streamRef, options);

            if (options is {ShowFileSize: false})
            {
                await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask AddLimboFile(string elementId, string filename, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.addLimboFile", linked, elementId, filename, options);

            if (options is {ShowFileSize: false})
            {
                await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask AddFiles(string elementId, List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await JsRuntime.InvokeVoidAsync("FilePondInterop.addFiles", linked, elementId, uriOrBase64EncodedData, options);

            if (options is {ShowFileSize: false})
            {
                await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.removeFile", linked, elementId, query, options);
    }

    public async ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.removeFiles", linked, elementId, query);
    }

    public async ValueTask ProcessFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.processFile", linked, elementId, query);
    }

    public async ValueTask ProcessFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.processFiles", linked, elementId, query);
    }

    public async ValueTask<object> PrepareFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await JsRuntime.InvokeAsync<object>("FilePondInterop.prepareFile", linked, elementId, query);
    }

    public async ValueTask<object[]> PrepareFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await JsRuntime.InvokeAsync<object[]>("FilePondInterop.prepareFiles", linked, elementId, query);
    }

    public async ValueTask<FilePondFileItem?> GetFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            var str = await JsRuntime.InvokeAsync<string>("FilePondInterop.getFile", linked, elementId, query);
        return JsonUtil.Deserialize<FilePondFileItem>(str);
        }
    }

    public async ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            var str = await JsRuntime.InvokeAsync<string>("FilePondInterop.getFiles", linked, elementId);
            return JsonUtil.Deserialize<List<FilePondFileItem>>(str);
        }
    }

    public async ValueTask Browse(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.browse", linked, elementId);
    }

    public async ValueTask Sort(string elementId, string compareFunctionName, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.sort", linked, elementId, compareFunctionName);
    }

    public async ValueTask MoveFile(string elementId, object query, int index, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.moveFile", linked, elementId, query, index);
    }

    public async ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.destroy", linked, elementId);
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.createObserver", linked, elementId);
    }

    public async ValueTask EnablePlugins(bool useCdn, List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _interopInitializer.Init(linked);

            List<FilePondPluginType> resultList = filePondPluginTypes.Except(_enabledPlugins).ToList();

            _enabledPlugins.AddRange(resultList);

            foreach (FilePondPluginType plugin in resultList)
            {
                (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(useCdn, plugin);

                if (style.uri != null)
                    await _resourceLoader.LoadStyle(style.uri, style.integrity, cancellationToken: linked);

                (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(useCdn, plugin);

                if (script.uri != null)
                    await _resourceLoader.LoadScript(script.uri, script.integrity, cancellationToken: linked);
            }

            // FilePond.js is added after the plugins in all the examples...
            await _scriptInitializer.Init(useCdn, linked);

            if (resultList.Count > 0)
            {
                List<string> strings = resultList.Select(c => c.Name.ToString()).ToList();
                await JsRuntime.InvokeVoidAsync("FilePondInterop.enablePlugins", linked, strings);
            }
        }
    }

    public async ValueTask EnableOtherPlugins(bool useCdn, List<string> filePondOtherPlugins, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _interopInitializer.Init(linked);

            List<string> resultList = filePondOtherPlugins.Except(_enabledOtherPlugins).ToList();

            _enabledOtherPlugins.AddRange(resultList);

            await _scriptInitializer.Init(useCdn, linked);

            if (resultList.Count > 0)
            {
                await JsRuntime.InvokeVoidAsync("FilePondInterop.enableOtherPlugins", linked, resultList);
            }
        }
    }

    public async ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

            using (source)
            {
                // First check if the file has content (non-zero size)
                var hasContent = await JsRuntime.InvokeAsync<bool>("FilePondInterop.hasFileContent", linked, elementId, query);

                if (!hasContent)
                {
                    _logger.LogWarning("File has no content (zero length), cannot create stream");
                    return null;
                }

                var blob = await JsRuntime
                                                .InvokeAsync<IJSStreamReference>("FilePondInterop.getFileAsBlob", linked, elementId, query)
                                                ;

                return await blob.OpenReadStreamAsync(maxAllowedSize, linked);
            }
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
            var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

            using (source)
            {
                // First check if the file has content (non-zero size)
                var hasContent = await JsRuntime.InvokeAsync<bool>("FilePondInterop.hasFileContent", linked, elementId, query);

                if (!hasContent)
                {
                    _logger.LogWarning("File has no content (zero length), cannot create stream");
                    return null;
                }

                var blob = await JsRuntime
                                                .InvokeAsync<IJSStreamReference>("FilePondInterop.getOriginalFileAsBlob", linked, elementId, query)
                                                ;

                return await blob.OpenReadStreamAsync(maxAllowedSize, linked);
            }
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
            var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

            using (source)
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
                    if (linked.IsCancellationRequested)
                        break;

                    try
                    {
                        // Skip zero-length files to avoid interop exceptions
                        var hasContent = await JsRuntime.InvokeAsync<bool>("FilePondInterop.hasFileContent", linked, elementId, fileId);
                        if (!hasContent)
                        {
                            _logger.LogWarning("File {FileId} has no content (zero length), skipping", fileId);
                            continue;
                        }

                        var jsStream = await JsRuntime.InvokeAsync<IJSStreamReference>("FilePondInterop.getFileAsBlob", linked, elementId, fileId);
                        Stream stream = await jsStream.OpenReadStreamAsync(maxAllowedSize, linked);
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get streams for files");
            return new List<Stream>();
        }
    }

    public async ValueTask SetValidationState(string elementId, bool isValid, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setValidationState", linked, elementId, isValid, errorMessage);
    }

    public async ValueTask SetFileSuccess(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSuccess", linked, elementId, fileId, isSuccess);
    }

    public async ValueTask SetFileSuccess(string elementId, int fileIndex, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSuccess", linked, elementId, fileIndex, isSuccess);
    }

    public async ValueTask SetAllFilesSuccess(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setAllFilesSuccess", linked, elementId, isSuccess);
    }

    public async ValueTask SetFileSuccessWhenReady(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setFileSuccessWhenReady", linked, elementId, fileId, isSuccess);
    }

    public async ValueTask SetAllFilesSuccessWhenReady(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await JsRuntime.InvokeVoidAsync("FilePondInterop.setAllFilesSuccessWhenReady", linked, elementId, isSuccess);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _interopInitializer.DisposeAsync();
        await _styleInitializer.DisposeAsync();
        await _scriptInitializer.DisposeAsync();
        await _interopStyleInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}
