using System.Collections.Concurrent;

namespace ThreadPool;

public class JobPool
{
	private readonly ConcurrentDictionary<JobPriorities, ConcurrentQueue<Job>> job_queues_;
	private bool lock_condition_;

	public JobPool()
	{
		job_queues_ = new ConcurrentDictionary<JobPriorities, ConcurrentQueue<Job>>();
		lock_condition_ = false;
	}

	public (bool, string?) Push(Job job)
	{
		if (job == null)
		{
			return (false, "cannot push an empty job");
		}
		if (lock_condition_)
		{
			return (false, "the system is locked; new tasks cannot be created");
		}

		var queue = job_queues_.GetOrAdd(job.Priority(), _ => new ConcurrentQueue<Job>());
		queue.Enqueue(job);

		job.JobPool(this);
		return (true, null);
	}

	public Job? Pop(IEnumerable<JobPriorities> priorities)
	{
		foreach (var prio in priorities)
		{
			if (job_queues_.TryGetValue(prio, out var queue))
			{
				if (queue.TryDequeue(out var job))
				{
					return job;
				}
			}
		}
		return null;
	}

	public void Clear() => job_queues_.Clear();

	public void Clear(JobPriorities priority)
		=> job_queues_.TryRemove(priority, out _);

	public void lock_(bool condition) => lock_condition_ = condition;
	public bool lock_() => lock_condition_;

	public long JobCount(IEnumerable<JobPriorities> priorities)
	{
		if (priorities == null)
		{
			long total = 0;
			foreach (var kvp in job_queues_)
				total += kvp.Value.Count;
			return total;
		}

		long count = 0;
		foreach (var prio in priorities)
		{
			if (job_queues_.TryGetValue(prio, out var queue))
			{
				count += queue.Count;
			}
		}
		return count;
	}
}
