using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day04A : Problem<Day04A.Input, int> {
    public readonly record struct Input(Input.Card[] Cards) {
        public readonly record struct Card(int Id, int[] Winning, int[] Have);
    }

    protected override Input PreProcess(string input) {
        List<Input.Card> cards = new();
        foreach (string line in input.Split('\n')) {
            Match match = RowRegex().Match(line);
            if (!match.Success) continue;
            cards.Add(
                new Input.Card(
                    int.Parse(match.Groups["id"].Value),
                    match.Groups["winning"].Value.Split(" ")
                         .Where(s => s.Length > 0)
                         .Select(int.Parse)
                         .ToArray(),
                    match.Groups["have"].Value.Split(" ")
                         .Where(s => s.Length > 0)
                         .Select(int.Parse)
                         .ToArray()
                )
            );
        }

        return new Input(cards.ToArray());
    }

    protected override int Solve(Input input) =>
        input.Cards.Sum(Score);

    private int Score(int matches) =>
        matches > 0
            ? 1 << (matches - 1)
            : 0;

    private int Score(Input.Card card) =>
        Score(card.Have.Intersect(card.Winning).Count());

    public static void Run() {
        new Day04A().Solve();
    }

    [GeneratedRegex(@"Card\s+(?<id>\d+):\s*(?<winning>[\d\s]*)\s*\|\s*(?<have>[\d\s]*)\s*")]
    private static partial Regex RowRegex();
}