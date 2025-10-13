using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatModels;

public static class SocketHelper
{

    public static void ReceiveFile(Socket socket, string filename)
    {
        // Спочатку отримуємо довжину файлу (8 байт для long)
        byte[] lengthBuffer = new byte[sizeof(long)];
        socket.Receive(lengthBuffer);
        // Конвертуємо байти в long - довжина файлу
        long fileLength = BitConverter.ToInt64(lengthBuffer, 0);

        // Тепер отримуємо сам файл
        using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[8192];
            long totalBytesRead = 0;
            while (totalBytesRead < fileLength)
            {
                int bytesRead = socket.Receive(buffer, 0, (int)Math.Min(buffer.Length, fileLength - totalBytesRead), SocketFlags.None);
                if (bytesRead == 0) break; // З'єднання закрито
                fs.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
            }
        }
    }

    public static void SendFile(Socket socket, string filename)
    {
        FileInfo fileInfo = new FileInfo(filename);
        long fileLength = fileInfo.Length;
        // Спочатку відправляємо довжину файлу (8 байт для long)
        byte[] lengthBuffer = BitConverter.GetBytes(fileLength);
        socket.Send(lengthBuffer);
        // Тепер відправляємо сам файл
        using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                socket.Send(buffer, 0, bytesRead, SocketFlags.None);
            }
        }
    }

    public static void SendString(Socket socket, string message)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        var dataLength = BitConverter.GetBytes(data.Length);    
        socket.Send(dataLength);
        socket.Send(data);
    }

    public static string ReceiveString(Socket socket)
    {
        byte[] lengthBuffer = new byte[sizeof(int)];
        socket.Receive(lengthBuffer);
        int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

        byte[] buffer = new byte[dataLength];

        int totalBytesRead = 0;
        while (totalBytesRead < dataLength)
        {
            int bytesRead = socket.Receive(buffer, totalBytesRead, dataLength - totalBytesRead, SocketFlags.None);
            totalBytesRead += bytesRead;
        }

        string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, dataLength);
        return receivedMessage;
    }
}
