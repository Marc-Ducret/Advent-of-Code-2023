namespace Advent_of_Code_2023;

public class Day21A : Problem<Day21A.Input, int> {
    public readonly record struct Input(int Steps, Grid<TileElement> Grid);

    public enum Tile {
        ROCK,
        START,
        PLOT
    }

    public record struct TileElement(Tile Value) : IGridElement {
        private static readonly (char @char, Tile tile)[] charsPairings = [
            ('S', Tile.START),
            ('.', Tile.PLOT),
            ('#', Tile.ROCK)
        ];

        private static readonly Dictionary<char, Tile> parse =
            charsPairings.ToDictionary();

        private static readonly Dictionary<Tile, char> debug =
            charsPairings.ToDictionary(pair => pair.tile, pair => pair.@char);

        public char DebugChar()   => debug.GetValueOrDefault(Value, '?');
        public void Parse(char c) => Value = parse[c];

        public static implicit operator Tile(TileElement element) => element.Value;
        public static implicit operator TileElement(Tile value)   => new(value);
    }

    public record struct IntElement(int Value) : IGridElement {
        public char DebugChar() =>
            Value switch {
                < 0  => '#',
                < 10 => (char)('0' + Value),
                _    => 'X'
            };

        public void Parse(char c) => Value = c - '0';

        public static implicit operator int(IntElement element) => element.Value;
        public static implicit operator IntElement(int value)   => new(value);
    }

    protected override Input PreProcess(string input) {
        string[] parts = input.Split("\n\n");
        return new Input(int.Parse(parts[0]), Grid<TileElement>.Parse(parts[1]));
    }

    private static Int2[] offsets = [
        new Int2(+1, +0),
        new Int2(+0, +1),
        new Int2(-1, +0),
        new Int2(+0, -1)
    ];

    protected override int Solve(Input input) {
        Int2 startPosition = -1;
        foreach (Int2 position in input.Grid.Positions()) {
            if (input.Grid[position] == Tile.START)
                startPosition = position;
        }

        HashSet<Int2> reachable = [startPosition];

        for (int i = 0; i < input.Steps; i++) {
            reachable =
                reachable.SelectMany(p => offsets
                                         .Select(o => p + o)
                                         .Where(p => input.Grid[p] != Tile.ROCK)
                          )
                         .ToHashSet();
        }

        return reachable.Count;
    }

    public static void Run() {
        new Day21A().Solve();
    }
}