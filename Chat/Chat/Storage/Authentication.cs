using System.Data.SQLite;
using Chat.Models;

namespace Chat.Storage;

public static class Authentication
{
    public static string ConnectionString = @"Data Source=D:\4\RISERMOLIN\Chat\Chat\Db\users.db;Version=3;";

    private const string AuthQuery =
        "SELECT Username, Password, Role FROM Users WHERE Username = @Username AND Password = @Password;";
    
    private const string ExistsQuery =
        "SELECT COUNT(*) FROM Users WHERE Username = @Username;";

    public const string CreateTableQuery = """
                                            CREATE TABLE IF NOT EXISTS Users (
                                                Username TEXT PRIMARY KEY,
                                                Password TEXT NOT NULL,
                                                Role TEXT NOT NULL
                                            );
                                            """;

    private const string CheckUserExistsQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username;";

    private const string RegisterUserQuery =
        "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role);";

    static Authentication()
    {
        InitializeDatabase();
    }

    private static void InitializeDatabase()
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(CreateTableQuery, connection);
        command.ExecuteNonQuery();

        Register("admin", "1234", "admin");
    }

    public static bool Exists(string username)
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();
        
        using var command = new SQLiteCommand(ExistsQuery, connection);
        command.Parameters.AddWithValue("@Username", username);

        var result = (long)command.ExecuteScalar();
        return result != 0;
    }

    public static User? Authenticate(string username, string password)
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        using var command = new SQLiteCommand(AuthQuery, connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Password", password);

        using var reader = command.ExecuteReader();
        if (reader.Read())
            return new User
            {
                Username = reader["Username"].ToString()!,
                Password = reader["Password"].ToString()!,
                Role = reader["Role"].ToString()!
            };
        Console.Beep();
        return null;
    }

    public static bool Register(string username, string password, string role)
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        using var checkCommand = new SQLiteCommand(CheckUserExistsQuery, connection);
        checkCommand.Parameters.AddWithValue("@Username", username);

        var userExists = (long)checkCommand.ExecuteScalar() > 0;
        if (userExists) return false;

        using var registerCommand = new SQLiteCommand(RegisterUserQuery, connection);
        registerCommand.Parameters.AddWithValue("@Username", username);
        registerCommand.Parameters.AddWithValue("@Password", password);
        registerCommand.Parameters.AddWithValue("@Role", role);

        registerCommand.ExecuteNonQuery();
        return true;
    }
}