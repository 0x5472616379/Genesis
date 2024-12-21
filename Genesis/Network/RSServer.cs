using Genesis.Environment;

namespace Genesis;

public class RSServer
{
    private bool _isRunning;

    public void Run()
    {
        _isRunning = true;
        ConnectionManager.Initialize();

        if (!Kernel.TryGetFrequency(out long frequency))
        {
            Console.WriteLine("High-resolution performance counter not supported.");
            return;
        }

        Kernel.InitializeTick(frequency, ServerConfig.TICK_RATE);

        while (_isRunning)
        {
            Kernel.StartTick();

            ConnectionManager.AcceptClients();

            World.Process();

            Kernel.WaitForNextTick();
            Kernel.WarnIfTickExceeded();
        }
    }
}