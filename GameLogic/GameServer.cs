using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Network;
using Network.gRPC;
using Matchmaking;
using GameLogic;
using Proto;
using Redis;


namespace GameLogic
{
    public class GameServer(int tcp_port, int grpc_port, RedisClient redis_client)
    {
        private TcpServer tcp_server_;
		private Server grpc_server_;
		private TickManager tick_manager_;
		private MatchmakingService matchmaking_service_;
		private CancellationTokenSource cts_;

		private int tcp_port_ = tcp_port;
		private int grpc_port_ = grpc_port;
		private RedisClient redis_client_ = redis_client;

		public async Task StartAsync()
		{
			Console.WriteLine("GameServer starting...");

			cts_ = new CancellationTokenSource();

			tcp_server_ = new TcpServer(tcp_port_);
			var tcp_task = tcp_server_.StartAsync();

			grpc_server_ = new Server
			{
				Services = { GameService.BindService(new gRPCGameServer()) },
				Ports = { new ServerPort("localhost", grpc_port_, ServerCredentials.Insecure) }
			};

			grpc_server_.Start();
			Console.WriteLine($"gRPC server listening on port {grpc_port_}");

			tick_manager_ = new TickManager();
			tick_manager_.Start();
			Console.WriteLine("TickManager started");

			matchmaking_service_ = new MatchmakingService(redis_client_);

			_ = tcp_task.Run(async () =>
			{
				while (!cts_.Token.IsCancellationRequested)
				{
					// TODO
					// Do something useful here
					// For example, check if the server is still running
					// and print out some useful information

					Console.WriteLine("GameServer is running...");

					matchmaking_service_.ProcessMatching();
					await Task.Delay(1000);
				}

			}, cts_.Token);

			// 여기서는 _tcpServer.StartAsync()도 비동기로 실행 중이므로,
            // 전체 서버가 동작 중인 상태가 됩니다.
            // 필요하다면 tcpTask를 await 하거나, 서버 종료 처리까지 대기할 수 있음.

            // 예시로, 바로 리턴하여 "비동기"로 서버가 구동되도록 만듦

			await Task.CompletedTask;
		}

		public async Task StopAsync()
		{
			Console.WriteLine("GameServer stopping...");

			cts_.Cancel();
			tick_manager_.Stop();
			tcp_server_.Stop();

			if (grpc_server_ != null)
			{
				await grpc_server_.ShutdownAsync();
			}

			Console.WriteLine("GameServer stopped");
		}

    }
}