using System.Data.SQLite;

namespace Chat.Storage;

public static class MessageHistory
{
    public static string ConnectionString = @"Data Source=D:\4\RISERMOLIN\Chat\Chat\Db\messages.db;Version=3;";

    public const string CreateTableQuery = """
                                            CREATE TABLE IF NOT EXISTS MessageHistory (
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                Username TEXT NOT NULL,
                                                Message TEXT NOT NULL,
                                                Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                                            );
                                            """;

    private const string InsertMessageQuery = @"
            INSERT INTO MessageHistory (Username, Message) VALUES (@Username, @Message);";

    private const string GetMessagesQuery = @"
            SELECT Message FROM MessageHistory WHERE Username = @Username ORDER BY Timestamp;";

    static MessageHistory()
    {
        InitializeDatabase();
    }

    private static void InitializeDatabase()
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(CreateTableQuery, connection);
        command.ExecuteNonQuery();
    }

    public static void LogMessage(string username, string message)
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(InsertMessageQuery, connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Message", message);

        command.ExecuteNonQuery();
    }

    public static List<string> GetMessages(string username)
    {
        var messages = new List<string>();

        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(GetMessagesQuery, connection);
        command.Parameters.AddWithValue("@Username", username);

        using var reader = command.ExecuteReader();
        while (reader.Read()) messages.Add(reader["Message"].ToString()!);

        return messages;
    }
}