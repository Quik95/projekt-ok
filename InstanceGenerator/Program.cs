// See https://aka.ms/new-console-template for more information

using System.Runtime.Serialization;

const int minTaskLength = 10;
const int maxTaskLength = 90;

var numberOfParallelProcessors = Random.Shared.Next(1, 31);
var numberOfTasks = Random.Shared.Next(numberOfParallelProcessors + 1, 301);
var tasks = Enumerable.Range(0, numberOfTasks).Select(_ => Random.Shared.Next(minTaskLength, maxTaskLength+1));
using var file = File.CreateText("instance.txt");

file.WriteLine(numberOfParallelProcessors);
file.WriteLine(numberOfTasks);
foreach (var task in tasks)
{
    file.WriteLine(task);
}