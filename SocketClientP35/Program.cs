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
    SendString(socket, userName);
    // Отримання відповіді ОК
    string response = ReceiveString(socket);
    // Відправка запиту на отримання повідомлень "GET_MESSAGES"
    SendString(socket, "GET_MESSAGES");
    // отримання повідомлень
    string messagesJson = ReceiveString(socket);
    // десеріалізація
    var messages = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessage>>(messagesJson);
    // вивід на екран
    foreach (var message in messages)
    {
        Console.WriteLine($"{message.Timestamp.ToString("dd.MM.yyyy H:m:s")} {message.From.Username}: {message.Text}");
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
    SendString(socket, userName);
    // Отримання відповіді ОК
    string response = ReceiveString(socket);
    // Відправка запиту на відправку повідомлень "SEND_MESSAGE"
    SendString(socket, "SEND_MESSAGE");
    // Відправка повідомлення
    string messageJson = System.Text.Json.JsonSerializer.Serialize(chatMessage);
    // Отримання відповіді ОК
    SendString(socket, messageJson);
    // Завершення з'єднання
    socket.Shutdown(SocketShutdown.Both);
    socket.Close();
}




//await workerAsync();


//async Task workerAsync()
//{
//    while (true)
//    {
//        try
//        {
//            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            socket.Connect(new IPEndPoint(ipAdress, port));

//            Console.WriteLine($"Підключено до сервера: {socket.RemoteEndPoint}");

//            // Передача даних серверу
//            string message = "Привіт від клієнта!";
//            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
//            socket.Send(data);

//            // Отримання даних від сервера
//            byte[] buffer = new byte[256];
//            int bytesRead = socket.Receive(buffer);
//            string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
//            Console.WriteLine($"Отримано повідомлення від сервера: {receivedMessage}");

//            socket.Shutdown(SocketShutdown.Both);
//            socket.Close();
//            Console.WriteLine("Відключено від сервера.");
//            await Task.Delay(1000);

//        }
//        catch (Exception ex)
//        {
//            Console.Write(ex.Message);
//        }
//    }
   
//}


//Console.ReadLine();


void SendString(Socket socket, string message)
{
    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
    socket.Send(data);
}

string ReceiveString(Socket socket)
{
    byte[] buffer = new byte[2048];
    int bytesRead = socket.Receive(buffer);
    string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
    return receivedMessage;
}