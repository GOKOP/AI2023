namespace ScheduleAnnealing;

public class Job
{
    public int Id { get; }
    // czas/koszt zadania
    public int Duration { get; }
    // zadania wymagane przez to zadanie
    public List<Job> RequiredJobs { get; set; }

    public Job(int id, int duration)
    {
        Id = id;
        Duration = duration;
        RequiredJobs = new List<Job>();
    }
}