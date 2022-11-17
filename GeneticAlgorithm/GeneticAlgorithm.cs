using OK.TaskSchedulingSolver;
using Task = OK.TaskSchedulingSolver.Task;

namespace GeneticAlgorithm;

public sealed class GeneticAlgorithm : TaskSchedulingSolver
{
    public GeneticAlgorithm(int numberOfProcessors, Task[] instance) : base(numberOfProcessors, instance)
    {
    }

    public override IEnumerable<Processor> GetSolvedInstance()
    {
        throw new NotImplementedException();
    }

    public override int GetLongest()
    {
        return Random.Shared.Next(
            Instance.Max().Duration,
            Instance.Aggregate(0, (acc, next) => acc + next.Duration)
        );
    }
}