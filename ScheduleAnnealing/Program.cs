// See https://aka.ms/new-console-template for more information

using ScheduleAnnealing;

Random rng = new Random();

var jobs = readJobs("jobs.txt");
var schedule = Schedule.NewRandom(rng, jobs, 5);
var bestSchedule = Annealing.SimulateAnnealing(rng, schedule, 5);

printSchedule(bestSchedule);

Job[] readJobs(string filePath)
{
    string[] lines = File.ReadAllLines(filePath);
    Job[] jobs = lines[0].Split(' ').Select((s, i) => new Job(i, Int32.Parse(s))).ToArray();

    foreach (string line in lines.Skip(1))
    {
        int[] indices = line.Split(' ').Select(s => Int32.Parse(s)).ToArray();
        if (indices.Length != 2) throw new ArgumentException("Bad contents of the input file");

        jobs[indices[0]].RequiredJobs.Add(jobs[indices[1]]);
    }

    return jobs;
}

void printSchedule(Schedule schedule)
{
    for(int i=0; i<schedule.ProcessorCount; ++i)
    {
        Console.Write($"Duration: {schedule.ProcessorTime(i)}   ");
        foreach (var job in schedule.Jobs[i])
        {
            Console.Write(job.Id);
            if (job.Id < 10) Console.Write(" ");

            for (int j = 0; j < job.Duration - 1; ++j)
            {
                Console.Write(". ");
            }
        }

        Console.WriteLine("");
    }
    
    Console.WriteLine($"Total duration: {schedule.TotalTime()}");
}