using System.Threading;

namespace ThreadPool;

public class ThreadWorker
{
    private string thread_worker_title_;
    private List<JobPriorities> priorities_;
    private System.WeakReference<JobPool> job_pool_;

    private bool pause_;
    private bool thread_stop_;

    private Thread? thread_;
    private AutoResetEvent condition_;

    public ThreadWorker(IEnumerable<JobPriorities> priorities, string worker_title)
    {
        priorities_ = priorities.ToList();
        thread_worker_title_ = worker_title;
        job_pool_ = new System.WeakReference<JobPool>(null);

        pause_ = false;
        thread_stop_ = false;

        condition_ = new AutoResetEvent(false);
    }

    public void JobPool(JobPool pool) => job_pool_ = new System.WeakReference<JobPool>(pool);

    public IEnumerable<JobPriorities> Priorities() => priorities_;
    public void Priorities(IEnumerable<JobPriorities> new_priorities) => priorities_ = new_priorities.ToList();

    public void WorkerTitle(string title) => thread_worker_title_ = title;
    public string WorkerTitle() => thread_worker_title_;

    public (bool, string?) Start()
    {
        if (thread_ != null && thread_.IsAlive)
        {
            return (false, "already started");
        }

        thread_stop_ = false;
        thread_ = new Thread(Run) { IsBackground = true };
        thread_.Start();

        return (true, null);
    }

    public (bool, string?) Stop()
    {
        if (thread_ == null)
        {
            return (false, "thread is not running");
        }

        thread_stop_ = true;
        condition_.Set(); // wake up

        thread_.Join();
        thread_ = null;

        return (true, null);
    }

    public void Pause(bool pause)
    {
        pause_ = pause;
        condition_.Set(); 
    }

    public void NotifyOne(JobPriorities priority)
    {
        if (priorities_.Contains(priority))
        {
            condition_.Set();
        }
    }

    private void Run()
    {
        while (true)
        {
            if (thread_stop_) break;
            if (pause_)
            {
                condition_.WaitOne(100);
                continue;
            }

            if (!job_pool_.TryGetTarget(out var pool) || pool == null)
            {
                break;
            }

            var job = pool.Pop(priorities_);
            if (job == null)
            {
                condition_.WaitOne(100);
                continue;
            }

            var (ok, error) = job.Work();
            if (!ok)
            {
                // 예: Console.WriteLine($"[Error] {error}");
            }
        }
    }
}
