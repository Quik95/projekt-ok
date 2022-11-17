// See https://aka.ms/new-console-template for more information

using OK.TaskSchedulingInstance;

foreach (var instance in new[]
         {
             new TaskSchedulingInstance("instance_20.txt"),
             new TaskSchedulingInstance("instance_100.txt"),
             new TaskSchedulingInstance("grupa1_20.txt"),
             new TaskSchedulingInstance("grupa1_100.txt"),
             new TaskSchedulingInstance("grupa2_20.txt"),
             new TaskSchedulingInstance("grupa2_100.txt")
         })
    instance.DisplayResults();