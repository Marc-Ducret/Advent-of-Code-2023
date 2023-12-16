namespace Advent_of_Code_2023;

public class Day16A : Problem<Day16A.Input, int> {
    public readonly record struct Input(Grid<TileElement> Grid);

    public enum Tile {
        EMPTY,
        MIRROR_POSITIVE,
        MIRROR_NEGATIVE,
        SPLITTER_HORIZONTAL,
        SPLITTER_VERTICAL
    }

    [Flags]
    public enum Direction : byte {
        EMPTY = 0,
        EAST  = 1 << 0,
        NORTH = 1 << 1,
        WEST  = 1 << 2,
        SOUTH = 1 << 3
    }

    private static readonly Dictionary<Direction, Int2> directionOffset = new() {
        [Direction.EAST]  = new Int2(+1, +0),
        [Direction.NORTH] = new Int2(+0, +1),
        [Direction.WEST]  = new Int2(-1, +0),
        [Direction.SOUTH] = new Int2(+0, -1)
    };

    private static readonly Dictionary<Int2, Direction> offsetToDirection =
        directionOffset.ToDictionary(entry => entry.Value, entry => entry.Key);

    private static readonly Dictionary<Direction, Direction> directionOpposite = new() {
        [Direction.EAST]  = Direction.WEST,
        [Direction.NORTH] = Direction.SOUTH,
        [Direction.WEST]  = Direction.EAST,
        [Direction.SOUTH] = Direction.NORTH
    };

    public record struct TileElement(Tile Value) : IGridElement {
        private static readonly (char @char, Tile tile)[] charsPairings = [
            ('.', Tile.EMPTY),
            ('/', Tile.MIRROR_POSITIVE),
            ('\\', Tile.MIRROR_NEGATIVE),
            ('-', Tile.SPLITTER_HORIZONTAL),
            ('|', Tile.SPLITTER_VERTICAL)
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

    public record struct DirectionElement(Direction Value) : IGridElement {
        private static readonly (char @char, Direction tile)[] charsPairings = [
            ('.', Direction.EMPTY),
            ('→', Direction.EAST),
            ('↑', Direction.NORTH),
            ('←', Direction.WEST),
            ('↓', Direction.SOUTH)
        ];

        private static readonly Dictionary<char, Direction> parse =
            charsPairings.ToDictionary();

        private static readonly Dictionary<Direction, char> debug =
            charsPairings.ToDictionary(pair => pair.tile, pair => pair.@char);

        public char DebugChar()   => debug.GetValueOrDefault(Value, '?');
        public void Parse(char c) => Value = parse[c];

        public static implicit operator Direction(DirectionElement element) => element.Value;
        public static implicit operator DirectionElement(Direction value)   => new(value);
    }

    protected override Input PreProcess(string input) =>
        new(Grid<TileElement>.Parse(input));

    private Grid<DirectionElement> SpreadLight(Grid<TileElement> grid) {
        Grid<DirectionElement> light = new(grid.Size);

        Stack<(Int2 position, Direction direction)> spreadStack = new();
        spreadStack.Push((new Int2(0, grid.Size.Y - 1), Direction.EAST));
        while (spreadStack.TryPop(out var spread)) {
            if (!grid.IsWithin(spread.position))
                continue;
            if ((light[spread.position] & spread.direction) != Direction.EMPTY)
                continue;

            light[spread.position] |= spread.direction;
            switch (grid[spread.position].Value) {
                case Tile.EMPTY: {
                    Spread(spread.direction);
                    break;
                }
                case Tile.MIRROR_POSITIVE: {
                    Int2 offset = directionOffset[spread.direction];
                    Spread(offsetToDirection[new Int2(offset.Y, offset.X)]);
                    break;
                }
                case Tile.MIRROR_NEGATIVE: {
                    Int2 offset = directionOffset[spread.direction];
                    Spread(offsetToDirection[-new Int2(offset.Y, offset.X)]);
                    break;
                }
                case Tile.SPLITTER_HORIZONTAL: {
                    if ((spread.direction & (Direction.EAST | Direction.WEST)) != Direction.EMPTY)
                        Spread(spread.direction);
                    else {
                        Spread(Direction.EAST);
                        Spread(Direction.WEST);
                    }

                    break;
                }
                case Tile.SPLITTER_VERTICAL: {
                    if ((spread.direction & (Direction.NORTH | Direction.SOUTH)) != Direction.EMPTY)
                        Spread(spread.direction);
                    else {
                        Spread(Direction.NORTH);
                        Spread(Direction.SOUTH);
                    }

                    break;
                }
            }

            continue;

            void Spread(Direction direction) =>
                spreadStack.Push((spread.position + directionOffset[direction], direction));
        }

        return light;
    }

    protected override int Solve(Input input) {
        Grid<DirectionElement> light = SpreadLight(input.Grid);

        return light.Positions().Count(p => light[p] != Direction.EMPTY);
    }

    public static void Run() {
        new Day16A().Solve();
    }
}