using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Chat.Models;
using Fleck;

namespace Chat.StressTests;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        const string ip = "127.0.0.1";
        const int port = 8080;
        var serverEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);

        List<Socket> clients = [];

        var auth = new AuthRequest { Username = "admin", Password = "1234", Type = "auth"};

        for (var i = 0; i < 100; i++)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect(ip, port);
            SendMessage(auth, socket, serverEndpoint);
            clients.Add(socket);
        }

        var msg = GenerateBigMsg(100);

        for (var i = 0; i < 1000; i++)
        {
            foreach (var client in clients)
            {
                SendMessage(msg, client, serverEndpoint);
            }

            Console.WriteLine($"Отправлено {(i + 1) * clients.Count} запросов");
        }

        Console.Read();
    }

    private static void SendMessage(object message, Socket socket, IPEndPoint serverEndpoint)
    {
        var jsonMessage = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
        var messageId = BitConverter.GetBytes(Guid.NewGuid().GetHashCode());
        var totalPackets = (int)Math.Ceiling((double)messageBytes.Length / 1024);

        for (var i = 0; i < totalPackets; i++)
        {
            var packetIndex = BitConverter.GetBytes(i);
            var totalPacketsBytes = BitConverter.GetBytes(totalPackets);

            var offset = i * 1024;
            var payloadSize = Math.Min(1024, messageBytes.Length - offset);
            var payload = new byte[payloadSize];
            Array.Copy(messageBytes, offset, payload, 0, payloadSize);

            var packet = new byte[12 + payload.Length];
            Buffer.BlockCopy(messageId, 0, packet, 0, 4);
            Buffer.BlockCopy(totalPacketsBytes, 0, packet, 4, 4);
            Buffer.BlockCopy(packetIndex, 0, packet, 8, 4);
            Buffer.BlockCopy(payload, 0, packet, 12, payload.Length);

            socket.SendTo(packet, serverEndpoint);
        }
    }

    private static MessageRequest GenerateBigMsg(int contentSize)
    {
        var content = new StringBuilder();
        for (var i = 0; i < contentSize; i++)
        {
            content.Append('s');
        }

        return new MessageRequest
        {
            Content = content.ToString(),
            Type = "message",
            IsDirect = false
        };
    }
}