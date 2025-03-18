namespace ThreadPool;

public class ThreadPool
{
    private string thread_title_;
    private List<ThreadWorker> thread_workers_;
    private JobPool job_pool_;

    private bool working_;
    private bool pause_;

    public ThreadPool(string title)
    {
        thread_title_ = title;
        thread_workers_ = new List<ThreadWorker>();
        job_pool_ = new JobPool();
        working_ = false;
        pause_ = false;
    }

    public void ThreadTitle(string new_title) => thread_title_ = new_title;
    public string ThreadTitle() => thread_title_;

    public JobPool JobPool() => job_pool_;

    public void Push(ThreadWorker worker)
    {
        if (worker == null) return;

        worker.JobPool(job_pool_);
        worker.Pause(pause_);
        worker.WorkerTitle($"{worker.WorkerTitle()} on {thread_title_}");

        thread_workers_.Add(worker);

        if (working_)
        {
            worker.Start();
        }
    }

    public (bool, string?) Push(Job job)
    {
        if (job_pool_ == null)
        {
            return (false, "cannot push a job into a null JobPool");
        }

        var result = job_pool_.Push(job);
        if (result.Item1)
        {
            // notify all workers that handle this job's priority
            foreach (var worker in thread_workers_)
            {
                worker.NotifyOne(job.Priority());
            }
        }
        return result;
    }

    public (bool, string?) Start()
    {
        if (working_)
        {
            return (false, "already started");
        }

        foreach (var worker in thread_workers_)
        {
            var (ok, err) = worker.Start();
            if (!ok)
            {
                return (false, err);
            }
        }
        working_ = true;
        return (true, null);
    }

    public void Stop(bool stop_immediately)
    {
        if (!working_) return;

        job_pool_.lock_(true);

        if (stop_immediately)
        {
            job_pool_.Clear();
        }

        foreach (var w in thread_workers_)
        {
            w.Stop();
        }

        job_pool_.lock_(false);
        working_ = false;
    }

    public void Pause(bool pause)
    {
        pause_ = pause;
        foreach (var w in thread_workers_)
        {
            w.Pause(pause);
        }
    }
}
