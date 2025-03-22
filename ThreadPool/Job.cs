using System;
using System.Collections.Generic;

namespace ThreadPool;
public class Job(
		JobPriorities priority,
		string title,
		bool use_time_stamp = false,
		Func<(bool, string?)>? callback = null)
{
	private string title_ = title;
	private JobPriorities priority_ = priority;
	private List<byte> data_ = new List<byte>();

	private bool use_time_stamp_ = use_time_stamp;
	private readonly Func<(bool, string?)>? callback_ = callback;
	private WeakReference<JobPool> job_pool_ = new WeakReference<JobPool>(null);

	public void JobPool(JobPool pool)
	{
		job_pool_ = new WeakReference<JobPool>(pool);
	}

	public JobPool? GetJobPool()
	{
		if (job_pool_.TryGetTarget(out var pool))
		{
			return pool;
		}
		return null;
	}

	public string Title()
	{
		return title_;
	}

	public void Title(string new_title)
	{
		title_ = new_title;
	}

	public JobPriorities Priority()
	{
		return priority_;
	}

	public (bool, string?) Work()
	{
		if (callback_ == null)
		{
			return (false, $"No callback for job: {title_}");
		}

		try
		{
			var (ok, message) = callback_.Invoke();
			return (ok, message);
		}
		catch (Exception ex)
		{
			return (false, $"Exception on job [{title_}]: {ex.Message}");
		}
	}
}
