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

        Console.WriteLine($"Instancja: {FilePath}");
        Console.WriteLine($"\tNajdłuższy czas wykonywania algorytmu heurystycznego: {heuristic}");
        Console.WriteLine($"\tNajdłuższy czas wykonywania algorytmu genetycznego: {genetic}");
        Console.WriteLine(
            $"\tNajdłuższy czas wykonywania algorytmu genetycznego zaczynając od wyniku heurystycznego: {heuristicGenetic}");
        Console.WriteLine();
    }

    public void DisplayGeneticStatistics()
    {
        var gh = new GeneticAlgorithm.GeneticAlgorithm(DegreeOfParallelism, Instance);
        DisplayResults();
        var s = gh.GetSolvedInstance();
        foreach (var processor in s) Console.WriteLine(processor);
    }
}