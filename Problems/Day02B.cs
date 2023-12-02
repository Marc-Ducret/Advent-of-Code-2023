using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day02B : Problem<Day02B.Input, int> {
    protected override string Id => "02_B";

    public readonly struct Input {
        public readonly Game[] games;

        public Input(Game[] games) {
            this.games = games;
        }

        public readonly struct Game {
            public readonly int     id;
            public readonly Cubes[] showings;

            public Game(int id, Cubes[] showings) {
                this.id       = id;
                this.showings = showings;
            }

            public bool Possible(Cubes cubes) =>
                showings.All(c => c.LowerThan(cubes));

            public Cubes MinimumPossible =>
                showings.Aggregate(default(Cubes), (a, b) => a.Max(b));
        }

        public struct Cubes {
            public int r, g, b;

            public bool LowerThan(in Cubes that) =>
                r <= that.r
             && g <= that.g
             && b <= that.b;

            public Cubes Max(in Cubes that) =>
                new() {
                    r = Math.Max(r, that.r),
                    g = Math.Max(g, that.g),
                    b = Math.Max(b, that.b),
                };

            public int Power => r * g * b;
        }
    }

    protected override Input PreProcess(TextReader input) {
        List<Input.Game> games = new();
        while (input.ReadLine() is { } line) {
            Match match = RowRegex().Match(line);
            if (!match.Success) continue;
            int id = int.Parse(match.Groups["id"].Value);

            Input.Cubes[] showings =
                match.Groups["showings"].Value.Split(";")
                     .Select(showing => {
                          Input.Cubes cubes = default;
                          foreach (Match cubeCountMatch in CubeCountRegex().Matches(showing)) {
                              int count = int.Parse(cubeCountMatch.Groups["count"].Value);
                              switch (cubeCountMatch.Groups["color"].Value) {
                                  case "red":
                                      cubes.r = count;
                                      break;
                                  case "green":
                                      cubes.g = count;
                                      break;
                                  case "blue":
                                      cubes.b = count;
                                      break;
                              }
                          }

                          return cubes;
                      })
                     .ToArray();
            games.Add(new Input.Game(id, showings));
        }

        return new Input(games.ToArray());
    }

    protected override int Solve(Input input) =>
        input.games
             .Sum(game => game.MinimumPossible.Power);

    public static void Run() {
        new Day02B().Solve();
    }

    [GeneratedRegex(@"Game (?<id>\d+): (?<showings>.+)")]
    private static partial Regex RowRegex();

    [GeneratedRegex(@"(?<count>\d+) (?<color>(red|green|blue))")]
    private static partial Regex CubeCountRegex();
}