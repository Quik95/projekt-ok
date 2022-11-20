using OK.TaskSchedulingSolver;
using Task = OK.TaskSchedulingSolver.Task;

namespace OK.GreedyHeuristic;

public sealed class GreedyHeuristic : TaskSchedulingSolver.TaskSchedulingSolver
{
    private (Processor[], int)? Solved;

    public GreedyHeuristic(int numberOfProcessors, Task[] instance) : base(numberOfProcessors, instance)
    {
    }

    public override Processor[] GetSolvedInstance()
    {
        if (Solved is null)
            Solve();
        return Solved!.Value.Item1;
    }

    private void Solve()
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

        var s = queue.UnorderedItems.Select(p => p.Element).ToList();
        s.Sort((lhs, rhs) => lhs.TotalTime() - rhs.TotalTime());
        Solved = (s.ToArray(), s.Last().TotalTime());
    }

    public override int GetLongest()
    {
        if (Solved is null)
            Solve();
        return Solved!.Value.Item2;
    }
}