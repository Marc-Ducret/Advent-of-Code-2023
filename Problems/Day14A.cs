namespace Advent_of_Code_2023;

public class Day14A : Problem<Day14A.Input, int> {
    public readonly record struct Input(Map Map);

    public readonly record struct Map(Tile[,] Tiles) {
        public Int2 Size => new(Tiles.GetLength(0), Tiles.GetLength(1));

        public bool IsWithin(in Int2 position) =>
            ((position >= 0) & (position < Size)).All;

        public Tile this[in Int2 position] =>
            IsWithin(position)
                ? Tiles[position.X, position.Y]
                : Tile.EMPTY;
    }

    public enum Tile {
        EMPTY,
        ROUNDED,
        CUBE
    }


    protected override Input PreProcess(string input) {
        Dictionary<char, Tile> parseTile = new() {
            ['.'] = Tile.EMPTY,
            ['O'] = Tile.ROUNDED,
            ['#'] = Tile.CUBE
        };

        string[] lines = input.Split('\n');
        Tile[,]  tiles = new Tile[lines.Min(l => l.Length), lines.Length];

        for (int y = 0; y < tiles.GetLength(1); y++)
        for (int x = 0; x < tiles.GetLength(0); x++) {
            tiles[x, y] = parseTile[lines[tiles.GetLength(1) - y - 1][x]];
        }

        return new Input(new Map(tiles));
    }

    private int SolveColumn(Map map, int x) {
        int upperLimit = map.Size.Y - 1;
        int sum        = 0;
        for (int y = map.Size.Y - 1; y >= 0; y--) {
            Int2 position = new(x, y);
            switch (map[position]) {
                case Tile.EMPTY:
                    break;
                case Tile.ROUNDED:
                    sum += upperLimit + 1;
                    upperLimit--;
                    break;
                case Tile.CUBE:
                    upperLimit = y - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return sum;
    }

    protected override int Solve(Input input) {
        int sum = 0;
        for (int x = 0; x < input.Map.Size.X; x++) {
            sum += SolveColumn(input.Map, x);
        }

        return sum;
    }

    public static void Run() {
        new Day14A().Solve();
    }
}