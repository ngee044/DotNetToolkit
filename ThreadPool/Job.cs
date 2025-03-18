using System;
using System.Collections.Generic;

namespace ThreadPool;
public class Job
{
	private string title_;
	private JobPriorities priority_;
	private List<byte> data_;

	private bool use_time_stamp_;

	private readonly Func<(bool, string?)>? callback_;

	private System.WeakReference<JobPool> job_pool_;

	public Job(
		JobPriorities priority,
		string title,
		bool use_time_stamp = false,
		Func<(bool, string?)>? callback = null)
	{
		priority_ = priority;
		title_ = title;
		use_time_stamp_ = use_time_stamp;
		callback_ = callback;

		data_ = new List<byte>();
		job_pool_ = new System.WeakReference<JobPool>(null);
	}

	public void JobPool(JobPool pool)
	{
		job_pool_ = new System.WeakReference<JobPool>(pool);
	}

	public JobPool GetJobPool()
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
