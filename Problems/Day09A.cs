using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day09A : Problem<Day09A.Input, long> {
    public readonly record struct Input(Sequence[] Sequences);

    public readonly record struct Sequence(long[] Values);

    protected override Input PreProcess(string input) {
        List<Sequence> sequences = new();
        foreach (string line in input.Split('\n'))
            sequences.Add(new Sequence(Numbers(line)));

        return new Input(sequences.ToArray());
        
        static long[] Numbers(string s) => NumberRegex().Matches(s).Select(m => long.Parse(m.Value)).ToArray();
    }

    private Sequence Derivative(Sequence sequence) {
        long[] values = new long[sequence.Values.Length - 1];
        for (long i = 0; i < values.Length; i++) {
            values[i] = sequence.Values[i + 1] - sequence.Values[i];
        }

        return new Sequence(values);
    }

    private bool IsZero(Sequence sequence) => sequence.Values.All(x => x == 0);

    private long Extrapolate(Sequence sequence) {
        if (IsZero(sequence)) return 0;
        return sequence.Values[^1] + Extrapolate(Derivative(sequence));
    }

    protected override long Solve(Input input) =>
        input.Sequences.Sum(Extrapolate);

    public static void Run() {
        new Day09A().Solve();
    }
    
    [GeneratedRegex(@"[-\d]+")]
    private static partial Regex NumberRegex();
}