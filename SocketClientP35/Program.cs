using ChatModels;
using ChatModels.Models;
using System.Net;
using System.Net.Sockets;

Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Client");

var ipAdress = IPAddress.Parse("192.168.1.141");
int port = 5000;

// Запитаємо у користувача його ім'я
Console.Write("Введіть ваше ім'я: ");
string userName = Console.ReadLine();

// Запитаємо у користувача його пароль
Console.Write("Введіть ваш пароль: ");
string password = Console.ReadLine();


var uploadDir = "UploadedFiles";
if (!Directory.Exists(uploadDir))
{
    Directory.CreateDirectory(uploadDir);
}

Task.Run(() =>
{
    while (true)
    {
        // Отримання нових повідомлень
        GetMessages();
        Thread.Sleep(2000);
    }
});

while (true)
{
    var text = Console.ReadLine();

    if (text == "/screenshot")
    {
        var filename = @"C:\Users\kvvkv\source\repos\SocketServerP35\SocketServerP35\Program.cs";
        SendFile(filename);

        SendMessage(new ChatMessage
        {
            From = new ChatUser { Username = userName },
            Timestamp = DateTime.Now,
            Text = $"Відправив файл: {Path.GetFileName(filename)}",
            Filename = Path.GetFileName(filename)
        });

        continue;
    }

    if (!string.IsNullOrEmpty(text))
    {
        SendMessage(new ChatMessage
        {
            From = new ChatUser { Username = userName },
            Timestamp = DateTime.Now,
            Text = text
        });
    }
}


// 1. Отримання нових повідомлень
void GetMessages()
{
    // Приєднання до сервера
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect(new IPEndPoint(ipAdress, port));

    // Відправка імені
    SocketHelper.SendString(socket, userName);
    // Отримання відповіді ОК
    string response = SocketHelper.ReceiveString(socket);

    // Відправити пароль
     SocketHelper.SendString(socket, password);
    // Отримання відповіді ОК / FAIL
    response = SocketHelper.ReceiveString(socket);
    if (response != "OK")
    {
        Console.WriteLine("Невірний пароль");
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        return;
    }

    // Відправка запиту на отримання повідомлень "GET_MESSAGES"
    SocketHelper.SendString(socket, "GET_MESSAGES");
    // отримання повідомлень
    string messagesJson = SocketHelper.ReceiveString(socket);
    // десеріалізація
    var messages = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(messagesJson);
    // вивід на екран
    foreach (var message in messages)
    {
        
        Console.WriteLine($"{message.Timestamp.ToString("dd.MM.yyyy HH:mm:ss")} {message.From.Username}: {message.Text}");
        if (!string.IsNullOrEmpty(message.Filename))
        {
            DownloadFile(message.From.Username, message.Filename);
            Console.WriteLine($"Файл {message.Filename} завантажено у папку {Path.Combine(uploadDir, message.From.Username)}");
        }
    }
    socket.Shutdown(SocketShutdown.Both);
    // Завершення з'єднання
    socket.Close();
}

// 2. Відправка повідомлення
void SendMessage(ChatMessage chatMessage)
{
    // Приєднання до сервера
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect(new IPEndPoint(ipAdress, port));
    // Відправка імені
    SocketHelper.SendString(socket, userName);
    // Отримання відповіді ОК
    string response = SocketHelper.ReceiveString(socket);
    
    // Відправити пароль
    SocketHelper.SendString(socket, password);
    // Отримання відповіді ОК / FAIL
    response = SocketHelper.ReceiveString(socket);
    if (response != "OK")
    {
        Console.WriteLine("Невірний пароль");
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        return;
    }
    // Відправка запиту на відправку повідомлень "SEND_MESSAGE"
    SocketHelper.SendString(socket, "SEND_MESSAGE");
    // Відправка повідомлення
    string messageJson = System.Text.Json.JsonSerializer.Serialize(chatMessage);
    SocketHelper.SendString(socket, messageJson);
    // Отримання відповіді ОК / FAIL
    response = SocketHelper.ReceiveString(socket);
    // Завершення з'єднання
    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
}



void SendFile(string filename)
{
    // Приєднання до сервера
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect(new IPEndPoint(ipAdress, port));
    // Відправка імені
    SocketHelper.SendString(socket, userName);
    // Отримання відповіді ОК
    string response = SocketHelper.ReceiveString(socket);

    // Відправити пароль
    SocketHelper.SendString(socket, password);
    // Отримання відповіді ОК / FAIL
    response = SocketHelper.ReceiveString(socket);
    if (response != "OK")
    {
        Console.WriteLine("Невірний пароль");
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        return;
    }
    
    SocketHelper.SendString(socket, "UPLOAD_FILE");

    response = SocketHelper.ReceiveString(socket); // "FILENAME"

    SocketHelper.SendString(socket, Path.GetFileName(filename));
    SocketHelper.SendFile(socket, filename);

    // Завершення з'єднання
    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
}


void DownloadFile(string fromUser, string filename)
{
    // Приєднання до сервера
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    socket.Connect(new IPEndPoint(ipAdress, port));
    // Відправка імені
    SocketHelper.SendString(socket, userName);
    // Отримання відповіді ОК
    string response = SocketHelper.ReceiveString(socket);
    // Відправити пароль
    SocketHelper.SendString(socket, password);
    // Отримання відповіді ОК / FAIL
    response = SocketHelper.ReceiveString(socket);
    if (response != "OK")
    {
        Console.WriteLine("Невірний пароль");
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        return;
    }
    SocketHelper.SendString(socket, "DOWNLOAD_FILE");
    
    response = SocketHelper.ReceiveString(socket);
    Console.WriteLine(response);
    //SocketHelper.SendString(clientSocket, "FROM");
    SocketHelper.SendString(socket, fromUser);
    
    response = SocketHelper.ReceiveString(socket);
    Console.WriteLine(response);
    //SocketHelper.SendString(clientSocket, "FILENAME");
    SocketHelper.SendString(socket, filename);
    response = SocketHelper.ReceiveString(socket);
    Console.WriteLine(response);
    if (response != "EXISTS")
    {
        Console.WriteLine("File not exists on server");
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
        return;
    }

    SocketHelper.SendString(socket, "OK");

    var downloadFilePath = Path.Combine(uploadDir, fromUser, filename);
    if (!Directory.Exists(Path.Combine(uploadDir, fromUser)))
    {
        Directory.CreateDirectory(Path.Combine(uploadDir, fromUser));
    }

    SocketHelper.ReceiveFile(socket, downloadFilePath);

    // Завершення з'єднання
    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
}