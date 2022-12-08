using OK.TaskSchedulingSolver;
using Task = OK.TaskSchedulingSolver.Task;

namespace OK.TaskSchedulingInstance;

public class TaskSchedulingInstance
{
    protected readonly int DegreeOfParallelism;
    public readonly string? FilePath;
    protected readonly Task[] Instance;

    public TaskSchedulingInstance(string filePath)
    {
        FilePath = filePath;
        var lines = File.ReadAllLines(filePath);
        DegreeOfParallelism = int.Parse(lines.First());

        Instance = lines.Skip(2).Where(line => !string.IsNullOrEmpty(line))
            .Select((l, i) => new Task(Duration: int.Parse(l), ID: i)).ToArray();
    }

    public void DisplayResults()
    {
        var gh = new GreedyHeuristic.GreedyHeuristic(DegreeOfParallelism, Instance);
        var ga = new GeneticAlgorithm.GeneticAlgorithm(DegreeOfParallelism, Instance);

        var heuristicSolution = gh.GetSolvedInstance().ToArray();
        var hToGen = new byte[Instance.Length];
        foreach (var processor in heuristicSolution)
        foreach (var (id, _) in processor.Tasks)
            hToGen[id] = (byte) processor.ID;

        var gha = new GeneticAlgorithm.GeneticAlgorithm(DegreeOfParallelism, Instance, hToGen.ToArray());
        var heuristic = gh.GetLongest();
        var genetic = ga.GetLongest();
        var heuristicGenetic = gha.GetLongest();
        var theoreticalBest = Instance.Select(t => t.Duration).Sum() / DegreeOfParallelism;

        Console.WriteLine($"Instancja: {FilePath}");
        Console.WriteLine($"\tTeoretyczny best: {theoreticalBest}");
        Console.WriteLine(
            $"\tNajdłuższy czas wykonywania algorytmu zachłannego: {heuristic} ({(double) heuristic / theoreticalBest * 100:F2})");
        Console.WriteLine(
            $"\tNajdłuższy czas wykonywania algorytmu genetycznego: {genetic} ({(double) genetic / theoreticalBest * 100:F2})");
        Console.WriteLine(
            $"\tNajdłuższy czas wykonywania algorytmu genetycznego zaczynając od wyniku zachłannego: {heuristicGenetic} ({(double) heuristicGenetic / theoreticalBest * 100:F2})");

        var col = Console.ForegroundColor;
        if (heuristic < genetic && heuristic < heuristicGenetic)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\tWynik negatywny!!!");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tWynik pozytywny");
        }

        Console.ForegroundColor = col;
        Console.WriteLine();
    }

    public void DisplayGeneticStatistics()
    {
        var gh = new GeneticAlgorithm.GeneticAlgorithm(DegreeOfParallelism, Instance);
        var s = gh.GetSolvedInstance();
        var processors = s as Processor[] ?? s.ToArray();
        foreach (var processor in processors) Console.WriteLine(processor);
        var mostTasks = processors.Select(p => p.Tasks.Count).Max();
        for (var i = 0; i <= mostTasks; i++)
            Console.WriteLine($"Number of processors with {i} tasks: {processors.Count(p => p.Tasks.Count == i)}");
        Console.WriteLine($"Time: {gh.GetLongest()}");
    }
}