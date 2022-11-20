using System.Diagnostics;
using OK.Extensions;
using OK.TaskSchedulingSolver;
using Task = OK.TaskSchedulingSolver.Task;

namespace OK.GeneticAlgorithm;

public sealed class GeneticAlgorithm : TaskSchedulingSolver.TaskSchedulingSolver
{
    private const int StartingPopulation = 10;
    private const int GenerationLimit = 1000;
    private const int PopulationLimit = 50;
    private const double MutationRate = 0.3;
    private const double SurvivalRate = 0.6;
    private static readonly Random Rng = new();
    private readonly TimeSpan _timeLimit = TimeSpan.FromSeconds(15);
    private (byte[], int)? _bestGenome;
    private List<byte[]> _population;

    public GeneticAlgorithm(int numberOfProcessors, Task[] instance) : base(numberOfProcessors, instance)
    {
        _population = new List<byte[]>
        (
            Enumerable.Range(1, StartingPopulation).Select(_ =>
                instance.Select(_ => (byte) Rng.Next(0, numberOfProcessors)).ToArray())
        );
        _population = _population.SortPopulation(Instance);
    }

    public GeneticAlgorithm(int numberOfProcessors, Task[] instance, byte[] startingPopulation) : base(
        numberOfProcessors, instance)
    {
        _population = new List<byte[]> {startingPopulation};
        _population.AddRange(Enumerable.Range(2, StartingPopulation).Select(_ =>
            instance.Select(_ => (byte) Rng.Next(0, numberOfProcessors)).ToArray()
        ));
        _population = _population.SortPopulation(Instance);
    }

    private void RunGeneration()
    {
        var sw = Stopwatch.StartNew();
        var i = 0;
        // foreach (var i in Enumerable.Range(1, GenerationLimit))
        do
        {
            var nextGen = _population.Take(2).ToList();
            var weights = _population.Select(c => c.FinishTime(Instance));
            var cumWeights = weights.Accumulate().Select(p => (double) p).ToList();
            foreach (var j in Enumerable.Range(1, (int) (_population.Count * SurvivalRate)))
            {
                var (parentA, parentB) = Rng.Choices(_population, cumWeights);
                var (offspringA, offspringB) = Rng.Crossover(parentA, parentB);
                Rng.Mutate(offspringA, ParallelProcessors.Length);
                Rng.Mutate(offspringB, ParallelProcessors.Length);
                nextGen.Add(offspringA);
                nextGen.Add(offspringB);
            }

            _population = nextGen;

            _population = _population.SortPopulation(Instance);
            _population = _population.Take(PopulationLimit).ToList();
            var generationBest = _population.First().FinishTime(Instance);
            if (generationBest < _bestGenome?.Item2 || _bestGenome is null)
                _bestGenome = (_population.First(), _population.First().FinishTime(Instance));

            if (i % 10000 == 0)
                Console.WriteLine($"After {i} generations: {_bestGenome.Value.Item2}");
            i += 1;
        } while (sw.Elapsed < _timeLimit);

        _population.SortPopulation(Instance);
        Console.WriteLine($"Time ran out after: {i} generations.");
    }

    public override IEnumerable<Processor> GetSolvedInstance()
    {
        if (_bestGenome is null)
            RunGeneration();
        var res = _bestGenome!.Value.Item1;
        var p = new List<Processor>(ParallelProcessors);
        foreach (var (t, i) in res.Select((t, i) => (t, i))) p.First(lp => lp.ID == t).AssignTask(Instance[i]);

        return p;
    }

    public override int GetLongest()
    {
        if (_bestGenome is null)
            RunGeneration();
        return _bestGenome!.Value.Item2;
    }
}