namespace ScheduleAnnealing;

// rozwiązanie oparte na rozwiązaniu pierwszego zadania z symulowanym wyszarzaniem
public static class Annealing
{
    static int ObjectiveFunction(Schedule solution)
    {
        return solution.TotalTime();
    }

    static Schedule ChooseSolution(Random rng, Schedule currentSolution, int neighbourhoodWidth)
    {
        return Schedule.NewInNeighborhood(rng, currentSolution, neighbourhoodWidth);
    }

    static bool MetropolisCondition(Random rng, Schedule oldSolution, Schedule newSolution, float temperature)
    {
        float difference = ObjectiveFunction(newSolution) - ObjectiveFunction(oldSolution);
        if (difference <= 0) return true;

        float probability = (float) Math.Exp(-difference / temperature);
        float randomFloat = (float) rng.NextDouble();
        if (randomFloat <= probability) return true;
        return false;
    }

    public static Schedule SimulateAnnealing(Random rng, Schedule initialSolution, int neighbourhoodWidth)
    {
        var bestSolution = initialSolution;
        var incumbentSolution = initialSolution;
        float temperature = 10;

        int temperatureLength = 10;
        int tempLengthIterations = 0;

        while (temperature >= 0)
        {
            var candidateSolution = ChooseSolution(rng, incumbentSolution, neighbourhoodWidth);

            if (MetropolisCondition(rng, incumbentSolution, candidateSolution, temperature))
            {
                incumbentSolution = candidateSolution;
                if (ObjectiveFunction(candidateSolution) < ObjectiveFunction(bestSolution))
                {
                    bestSolution = candidateSolution;
                }
            }

            tempLengthIterations++;
            if (tempLengthIterations >= temperatureLength)
            {
                temperature -= 0.1f;
                tempLengthIterations = 0;
            }
        }

        return bestSolution;
    }
}