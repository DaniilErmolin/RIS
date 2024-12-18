namespace Chat.Models;

public class RequestBase
{
    public string Type { get; set; } = string.Empty;
}

public class AuthRequest : RequestBase
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest : RequestBase
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class MessageRequest : RequestBase
{
    public string Content { get; set; } = string.Empty;
    public bool IsDirect { get; set; } = false;
    public string Receiver {  get; set; } = string.Empty;
}

public class HistoryRequest : RequestBase
{
    public string Username { get; set; } = string.Empty;
}