using System;
using System.Threading;

namespace GameLogic
{
    public class TickManager
    {
        private Thread tick_thread_ = null;
		private bool is_running_ = false;
		private readonly int tick_interval_ms_ = 50;

		public void Start()
		{
			is_running_ = true;
			tick_thread_ = new Thread(TickLoop);
			tick_thread_.Start();
		}

		public void TickLoop()
		{
			while (is_running_)
			{
				var start_time = DateTime.UtcNow;

				// TODO
				// Do game logic here

				// 1) 게임 월드의 캐릭터 위치, HP, 버프, 기타 로직 업데이트
                // 2) 세션별로 변경 사항을 푸시 (브로드캐스트/멀티캐스트)
                // 3) Redis나 DB와 동기화할 필요가 있으면 처리

                // 예: 일정 세션 목록에 상태 전송
                // SessionManager.Instance.BroadcastStateUpdate(...);

				var elapsed_time = DateTime.UtcNow - start_time;
				var sleep_time = tick_interval_ms_ - (int)elapsed_time.TotalMilliseconds;
				if (sleep_time > 0)
				{
					Thread.Sleep(sleep_time);
				}
			}
		}

		public void Stop()
		{
			is_running_ = false;
			tick_thread_?.Join();
		}
    }
}