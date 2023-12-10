using System.Text;

namespace Advent_of_Code_2023;

public class Day10B : Problem<Day10B.Input, int> {
    public readonly record struct Input(Map Map);

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
    }

    [Flags]
    public enum Tile {
        EMPTY    = 0,
        STARTING = 1 << 1,
        EAST     = 1 << 2,
        NORTH    = 1 << 3,
        WEST     = 1 << 4,
        SOUTH    = 1 << 5,
        PIPE_NS  = NORTH | SOUTH,
        PIPE_NE  = NORTH | EAST,
        PIPE_NW  = NORTH | WEST,
        PIPE_EW  = EAST  | WEST,
        PIPE_SE  = SOUTH | EAST,
        PIPE_SW  = SOUTH | WEST,
        DEBUG    = 1 << 6
    }

    private static Dictionary<Tile, Int2> directionOffset = new() {
        [Tile.EAST]  = new Int2(+1, +0),
        [Tile.NORTH] = new Int2(+0, +1),
        [Tile.WEST]  = new Int2(-1, +0),
        [Tile.SOUTH] = new Int2(+0, -1)
    };

    private static Dictionary<Tile, Tile> directionOpposite = new() {
        [Tile.EAST]  = Tile.WEST,
        [Tile.NORTH] = Tile.SOUTH,
        [Tile.WEST]  = Tile.EAST,
        [Tile.SOUTH] = Tile.NORTH
    };

    protected override Input PreProcess(string input) {
        Dictionary<char, Tile> parseTile = new() {
            ['.'] = Tile.EMPTY,
            ['S'] = Tile.STARTING,
            ['|'] = Tile.PIPE_NS,
            ['L'] = Tile.PIPE_NE,
            ['J'] = Tile.PIPE_NW,
            ['-'] = Tile.PIPE_EW,
            ['F'] = Tile.PIPE_SE,
            ['7'] = Tile.PIPE_SW
        };

        string[] lines = input.Split('\n');
        Tile[,]  tiles = new Tile[lines.Min(l => l.Length), lines.Length];

        for (int y = 0; y < tiles.GetLength(1); y++)
        for (int x = 0; x < tiles.GetLength(0); x++) {
            tiles[x, y] = parseTile[lines[tiles.GetLength(1) - y - 1][x]];
        }

        return new Input(new Map(tiles));
    }

    private static Dictionary<Tile, char> debugChars = new() {
        [Tile.EMPTY]    = '.',
        [Tile.STARTING] = 'S',
        [Tile.EAST]     = '→',
        [Tile.NORTH]    = '↑',
        [Tile.WEST]     = '←',
        [Tile.SOUTH]    = '↓',
        [Tile.PIPE_NS]  = '║',
        [Tile.PIPE_NE]  = '╚',
        [Tile.PIPE_NW]  = '╝',
        [Tile.PIPE_EW]  = '═',
        [Tile.PIPE_SE]  = '╔',
        [Tile.PIPE_SW]  = '╗',
        [Tile.DEBUG]    = '×'
    };

    public struct Follower {
        private Map  map;
        private Int2 position;
        private Map  directionMap;
        private Map  loopMap;
        private Tile direction;

        public Follower(Map map) {
            this.map     = map;
            directionMap = new Map(new Tile[map.Size.X, map.Size.Y]);
            loopMap      = new Map(new Tile[map.Size.X, map.Size.Y]);
            FindStart();
        }

        public Tile Current => map[Position];

        private Int2 Position {
            get => position;
            set {
                position          =  value;
                loopMap[position] |= directionOpposite[Direction];
            }
        }

        private Tile Direction {
            get => direction;
            set {
                direction              =  value;
                loopMap[position]      |= direction;
                directionMap[position] =  direction;
            }
        }

        private void FindStart() {
            for (int y = 0; y < map.Size.Y; y++)
            for (int x = 0; x < map.Size.X; x++) {
                position = new Int2(x, y);
                if (Current == Tile.STARTING) {
                    foreach (Tile direction in directionOffset.Keys) {
                        Tile neighbor = map[position + directionOffset[direction]];
                        if ((neighbor & directionOpposite[direction]) != Tile.EMPTY) {
                            Direction = direction;
                            return;
                        }
                    }

                    throw new Exception("No pipe connected to starting position found.");
                }
            }

            throw new Exception("No starting position found.");
        }

        public void Follow() {
            Position += directionOffset[Direction];

            Direction =
                Current == Tile.STARTING
                    ? directionMap[Position]
                    : Current & ~ directionOpposite[Direction];
        }

        public Tile LoopTile(Int2 p) => loopMap[p];
    }

    protected override int Solve(Input input) {
        Follower follower = new(input.Map);

        do {
            follower.Follow();
        } while (follower.Current != Tile.STARTING);

        bool[] previousRowCornerIn = new bool[input.Map.Size.X - 1];
        bool[] currentRowCornerIn  = new bool[input.Map.Size.X - 1];

        Map debugInsideMap = new(new Tile[input.Map.Size.X, input.Map.Size.Y]);
        for (int y = 0; y < input.Map.Size.Y; y++)
        for (int x = 0; x < input.Map.Size.X; x++) {
            Int2 position = new(x, y);
            debugInsideMap[position] = follower.LoopTile(position);
        }

        int insideCount = 0;
        for (int y = 0; y < input.Map.Size.Y - 1; y++) {
            bool cornerIn = false;
            for (int x = 0; x < input.Map.Size.X - 1; x++) {
                Int2 position = new(x, y);

                if ((follower.LoopTile(position) & Tile.NORTH) != Tile.EMPTY)
                    cornerIn = !cornerIn;

                currentRowCornerIn[x] = cornerIn;

                if (x > 0
                 && currentRowCornerIn[x]
                 && currentRowCornerIn[x - 1]
                 && previousRowCornerIn[x]
                 && previousRowCornerIn[x - 1]
                 && follower.LoopTile(position) == Tile.EMPTY) {
                    insideCount++;
                    debugInsideMap[position] = Tile.DEBUG;
                }
            }

            (previousRowCornerIn, currentRowCornerIn) = (currentRowCornerIn, previousRowCornerIn);
        }

        return insideCount;
    }

    public static void Run() {
        new Day10B().Solve();
    }
}