using Soenneker.Blazor.FilePond.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Blazor.FilePond.Tests;

[Collection("Collection")]
public class FilePondInteropTests : FixturedUnitTest
{
    private readonly IFilePondInterop _interop;

    public FilePondInteropTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _interop = Resolve<IFilePondInterop>(true);
    }
}
