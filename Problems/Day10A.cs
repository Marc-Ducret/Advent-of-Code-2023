namespace Advent_of_Code_2023;

public class Day10A : Problem<Day10A.Input, int> {
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

    [Flags]
    public enum Tile {
        EMPTY    = 0b000000,
        STARTING = 0b000001,
        EAST     = 0b000010,
        NORTH    = 0b000100,
        WEST     = 0b001000,
        SOUTH    = 0b010000,
        PIPE_NS  = NORTH | SOUTH,
        PIPE_NE  = NORTH | EAST,
        PIPE_NW  = NORTH | WEST,
        PIPE_EW  = EAST  | WEST,
        PIPE_SE  = SOUTH | EAST,
        PIPE_SW  = SOUTH | WEST
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

    public struct Follower {
        public Map  map;
        public Int2 position;
        public Tile direction;

        public Tile Current => map[position];

        public void FindStart() {
            for (int y = 0; y < map.Size.Y; y++)
            for (int x = 0; x < map.Size.X; x++) {
                position = new Int2(x, y);
                if (Current == Tile.STARTING) {
                    foreach (Tile direction in directionOffset.Keys) {
                        Tile neighbor = map[position + directionOffset[direction]];
                        if ((neighbor & directionOpposite[direction]) != Tile.EMPTY) {
                            this.direction = direction;
                            return;
                        }
                    }

                    throw new Exception("No pipe connected to starting position found.");
                }
            }

            throw new Exception("No starting position found.");
        }

        public void Follow() {
            position  += directionOffset[direction];
            direction =  Current & ~ directionOpposite[direction];
        }
    }

    protected override int Solve(Input input) {
        Follower follower = new() {
            map = input.Map
        };
        follower.FindStart();

        int step = 0;
        do {
            follower.Follow();
            step++;
        } while (follower.Current != Tile.STARTING);

        return step / 2;
    }

    public static void Run() {
        new Day10A().Solve();
    }
}