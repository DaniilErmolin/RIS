namespace Client.Models;

public class RequestBase
{
    public string Type { get; set; } = string.Empty;
}

public class AuthRequest : RequestBase
{
    public AuthRequest()
    {
        Type = "auth";
    }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest : RequestBase
{
    public RegisterRequest()
    {
        Type = "reg";
    }

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class MessageModel : RequestBase
{
    public MessageModel()
    {
        Type = "message";
    }

    public string Content { get; set; } = string.Empty;
    public bool IsDirect { get; set; } = false;

    public string Receiver { get; set; } = string.Empty;
}

public class HistoryRequest : RequestBase
{
    public HistoryRequest()
    {
        Type = "history";
    }

    public string Username { get; set; } = string.Empty;
}