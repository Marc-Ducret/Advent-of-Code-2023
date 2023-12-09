using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day06A : Problem<Day06A.Input, int> {
    public readonly record struct Input(Input.Race[] Races) {
        public readonly record struct Race(int Time, int DistanceRecord);
    }

    protected override Input PreProcess(string input) {
        string[] lines           = input.Split('\n');
        int[]    times           = Numbers(lines[0]);
        int[]    distanceRecords = Numbers(lines[1]);

        return new Input(times.Zip(distanceRecords).Select(pair => new Input.Race(pair.First, pair.Second)).ToArray());

        static int[] Numbers(string s) => NumberRegex().Matches(s).Select(m => int.Parse(m.Value)).ToArray();
    }

    public int Distance(int pressTime, int totalTime) => pressTime * (totalTime - pressTime);

    public int WaysToWins(Input.Race race) =>
        Enumerable.Range(0, race.Time + 1).Count(pressTime => Distance(pressTime, race.Time) > race.DistanceRecord);

    protected override int Solve(Input input) =>
        input.Races
             .Select(WaysToWins)
             .Aggregate((a, b) => a * b);

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    public static void Run() {
        new Day06A().Solve();
    }
}