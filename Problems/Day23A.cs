using System.Collections;

namespace Advent_of_Code_2023;

public class Day23A : Problem<Day23A.Input, int> {
    public readonly record struct Input(Grid<TileElement> Grid);

    public enum Tile {
        FOREST,
        PATH,
        EAST,
        NORTH,
        WEST,
        SOUTH
    }

    public record struct TileElement(Tile Value) : IGridElement {
        private static readonly (char @char, Tile tile)[] charsPairings = [
            ('#', Tile.FOREST),
            ('.', Tile.PATH),
            ('>', Tile.EAST),
            ('^', Tile.NORTH),
            ('<', Tile.WEST),
            ('v', Tile.SOUTH)
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

    protected override Input PreProcess(string input) {
        return new Input(Grid<TileElement>.Parse(input));
    }

    private static Int2[] pathOffsets = [
        new Int2(+1, +0),
        new Int2(+0, +1),
        new Int2(-1, +0),
        new Int2(+0, -1)
    ];

    private static Int2[] eastOffsets  = [new Int2(+1, +0)];
    private static Int2[] northOffsets = [new Int2(+0, +1)];
    private static Int2[] westOffsets  = [new Int2(-1, +0)];
    private static Int2[] southOffsets = [new Int2(+0, -1)];

    private Int2[] Offsets(Tile tile) =>
        tile switch {
            Tile.FOREST => Array.Empty<Int2>(),
            Tile.PATH   => pathOffsets,
            Tile.EAST   => eastOffsets,
            Tile.NORTH  => northOffsets,
            Tile.WEST   => westOffsets,
            Tile.SOUTH  => southOffsets,
            _           => throw new ArgumentOutOfRangeException()
        };

    protected override int Solve(Input input) {
        Int2 start = default;
        Int2 end   = default;

        for (int x = 0; x < input.Grid.Size.X; x++) {
            if (input.Grid[new Int2(x, input.Grid.Size.Y - 1)] == Tile.PATH)
                start = new Int2(x, input.Grid.Size.Y - 1);
            if (input.Grid[new Int2(x, 0)] == Tile.PATH)
                end = new Int2(x, 0);
        }

        return Visit(new HashSet<Int2>(), start) ?? -1;

        int? Visit(ISet<Int2> visited, Int2 position) {
            if (!visited.Add(position)) return null;

            Tile tile = input.Grid[position];
            int? length;
            if (tile          == Tile.FOREST) length = null;
            else if (position == end) length         = visited.Count - 1;
            else {
                length = null;
                foreach (Int2 offset in Offsets(tile)) {
                    if (Visit(visited, position + offset) is { } offsetLength)
                        if (length is not { } currentLength || currentLength < offsetLength)
                            length = offsetLength;
                }
            }

            visited.Remove(position);

            return length;
        }
    }

    public static void Run() {
        new Day23A().Solve();
    }
}