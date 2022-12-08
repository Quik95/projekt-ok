using System.Diagnostics;
using OK.TaskSchedulingSolver;
using Task = OK.TaskSchedulingSolver.Task;

namespace OK.GeneticAlgorithm;

public sealed class GeneticAlgorithm : TaskSchedulingSolver.TaskSchedulingSolver
{
    private const int StartingPopulation = 10;
    private const int PopulationLimit = 1000;
    private const int TournamentSize = 4;
    private const double ElitismCarryOver = 0.1;
    private const double MutationRate = 0.2;
    private const double SurvivalRate = 1;

    private static readonly Random Rng = new(42);
    private readonly TimeSpan _timeLimit = TimeSpan.FromSeconds(5);

    private Genome? _bestGenome;
    private Genome[] _population;

    public GeneticAlgorithm(int numberOfProcessors, Task[] instance) : base(numberOfProcessors, instance)
    {
        _population =
            Enumerable.Range(1, StartingPopulation)
                .Select(_ =>
                    new Genome(
                        instance.Select(_ => (byte) Rng.Next(0, numberOfProcessors)).ToArray(),
                        instance.Select(t => t.Duration).ToArray()
                    )
                )
                .ToArray();
        SortPopulation();
    }

    public GeneticAlgorithm(int numberOfProcessors, Task[] instance, byte[] startingPopulation) : base(
        numberOfProcessors, instance)
    {
        _population = Enumerable
            .Range(2, StartingPopulation)
            .Select(_ =>
                new Genome(
                    instance.Select(_ => (byte) Rng.Next(0, numberOfProcessors)).ToArray(),
                    instance.Select(t => t.Duration).ToArray()
                )
            )
            .Append(new Genome(startingPopulation, instance.Select(t => t.Duration).ToArray()))
            .ToArray();
        SortPopulation();
    }

    private void RunGeneration()
    {
        var sw = Stopwatch.StartNew();
        var i = 0;
        var offspringCount = (int) (_population.Length * SurvivalRate);
        while (sw.Elapsed < _timeLimit)
        {
            var nextGen = _population.Take(2).ToList();
            foreach (var _ in Enumerable.Range(1, offspringCount))
            {
                var (parentA, parentB) = (Choices(_population), Choices(_population));
                var (offspringA, offspringB) = Genome.Crossover(Rng, parentA, parentB);
                offspringA.Mutate(Rng, MutationRate);
                offspringB.Mutate(Rng, MutationRate);
                nextGen.Add(offspringA);
                nextGen.Add(offspringB);
            }

            _population = nextGen.Concat(_population.Take((int) (_population.Length * ElitismCarryOver))).ToArray();
            SortPopulation();
            _population = _population.Take(PopulationLimit).ToArray();

            var generationBest = _population.First();
            if (generationBest.Fitness < _bestGenome?.Fitness || _bestGenome is null)
                _bestGenome = generationBest;

            i += 1;
        }

        SortPopulation();
        Console.WriteLine($"Time ran out after: {i} generations.");
    }

    public override IEnumerable<Processor> GetSolvedInstance()
    {
        if (_bestGenome is null)
            RunGeneration();
        var res = _bestGenome!.Sequence;
        var p = new List<Processor>(ParallelProcessors);
        foreach (var (t, i) in res.Select((t, i) => (t, i))) p.First(lp => lp.ID == t).AssignTask(Instance[i]);

        return p;
    }

    public override int GetLongest()
    {
        if (_bestGenome is null)
            RunGeneration();
        return _bestGenome!.Fitness;
    }

    private void SortPopulation()
    {
        _population = _population.OrderBy(c => c.Fitness).ToArray();
    }

    private static Genome Choices(IReadOnlyList<Genome> choices)
    {
        var best = choices[Rng.Next(choices.Count)];
        for (var i = 0; i < TournamentSize; i++)
        {
            var pretender = choices[Rng.Next(choices.Count)];
            if (pretender.Fitness < best.Fitness)
                best = pretender;
        }

        return best;
    }
}

public sealed class Genome
{
    public Genome(byte[] sequence, int[] instance)
    {
        Sequence = sequence;
        Instance = instance;
        Fitness = RecalculateFitness();
    }

    public byte[] Sequence { get; private set; }
    public int Fitness { get; private set; }
    private int[] Instance { get; }

    private int RecalculateFitness()
    {
        return Sequence.Zip(Instance)
            .GroupBy(pair => pair.First)
            .Select(pair => (pair.Key, pair.Select(p => p.Second)))
            .Select(pair => (pair.Key, pair.Item2.Sum()))
            .MaxBy(pair => pair.Item2).Item2;
    }

    public void Mutate(Random rng, double probability)
    {
        var choice = rng.NextDouble();
        if (!(probability >= choice)) return;

        var (one, two) = (rng.Next(0, Sequence.Length), rng.Next(0, Sequence.Length));
        (Sequence[one], Sequence[two]) = (Sequence[two], Sequence[one]);

        Fitness = RecalculateFitness();
    }

    public static (Genome, Genome) Crossover(Random rng, Genome genomeLeft, Genome genomeRight)
    {
        var p = rng.Next(1, genomeLeft.Sequence.Length);
        var leftPart = (byte[]) genomeLeft.Sequence[p..].Clone();
        var newLeft = genomeLeft.Sequence = genomeLeft.Sequence[..p].Concat(genomeRight.Sequence[p..]).ToArray();
        var newRight = genomeRight.Sequence = genomeRight.Sequence[..p].Concat(leftPart).ToArray();

        return (new Genome(newLeft, genomeLeft.Instance), new Genome(newRight, genomeRight.Instance));
    }
}