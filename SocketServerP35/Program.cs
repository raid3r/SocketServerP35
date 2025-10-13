using ChatModels.Models;
using ChatModels;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Server");


var ipAdress = IPAddress.Parse("192.168.1.141");
int port = 5000;


Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

var messageBoxes = LoadData();

var uploadDir = "UploadedFiles";
if (!Directory.Exists(uploadDir))
{
    Directory.CreateDirectory(uploadDir);
}


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
    string userName = SocketHelper.ReceiveString(clientSocket);
    // Сервер відповідає привітанням
    SocketHelper.SendString(clientSocket, "OK");

    Console.WriteLine($"User: {userName}");

    // Отримати пароль від користувача
    string password = SocketHelper.ReceiveString(clientSocket);

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
            SaveData(messageBoxes);
        }
    }

    if (!validPassword)
    {
        Console.WriteLine("Invalid password");
        // Пароль невірний
        SocketHelper.SendString(clientSocket, "FAIL");
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        return;
    }

    SocketHelper.SendString(clientSocket, "OK");


    // Клієнт передає запит "GET_MESSAGES" / "SEND_MESSAGE"
    string command = SocketHelper.ReceiveString(clientSocket);

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
                if (messageBox.Messages.Count > 0)
                {
                    Console.WriteLine($"Sent {messageBox.Messages.Count} messages to {userName}");
                    messageBox.Messages.Clear();
                    SaveData(messageBoxes);
                }
            }
            // Відправити всі повідомлення
            Console.WriteLine(messagesJson);
            SocketHelper.SendString(clientSocket, messagesJson);
            break;

        // 2. надіслати повідомлення "SEND_MESSAGE"
        case "SEND_MESSAGE":

            SocketHelper.SendString(clientSocket, "OK");
            // Отримати повідомлення
            var messageJson = SocketHelper.ReceiveString(clientSocket);
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
                    SaveData(messageBoxes);
                }
            }
            // Сервер відповідає OK
            Console.WriteLine("Message added to boxes");
            //SocketHelper.SendString(clientSocket, "OK");
            break;

        // 3. Завантажити файл з клієнта на сервер
        case "UPLOAD_FILE":
            SocketHelper.SendString(clientSocket, "FILENAME");
            // Отримати файл
            var fileName = SocketHelper.ReceiveString(clientSocket);
            Console.WriteLine($"File to upload: {fileName}");
            var userDir = Path.Combine(uploadDir, userName);
            if (!Directory.Exists(userDir))
            {
                Directory.CreateDirectory(userDir);
            }
            //SocketHelper.ReceiveFile(clientSocket, Path.Combine(userDir, fileName));

            break;

        // 4. Завантажити файл з сервера на клієнт
        case "DOWNLOAD_FILE":
            break;


        default:
            break;

    }
    // Завершити сеанс
    clientSocket.Shutdown(SocketShutdown.Both);
    clientSocket.Close();
}

void SaveData(List<ChatMessageBox> messageBoxesToSave)
{
    var fileName = "messageboxes.json";
    var json = JsonSerializer.Serialize(messageBoxesToSave);
    File.WriteAllText(fileName, json);
}

List<ChatMessageBox> LoadData()
{
    var fileName = "messageboxes.json";
    List<ChatMessageBox> loadedMessageBoxes = [];
    if (File.Exists(fileName))
    {
        var json = File.ReadAllText(fileName);
        loadedMessageBoxes = System.Text.Json.JsonSerializer.Deserialize<List<ChatMessageBox>>(json) ?? [];
    }
    return loadedMessageBoxes;
}


/*
 * Зробити щоб дані на сервері про юзерів та їхні скриньки зберігалися в файл
 * шоб не втрачалися при перезапуску сервера
 * 
 * 
 * 
 * 
 */ 