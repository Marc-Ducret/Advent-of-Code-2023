namespace Advent_of_Code_2023;

public class Day11A : Problem<Day11A.Input, int> {
    public readonly record struct Input(Map Map);

    public readonly record struct Galaxy(Int2 Position);

    public readonly record struct Map(Tile[,] Tiles) {
        public Int2 Size => new(Tiles.GetLength(0), Tiles.GetLength(1));

        public bool IsWithin(in Int2 position) =>
            ((position >= 0) & (position < Size)).All;

        public Tile this[in Int2 position] {
            get =>
                IsWithin(position)
                    ? Tiles[position.X, position.Y]
                    : Tile.EMPTY;
            set {
                if (IsWithin(position))
                    Tiles[position.X, position.Y] = value;
            }
        }
    }

    public enum Tile {
        EMPTY  = 0,
        GALAXY = 1
    }

    protected override Input PreProcess(string input) {
        Dictionary<char, Tile> parseTile = new() {
            ['.'] = Tile.EMPTY,
            ['#'] = Tile.GALAXY
        };

        string[] lines = input.Split('\n');
        Tile[,]  tiles = new Tile[lines.Min(l => l.Length), lines.Length];

        for (int y = 0; y < tiles.GetLength(1); y++)
        for (int x = 0; x < tiles.GetLength(0); x++) {
            tiles[x, y] = parseTile[lines[tiles.GetLength(1) - y - 1][x]];
        }

        return new Input(new Map(tiles));
    }


    protected override int Solve(Input input) {
        List<int> yExpansions = [];
        {
            for (int y = 0; y < input.Map.Size.Y; y++) {
                bool allEmpty = true;
                for (int x = 0; x < input.Map.Size.X; x++) {
                    allEmpty &= input.Map[new Int2(x, y)] == Tile.EMPTY;
                }

                if (allEmpty)
                    yExpansions.Add(y);
            }
        }

        List<int> xExpansions = [];
        {
            for (int x = 0; x < input.Map.Size.X; x++) {
                bool allEmpty = true;
                for (int y = 0; y < input.Map.Size.Y; y++) {
                    allEmpty &= input.Map[new Int2(x, y)] == Tile.EMPTY;
                }

                if (allEmpty)
                    xExpansions.Add(x);
            }
        }

        List<Galaxy> galaxies = [];

        {
            Int2 expansion = 0;

            for (int y = 0; y < input.Map.Size.Y; y++) {
                expansion.X = 0;
                if (expansion.Y < yExpansions.Count && yExpansions[expansion.Y] == y)
                    expansion.Y++;
                for (int x = 0; x < input.Map.Size.X; x++) {
                    if (expansion.X < xExpansions.Count && xExpansions[expansion.X] == x)
                        expansion.X++;

                    Int2 mapPosition = new(x, y);

                    if (input.Map[mapPosition] == Tile.GALAXY)
                        galaxies.Add(new Galaxy(mapPosition + expansion));
                }
            }
        }

        int sum = 0;

        for (int i = 0; i < galaxies.Count; i++)
        for (int j = i + 1; j < galaxies.Count; j++) {
            sum += Int2.CSum(Int2.Abs(galaxies[i].Position - galaxies[j].Position));
        }

        return sum;
    }

    public static void Run() {
        new Day11A().Solve();
    }
}