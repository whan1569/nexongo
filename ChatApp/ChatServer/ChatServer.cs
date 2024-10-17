using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class ChatServer
{
    private static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 4321);
        listener.Start();
        Console.WriteLine("Server started on port 4321...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("Client connected...");

            // 새로운 클라이언트에 대해 수신 대기
            Task.Run(() => HandleClient(client));
        }
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytes;

        while ((bytes = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytes);
            Console.WriteLine("Received: " + message);

            // 연결된 모든 클라이언트에게 메시지 전송
            Broadcast(message, client);
        }

        clients.Remove(client);
        client.Close();
        Console.WriteLine("Client disconnected...");
    }

    private static void Broadcast(string message, TcpClient excludeClient)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);

        foreach (TcpClient client in clients)
        {
            if (client != excludeClient)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
