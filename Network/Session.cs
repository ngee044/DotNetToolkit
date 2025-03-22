using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Network
{
    public class Session(int session_id, Socket socket, Action<int> remove_callback)
    {
        private readonly int session_id_ = session_id;
		private readonly Socket socket_ = socket;
		private readonly Action<int> remove_callback_ = remove_callback;
		private bool connected_ = true;

		private byte[]? recv_buffer_ = PooledBufferManager.RentBuffer(buffer_size_);
		private const int buffer_size_ = 1024;

		public void Start()
		{
			_ = ReceiveLoop();
		}

		private async Task ReceiveLoop()
		{
			if (recv_buffer_ == null)
			{
				Console.WriteLine("Error: Receive buffer is null");
				Disconnect();
				return;
			}

			try
			{
				while (connected_)
				{
					var recv_size = await socket_.ReceiveAsync(new ArraySegment<byte>(recv_buffer_), SocketFlags.None);
					if (recv_size == 0)
					{
						Disconnect();
						break;
					}

					HandlePacket(recv_buffer_, recv_size);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				Disconnect();
			}
		}

		private void HandlePacket(byte[] buffer, int size)
		{
			string json = Encoding.UTF8.GetString(buffer, 0, size);
			Console.WriteLine($"Received: {json}");

			PacketHandler.HandleJsonMessage(json, this);
		}

		public async Task SendAsync(string json)
		{
			if (!connected_)
			{
				return;
			}

			try
			{
				var data = Encoding.UTF8.GetBytes(json);
				await socket_.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				Disconnect();
			}
		}

		public void Disconnect()
		{
			if (connected_)
			{
				connected_ = false;
				socket_?.Close();

				if (recv_buffer_ != null)
				{
					PooledBufferManager.ReturnBuffer(recv_buffer_);
					recv_buffer_ = null;
				}

				remove_callback_?.Invoke(session_id_);
			}
		}
    }
}