using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Client.Models;
using Client.Services;

namespace Client.Views;

public partial class MainWindow
{
    private readonly ChatService _chatService;
    private readonly MessageHandler _messageHandler;



    public MainWindow()
    {
        InitializeComponent();
        _chatService = new ChatService();
        _messageHandler = new MessageHandler(AppendToChat, ShowMessageBox, AppendToHistoryBox, ShowChatPanel);
        ShowConnectionPanel();
    }

    #region Input Events

    private void MessageInput_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) SendMessageButton_Click(sender, e);
    }

    #endregion

    #region UI Panels Management

    private void AppendToHistoryBox(string[] history)
    {
        Dispatcher.Invoke(() =>
        {
            HistoryBox.Text = string.Join('\n', history);
            HistoryPanel.Visibility = Visibility.Visible;
        });
    }

    private void ShowConnectionPanel(object sender, RoutedEventArgs routedEventArgs)
    {
        ShowConnectionPanel();
    }

    private void ShowConnectionPanel()
    {
        Dispatcher.Invoke(() =>
        {
            ConnectionPanel.Visibility = Visibility.Visible;
            AuthPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Collapsed;
        });
    }

    private void ShowAuthPanel(object sender, RoutedEventArgs routedEventArgs)
    {
        ShowAuthPanel();
    }

    private void ShowAuthPanel()
    {
        Dispatcher.Invoke(() =>
        {
            ConnectionPanel.Visibility = Visibility.Collapsed;
            AuthPanel.Visibility = Visibility.Visible;
            ChatPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Collapsed;
        });
    }

    private void ShowChatPanel(object sender, RoutedEventArgs routedEventArgs)
    {
        ShowChatPanel();
    }

    private void ShowChatPanel()
    {
        Dispatcher.Invoke(() =>
        {
            ConnectionPanel.Visibility = Visibility.Collapsed;
            AuthPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Visible;
            HistoryPanel.Visibility = Visibility.Collapsed;
        });
    }

    private void ShowHistoryPanel(object sender, RoutedEventArgs routedEventArgs)
    {
        ShowHistoryPanel();
    }

    private void ShowHistoryPanel()
    {
        Dispatcher.Invoke(() =>
        {
            ConnectionPanel.Visibility = Visibility.Collapsed;
            AuthPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Visible;
        });
    }

    #endregion

    #region Connection and Authentication

    private void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!_chatService.IsConnected)
            {
                var serverIp = ServerIP.Text.Trim();
                var serverPort = int.Parse(ServerPort.Text.Trim());

                _chatService.Connect(serverIp, serverPort);
                AppendToChat("Connected to server.");
                Task.Run(ReceiveMessages);
            }

            ShowAuthPanel();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var username = AuthUsername.Text.Trim();
        var password = AuthPassword.Password.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return;

        if (_chatService.SendMessage(new AuthRequest { Username = username, Password = password })) return;
        ShowConnectionPanel();
        MessageBox.Show("Error, while sending request. Please reconnect.", "Error", MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var username = AuthUsername.Text.Trim();
        var password = AuthPassword.Password.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return;

        if (_chatService.SendMessage(new RegisterRequest { Username = username, Password = password })) return;
        ShowConnectionPanel();
        MessageBox.Show("Error, while sending request. Please reconnect.", "Error", MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    #endregion

    #region Chat Functionality

    private void SendMessageButton_Click(object sender, RoutedEventArgs e)
    {
        var messageContent = MessageInput.Text.Trim();

        if (string.IsNullOrWhiteSpace(messageContent)) return;

        var receiver = DirectNickname.Text.Trim();
        var isDirect = string.IsNullOrWhiteSpace(receiver);

        var chatMessage = new MessageModel
        {
            Type = "message",
            Content = messageContent,
            IsDirect = !isDirect,
            Receiver = receiver
        };

        if (_chatService.SendMessage(chatMessage))
        {
            MessageInput.Clear();
        }
        else
        {
            ShowConnectionPanel();
            MessageBox.Show("Error, while sending request. Please reconnect.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ViewHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        var username = HistoryUsername.Text.Trim();

        if (string.IsNullOrWhiteSpace(username)) return;

        if (_chatService.SendMessage(new HistoryRequest { Username = username })) return;
        ShowConnectionPanel();
        MessageBox.Show("Error, while sending request. Please reconnect.", "Error", MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
    
    #endregion

    #region Message Processing

    private void ReceiveMessages()
    {
        try
        {
            while (true)
            {
                var message = _chatService.ReceiveMessage();
                if (string.IsNullOrEmpty(message)) continue;
                _messageHandler.ProcessMessage(message);
            }
        }
        catch (Exception ex)
        {
            AppendToChat($"Error receiving message: {ex.Message}");
        }
    }

    private void AppendToChat(string message)
    {
        Dispatcher.Invoke(() =>
        {
            var paragraph = new Paragraph();

            if (!string.IsNullOrWhiteSpace(message)) paragraph.Inlines.Add(new Run(message));

            ChatBox.Document.Blocks.Add(paragraph);
            ChatBox.ScrollToEnd();
        });
    }

    private void ShowMessageBox(string message)
    {
        Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    #endregion
}