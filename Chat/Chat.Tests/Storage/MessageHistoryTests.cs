using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Chat.Storage;
using NUnit.Framework;

namespace Chat.Tests.Storage;

[TestFixture]
public class MessageHistoryTests
{
    public const string TestConnectionString = @"Data Source=D:\4\RISERMOLIN\Chat\Chat.Tests\DB\MSG.db";

    [SetUp]
    public void SetUp()
    {
        // Создаем тестовую базу данных в памяти
        using var connection = new SQLiteConnection(TestConnectionString);
        connection.Open();

        using (var dropCommand = new SQLiteCommand("DROP table if exists MessageHistory", connection))
        {
            dropCommand.ExecuteNonQuery();
        }

        // Создаем таблицу для тестов
        using var command = new SQLiteCommand(MessageHistory.CreateTableQuery, connection);
        command.ExecuteNonQuery();

        // Устанавливаем тестовое подключение
        MessageHistory.ConnectionString = TestConnectionString;
    }

    [Test]
    public void LogMessage_ShouldAddMessageToDatabase()
    {
        // Действие
        MessageHistory.LogMessage("testUser", "Hello, world!");

        // Проверка
        using var connection = new SQLiteConnection(TestConnectionString);
        connection.Open();

        using var command = new SQLiteCommand("SELECT COUNT(*) FROM MessageHistory WHERE Username = 'testUser';", connection);
        var count = (long)command.ExecuteScalar();
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void GetMessages_ShouldRetrieveLoggedMessages()
    {
        // Подготовка
        MessageHistory.LogMessage("testUser", "First message");
        MessageHistory.LogMessage("testUser", "Second message");

        // Действие
        var messages = MessageHistory.GetMessages("testUser");

        // Проверка
        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages[0], Is.EqualTo("First message"));
        Assert.That(messages[1], Is.EqualTo("Second message"));
    }

    [Test]
    public void GetMessages_ShouldReturnEmptyListForUnknownUser()
    {
        // Действие
        var messages = MessageHistory.GetMessages("unknownUser");

        // Проверка
        Assert.That(messages, Is.Empty);
    }
}
