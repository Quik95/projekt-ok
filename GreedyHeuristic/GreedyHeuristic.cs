using OK.TaskSchedulingSolver;
using Task = OK.TaskSchedulingSolver.Task;

namespace OK.GreedyHeuristic;

public sealed class GreedyHeuristic : TaskSchedulingSolver.TaskSchedulingSolver
{
    public GreedyHeuristic(int numberOfProcessors, Task[] instance) : base(numberOfProcessors, instance)
    {
    }

    public override IEnumerable<Processor> GetSolvedInstance()
    {
        var queue = new PriorityQueue<Processor, int>(ParallelProcessors.Select(p => (p, 0)),
            Comparer<int>.Create((lhs, rhs) => lhs - rhs));
        queue.EnsureCapacity(ParallelProcessors.Length);

        foreach (var task in Instance)
        {
            var top = queue.Dequeue();
            top.AssignTask(task);
            queue.Enqueue(top, top.TotalTime());
        }

        return queue.UnorderedItems.Select(p => p.Element).ToArray();
    }

    public override int GetLongest()
    {
        var solved = GetSolvedInstance().ToList();
        solved.Sort((lhs, rhs) => lhs.TotalTime() - rhs.TotalTime());
        return solved.Last().TotalTime();
    }
}