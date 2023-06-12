using GAF;
using GAF.Extensions;
using GAF.Operators;

namespace ScheduleAnnealing;

public class GeneticSolver
{
    private int processorCount;

    public GeneticSolver(int processorCount)
    {
        this.processorCount = processorCount;
    }
    
    // fitness function to czas wykonywania harmonogramu
    private double FitnessFunction(Chromosome chromosome)
    {
        var schedule = Schedule.NewFromJobs(chromosome.Genes.Select(g => (Job) g.ObjectValue), processorCount);
        return 1.0 - (float)schedule.TotalTime() / 100000;
    }

    private bool Termination(Population population, int currentGeneration, long currentEvaluation)
    {
        return currentGeneration >= 500;
    }
    
    public Schedule FindBestSchedule(Job[] jobs)
    {
        int populationSize = 100;
        var population = new Population();

        for (int i = 0; i < populationSize; ++i)
        {
            var chromosome = new Chromosome();
            chromosome.Genes.AddRange(jobs.Select(j => new Gene(j)));
            chromosome.Genes.ShuffleFast();
            population.Solutions.Add(chromosome);
        }

        // dobiera 5% najlepszych rozwiązań
        var elite = new Elite(5);
        
        var crossover = new Crossover(0.8)
        {
            // bierze sekcję genów między dwoma punktami z jednego rodzica,
            // następnie z tego samego rodzica bierze pozostałe geny w kolejności, w jakiej pojawiają się one u drugiego
            // zapobiega to tworzeniu chromosomów ze zduplikowanymi lub brakującymi genami,
            // co dzieje się przy stosowaniu SinglePoint lub DoublePoint
            CrossoverType = CrossoverType.DoublePointOrdered
        };
        var mutate = new SwapMutate(0.02);

        var genetic = new GeneticAlgorithm(population, FitnessFunction);
        genetic.Operators.Add(elite);
        genetic.Operators.Add(crossover);
        genetic.Operators.Add(mutate);
        
        genetic.Run(Termination);

        var bestJobs = genetic.Population.GetTop(1)[0].Genes.Select(g => (Job) g.ObjectValue);
        return Schedule.NewFromJobs(bestJobs, processorCount);
    }
}