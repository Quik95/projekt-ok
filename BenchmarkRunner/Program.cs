// See https://aka.ms/new-console-template for more information

using OK.TaskSchedulingInstance;

var instances = Directory.GetFiles("./data", "*.txt").Select(path => new TaskSchedulingInstance(path)).ToArray();
//
// var instances = new[]
// {
//     new TaskSchedulingInstance("data/instance_20.txt"),
//     new TaskSchedulingInstance("data/instance_100.txt"),
//     new TaskSchedulingInstance("data/grupa1_20.txt"),
//     new TaskSchedulingInstance("data/grupa1_100.txt"),
//     new TaskSchedulingInstance("data/grupa2_20.txt"),
//     new TaskSchedulingInstance("data/grupa2_100.txt"),
//     new TaskSchedulingInstance("data/m50.txt"),
//     new TaskSchedulingInstance("data/ekursy_pcmax.txt"),
// };

foreach (var instance in instances) instance.DisplayResults();
// new TaskSchedulingInstance("data/m50.txt").DisplayGeneticStatistics();