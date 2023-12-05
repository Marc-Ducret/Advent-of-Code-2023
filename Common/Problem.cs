using System.Diagnostics;
using System.Globalization;

namespace Advent_of_Code_2023;

public abstract class Problem<TInput, TOutput> {
    protected abstract TInput  PreProcess(TextReader input);
    protected abstract TOutput Solve(TInput          input);

    private TInput ReadInput(string path) {
        using FileStream   stream = File.OpenRead(path);
        using StreamReader reader = new(stream);
        return PreProcess(reader);
    }

    private string SolveFile(string path) {
        TInput  input  = ReadInput(path);
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
        TInput    input     = ReadInput($"{FileName}.input");
        Stopwatch stopwatch = new();

        TimeSpan timeBudget = TimeSpan.FromSeconds(1);

        TimeSpan roughEstimate = Measure(1);
        int      samples       = (int)Math.Ceiling(timeBudget.Divide(roughEstimate) / 2);

        Measure(samples); // Warm
        TimeSpan runsEstimate = Measure(samples);
        Console.WriteLine($"Benchmark: {runsEstimate.Divide(samples).TotalMilliseconds:F2} ms ({samples} samples)");

        return;

        TimeSpan Measure(int runs) {
            stopwatch.Reset();
            stopwatch.Start();
            for (int i = 0; i < runs; i++) {
                Solve(input);
            }

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }

    private static string InputsFolder => Path.Combine("..", "..", "..", "Inputs");

    private string FileName => Path.Combine(InputsFolder, GetType().Name);
}