using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Blazor.FilePond.Dtos;

namespace Soenneker.Blazor.FilePond;

internal sealed record ServerProcessRegistration(Func<FilePondServerProcessRequest, CancellationToken, ValueTask<string>> Handler, CancellationToken CancellationToken);
