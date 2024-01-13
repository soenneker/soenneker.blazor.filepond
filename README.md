[![](https://img.shields.io/nuget/v/soenneker.blazor.filepond.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.filepond/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.filepond/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.filepond/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.filepond.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.filepond/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.FilePond
### A Blazor interop library for the file upload library [FilePond](https://pqina.nl/filepond/)

This library simplifies the integration of FilePond into Blazor applications, providing access to options, methods, plugins, and events. A demo project showcasing common usages is included.

Diligence was taken to align the API, options, and events closely with the JS. Refer to the [FilePond documentation](https://pqina.nl/filepond/docs/) for details.

## Installation

```
dotnet add package Soenneker.Blazor.FilePond
```

### 1. Add the following to your `_Imports.razor` file

```razor
@using Soenneker.Blazor.FilePond
```

### 2. Add the following to your `Startup.cs` file

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddFilePond();
}
```

### 3. Add the stylesheet to your `wwwroot/index.html` file

```html
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/filepond@4.30.6/dist/filepond.min.css">
```

### 4. Add the scripts to your `wwwroot/index.html` file

```html
<script src="https://cdn.jsdelivr.net/npm/filepond@4.30.6/dist/filepond.min.js"></script>
<script src="_content/Soenneker.Blazor.FilePond/filepondinterop.js"></script>
```

## Usage

```razor
<FilePond @ref="FilePond" Options="_options" OnAddFile="OnAddFile"></FilePond>

@code{
    private FilePond? FilePond { get; set; }

    private readonly FilePondOptions _options = new()
    {
        MaxFiles = 20,
        AllowMultiple = true
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Add any plugins you want to use
            //await FilePond!.EnablePlugins(FilePondPluginType.FileValidateType, FilePondPluginType.ImagePreview);
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

⚠️ While 95%+ of the FilePond JS has been implemented, there are a few features not yet supported. If you need assistance or want to request a new feature, please open an issue or submit a pull request.
