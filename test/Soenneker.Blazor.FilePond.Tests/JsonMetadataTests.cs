using System.Text.Json;
using System.Threading.Tasks;
using Soenneker.Blazor.FilePond.Dtos;
using Soenneker.Blazor.FilePond.Enums;
using Soenneker.Utils.Json;

namespace Soenneker.Blazor.FilePond.Tests;

public class JsonMetadataTests
{
    [Test]
    public async Task File_origin_remains_numeric_and_round_trips()
    {
        var metadata = LibraryJsonContext.Get<FilePondFileItem>();
        var value = JsonUtil.Deserialize("{\"id\":\"file-1\",\"origin\":1}", metadata)!;
        await Assert.That(value.Origin).IsEqualTo(FilePondFileOrigin.Input);
        using JsonDocument document = JsonDocument.Parse(JsonUtil.Serialize(value, metadata));
        await Assert.That(document.RootElement.GetProperty("origin").GetInt32()).IsEqualTo(1);
    }
}
