using System.Threading;

namespace VeriBot.Services;

public class CancellationService
{
    private readonly CancellationTokenSource _cts;

    public CancellationToken Token => _cts.Token;

    public CancellationService()
    {
        _cts = new CancellationTokenSource();
    }

    public void Cancel() => _cts.Cancel();
}