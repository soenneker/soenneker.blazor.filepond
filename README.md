[![](https://img.shields.io/nuget/v/soenneker.blazor.filepond.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/soenneker.blazor.filepond/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.filepond/publish-package.yml?style=for-the-badge&logo=github)](https://github.com/soenneker/soenneker.blazor.filepond/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.filepond.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/soenneker.blazor.filepond/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.blazor.filepond)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.filepond/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.filepond/actions/workflows/codeql.yml)

# Soenneker.Blazor.FilePond

A Blazor component and interop API for [FilePond](https://pqina.nl/filepond/), including plugin loading, typed events, browser-file streams, validation styling, and Blazor-owned upload processing.

## Installation

```bash
dotnet add package Soenneker.Blazor.FilePond
```

Register the scoped interop service in `Program.cs`:

```csharp
using Soenneker.Blazor.FilePond.Registrars;

builder.Services.AddFilePondInteropAsScoped();
```

Scripts and styles are loaded on demand. Do not add FilePond assets to the page separately.

## Select files

```razor
@using Soenneker.Blazor.FilePond
@using Soenneker.Blazor.FilePond.Dtos
@using Soenneker.Blazor.FilePond.Enums
@using Soenneker.Blazor.FilePond.Options

<FilePond @ref="_pond"
          Options="_options"
          OnAddFile="HandleFileAdded" />

@code {
    private const long MaxFileBytes = 10 * 1024 * 1024;
    private FilePond? _pond;

    private readonly FilePondOptions _options = new()
    {
        AllowMultiple = true,
        MaxFiles = 5,
        MaxFileSize = MaxFileBytes,
        AcceptedFileTypes = ["image/jpeg", "image/png"],
        EnabledPlugins =
        [
            FilePondPluginType.FileValidateSize,
            FilePondPluginType.FileValidateType,
            FilePondPluginType.ImagePreview
        ]
    };

    private async Task HandleFileAdded((FilePondError? Error, FilePondFileItem File) result)
    {
        if (result.Error is not null)
            return;

        await using Stream? stream = await _pond!.GetStreamForFile(
            result.File.Id,
            maxAllowedSize: MaxFileBytes);

        if (stream is null)
            return;

        // Read or upload the stream here.
    }
}
```

Selecting a file does not persist it. Use the returned stream, configure FilePond's `Server` options, or supply `OnServerProcess` to upload it.

`GetStreamForFile` returns the output after enabled transforms. Use `GetOriginalStreamForFile` when you need the browser-selected original. Dispose every returned stream. If neither `maxAllowedSize` nor `Options.MaxFileSize` is set, stream opening is limited to 2 MB.

## Handle `server.process` in Blazor

Use `OnServerProcess` when FilePond should delegate each upload to your Blazor code. Return the permanent or temporary server ID that FilePond should associate with the item.

```razor
@inject HttpClient Http

<FilePond Options="_options" OnServerProcess="Upload" />

@code {
    private const long MaxFileBytes = 10 * 1024 * 1024;

    private readonly FilePondOptions _options = new()
    {
        InstantUpload = true,
        MaxFileSize = MaxFileBytes,
        EnabledPlugins = [FilePondPluginType.FileValidateSize]
    };

    private async ValueTask<string> Upload(
        FilePondServerProcessRequest request,
        CancellationToken cancellationToken)
    {
        await using Stream? stream = await request.GetStream(
            MaxFileBytes,
            cancellationToken);

        if (stream is null)
            throw new InvalidOperationException("The selected file could not be opened.");

        await request.ReportProgress(
            isLengthComputable: true,
            loaded: 0,
            total: request.File.FileSize,
            cancellationToken);

        using var content = new StreamContent(stream);
        using HttpResponseMessage response = await Http.PostAsync(
            "api/uploads",
            content,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await request.ReportProgress(
            true,
            request.File.FileSize,
            request.File.FileSize,
            cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
```

The callback token is canceled when FilePond aborts the upload or the component is disposed. Let cancellation and upload failures propagate so FilePond can show the failed or canceled state. `ReportProgress` updates the UI only; call it with real byte counts when your upload client exposes progress.

## Validation and security

```csharp
await _pond!.SetValidationState(false, "Choose at least one image.");
await _pond.SetValidationState(true);
```

`IsValid`, `ValidationErrorMessage`, and `SetValidationState` control presentation. Client-side type, extension, count, and size checks are usability features—not a security boundary. Enforce authorization and limits on the receiving server, generate storage names instead of trusting `Filename`, inspect the actual content type/signature, and keep uploaded files outside executable web roots.

## CDN and plugins

`UseCdn` defaults to `true`; set it to `false` to load the packaged FilePond and official-plugin assets. `EnabledPlugins` loads supported plugins for the scoped session before the pond is created. Scripts for `EnabledOtherPlugins` must already exist on the page and the list must contain their JavaScript global names.

For the full options and method surface, use the typed `FilePondOptions` and `IFilePond` APIs; their names follow FilePond's corresponding options and methods.
