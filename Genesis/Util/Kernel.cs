using Genesis;
using System.Runtime.InteropServices;

static class Kernel
{
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long lpFrequency);

    private static long _frequency;
    private static long _ticksPerTickRate;
    private static long _startTicks;

    public static bool TryGetFrequency(out long frequency)
    {
        return QueryPerformanceFrequency(out frequency);
    }

    public static void InitializeTick(long frequency, int tickRateMs)
    {
        _frequency = frequency;
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

    public static void WarnIfTickExceeded()
    {
        double tickDuration = GetLastTickDurationMs();
        if ((int)tickDuration > ServerConfig.TICK_RATE)
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
        return (currentTicks - _startTicks) * 1000.0 / _frequency;
    }

    public static long GetPerformanceCounter()
    {
        QueryPerformanceCounter(out long performanceCount);
        return performanceCount;
    }
}