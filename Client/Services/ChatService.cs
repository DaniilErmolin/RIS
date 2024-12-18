using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client.Services;

public class ChatService
{
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private IPEndPoint? _serverEndpoint;
    private readonly IPEndPoint _localEndpoint = new(IPAddress.Any, 0);

    private readonly Dictionary<int, List<byte[]?>> _messageBuffer = new();
    private readonly HashSet<int> _completedMessages = [];

    public bool IsConnected => _serverEndpoint != null;

    public void Connect(string ip, int port)
    {
        _serverEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        _socket.Bind(_localEndpoint);
    }

    public bool SendMessage(object message)
    {
        if (!IsConnected) return false;
        try
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

                _socket.SendTo(packet, _serverEndpoint!);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string ReceiveMessage()
    {
        var buffer = new byte[1036];
        var senderEndpoint = new IPEndPoint(IPAddress.Any, 0) as EndPoint;

        var receivedBytes = _socket.ReceiveFrom(buffer, ref senderEndpoint);

        var messageId = BitConverter.ToInt32(buffer, 0);
        if (_completedMessages.Contains(messageId)) return string.Empty;
        var totalPackets = BitConverter.ToInt32(buffer, 4);
        var packetIndex = BitConverter.ToInt32(buffer, 8);
        var payload = new byte[receivedBytes - 12];
        Array.Copy(buffer, 12, payload, 0, payload.Length);

        lock (_messageBuffer)
        {
            if (!_messageBuffer.ContainsKey(messageId))
                _messageBuffer[messageId] = 
                    Enumerable.Range(0, totalPackets).Select(_ => (byte[]?)null).ToList();

            _messageBuffer[messageId][packetIndex] = payload;

            if (_messageBuffer[messageId].All(p => p != null))
            {
                _completedMessages.Add(messageId);

                var completeMessage = _messageBuffer[messageId]
                    .SelectMany(packet => packet)
                    .ToArray();

                _messageBuffer.Remove(messageId);
                return Encoding.UTF8.GetString(completeMessage);
            }
        }

        return string.Empty; 
    }
}
