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
        Console.WriteLine($"# {GetType().Name} #");
        if (Test()) {
            Console.WriteLine("Problem Output");
            Console.WriteLine("──────────────────");
            Console.WriteLine(SolveFile($"{FileName}.input"));
            Console.WriteLine("──────────────────");
        }
    }

    private static string InputsFolder => Path.Combine("..", "..", "..", "Inputs");

    private string FileName => Path.Combine(InputsFolder, GetType().Name);
}