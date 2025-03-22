using Grpc.Core;
using gRPCService.Proto;
using System.Text.Json;
using System.Threading.Tasks;

namespace Network.gRPC
{
    public class gRPCGameServer : GameService.GameServiceBase
    {
        public override Task<JsonResponse> SendJsonPayload(JsonRequest request, ServerCallContext context)
		{
			var data = JsonSerializer.Deserialize<MyRequestData>(request.JsonData) ?? new MyRequestData();
			var result = GameLogicHandler.Process(data);

			var jsonResponse = new JsonResponse
			{
				JsonData = JsonSerializer.Serialize(result)
			};

			return Task.FromResult(jsonResponse);
		}

		public override Task<JsonResponse> ServerToServerCall(JsonRequest request, ServerCallContext context)
		{
			var data = JsonSerializer.Deserialize<ServerRequestData>(request.JsonData) ?? new ServerRequestData();
			var result = ServerLogicHandler.Process(data);

			var jsonResponse = new JsonResponse
			{
				JsonData = JsonSerializer.Serialize(result)
			};

			return Task.FromResult(jsonResponse);
		}

		public class MyRequestData
		{
			public string? Command { get; set; }
			public string? PlayerId { get; set; }
		}

		public class ServerRequestData
		{
			public string? Action { get; set; }
			public string? Payload { get; set; }
		}

		public static class ServerLogicHandler
		{
			public static object Process(ServerRequestData data)
			{
				return new { success = true, info = "ServerLogic processed." };
			}
		}

		public static class GameLogicHandler
		{
			public static object Process(MyRequestData data)
			{
				return new { success = true, info = "GameLogic processed." };
			}
		}

    }
}