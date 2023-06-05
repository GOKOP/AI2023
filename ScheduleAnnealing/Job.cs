namespace ScheduleAnnealing;

public class Job
{
    public int Id { get; }
    // czas/koszt zadania
    public int Duration { get; }
    // zadanie wymagające tego zadania
    public Job? RequiredJob { get; set; }

    public Job(int id, int duration)
    {
        Id = id;
        Duration = duration;
        RequiredJob = null;
    }
}