using System.Collections.Specialized;
namespace ScheduleAnnealing;

public class Schedule
{
    // zadania; pierwszy indeks odpowiada procesorowi
    public List<Job>[] Jobs { get; }
    
    // zadania obecnie w harmonogramie i ich czas zakończenia; do sprawdzania czy zadanie zostało już dodane
    // oraz do pozyskania zadań w kolejności dodawania ich do harmonogramu (dlatego OrderedDictionary)
    private OrderedDictionary jobLookup;
    private Job[] JobsInAddOrder => jobLookup.Keys.Cast<Job>().ToArray();
    public int ProcessorCount => Jobs.Length;

    public int ProcessorTime(int processor)
    {
        if (!Jobs[processor].Any())
        {
            return 0;
        }
        return Jobs[processor].Sum(j => j.Duration);
    }

    public int TotalTime()
    {
        var times = new List<int>();
        for (int i = 0; i < Jobs.Length; ++i)
        {
            times.Add(ProcessorTime(i));
        }

        return times.Max();
    }

    // dodaje zadanie do najmniej obciążonego procesora; jeśli zadanie jest zależne od innego, wymagane jest dodawane najpierw
    // jeśli dodane zadanie zaczyna się wcześniej niż kończy się wymagane, brany jest następny namniej obciążony procesor
    // zwraca czas zakończenia zadania
    private int AddJob(Job job)
    {
        var alreadyAddedJobTime = jobLookup[job];
        if (alreadyAddedJobTime is not null) return (int) alreadyAddedJobTime;
        
        int requiredJobEndTime = -1;
        if (job.RequiredJob is not null)
        {
            requiredJobEndTime = AddJob(job.RequiredJob);
        }

        var ignoredProcessors = new List<int>();
        int chosenProcessor = -1;
        int chosenProcessorTime = Int32.MaxValue;
        while (chosenProcessor == -1)
        {
            for (int i = 0; i < Jobs.Length; ++i)
            {
                if (ignoredProcessors.Contains(i))
                {
                    continue;
                }
                
                int time = ProcessorTime(i);
                if (time < chosenProcessorTime)
                {
                    chosenProcessor = i;
                    chosenProcessorTime = time;
                }
            }

            if (chosenProcessorTime < requiredJobEndTime)
            {
                ignoredProcessors.Add(chosenProcessor);
                chosenProcessor = -1;
                chosenProcessorTime = Int32.MaxValue;
            }
        }
        
        Jobs[chosenProcessor].Add(job);
        jobLookup.Add(job, chosenProcessorTime + job.Duration);
        return chosenProcessorTime + job.Duration;
    }

    // prywatny konstruktor w celu korzystania z factory methods
    private Schedule(int processors)
    {
        jobLookup = new OrderedDictionary();
        Jobs = new List<Job>[processors];
        for (int i = 0; i < Jobs.Length; ++i)
        {
            Jobs[i] = new List<Job>();
        }
    }

    // tutaj jobs to po prostu lista zadań jakie są
    public static Schedule NewRandom(Random rng, Job[] jobs, int processors)
    {
        var schedule = new Schedule(processors);
        foreach (int index in Enumerable.Range(0, jobs.Length).OrderBy(x => rng.Next()))
        {
            schedule.AddJob(jobs[index]);
        }

        return schedule;
    }

    // bierze zadania z istniejące harmonogramu w kolejności, w jakiej były dodawane
    // i dodaje w nieznacznie zmienionej kolejności na podstawie neighborhoodSize
    public static Schedule NewInNeighborhood(Random rng, Schedule schedule, int neighborhoodSize)
    {
        var newSchedule = new Schedule(schedule.ProcessorCount);
        var jobs = schedule.JobsInAddOrder;

        int index = 0;
        var reorderedJobs = jobs.OrderBy(j => index++ + rng.Next(-neighborhoodSize, neighborhoodSize));

        foreach (var job in reorderedJobs)
        {
            newSchedule.AddJob(job);
        }

        return newSchedule;
    }
}