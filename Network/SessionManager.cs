using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Network
{
    public class SessionManager
    {
        private static readonly Lazy<SessionManager> instance_ = new Lazy<SessionManager>(() => new SessionManager());
		public static SessionManager Instance => instance_.Value;

		private readonly ConcurrentDictionary<int, Session> sessions_ = new ConcurrentDictionary<int, Session>();
		private int session_id_generator_ = 0;

		private SessionManager() { }

		public Session? FindSession(int session_id)
		{
			if (sessions_.TryGetValue(session_id, out var session))
			{
				return session;
			}
			return null;
		}

		public Session? CreateSession(Socket socket)
		{
			int session_id = System.Threading.Interlocked.Increment(ref session_id_generator_);
			var session = new Session(session_id, socket, RemoveSession);
			if (sessions_.TryAdd(session_id, session))
			{
				Console.WriteLine($"Session created: {session_id}");

				session.Start();
				return session;
			}
			return null;
		}

		private void RemoveSession(int session_id)
		{
			if (sessions_.TryRemove(session_id, out var session))
			{
				Console.WriteLine($"Session removed: {session_id}");
			}
		}
    }
}