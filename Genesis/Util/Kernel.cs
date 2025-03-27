using System.Diagnostics;

static class Kernel
{
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private static long _ticksPerTickRate;
    private static long _startTicks;

    public static bool TryGetFrequency(out long frequency)
    {
        if (Stopwatch.IsHighResolution)
        {
            frequency = Stopwatch.Frequency;
            return true;
        }
        frequency = 0;
        return false;
    }

    public static void InitializeTick(long frequency, int tickRateMs)
    {
        _ticksPerTickRate = (frequency / 1000) * tickRateMs;
    }

    public static void StartTick()
    {
        _startTicks = GetPerformanceCounter();
    }

    public static void WaitForNextTick()
    {
        long targetTicks = _startTicks + _ticksPerTickRate;

        while (true)
        {
            long currentTicks = GetPerformanceCounter();
            if (currentTicks >= targetTicks)
            {
                break;
            }
        }
    }

    public static void WarnIfTickExceeded(int tickRate)
    {
        double tickDuration = GetLastTickDurationMs();
        if ((int)tickDuration > tickRate)
        {
            Console.WriteLine($"Warning: Tick duration exceeded! Took {tickDuration:F2} ms.");
        }
    }

    public static void PrintTickDuration()
    {
        Console.WriteLine($"Total tick duration: {GetLastTickDurationMs():F2} ms");
    }
    
    public static double GetLastTickDurationMs()
    {
        long currentTicks = GetPerformanceCounter();
        return (currentTicks - _startTicks) * 1000.0 / Stopwatch.Frequency;
    }

    public static long GetPerformanceCounter()
    {
        return _stopwatch.ElapsedTicks;
    }
}