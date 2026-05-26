using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Logic;

public class VisualStopwatch
{
    public VisualStopwatch()
    {
        StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public double CurrentTime { get; private set; }
    public double StartTime { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task RunStopwatch(CancellationToken token)
    {

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1.0 / ILogic.FPS));

        try
        {
            while (await timer.WaitForNextTickAsync(token))
            {
                CurrentTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - StartTime) / 1000.0;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }
        catch (OperationCanceledException)
        {
            // expected, token triggered cancellation
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
