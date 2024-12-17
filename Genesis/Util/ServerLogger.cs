using System.Net;
using System.Net.Sockets;

namespace Genesis;

public class ServerLogger
{
    public static void WarnAboutDeficit(TimeSpan sleepTime, double elapsedMilliseconds)
    {
        Console.WriteLine($"Server can't keep up!\nElapsed: {elapsedMilliseconds} ms\nDeficit: {-sleepTime.TotalMilliseconds} ms.");
    }

    public static void IncomingConnectionMessage(TcpClient tcpClient)
    {
        Console.WriteLine($"Incoming Connection From: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString()}");
    }
}