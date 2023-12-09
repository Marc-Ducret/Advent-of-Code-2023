using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day04B : Problem<Day04B.Input, int> {
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

    protected override int Solve(Input input) {
        int[] counts = new int[input.Cards.Length];
        for (int i = 0; i < input.Cards.Length; i++) {
            counts[i] = 1;
        }
        
        for (int i = 0; i < input.Cards.Length; i++) {
            Input.Card card = input.Cards[i];
            for (int j = 1; j <= Matches(card); j++) {
                if (i + j < input.Cards.Length)
                    counts[i + j] += counts[i];
            }
        }

        return counts.Sum();
    }

    private int Matches(Input.Card card) =>
        card.Have.Intersect(card.Winning).Count();

    public static void Run() {
        new Day04B().Solve();
    }

    [GeneratedRegex(@"Card\s+(?<id>\d+):\s*(?<winning>[\d\s]*)\s*\|\s*(?<have>[\d\s]*)\s*")]
    private static partial Regex RowRegex();
}