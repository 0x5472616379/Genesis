namespace Genesis;

public class ServerLogger
{
    public static void WarnAboutDeficit(TimeSpan sleepTime, double elapsedMilliseconds)
    {
        Console.WriteLine($"Server can't keep up!\nElapsed: {elapsedMilliseconds} ms\nDeficit: {-sleepTime.TotalMilliseconds} ms.");
    }
}