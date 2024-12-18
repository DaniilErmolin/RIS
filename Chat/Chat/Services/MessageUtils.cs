using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Chat.Models;

namespace Chat.Services;

public static class MessageUtils
{
    private static readonly Dictionary<int, List<byte[]?>> MessageBuffer = new();
    private static readonly HashSet<int> CompletedMessages = [];

    /// <summary>
    /// Разбирает входящие пакеты и собирает большие сообщения.
    /// </summary>
    public static string? ReceiveLargeMessage(Socket socket, ref EndPoint remoteEndpoint)
    {
        var buffer = new byte[1036];
        var receivedBytes = socket.ReceiveFrom(buffer, ref remoteEndpoint);

        var messageId = BitConverter.ToInt32(buffer, 0);
        if (CompletedMessages.Contains(messageId)) return null;
        var totalPackets = BitConverter.ToInt32(buffer, 4);
        var packetIndex = BitConverter.ToInt32(buffer, 8);

        var payload = new byte[receivedBytes - 12];
        Array.Copy(buffer, 12, payload, 0, payload.Length);

        lock (MessageBuffer)
        {
            if (!MessageBuffer.ContainsKey(messageId))
                MessageBuffer[messageId] =
                    Enumerable.Range(0, totalPackets).Select(_ => (byte[]?)null).ToList();


            MessageBuffer[messageId][packetIndex] = payload;

            if (MessageBuffer[messageId].All(p => p != null))
            {
                CompletedMessages.Add(messageId);
                var completeMessage = MessageBuffer[messageId]
                    .SelectMany(packet => packet)
                    .ToArray();

                MessageBuffer.Remove(messageId);
                return Encoding.UTF8.GetString(completeMessage);
            }
        }

        return null;
    }

    /// <summary>
    /// Отправляет большое сообщение, разбивая его на пакеты.
    /// </summary>
    public static void SendLargeMessage(Socket socket, string jsonMessage, EndPoint remoteEndpoint)
    {
        try
        {
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

                //socket.SendTo(packet, remoteEndpoint);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    /// <summary>
    /// Десериализует JSON в строку.
    /// </summary>
    public static string DeserializeMessage(byte[] data)
    {
        return Encoding.UTF8.GetString(data);
    }

    /// <summary>
    /// Отправляет ответ (объект) клиенту.
    /// </summary>
    public static void SendResponse(Socket serverSocket, object response, EndPoint clientEndpoint)
    {
        var responseJson = JsonSerializer.Serialize(response);
        SendLargeMessage(serverSocket, responseJson, clientEndpoint);
    }

    /// <summary>
    /// Отправляет строковый ответ клиенту.
    /// </summary>
    public static void SendResponse(Socket serverSocket, string response, EndPoint clientEndpoint)
    {
        SendLargeMessage(serverSocket, response, clientEndpoint);
    }

    /// <summary>
    /// Десериализует JSON-запрос в объект запроса.
    /// </summary>
    public static RequestBase? DeserializeRequest(string jsonMessage)
    {
        using var document = JsonDocument.Parse(jsonMessage);
        var root = document.RootElement;

        if (!root.TryGetProperty("Type", out var typeProperty))
            throw new Exception("Type property not found in JSON");

        var type = typeProperty.GetString();
        return type switch
        {
            "auth" => JsonSerializer.Deserialize<AuthRequest>(jsonMessage),
            "reg" => JsonSerializer.Deserialize<RegisterRequest>(jsonMessage),
            "message" => JsonSerializer.Deserialize<MessageRequest>(jsonMessage),
            "history" => JsonSerializer.Deserialize<HistoryRequest>(jsonMessage),
            _ => throw new Exception("Unknown request type")
        };
    }
}
