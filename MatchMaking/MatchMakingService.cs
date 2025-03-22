using System;
using System;
using System.Collections.Concurrent;
using Redis

namespace MatchMaking
{
    public class MatchMakingService(Redis redis)
    {
        private Redis redis_ = redis;
		private ConcurrentQueue<int> match_queue_ = new ConcurrentQueue<int>();
		
		public void EnqueuePlayer(int player_id)
		{
			match_queue_.Enqueue(player_id);

			var db = redis_.GetDatabase();
			db.StringSet($"player:{player_id}:status", "waiting");

			Console.WriteLine($"Player {player_id} is waiting for match");
		}

		public void ProcessMatching()
		{
			if (match_queue_.Count >= 2)
			{
				if (match_queue_.TryDequeue(out int p1) && match_queue_.TryDequeue(out int p2))
				{
					// TODO
					// Match players
					// Set status to "matched"
					// Notify players
					
					var db = redis_.GetDatabase();
					db.StringSet($"player:{p1}:status", "matched");
					db.StringSet($"player:{p2}:status", "matched");

					Console.WriteLine($"Matched players: {p1} and {p2}");
				}
			}
		}
    }
}