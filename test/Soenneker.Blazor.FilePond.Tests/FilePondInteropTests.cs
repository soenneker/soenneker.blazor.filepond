using Soenneker.Blazor.FilePond.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Blazor.FilePond.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class FilePondInteropTests : HostedUnitTest
{
    private readonly IFilePondInterop _util;

    public FilePondInteropTests(Host host) : base(host)
    {
        _util = Resolve<IFilePondInterop>(true);
    }

    [Test]
    public void Default()
    {

    }

}
