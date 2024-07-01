[![](https://img.shields.io/nuget/v/soenneker.blazor.filepond.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.filepond/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.filepond/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.filepond/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.filepond.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.filepond/)

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

⚠️ While 95%+ of the FilePond JS has been implemented, there are a few features not yet supported. If you need assistance or want to request a new feature, please open an issue or submit a pull request.
