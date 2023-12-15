using System.Collections;
using System.Text;

namespace Advent_of_Code_2023;

public class Day14B : Problem<Day14B.Input, int> {
    public readonly record struct Input(Map Map);

    public readonly struct Map(Tile[,] tiles) : IEquatable<Map> {
        private readonly Tile[,] _tiles = tiles;

        public Int2 Size => new(_tiles.GetLength(0), _tiles.GetLength(1));

        public bool IsWithin(in Int2 position) =>
            ((position >= 0) & (position < Size)).All;

        public Tile this[in Int2 position] {
            get =>
                IsWithin(position)
                    ? _tiles[position.X, position.Y]
                    : Tile.EMPTY;
            set {
                if (IsWithin(position))
                    _tiles[position.X, position.Y] = value;
            }
        }

        public IEnumerable<Int2> Positions() {
            Int2 size = Size;
            return Enumerable.Range(0, size.Y)
                             .SelectMany(
                                  y => Enumerable.Range(0, size.X)
                                                 .Select(x => new Int2(x, y)));
        }

        public string DebugString {
            get {
                StringBuilder builder = new();
                for (int y = Size.Y - 1; y >= 0; y--) {
                    for (int x = 0; x < Size.X; x++) {
                        builder.Append(debugChars.GetValueOrDefault(this[new Int2(x, y)], '?'));
                    }

                    builder.Append('\n');
                }

                return builder.ToString();
            }
        }

        public Map Copy() {
            Map map = new(new Tile[Size.X, Size.Y]);
            foreach (Int2 position in Positions()) {
                map[position] = this[position];
            }

            return map;
        }

        public override int GetHashCode() {
            HashCode hash = new();
            foreach (Int2 position in Positions()) {
                hash.Add(this[position]);
            }

            return hash.ToHashCode();
        }

        public bool Equals(Map other) {
            foreach (Int2 position in Positions()) {
                if (this[position] != other[position])
                    return false;
            }

            return true;
        }

        public override bool Equals(object?  obj)             => obj is Map other && Equals(other);
        public static   bool operator ==(Map left, Map right) => left.Equals(right);
        public static   bool operator !=(Map left, Map right) => !(left == right);
    }

    private static Dictionary<Tile, char> debugChars = new() {
        [Tile.EMPTY]   = '.',
        [Tile.ROUNDED] = 'O',
        [Tile.CUBE]    = '#'
    };

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

    private Int2[] tiltDirections = [
        new(+0, +1),
        new(-1, +0),
        new(+0, -1),
        new(+1, +0)
    ];

    private Map TiltCycle(Map map) {
        foreach (Int2 direction in tiltDirections) {
            map = Tilt(map, direction);
        }

        return map;
    }

    private Map Tilt(Map map, Int2 direction) {
        Map newMap = map.Copy();
        {
            foreach (Int2 p in newMap.Positions()) {
                if (newMap[p] == Tile.ROUNDED)
                    newMap[p] = Tile.EMPTY;
            }
        }

        Int2 iterationDirection     = -direction;
        Int2 iterationPerpendicular = new(iterationDirection.Y, iterationDirection.X);

        Int2 position = (iterationDirection >= 0).All
                            ? 0
                            : map.Size - 1;

        while (map.IsWithin(position)) {
            Int2 nextRoundedPosition = position;

            while (map.IsWithin(position)) {
                switch (map[position]) {
                    case Tile.EMPTY:
                        break;
                    case Tile.ROUNDED:
                        newMap[nextRoundedPosition] =  Tile.ROUNDED;
                        nextRoundedPosition         -= direction;
                        break;
                    case Tile.CUBE:
                        nextRoundedPosition = position - direction;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                position += iterationDirection;
            }

            position -= iterationDirection * map.Size;
            position += iterationPerpendicular;
        }

        return newMap;
    }

    private int Score(Map map) =>
        map.Positions()
           .Where(p => map[p] == Tile.ROUNDED)
           .Sum(p => p.Y + 1);

    protected override int Solve(Input input) {
        Dictionary<Map, int> seen = new();
        List<Map>            maps = new();

        Map map = input.Map;
        while (!seen.ContainsKey(map)) {
            seen.Add(map, maps.Count);
            maps.Add(map);
            map = TiltCycle(map);
        }

        int cycleStart  = seen[map];
        int cycleLength = maps.Count - cycleStart;

        const int tiltCycleCount = 1_000_000_000;

        return Score(maps[cycleStart + (tiltCycleCount - cycleStart) % cycleLength]);
    }

    public static void Run() {
        new Day14B().Solve();
    }
}