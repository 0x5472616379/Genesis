using Genesis.Configuration;
using Genesis.Environment;
using Genesis.Network;

namespace Genesis;

public class RSServer
{
    private bool _isRunning;

    public void Run()
    {

        if (!Kernel.TryGetFrequency(out long frequency))
        {
            Console.WriteLine("High-resolution performance counter not supported.");
            return;
        }

        ConnectionManager.Initialize();

        Kernel.InitializeTick(frequency, ServerConfig.TICK_RATE);

        _isRunning = true;

        while (_isRunning)
        {
            Kernel.StartTick();

            ConnectionManager.AcceptClients();

            World.Process();

            Kernel.WaitForNextTick();
            Kernel.WarnIfTickExceeded(600);
            // Kernel.PrintTickDuration();
        }
    }
}