using ChatModels.Models;
using System.Net;
using System.Net.Sockets;

Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Server");


var ipAdress = IPAddress.Parse("192.168.1.141");
int port = 5000;


Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

var messageBoxes = new List<ChatMessageBox>();


// Чат Клієнт - Сервер
// 
// З'єднання постійне
// Сеансові з'єднання

// Сервер - поштові скриньки для повідомлень
// Клієнт - має ім'я та відповідну скриньку

try
{
    socket.Bind(new IPEndPoint(ipAdress, port));
    socket.Listen(5);
    Console.WriteLine("Очікуємо з'єднання...");

    while (true)
    {
        Socket clientSocket = socket.Accept();
        Task.Run(() => HandleClient(clientSocket));
    }

}
catch (Exception ex)
{
    Console.Write(ex.Message);
}


void HandleClient(Socket clientSocket)
{
    Console.WriteLine($"Клієнт підключився: {clientSocket.LocalEndPoint}");

    // Протокол взаємодії

    // Клієнт передає своє ім'я 
    string userName = ReceiveString(clientSocket);
    // Сервер відповідає привітанням
    SendString(clientSocket, "OK");

    Console.WriteLine($"User: {userName}");

    // Отримати пароль від користувача
    string password = ReceiveString(clientSocket);

    // Перевірка паролю
    bool validPassword = false;
    lock (messageBoxes)
    {
        var messageBox = messageBoxes.FirstOrDefault(mb => mb.User.Username == userName);
        // Правильний пароль - якщо скринька не існує (новий користувач) або існує та пароль співпадає (старий користувач)
        validPassword = messageBox == null || (messageBox != null && messageBox.Password == password);

        // Користувача ще немає - створити нову скриньку
        if (messageBox == null)
        {
            Console.WriteLine("New user added");
            messageBoxes.Add(new ChatMessageBox
            {
                User = new ChatUser { Username = userName },
                Password = password
            });
        }
    }

    if (!validPassword)
    {
        Console.WriteLine("Invalid password");
        // Пароль невірний
        SendString(clientSocket, "FAIL");
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        return;
    }

    SendString(clientSocket, "OK");


    // Клієнт передає запит "GET_MESSAGES" / "SEND_MESSAGE"
    string command = ReceiveString(clientSocket);

    Console.WriteLine($"Command: {command}");

    switch (command)
    {
        // 1. отримати нові повідомлення "GET_MESSAGES"
        case "GET_MESSAGES":
            // Знайти скриньку по імені
            string messagesJson = string.Empty;
            lock (messageBoxes)
            {
                var messageBox = messageBoxes.FirstOrDefault(mb => mb.User.Username == userName);
                if (messageBox == null)
                {
                    messageBox = new ChatMessageBox { User = new ChatUser { Username = userName } };
                    messageBoxes.Add(messageBox);
                }
                // Відправити всі повідомлення зі скриньки - серіалізувати в json рядок
                messagesJson = System.Text.Json.JsonSerializer.Serialize(messageBox.Messages);
                // Очистити скриньку
                messageBox.Messages.Clear();
            }
            // Відправити всі повідомлення
            Console.WriteLine(messagesJson);
            SendString(clientSocket, messagesJson);
            break;

        // 2. надіслати повідомлення "SEND_MESSAGE"
        case "SEND_MESSAGE":

            SendString(clientSocket, "OK");
            // Отримати повідомлення
            var messageJson = ReceiveString(clientSocket);
            Console.WriteLine("New message");
            Console.WriteLine(messageJson);
            // десеріалізувати з json рядка
            var message = System.Text.Json.JsonSerializer.Deserialize<ChatMessage>(messageJson);
            if (message != null)
            {
                lock (messageBoxes)
                {
                    // Кладемо в усі скриньки, крім відправника
                    foreach (var box in messageBoxes)
                    {
                        if (box.User.Username != userName)
                        {
                            box.Messages.Add(message);
                        }
                    }
                }
            }
            // Сервер відповідає OK
            Console.WriteLine("Message added to boxes");
            SendString(clientSocket, "OK");
            break;

        default:
            break;

    }
    // Завершити сеанс
    clientSocket.Shutdown(SocketShutdown.Both);
    clientSocket.Close();
}


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


// 1 - 65535

/*
 21 - FTP
 22 - SSH
 23 - Telnet
 25 - SMTP
 80 - HTTP
 110 - POP3
 143 - IMAP
 443 - HTTPS
 3306 - MySQL
 5432 - PostgreSQL
 6379 - Redis
 27017 - MongoDB
 5000 - Наш сервер
  
 */



// 


