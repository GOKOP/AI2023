// See https://aka.ms/new-console-template for more information

float ObjectiveFunction(float solution)
{
    return (float)Math.Pow(solution, 2);
}

float ChooseSolution(Random randomGenerator, float currentSolution, float neighbourhoodWidth, (float, float) problemSpace)
{
    // losowa liczba w sąsiedztwie currentSolution

    float result = problemSpace.Item1 - 1; // niedobre

    while (result < problemSpace.Item1 || result > problemSpace.Item2)
    {
        float randomFloat = (float)randomGenerator.NextDouble();
        float lowerBound = currentSolution - neighbourhoodWidth / 2;
        result = randomFloat * neighbourhoodWidth + lowerBound;
    }

    return result;
}

bool MetropolisCondition(Random randomGenerator, float oldSolution, float newSolution, float temperature)
{
    float difference = ObjectiveFunction(newSolution) - ObjectiveFunction(oldSolution);
    if (difference <= 0) return true;

    float probability = (float)Math.Exp(-difference / temperature);
    float randomFloat = (float)randomGenerator.NextDouble();
    if (randomFloat <= probability) return true;
    return false;
}

float Annealing(float initialSolution, (float, float) problemSpace, float neighbourhoodWidth)
{
    float bestSolution = initialSolution;
    float incumbentSolution = initialSolution;
    float temperature = 10;
    var randomGenerator = new Random();

    int temperatureLength = 10;
    int tempLengthIterations = 0;

    while(temperature >= 0)
    {
        float candidateSolution = ChooseSolution(randomGenerator, incumbentSolution, neighbourhoodWidth, problemSpace);

        if (MetropolisCondition(randomGenerator, incumbentSolution, candidateSolution, temperature))
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


var random = new Random();
float randomFloat = (float) random.NextDouble();

(float, float) problemSpace = (-10, 10);
float initialSolution = randomFloat * (problemSpace.Item2 - problemSpace.Item1) + problemSpace.Item1;
    
float solution = Annealing(initialSolution, problemSpace, 2);
Console.WriteLine(solution);
