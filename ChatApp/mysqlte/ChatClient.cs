using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class ChatClient
{
    static void Main(string[] args)
    {
        try
        {
            TcpClient client = new TcpClient("127.0.0.1", 4321);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Connected to server!");

            // 사용자 ID 입력
            Console.Write("Enter your ID: ");
            string clientId = Console.ReadLine();

            // 서버로 메시지를 보내는 쓰레드 실행
            Task.Run(() => SendMessage(stream, clientId));

            // 서버에서 메시지를 받는 쓰레드 실행
            ReceiveMessage(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    private static void SendMessage(NetworkStream stream, string clientId)
    {
        while (true)
        {
            string message = Console.ReadLine();
            byte[] buffer = Encoding.UTF8.GetBytes(clientId + ": " + message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    private static void ReceiveMessage(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int bytes;

        while ((bytes = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytes);
            Console.WriteLine("Received: " + message);
        }
    }
}
