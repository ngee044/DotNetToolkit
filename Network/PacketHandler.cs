using System.Text.Json;

namespace Network
{
    public static class PacketHandler
    {
        public static void HandleJsonMessage(string json, Session session)
		{
			try
			{
				var packet = JsonSerializer.Deserialize<GenericPacket>(json) ?? new GenericPacket();

				switch (packet.Command)
				{
					case "MOVE":
						HandleMove(packet, session);
						break;
					case "ATTACK":
						HandleAttack(packet, session);
						break;
					default:
						Console.WriteLine($"Unknown command: {packet.Command}");
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}

		private static void HandleMove(GenericPacket packet, Session session)
		{
			Console.WriteLine($"Player {packet.PlayerId} moved to ({packet.X}, {packet.Y})");

			var response = new { Command = "MOVE_ACK", result = "OK"};
			var json = JsonSerializer.Serialize(response);
			_ = session.SendAsync(json);
		}

		private static void HandleAttack(GenericPacket packet, Session session)
		{
			Console.WriteLine($"Player {packet.PlayerId} attacked");

			var response = new { Command = "ATTACK_ACK", result = "OK" };
            var json = JsonSerializer.Serialize(response);
            _ = session.SendAsync(json);
		}

		public class GenericPacket
		{
			public string? Command { get; set; }
			public int PlayerId { get; set; }
			public float X { get; set; }
			public float Y { get; set; }
		}
    }
}