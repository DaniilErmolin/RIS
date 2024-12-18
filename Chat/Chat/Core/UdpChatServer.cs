using System.Net;
using System.Net.Sockets;
using Chat.Services;

namespace Chat.Core;

public class UdpChatServer
{
    private readonly ClientManager _clientManager = new();
    private readonly int _port;
    private readonly Socket _udpSocket;

    public UdpChatServer(int port)
    {
        _port = port;
        _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _udpSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
    }

    public void Start()
    {
        Console.WriteLine($"UDP chat server started on port {_port}...");
        
        while (true)
        {
            var clientEndpoint = new IPEndPoint(IPAddress.Any, 0) as EndPoint;
            var message = MessageUtils.ReceiveLargeMessage(_udpSocket, ref clientEndpoint);

            if (!string.IsNullOrEmpty(message))
            {
                ThreadPool.QueueUserWorkItem(_ =>
                    _clientManager.HandleClient(message, (IPEndPoint)clientEndpoint, _udpSocket));
            }
            Console.Beep();
        }
    }
}