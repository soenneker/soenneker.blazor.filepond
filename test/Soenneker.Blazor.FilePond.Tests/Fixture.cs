using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Soenneker.Fixtures.Unit;
using Soenneker.Utils.Test;
using Soenneker.Blazor.MockJsRuntime.Registrars;
using Soenneker.Blazor.FilePond.Registrars;

namespace Soenneker.Blazor.FilePond.Tests;

public class Fixture : UnitFixture
{
    public override System.Threading.Tasks.ValueTask InitializeAsync()
    {
        SetupIoC(Services);

        Services.AddMockJsRuntimeAsScoped();

        return base.InitializeAsync();
    }

    private static void SetupIoC(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: true);
        });

        IConfiguration config = TestUtil.BuildConfig();
        services.AddSingleton(config);

        services.AddFilePondInteropAsScoped();
    }
}
