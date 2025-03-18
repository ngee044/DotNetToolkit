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

    public void thread_title(string new_title) => thread_title_ = new_title;
    public string thread_title() => thread_title_;

    public JobPool job_pool() => job_pool_;

    public void push(ThreadWorker worker)
    {
        if (worker == null) return;

        worker.job_pool(job_pool_);
        worker.pause(pause_);
        worker.worker_title($"{worker.worker_title()} on {thread_title_}");

        thread_workers_.Add(worker);

        if (working_)
        {
            worker.start();
        }
    }

    public (bool, string?) push(Job job)
    {
        if (job_pool_ == null)
        {
            return (false, "cannot push a job into a null JobPool");
        }

        var result = job_pool_.push(job);
        if (result.Item1)
        {
            // notify all workers that handle this job's priority
            foreach (var worker in thread_workers_)
            {
                worker.notify_one(job.priority());
            }
        }
        return result;
    }

    public (bool, string?) start()
    {
        if (working_)
        {
            return (false, "already started");
        }

        foreach (var worker in thread_workers_)
        {
            var (ok, err) = worker.start();
            if (!ok)
            {
                return (false, err);
            }
        }
        working_ = true;
        return (true, null);
    }

    public void stop(bool stop_immediately)
    {
        if (!working_) return;

        job_pool_.lock_(true);

        if (stop_immediately)
        {
            job_pool_.clear();
        }

        foreach (var w in thread_workers_)
        {
            w.stop();
        }

        job_pool_.lock_(false);
        working_ = false;
    }

    public void pause(bool pause)
    {
        pause_ = pause;
        foreach (var w in thread_workers_)
        {
            w.pause(pause);
        }
    }
}
