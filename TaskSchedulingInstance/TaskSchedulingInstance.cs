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

        Instance = lines.Skip(2).Select((l, i) => new Task(Duration: int.Parse(l), ID: i)).ToArray();
    }

    public void DisplayResults()
    {
        var gh = new GreedyHeuristic.GreedyHeuristic(DegreeOfParallelism, Instance);
        var ga = new GeneticAlgorithm.GeneticAlgorithm(DegreeOfParallelism, Instance);

        Console.WriteLine($"Instancja: {FilePath}");
        Console.WriteLine($"\tNajdłuższy czas wykonywania algorytmu heurystycznego: {gh.GetLongest()}");
        Console.WriteLine($"\tNajdłuższy czas wykonywania algorytmu genetycznego: {ga.GetLongest()}");
        Console.WriteLine();
    }
}