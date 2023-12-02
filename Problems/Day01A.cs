namespace Advent_of_Code_2023;

public class Day01A : Problem<Day01A.Input, int> {
    protected override string Id => "01_A";

    public readonly struct Input {
        public readonly (int firstDigit, int lastDigit)[] rows;

        public Input((int, int)[] rows) {
            this.rows = rows;
        }
    }

    protected override Input PreProcess(TextReader input) {
        List<(int, int)> rows = new();
        while (input.ReadLine() is { } line) {
            if (!line.Any(IsDigit)) continue;
            rows.Add((line.First(IsDigit) - '0', line.Last(IsDigit) - '0'));
        }

        return new Input(rows.ToArray());

        static bool IsDigit(char c) => c is >= '0' and <= '9';
    }

    protected override int Solve(Input input) =>
        input.rows
             .Select(row => 10 * row.firstDigit + row.lastDigit)
             .Sum();

    public static void Run() {
        new Day01A().Solve();
    }
}