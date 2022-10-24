namespace OK;

public record struct Processor(string ID, List<int> Tasks)
{
    public int TotalTime() => Tasks.Sum();

    public void AssignTask(int task)
    {
        Tasks.Add(task);
    }
}

interface ITaskSchedulingSolver
{
    public Processor[] Solve();
}

public sealed class GreedyHeuristic : ITaskSchedulingSolver
{
    private int[] Instance;
    private Processor[] ParallelProcessors;

    public GreedyHeuristic(int numberOfProcessors, int[] instance)
    {
        Instance = instance;
        ParallelProcessors = Enumerable.Range(0, numberOfProcessors).Select(id => new Processor(id.ToString(), new List<int>())).ToArray();
    }

    public GreedyHeuristic(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var n = int.Parse(lines.First());
        
        Instance = lines.Skip(2).Select(int.Parse).ToArray();
        ParallelProcessors = Enumerable.Range(0, n).Select(id => new Processor(id.ToString(), new List<int>())).ToArray();
    }

    public Processor[] Solve()
    {
        var queue = new PriorityQueue<Processor, int>(ParallelProcessors.Select(p => (p, 0)), Comparer<int>.Create((lhs, rhs) => lhs - rhs));
        queue.EnsureCapacity(ParallelProcessors.Length);

        foreach (var task in Instance)
        {
            var top = queue.Dequeue();
            top.AssignTask(task);
            queue.Enqueue(top, top.TotalTime());
        }

        return queue.UnorderedItems.Select(p => p.Element).ToArray();
    }
}

internal static class Program
{
    public static void Main()
    {
        // var greedySolver = new GreedyHeuristic(2, new []{5, 4, 3, 2, 1});
        var greedySolver = new GreedyHeuristic("test_instance.txt");
        var solvedInstance = greedySolver.Solve();
        var sorted = solvedInstance.ToList();
        sorted.Sort((lhs, rhs) => lhs.TotalTime() - rhs.TotalTime());
        foreach (var processor in sorted)
        {
            Console.WriteLine($"{processor} => PCmax: {processor.TotalTime()} | Tasks: {processor.Tasks.Count}");
        }
    }
}