using ChatModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatModels;

public class ChatClient
{
    public string ServerAddress { get; set; }
    public int ServerPort { get; set; }

    public string UserName { get; set; }
    public string Password { get; set; }

    public string UploadDir { get; set; } = "UploadedFiles";

    public ChatClient(string serverAddress, int serverPort, string uploadDir = "UploadedFiles")
    {
        ServerAddress = serverAddress;
        ServerPort = serverPort;
        UploadDir = uploadDir;

        if (!Directory.Exists(UploadDir))
        {
            Directory.CreateDirectory(UploadDir);
        }
    }


    private Socket Connect()
    {
        // Приєднання до сервера
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort));

        // Відправка імені
        SocketHelper.SendString(socket, UserName);
        // Отримання відповіді ОК
        string response = SocketHelper.ReceiveString(socket);

        // Відправити пароль
        SocketHelper.SendString(socket, Password);
        // Отримання відповіді ОК / FAIL
        response = SocketHelper.ReceiveString(socket);
        if (response != "OK")
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            throw new UnauthorizedAccessException("Невірний пароль");
        }
        return socket;
    }


    // 1. Отримання нових повідомлень
    public List<ChatMessage> GetMessages()
    {
        var socket = Connect();

        // Відправка запиту на отримання повідомлень "GET_MESSAGES"
        SocketHelper.SendString(socket, "GET_MESSAGES");
        // отримання повідомлень
        string messagesJson = SocketHelper.ReceiveString(socket);
        // десеріалізація
        var messages = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(messagesJson) ?? [];

        foreach (var message in messages)
        {
            if (!string.IsNullOrEmpty(message.Filename))
            {
                DownloadFile(message.From.Username, message.Filename);
            }
        }
        // Завершення з'єднання
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        return messages ?? [];
    }

    // 2. Відправка повідомлення
    public void SendMessage(ChatMessage chatMessage)
    {
        var socket = Connect();
        
        // Відправка запиту на відправку повідомлень "SEND_MESSAGE"
        SocketHelper.SendString(socket, "SEND_MESSAGE");
        // Відправка повідомлення
        string messageJson = System.Text.Json.JsonSerializer.Serialize(chatMessage);
        SocketHelper.SendString(socket, messageJson);
        // Отримання відповіді ОК / FAIL
        var response = SocketHelper.ReceiveString(socket);
        // Завершення з'єднання
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    public void SendFile(string filename)
    {
        var socket = Connect();

        SocketHelper.SendString(socket, "UPLOAD_FILE");

        var response = SocketHelper.ReceiveString(socket); // "FILENAME"

        SocketHelper.SendString(socket, Path.GetFileName(filename));
        SocketHelper.SendFile(socket, filename);

        // Завершення з'єднання
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

    void DownloadFile(string fromUser, string filename)
    {
        var socket = Connect();

        SocketHelper.SendString(socket, "DOWNLOAD_FILE");

        var response = SocketHelper.ReceiveString(socket);
        SocketHelper.SendString(socket, fromUser);
        response = SocketHelper.ReceiveString(socket);
        SocketHelper.SendString(socket, filename);
        response = SocketHelper.ReceiveString(socket);
        if (response != "EXISTS")
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return;
        }

        SocketHelper.SendString(socket, "OK");

        var downloadFilePath = Path.Combine(UploadDir, fromUser, filename);
        if (!Directory.Exists(Path.Combine(UploadDir, fromUser)))
        {
            Directory.CreateDirectory(Path.Combine(UploadDir, fromUser));
        }

        SocketHelper.ReceiveFile(socket, downloadFilePath);

        // Завершення з'єднання
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }

}
