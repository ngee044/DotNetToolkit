using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace Network
{
    public class TcpServer(int port)
    {
        private Socket? listen_socket_;
		private int port_ = port;
		private bool running_;

		public async Task StartAsync()
		{
			running_ = true;
			listen_socket_ = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listen_socket_.Bind(new IPEndPoint(IPAddress.Any, port_));
			listen_socket_.Listen(10);

			Console.WriteLine($"Server started on port {port_}");

			while (running_)
			{
				try
				{
					var client_socket = await listen_socket_.AcceptAsync();
					Console.WriteLine($"Client connected: {client_socket.RemoteEndPoint}");

					SessionManager.Instance.CreateSession(client_socket);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error: {ex.Message}");
				}
			}
		}

		public void Stop()
		{
			running_ = false;
			listen_socket_?.Close();
		}
    }
}