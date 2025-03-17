using System;
using System.Collections.Generic;

namespace ThreadPool;

public enum JobPriorities
{
    Top,
    High,
    Normal,
    Low,
    LongTerm
}

public static class JobPrioritiesHelper
{
    public static string priority_string(JobPriorities priority) => priority switch
    {
        JobPriorities.Top      => "Top",
        JobPriorities.High     => "High",
        JobPriorities.Normal   => "Normal",
        JobPriorities.Low      => "Low",
        JobPriorities.LongTerm => "LongTerm",
        _                      => "Unknown"
    };

    public static string priority_string(IEnumerable<JobPriorities> priorities)
        => "[ " + string.Join(", ", priorities.Select(priority_string)) + " ]";
}
