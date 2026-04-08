using System;
using System.Collections.Concurrent;
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
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Blazor.FilePond.Utils;
using Soenneker.Blazor.FilePond.Enums;
using Soenneker.Blazor.FilePond.Constants;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Extensions.String;
using Soenneker.Utils.CancellationScopes;
using System.Text.Json;

namespace Soenneker.Blazor.FilePond;

/// <inheritdoc cref="IFilePondInterop"/>
public sealed class FilePondInterop : IFilePondInterop
{
    private readonly ILogger<FilePondInterop> _logger;
    private readonly List<FilePondPluginType> _enabledPlugins = [];
    private readonly List<string> _enabledOtherPlugins = [];
    private readonly IResourceLoader _resourceLoader;
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly ConcurrentDictionary<string, ServerProcessRegistration> _serverProcessRegistrations = new();
    private readonly ConcurrentDictionary<string, ServerProcessContext> _activeServerProcesses = new();

    private readonly AsyncInitializer _interopInitializer;
    private readonly AsyncInitializer<bool> _styleInitializer;
    private readonly AsyncInitializer<bool> _scriptInitializer;
    private readonly AsyncInitializer _interopStyleInitializer;

    private const string _wrapperModulePath = "_content/Soenneker.Blazor.FilePond/js/filepondinterop.js";

    private readonly CancellationScope _cancellationScope = new();
    private DotNetObjectReference<FilePondInterop>? _dotNetReference;

    public FilePondInterop(ILogger<FilePondInterop> logger, IResourceLoader resourceLoader, IModuleImportUtil moduleImportUtil)
    {
        _logger = logger;
        _resourceLoader = resourceLoader;
        _moduleImportUtil = moduleImportUtil;

        _interopInitializer = new AsyncInitializer(InitializeInterop);
        _styleInitializer = new AsyncInitializer<bool>(InitializeStyle);
        _scriptInitializer = new AsyncInitializer<bool>(InitializeScript);
        _interopStyleInitializer = new AsyncInitializer(InitializeInteropStyle);
    }

    private async ValueTask InitializeInterop(CancellationToken token)
    {
        _ = await _moduleImportUtil.GetContentModuleReference(_wrapperModulePath, token);
    }

    private async ValueTask InitializeStyle(bool useCdn, CancellationToken token)
    {
        (string? uri, string? integrity) style = FilePondUtil.GetUriAndIntegrityForStyle(useCdn);
        await _resourceLoader.LoadStyle(style.uri!, style.integrity, cancellationToken: token);
    }

    private async ValueTask InitializeScript(bool useCdn, CancellationToken token)
    {
        (string? uri, string? integrity) script = FilePondUtil.GetUriAndIntegrityForScript(useCdn);
        await _resourceLoader.LoadScriptAndWaitForVariable(script.uri!, "FilePond", script.integrity, cancellationToken: token);
    }

    private async ValueTask InitializeInteropStyle(CancellationToken token)
    {
        await _resourceLoader.LoadStyle("_content/Soenneker.Blazor.FilePond/css/filepondinterop.css", cancellationToken: token);
    }

    private async ValueTask<IJSObjectReference> GetModule(CancellationToken cancellationToken)
    {
        return await _moduleImportUtil.GetContentModuleReference(_wrapperModulePath, cancellationToken);
    }

