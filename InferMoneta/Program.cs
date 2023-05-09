// See https://aka.ms/new-console-template for more information

using Microsoft.ML.Probabilistic.Models;

var rng = new Random();
bool[] data = new bool[100];

for (int i = 0; i < data.Length; i++)
{
    int val = rng.Next(0, 2);
    data[i] = val == 1;
}

Variable<double> mean = Variable.Beta(1, 1);

foreach (var toss in data)
{
    Variable<bool> x = Variable.Bernoulli(mean);
    x.ObservedValue = toss;
}

var engine = new InferenceEngine();
Console.WriteLine("mean: " + engine.Infer(mean));