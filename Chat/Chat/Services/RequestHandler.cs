using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Chat.Models;
using Chat.Storage;

namespace Chat.Services;

public static class RequestHandler
{
    public static void ProcessRequest(string message, IPEndPoint clientEndpoint, 
        ConcurrentDictionary<IPEndPoint, User> clients, Socket serverSocket)
    {
        var baseRequest = MessageUtils.DeserializeRequest(message);
        if (baseRequest == null) return;

        switch (baseRequest.Type)
        {
            case "auth":
                HandleAuth(clientEndpoint, baseRequest as AuthRequest, clients, serverSocket);
                break;

            case "reg":
                HandleRegistration(clientEndpoint, baseRequest as RegisterRequest, serverSocket);
                break;

            case "message":
                HandleMessage(clientEndpoint, baseRequest as MessageRequest, clients, serverSocket);
                break;

            case "history":
                HandleHistory(clientEndpoint, baseRequest as HistoryRequest, clients, serverSocket);
                break;
        }
    }

    private static void HandleAuth(IPEndPoint clientEndpoint, AuthRequest? request,
        ConcurrentDictionary<IPEndPoint, User> clients, Socket serverSocket)
    {
        if (request == null) return;
        var user = Authentication.Authenticate(request.Username, request.Password);

        if (user != null)
        {
            clients[clientEndpoint] = user;

            var response = new AuthResponse
            {
                Type = "auth",
                Status = "success",
                Message = $"Авторизован как {user.Username}",
                Username = user.Username
            };

            MessageUtils.SendResponse(serverSocket, response, clientEndpoint);
        }
        else
        {
            var response = new AuthResponse
            {
                Type = "auth",
                Status = "failed",
                Message = "Неверные данные"
            };

            MessageUtils.SendResponse(serverSocket, response, clientEndpoint);
        }
    }

    private static void HandleRegistration(IPEndPoint clientEndpoint, RegisterRequest? request, Socket serverSocket)
    {
        if (request == null) return;
        if (Authentication.Register(request.Username, request.Password, "user"))
        {
            var response = new RegisterResponse
            {
                Type = "reg",
                Status = "success",
                Message = "Успешная регистрация",
                Username = request.Username
            };

            MessageUtils.SendResponse(serverSocket, response, clientEndpoint);
        }
        else
        {
            var response = new RegisterResponse
            {
                Type = "reg",
                Status = "failed",
                Message = "Такой пользователь уже существует"
            };

            MessageUtils.SendResponse(serverSocket, response, clientEndpoint);
        }
    }

    private static void HandleMessage(IPEndPoint clientEndpoint, MessageRequest? request,
        ConcurrentDictionary<IPEndPoint, User> clients, Socket serverSocket)
    {
        if (request == null) return;
        var user = clients.GetValueOrDefault(clientEndpoint);
        if (user == null) return;

        MessageHistory.LogMessage(user.Username, request.Content);
        if (!request.IsDirect)
        {
            var response = new MessageResponse
            {
                Type = "message",
                Status = "success",
                Username = user.Username,
                Content = request.Content,
                IsDirect = false
            };

            BroadcastMessage(response, clients.Keys, serverSocket);
        }
        else
        {
            var response = new MessageResponse
            {
                Type = "message",
                Status = "success",
                Username = user.Username,
                Content = request.Content,
                IsDirect = true
            };
            foreach (var client in clients)
            {
                if (client.Value.Username == request.Receiver)
                    SendDirectMessage(response, client.Key, serverSocket);
            }
        }
    }

    private static void HandleHistory(IPEndPoint clientEndpoint, HistoryRequest? request,
        ConcurrentDictionary<IPEndPoint, User> clients, Socket serverSocket)
    {
        if (request == null) return;
        var user = clients.GetValueOrDefault(clientEndpoint);
        var exists = Authentication.Exists(request.Username);

        switch (exists)
        {
            case true when user != null && (user.Username == request.Username || user.Role == "admin"):
            {
                var history = MessageHistory.GetMessages(request.Username);

                var response = new HistoryResponse
                {
                    Type = "history",
                    Status = "success",
                    Username = request.Username,
                    Messages = history
                };

                MessageUtils.SendResponse(serverSocket, response, clientEndpoint);
                break;
            }
            case false:
            {
                var response = new HistoryResponse
                {
                    Type = "history",
                    Status = "failed",
                    Message = "Такого пользователя не существует",
                    Username = request.Username
                };

                MessageUtils.SendResponse(serverSocket, response, clientEndpoint);
                break;
            }
            default:
            {
                var response = new HistoryResponse
                {
                    Type = "history",
                    Status = "failed",
                    Message = "Отказано в доступе",
                    Username = request.Username
                };

                MessageUtils.SendResponse(serverSocket, response, clientEndpoint);
                break;
            }
        }
    }

    private static void BroadcastMessage(object message, IEnumerable<IPEndPoint> clients, Socket serverSocket)
    {
        var responseJson = JsonSerializer.Serialize(message);
        foreach (var client in clients)
        {
            MessageUtils.SendResponse(serverSocket, responseJson, client);
        }
    }

    private static void SendDirectMessage(object message, IPEndPoint client, Socket serverSocket)
    {
        var responseJson = JsonSerializer.Serialize(message);
        MessageUtils.SendResponse(serverSocket, responseJson, client);
    }
}
