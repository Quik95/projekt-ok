using Task = OK.TaskSchedulingSolver.Task;

namespace OK.Extensions;

public static class ExtensionMethods
{
    public static int FinishTime(this IEnumerable<byte> specimen, IReadOnlyList<Task> tasks)
    {
        return specimen.Zip(tasks.Select(t => t.Duration))
            .GroupBy(pair => pair.First)
            .Select(pair => (pair.Key, pair.Select(p => p.Second)))
            .Select(pair => (pair.Key, pair.Item2.Sum()))
            .MaxBy(pair => pair.Item2).Item2;
    }

    public static int Fitness(this IEnumerable<byte> specimen, IReadOnlyList<Task> tasks, int limit)
    {
        var ft = specimen.FinishTime(tasks);
        return ft > limit ? 0 : ft;
    }

    public static IEnumerable<int> Accumulate(this IEnumerable<int> t)
    {
        var cum = 0;
        foreach (var next in t)
        {
            cum += next;
            yield return cum;
        }
    }

    public static (byte[], byte[]) Choices(this Random rng, IEnumerable<byte[]> choices, List<double> cumWeights)
    {
        var bytesEnumerable = choices as byte[][] ?? choices.ToArray();
        var total = cumWeights.Last();
        var idx = cumWeights.BinarySearch(rng.NextDouble() * total);
        int idx2;
        do
        {
            idx2 = cumWeights.BinarySearch(rng.NextDouble() * total);
        } while (idx == idx2);

        return (bytesEnumerable[idx >= 0 ? idx : ~idx], bytesEnumerable[idx2 >= 0 ? idx2 : ~idx2]);
    }

    public static (byte[], byte[]) Crossover(this Random rng, byte[] genomeLeft, byte[] genomeRight)
    {
        var p = rng.Next(1, genomeLeft.Length);
        return (
            genomeLeft[..p].Concat(genomeRight[p..]).ToArray(),
            genomeRight[..p].Concat(genomeLeft[p..]).ToArray()
        );
    }

    public static void Mutate(this Random rng, byte[] genome, int n, double probability = 0.3)
    {
        var choice = rng.NextDouble();
        if (!(probability >= choice)) return;

        var (one, two) = (rng.Next(0, genome.Length), rng.Next(0, genome.Length));
        (genome[one], genome[two]) = (genome[two], genome[one]);
    }

    public static List<byte[]> SortPopulation(this IEnumerable<byte[]> p, IReadOnlyList<Task> instance)
    {
        return p.Select(p => (p, p.FinishTime(instance))).OrderBy(p => p.Item2).Select(p => p.p).ToList();
    }
}