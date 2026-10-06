using System.Text.Json;
using Soenneker.Blazor.FilePond;

var configuration = JsonSerializer.Deserialize("{}", LibraryJsonContext.Default.FilePondOptions)!;
var payload = JsonSerializer.SerializeToElement(configuration, LibraryJsonContext.Default.FilePondOptions);
Check(payload.ValueKind == JsonValueKind.Object, "configuration wire object");
var options = LibraryJsonContext.WithContext(SmokeJsonContext.Default);
var custom = JsonSerializer.SerializeToElement<object>(new SmokePayload { Value = "custom" }, (System.Text.Json.Serialization.Metadata.JsonTypeInfo<object>)options.GetTypeInfo(typeof(object)));
Check(custom.GetProperty("value").GetString() == "custom", "application-generated metadata");
var add = JsonSerializer.Deserialize("""{"index":3,"mimeType":"image/png"}""", LibraryJsonContext.Default.FilePondAddFileOptions)!;
Check(add.Index == 3 && add.MimeType == "image/png", "add-file options");
var remove = JsonSerializer.Deserialize("""{"revert":false}""", LibraryJsonContext.Default.FilePondRemoveFileOptions)!;
Check(remove.Revert == false, "remove-file options");

Console.WriteLine("Trimmed JSON smoke checks passed.");

static void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
}
