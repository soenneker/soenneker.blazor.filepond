using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.FilePond.Abstract;
using Soenneker.Blazor.Utils.InteropEventListener.Registrars;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Blazor.FilePond.Registrars;

/// <summary>
/// A Blazor interop library for the file upload library FilePond
/// </summary>
public static class FilePondInteropRegistrar
{
    /// <summary>
    /// Adds <see cref="IFilePondInterop"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddFilePondInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped()
                .AddInteropEventListenerAsScoped()
                .TryAddScoped<IFilePondInterop, FilePondInterop>();

        return services;
    }
}
