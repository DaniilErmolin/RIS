using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Chat.Models;

namespace Chat.Services;

public class ClientManager
{
    private readonly ConcurrentDictionary<IPEndPoint, User> _clients = new();

    public void HandleClient(string message, IPEndPoint clientEndpoint, Socket serverSocket)
    {
        try
        {
            if (!_clients.ContainsKey(clientEndpoint))
            {
                _clients[clientEndpoint] = new User { Username = "Anon", Role = "user" };
                Console.WriteLine($"New client connected: {clientEndpoint}");
            }

            Console.WriteLine($"Message from {clientEndpoint}: {message}");
            RequestHandler.ProcessRequest(message, clientEndpoint, _clients, serverSocket);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client {clientEndpoint}: {ex.Message}");
        }
    }
}