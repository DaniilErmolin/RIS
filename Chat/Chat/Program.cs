using Chat.Core;

namespace Chat;

internal static class Program
{
    public static void Main()
    {
        ThreadPool.SetMinThreads(100, 0);
        var chatServer = new UdpChatServer(8080);
        chatServer.Start();
    }
}