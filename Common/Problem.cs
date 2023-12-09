using System.Diagnostics;

namespace Advent_of_Code_2023;

public abstract class Problem<TInput, TOutput> {
    protected abstract TInput  PreProcess(string input);
    protected abstract TOutput Solve(TInput      input);

    private string SolveFile(string path) {
        TInput  input  = PreProcess(File.ReadAllText(path));
        TOutput output = Solve(input);
        return output!.ToString()!;
    }

    private bool Test() {
        string output   = SolveFile($"{FileName}.test.input");
        string expected = File.ReadAllText($"{FileName}.test.output");

        bool success = output == expected;

        Console.WriteLine($"Test {(success ? "Succeeded" : "Failed")}");
        Console.WriteLine("Test Output");
        Console.WriteLine("──────────────────");
        Console.WriteLine(output);
        Console.WriteLine("──────────────────");

        if (!success) {
            Console.WriteLine("Test Expected");
            Console.WriteLine("──────────────────");
            Console.WriteLine(expected);
            Console.WriteLine("──────────────────");
        }

        return success;
    }

    public void Solve() {
        Console.WriteLine($"╔════════╗");
        Console.WriteLine($"║ {GetType().Name} ║");
        Console.WriteLine($"╚════════╝");
        if (Test()) {
            Console.WriteLine("Problem Output");
            Console.WriteLine("──────────────────");
            Console.WriteLine(SolveFile($"{FileName}.input"));
            Console.WriteLine("──────────────────");
        }

        Benchmark();
    }

    public void Benchmark() {
        string stringInput = File.ReadAllText($"{FileName}.input");

        TInput    input     = PreProcess(stringInput);
        Stopwatch stopwatch = new();

        Console.WriteLine($"Benchmark");
        (TimeSpan preProcessTook, int preProcessSamples) =
            Estimate(TimeSpan.FromSeconds(1), () => PreProcess(stringInput));
        Console.WriteLine($"  PreProcess: {preProcessTook.TotalMilliseconds:F2} ms ({preProcessSamples} samples)");
        (TimeSpan solveTook, int solveSamples) =
            Estimate(TimeSpan.FromSeconds(1), () => Solve(input));
        Console.WriteLine($"  Solve     : {solveTook.TotalMilliseconds:F2} ms ({solveSamples} samples)");

        return;

        TimeSpan Measure(int runs, Action action) {
            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 0; i < runs; i++) {
                action();
            }

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        (TimeSpan took, int samples) Estimate(TimeSpan timeBudget, Action action) {
            TimeSpan roughEstimate = Measure(1, action);
            int      samples       = (int)Math.Ceiling(timeBudget.Divide(roughEstimate) / 2);

            Measure(samples, action); // Warm
            TimeSpan runsEstimate = Measure(samples, action);
            return (runsEstimate.Divide(samples), samples);
        }
    }

    private static string InputsFolder => Path.Combine("..", "..", "..", "Inputs");

    private string FileName => Path.Combine(InputsFolder, GetType().Name);
}