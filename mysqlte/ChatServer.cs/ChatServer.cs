using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;  // MySQL 라이브러리 추가

class ChatServer
{
    private static List<TcpClient> clients = new List<TcpClient>();
    private static string connectionString = "Server=localhost;Database=chat_db;User ID=root;Password=1234;";

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

            // 메시지에서 클라이언트 아이디와 메시지를 분리 (형식: "client_id: message")
            var splitIndex = message.IndexOf(": ");
            if (splitIndex > -1)
            {
                string clientId = message.Substring(0, splitIndex);
                string clientMessage = message.Substring(splitIndex + 2);

                // MySQL에 메시지 저장
                SaveMessageToDatabase(clientId, clientMessage);
            }

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

    private static void SaveMessageToDatabase(string clientId, string message)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO messages (client_id, message) VALUES (@client_id, @message)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@client_id", clientId);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Message saved to database.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving message to database: " + ex.Message);
            }
        }
    }
}
