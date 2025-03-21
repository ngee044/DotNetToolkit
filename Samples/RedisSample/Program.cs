using System;
using Redis.RedisClient;

namespace RedisSample
{
	public static class program
	{
		public static void Main(string[] args)
		{
			var client = new RedisClient(
				connection_string: "localhost:6379",
				use_aws_ecs: false,
				tls_options: null
			);

			bool connected = client.Connect();
			if (!connected)
			{
				Console.WriteLine("Failed to connect to Redis");
				return;
			}

			client.SetString("test", "Hello, Redis!");

			var value = client.GetString("test");
			Console.WriteLine($"Value: {value}");

			client.Dispose();
			Console.WriteLine("Done");
		}
	}
}