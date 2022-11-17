namespace OK.TaskSchedulingSolver;

public abstract class TaskSchedulingSolver
{
    public string? FilePath;
    protected Task[] Instance;
    protected Processor[] ParallelProcessors;

    protected TaskSchedulingSolver(int numberOfProcessors, Task[] instance)
    {
        Instance = (Task[]) instance.Clone();
        ParallelProcessors = Enumerable.Range(0, numberOfProcessors)
            .Select(id => new Processor(id.ToString(), new List<Task>())).ToArray();
    }

    public abstract IEnumerable<Processor> GetSolvedInstance();
    public abstract int GetLongest();
}

public record struct Processor(string ID, List<Task> Tasks)
{
    public int TotalTime()
    {
        return Tasks.Select(t => t.Duration).Sum();
    }

    public void AssignTask(Task task)
    {
        Tasks.Add(task);
    }

    public override string ToString()
    {
        return
            $"{ID} => End time: {TotalTime()} | Number of tasks: {Tasks.Count} | Assigned tasks: {string.Join(", ", Tasks.Select(t => t.ToString()))}";
    }
}

public record struct Task(int ID, int Duration) : IComparable<Task>
{
    public int CompareTo(Task other)
    {
        if (Duration < other.Duration) return -1;
        if (Duration > other.Duration) return 1;
        return 0;
    }
}