using System.Diagnostics;
using Genesis.Environment;

namespace Genesis;

public class RSServer
{
    private bool _isRunning;
    
    public void Run()
    {
        _isRunning = true;
        var stopwatch = StartStopwatch();
        ConnectionManager.Initialize();

        while (_isRunning)
        {
            ConnectionManager.AcceptClients();
            World.Process();
            SleepIfRequired(stopwatch);
        }
    }

    private Stopwatch StartStopwatch()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        return stopwatch;
    }

    private void SleepIfRequired(Stopwatch stopwatch)
    {
        stopwatch.Stop();
        var sleepTime = CalculateSleepTime(stopwatch.Elapsed.TotalMilliseconds);

        if (sleepTime > TimeSpan.Zero)
            Thread.Sleep(sleepTime);
        else
            ServerLogger.WarnAboutDeficit(sleepTime, stopwatch.Elapsed.TotalMilliseconds);
    }

    private TimeSpan CalculateSleepTime(double elapsedMilliseconds)
    {
        return TimeSpan.FromMilliseconds(ServerConfig.TICK_RATE - elapsedMilliseconds);
    }
}