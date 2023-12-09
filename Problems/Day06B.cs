using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day06B : Problem<Day06B.Input, int> {
    public readonly record struct Input(Input.Race race) {
        public readonly record struct Race(long Time, long DistanceRecord);
    }

    protected override Input PreProcess(string input) {
        string[] lines           = input.Split('\n');
        long[]   times           = Numbers(lines[0].Replace(" ", ""));
        long[]   distanceRecords = Numbers(lines[1].Replace(" ", ""));

        return new Input(times.Zip(distanceRecords).Select(pair => new Input.Race(pair.First, pair.Second)).First());

        static long[] Numbers(string s) => NumberRegex().Matches(s).Select(m => long.Parse(m.Value)).ToArray();
    }

    public long Distance(long pressTime, long totalTime) => pressTime * (totalTime - pressTime);

    public int WaysToWins(Input.Race race) {
        double extent = Math.Sqrt(Math.Pow(race.Time, 2) - 4   * race.DistanceRecord);
        int    start  = (int)Math.Ceiling((race.Time - extent) / 2);
        int    end    = (int)Math.Floor((race.Time   + extent) / 2);

        return end - start + 1;
    }

    protected override int Solve(Input input) =>
        WaysToWins(input.race);

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    public static void Run() {
        new Day06B().Solve();
    }
}