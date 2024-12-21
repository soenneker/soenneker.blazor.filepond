using Soenneker.Blazor.FilePond.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Blazor.FilePond.Tests;

[Collection("Collection")]
public class FilePondInteropTests : FixturedUnitTest
{
    private readonly IFilePondInterop _util;

    public FilePondInteropTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IFilePondInterop>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
