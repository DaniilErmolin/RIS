using System.Text.Json;
using System.Windows.Media;
using Client.Models;

namespace Client.Services;

public class MessageHandler(
    Action<string> appendToChat,
    Action<string> showMessageBox,
    Action<string[]> showHistoryPanel,
    Action openChat)
{
    public void ProcessMessage(string jsonMessage)
    {
        try
        {
            var baseResponse = DeserializeResponse(jsonMessage);
            if (baseResponse == null) throw new Exception("Invalid message format");
            switch (baseResponse.Type)
            {
                case "auth":
                    HandleAuthResponse(baseResponse as AuthResponse);
                    break;
                case "reg":
                    HandleRegResponse(baseResponse as RegisterResponse);
                    break;
                case "message":
                    HandleChatMessage(baseResponse as MessageResponse);
                    break;
                case "history":
                    HandleHistoryResponse(baseResponse as HistoryResponse);
                    break;
                default:
                    appendToChat($"[Неизвестный]: {jsonMessage}");
                    break;
            }
        }
        catch (Exception ex)
        {
            appendToChat($"Ошибка обработки сообщения: {ex.Message}");
        }
    }

    private static ResponseBase? DeserializeResponse(string jsonMessage)
    {
        using var document = JsonDocument.Parse(jsonMessage);
        var root = document.RootElement;

        if (!root.TryGetProperty("Type", out var typeProperty))
            throw new Exception("Type property not found in JSON");
        var type = typeProperty.GetString();
        return type switch
        {
            "auth" => JsonSerializer.Deserialize<AuthResponse>(jsonMessage),
            "reg" => JsonSerializer.Deserialize<RegisterResponse>(jsonMessage),
            "message" => JsonSerializer.Deserialize<MessageResponse>(jsonMessage),
            "history" => JsonSerializer.Deserialize<HistoryResponse>(jsonMessage),
            _ => throw new Exception("Unknown response type")
        };
    }


    private void HandleRegResponse(RegisterResponse? response)
    {
        if (response != null)
            showMessageBox(response.Status == "success"
                ? "Успешная регистрация"
                : response.Message);
    }

    private void HandleAuthResponse(AuthResponse? response)
    {
        if (response == null) return;
        if (response.Status == "success")
        {
            appendToChat($"Добро пожаловать, {response.Username}!");
            openChat();
        }
        else
        {
            showMessageBox(response.Message);
        }
    }

    private void HandleChatMessage(MessageResponse? response)
    {
        if (response == null) return;
        var username = response.Username;
        var content = response.Content;

        appendToChat($"{(response.IsDirect ? "Личное сообщение: " : "")}[{username}]: {content}");
    }

    private void HandleHistoryResponse(HistoryResponse? response)
    {
        if (response == null) return;
        if (response.Status == "success")
            showHistoryPanel(response.Messages);
        else
            showMessageBox($"Ошибка: {response.Message}");
    }
}