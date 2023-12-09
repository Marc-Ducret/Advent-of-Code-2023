using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day01B : Problem<Day01B.Input, int> {
    public readonly struct Input {
        public readonly (int firstDigit, int lastDigit)[] rows;

        public Input((int, int)[] rows) {
            this.rows = rows;
        }
    }

    protected override Input PreProcess(string input) {
        string[] digitsSpelled = {
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        };
        List<(int, int)> rows = new();
        foreach (string line in input.Split('\n')) {
            MatchCollection matches = DigitRegex().Matches(line);
            if (!matches.Any()) continue;
            rows.Add((ParseDigit(matches.First().Groups[1].Value), ParseDigit(matches.Last().Groups[1].Value)));
        }

        return new Input(rows.ToArray());

        int ParseDigit(string digit) {
            if (IsDigit(digit[0]))
                return digit[0] - '0';
            int index = Array.IndexOf(digitsSpelled, digit);
            if (index <= 0) throw new ArgumentException($"Invalid digit {digit}", nameof(digit));
            return index;
        }

        static bool IsDigit(char c) => c is >= '0' and <= '9';
    }

    protected override int Solve(Input input) =>
        input.rows
             .Select(row => 10 * row.firstDigit + row.lastDigit)
             .Sum();

    public static void Run() {
        new Day01B().Solve();
    }

    [GeneratedRegex(@"(?=(\d|one|two|three|four|five|six|seven|eight|nine))")]
    private static partial Regex DigitRegex();
}