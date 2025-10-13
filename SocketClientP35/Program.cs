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
