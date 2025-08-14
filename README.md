[![](https://img.shields.io/nuget/v/soenneker.blazor.filepond.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/soenneker.blazor.filepond/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.filepond/publish-package.yml?style=for-the-badge&logo=github)](https://github.com/soenneker/soenneker.blazor.filepond/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.filepond.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/soenneker.blazor.filepond/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.blazor.filepond)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.FilePond
### A Blazor interop library for the file upload library [FilePond](https://pqina.nl/filepond/)

This library simplifies the integration of FilePond into Blazor applications, providing access to options, methods, plugins, and events. A demo project showcasing common usages is included.

Diligence was taken to align the Blazor API with JS. Refer to the [FilePond documentation](https://pqina.nl/filepond/docs/) for details.

## Installation

```
dotnet add package Soenneker.Blazor.FilePond
```

### Add the following to your `Startup.cs` file

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddFilePond();
}
```

⚠ Do not include styles or scripts on the page as they get lazily injected automatically, including most plugins.

## Usage

```razor
@using Soenneker.Blazor.FilePond

<FilePond @ref="FilePond" Options="_options" OnAddFile="OnAddFile"></FilePond>

@code{
    private FilePond? FilePond { get; set; }

    private readonly FilePondOptions _options = new()
    {
        MaxFiles = 20,
        AllowMultiple = true,
        EnabledPlugins = [FilePondPluginType.ImagePreview]
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await FilePond.AddFile("https://picsum.photos/500/500");
        }
    }

    private async Task OnAddFile((FilePondError? error, FilePondFileItem fileItem) obj)
    {
        Logger.LogInformation("OnAddFile fired: Filename: {fileName}", obj.fileItem.Filename);
        Stream? stream = await FilePond!.GetStreamForFile();
        // do something with the stream
        await stream.DisposeAsync();
    }
}
```

## Validation Features

The FilePond component supports validation states with visual feedback and error messages.

### Basic Validation

```razor
<FilePond @ref="FilePond" 
          IsValid="@_isValid"
          ValidationErrorMessage="@_validationErrorMessage">
</FilePond>

@code {
    private bool _isValid = true;
    private string? _validationErrorMessage;

    private async Task ValidateFiles()
    {
        var files = await FilePond!.GetFiles();
        if (files?.Count == 0)
        {
            await FilePond.SetValidationState(false, "Please select at least one file.");
        }
        else
        {
            await FilePond.SetValidationState(true);
        }
    }
}
```

### Programmatic Validation Control

```csharp
// Set validation state
await FilePond.SetValidationState(false, "Custom error message");

// Clear validation state
await FilePond.SetValidationState(true);
```

## File Success States

You can programmatically set files to appear green (success state) within FilePond.

### Setting Individual File Success

```csharp
// Set a specific file as successful by ID
await FilePond.SetFileSuccess(fileId, true);

// Set a specific file as successful by index
await FilePond.SetFileSuccess(0, true);

// Clear success state
await FilePond.SetFileSuccess(fileId, false);

// Set file success when the file is fully processed and ready (recommended)
await FilePond.SetFileSuccessWhenReady(fileId, true);
```

### Setting All Files Success

```csharp
// Set all files as successful
await FilePond.SetAllFilesSuccess(true);

// Clear all success states
await FilePond.SetAllFilesSuccess(false);
```

### Example: Auto-success on File Upload

```csharp
private async Task OnAddFile((FilePondError? error, FilePondFileItem fileItem) obj)
{
    // Process the file...
    
    // Set the file as successful when it's ready (recommended approach)
    await FilePond!.SetFileSuccessWhenReady(obj.fileItem.Id, true);
}
```

## Demo

Check out the demo project for complete examples:
- Basic usage: `/`
- Validation & Success features: `/validation-demo`