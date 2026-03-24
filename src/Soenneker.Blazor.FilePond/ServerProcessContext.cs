using System.Threading;

namespace Soenneker.Blazor.FilePond;

internal sealed class ServerProcessContext
{
    public string ElementId { get; }
    public CancellationTokenSource CancellationTokenSource { get; }

    public ServerProcessContext(string elementId, CancellationTokenSource cancellationTokenSource)
    {
        ElementId = elementId;
        CancellationTokenSource = cancellationTokenSource;
    }
}