    private async ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object?[] args)
    {
        IJSObjectReference module = await GetModule(cancellationToken);
        await module.InvokeVoidAsync(identifier, cancellationToken, args);
    }

    private async ValueTask<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken, params object?[] args)
    {
        IJSObjectReference module = await GetModule(cancellationToken);
        return await module.InvokeAsync<T>(identifier, cancellationToken, args);
    }

    public async ValueTask Initialize(CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await _interopInitializer.Init(linked);
    }

    public async ValueTask Create(string elementId, FilePondOptions? options = null, bool useBlazorServerProcess = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

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

            object? dotNetReference = useBlazorServerProcess ? GetOrCreateDotNetReference() : null;

            await InvokeVoidAsync("create", linked, elementId, json, dotNetReference, useBlazorServerProcess);

            // Handle global ShowFileSize option
            if (options is {ShowFileSize: false})
            {
                await InvokeVoidAsync("setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask SetOptions(string elementId, FilePondOptions options, bool useBlazorServerProcess = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        string json = JsonUtil.Serialize(options)!;

        using (source)
        {
            object? dotNetReference = useBlazorServerProcess ? GetOrCreateDotNetReference() : null;

            await InvokeVoidAsync("setOptions", linked, elementId, json, dotNetReference, useBlazorServerProcess);

            await InvokeVoidAsync("setFileSizeVisibility", linked, elementId, options.ShowFileSize);
        }
    }

    public async ValueTask AddFile(string elementId, string uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await InvokeVoidAsync("addFile", linked, elementId, uriOrBase64EncodedData, options);

            if (options is {ShowFileSize: false})
            {
                await InvokeVoidAsync("setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask AddFile(string elementId, Stream stream, FilePondAddFileOptions? options = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        using var streamRef = new DotNetStreamReference(stream, leaveOpen: true);

        using (source)
        {
            await InvokeVoidAsync("addFileFromStream", linked, elementId, streamRef, options);

            if (options is {ShowFileSize: false})
            {
                await InvokeVoidAsync("setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask AddLimboFile(string elementId, string filename, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await InvokeVoidAsync("addLimboFile", linked, elementId, filename, options);

            if (options is {ShowFileSize: false})
            {
                await InvokeVoidAsync("setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask AddFiles(string elementId, List<string> uriOrBase64EncodedData, FilePondAddFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await InvokeVoidAsync("addFiles", linked, elementId, uriOrBase64EncodedData, options);

            if (options is {ShowFileSize: false})
            {
                await InvokeVoidAsync("setFileSizeVisibility", linked, elementId, false);
            }
        }
    }

    public async ValueTask RemoveFile(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("removeFile", linked, elementId, query, options);
    }

    public async ValueTask RemoveFiles(string elementId, object? query = null, FilePondRemoveFileOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("removeFiles", linked, elementId, query);
    }

    public async ValueTask ProcessFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("processFile", linked, elementId, query);
    }

    public async ValueTask ProcessFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("processFiles", linked, elementId, query);
    }

    public async ValueTask<object> PrepareFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            return await InvokeAsync<object>("prepareFile", linked, elementId, query);
    }

    public async ValueTask<object[]> PrepareFiles(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            return await InvokeAsync<object[]>("prepareFiles", linked, elementId, query);
    }

    public async ValueTask<FilePondFileItem?> GetFile(string elementId, object? query = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            var str = await InvokeAsync<string>("getFile", linked, elementId, query);
        return JsonUtil.Deserialize<FilePondFileItem>(str);
        }
    }

    public async ValueTask<List<FilePondFileItem>?> GetFiles(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            var str = await InvokeAsync<string>("getFiles", linked, elementId);
            return JsonUtil.Deserialize<List<FilePondFileItem>>(str);
        }
    }

    public async ValueTask Browse(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("browse", linked, elementId);
    }

    public async ValueTask Sort(string elementId, string compareFunctionName, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("sort", linked, elementId, compareFunctionName);
    }

    public async ValueTask MoveFile(string elementId, object query, int index, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("moveFile", linked, elementId, query, index);
    }

    public async ValueTask Destroy(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("destroy", linked, elementId);
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("createObserver", linked, elementId);
    }

    public async ValueTask EnablePlugins(bool useCdn, List<FilePondPluginType> filePondPluginTypes, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

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
                await InvokeVoidAsync("enablePlugins", linked, strings);
            }
        }
    }

    public async ValueTask EnableOtherPlugins(bool useCdn, List<string> filePondOtherPlugins, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _interopInitializer.Init(linked);

            List<string> resultList = filePondOtherPlugins.Except(_enabledOtherPlugins).ToList();

            _enabledOtherPlugins.AddRange(resultList);

            await _scriptInitializer.Init(useCdn, linked);

            if (resultList.Count > 0)
            {
                await InvokeVoidAsync("enableOtherPlugins", linked, resultList);
            }
        }
    }

    public async ValueTask AddEventListener(string functionName, string elementId, string eventName, object dotNetCallback,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            string identifier = functionName.Replace("FilePondInterop.", "");
            await InvokeVoidAsync(identifier, linked, elementId, eventName, dotNetCallback);
        }
    }

    public async ValueTask<Stream?> GetStreamForFile(string elementId, object? query = null, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

            using (source)
            {
                // First check if the file has content (non-zero size)
                var hasContent = await InvokeAsync<bool>("hasFileContent", linked, elementId, query);

                if (!hasContent)
                {
                    _logger.LogWarning("File has no content (zero length), cannot create stream");
                    return null;
                }

                var blob = await InvokeAsync<IJSStreamReference>("getFileAsBlob", linked, elementId, query)
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
            CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

            using (source)
            {
                // First check if the file has content (non-zero size)
                var hasContent = await InvokeAsync<bool>("hasFileContent", linked, elementId, query);

                if (!hasContent)
                {
                    _logger.LogWarning("File has no content (zero length), cannot create stream");
                    return null;
                }

                var blob = await InvokeAsync<IJSStreamReference>("getOriginalFileAsBlob", linked, elementId, query)
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
        List<string> fileIds = files.Select(f => f.Id).ToList();
        return await GetStreamsForFiles(elementId, fileIds, maxAllowedSize, cancellationToken);
    }

    public async ValueTask<List<Stream>> GetStreamsForFiles(string elementId, List<string> fileIds, long maxAllowedSize = FilePondConstants.DefaultMaximumSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

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
                        var hasContent = await InvokeAsync<bool>("hasFileContent", linked, elementId, fileId);
                        if (!hasContent)
                        {
                            _logger.LogWarning("File {FileId} has no content (zero length), skipping", fileId);
                            continue;
                        }

                        var jsStream = await InvokeAsync<IJSStreamReference>("getFileAsBlob", linked, elementId, fileId);
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
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("setValidationState", linked, elementId, isValid, errorMessage);
    }

    public async ValueTask SetFileSuccess(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("setFileSuccess", linked, elementId, fileId, isSuccess);
    }

    public async ValueTask SetFileSuccess(string elementId, int fileIndex, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("setFileSuccess", linked, elementId, fileIndex, isSuccess);
    }

    public async ValueTask SetAllFilesSuccess(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("setAllFilesSuccess", linked, elementId, isSuccess);
    }

    public async ValueTask SetFileSuccessWhenReady(string elementId, string fileId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("setFileSuccessWhenReady", linked, elementId, fileId, isSuccess);
    }

    public async ValueTask SetAllFilesSuccessWhenReady(string elementId, bool isSuccess = true, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("setAllFilesSuccessWhenReady", linked, elementId, isSuccess);
    }

    public async ValueTask ReportServerProcessProgress(string elementId, string processId, bool isLengthComputable, long loaded, long total,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await InvokeVoidAsync("reportServerProcessProgress", linked, elementId, processId, isLengthComputable, loaded, total);
    }

    public void RegisterServerProcessHandler(string elementId, Func<FilePondServerProcessRequest, CancellationToken, ValueTask<string>> handler,
        CancellationToken cancellationToken = default)
    {
        _serverProcessRegistrations[elementId] = new ServerProcessRegistration(handler, cancellationToken);
    }

    public void UnregisterServerProcessHandler(string elementId)
    {
        _serverProcessRegistrations.TryRemove(elementId, out _);

        foreach (KeyValuePair<string, ServerProcessContext> kvp in _activeServerProcesses.Where(kvp => kvp.Value.ElementId == elementId).ToList())
        {
            if (_activeServerProcesses.TryRemove(kvp.Key, out ServerProcessContext? context))
            {
                context.CancellationTokenSource.Cancel();
                context.CancellationTokenSource.Dispose();
            }
        }
    }

    [JSInvokable("ProcessFileJs")]
    public async Task<string> ProcessFileJs(string elementId, string processId, string fieldName, string fileJson, string? metadataJson)
    {
        if (!_serverProcessRegistrations.TryGetValue(elementId, out ServerProcessRegistration? registration))
            throw new InvalidOperationException($"No Blazor server.process handler registered for FilePond element '{elementId}'.");

        FilePondFileItem? file = JsonUtil.Deserialize<FilePondFileItem>(fileJson);

        if (file == null || !file.Id.HasContent())
            throw new InvalidOperationException("Unable to resolve the FilePond file item for Blazor-driven server.process");

        FilePondFileItem resolvedFile = file;

        JsonElement? metadata = null;

        if (metadataJson.HasContent())
        {
            using JsonDocument metadataDocument = JsonDocument.Parse(metadataJson!);
            metadata = metadataDocument.RootElement.Clone();
        }

        var processCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationScope.CancellationToken, registration.CancellationToken);
        _activeServerProcesses[processId] = new ServerProcessContext(elementId, processCancellationTokenSource);

        try
        {
            var request = new FilePondServerProcessRequest(processId, fieldName, resolvedFile, metadata,
                (maxAllowedSize, cancellationToken) => GetStreamForFile(elementId, resolvedFile.Id, maxAllowedSize ?? FilePondConstants.DefaultMaximumSize, cancellationToken),
                (isLengthComputable, loaded, total, cancellationToken) => ReportServerProcessProgress(elementId, processId, isLengthComputable, loaded, total, cancellationToken));

            return await registration.Handler(request, processCancellationTokenSource.Token);
        }
        finally
        {
            if (_activeServerProcesses.TryRemove(processId, out ServerProcessContext? context))
                context.CancellationTokenSource.Dispose();
        }
    }

    [JSInvokable("AbortServerProcessJs")]
    public Task AbortServerProcessJs(string elementId, string processId)
    {
        if (_activeServerProcesses.TryGetValue(processId, out ServerProcessContext? context) && context.ElementId == elementId &&
            _activeServerProcesses.TryRemove(processId, out context))
        {
            context.CancellationTokenSource.Cancel();
            context.CancellationTokenSource.Dispose();
        }

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (ServerProcessContext context in _activeServerProcesses.Values)
        {
            context.CancellationTokenSource.Cancel();
            context.CancellationTokenSource.Dispose();
        }

        _activeServerProcesses.Clear();
        _serverProcessRegistrations.Clear();
        _dotNetReference?.Dispose();
        await _moduleImportUtil.DisposeContentModule(_wrapperModulePath);
        await _interopInitializer.DisposeAsync();
        await _styleInitializer.DisposeAsync();
        await _scriptInitializer.DisposeAsync();
        await _interopStyleInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }

    private DotNetObjectReference<FilePondInterop> GetOrCreateDotNetReference()
    {
        _dotNetReference ??= DotNetObjectReference.Create(this);
        return _dotNetReference;
    }
}
