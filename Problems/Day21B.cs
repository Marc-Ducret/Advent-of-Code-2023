namespace Advent_of_Code_2023;

public class Day21B : Problem<Day21B.Input, long> {
    public readonly record struct Input(int Steps, Grid<TileElement> Grid);

    public enum Tile {
        START,
        PLOT,
        ROCK
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

    protected override Input PreProcess(string input) {
        string[] parts = input.Split("\n\n");
        return new Input(int.Parse(parts[0]), Grid<TileElement>.Parse(parts[1]));
    }

    private struct Precomputed() {
        private          int       countOdd, countEven;
        private readonly List<int> countByDistance = [];

        public int CurrentDistance => countByDistance.Count - 1;

        public void IncrementDistance() {
            countByDistance.Add((CurrentDistance + 1) % 2 == 0
                                    ? countEven
                                    : countOdd);
        }

        public void AddForCurrentDistance() {
            countByDistance[^1]++;
            if (CurrentDistance % 2 == 0)
                countEven++;
            else
                countOdd++;
        }

        public bool IsFull(int distance) => distance >= countByDistance.Count;

        public int Get(int distance) {
            if (distance < 0)
                return 0;
            if (distance < countByDistance.Count)
                return countByDistance[distance];
            return distance % 2 == 0
                       ? countEven
                       : countOdd;
        }

        public int GetEven => countEven;
        public int GetOdd  => countOdd;
    }

    private Precomputed Precompute(in Grid<TileElement> grid, in Int2 startPosition) {
        Precomputed precomputed = new();

        HashSet<Int2>                        seen    = [];
        Queue<(Int2 position, int distance)> toVisit = new();
        toVisit.Enqueue((startPosition, 0));
        seen.Add(startPosition);

        while (toVisit.TryDequeue(out var element)) {
            if (element.distance > precomputed.CurrentDistance)
                precomputed.IncrementDistance();
            if (element.distance != precomputed.CurrentDistance)
                throw new Exception("Precompute failure");

            precomputed.AddForCurrentDistance();

            foreach (Int2 offset in offsets) {
                Int2 neighbor = element.position + offset;
                if (!grid.IsWithin(neighbor)) continue;
                if (!seen.Add(neighbor)) continue;
                if (grid[neighbor] == Tile.ROCK) continue;
                toVisit.Enqueue((neighbor, element.distance + 1));
            }
        }

        return precomputed;
    }

    private static Int2[] offsets = [
        new Int2(+1, +0),
        new Int2(+0, +1),
        new Int2(-1, +0),
        new Int2(+0, -1)
    ];

    protected override long Solve(Input input) {
        Int2 startPosition = -1;
        foreach (Int2 position in input.Grid.Positions()) {
            if (input.Grid[position] == Tile.START)
                startPosition = position;
        }

        Dictionary<Int2, Precomputed> cache = new();

        long sum = 0;

        Int2 maxGridOffset = input.Steps / input.Grid.Size + 1;
        for (int y = -maxGridOffset.Y; y <= maxGridOffset.Y; y++) {
            // for (int x = -maxGridOffset.X; x <= maxGridOffset.X; x++) {
            //     Int2 gridOffset        = new(x, y);
            //     Int2 localEntryPoint   = LocalEntryPoint(gridOffset);
            //     int  gridEntryDistance = GridEntryDistance(gridOffset);
            //     int  distance          = input.Steps - gridEntryDistance;
            //     int  count             = Precomputed(localEntryPoint).Get(distance);
            //     sum += count;
            // }
            int verticalDistance = GridEntryDistance(new Int2(0, y));

            {
                int         x           = -(input.Steps - verticalDistance) / input.Grid.Size.X - 1;
                Int2        localEntry  = LocalEntryPoint(new Int2(x, y));
                Precomputed precomputed = Precomputed(localEntry);

                while (x < 0) {
                    int distance = input.Steps - GridEntryDistance(new Int2(x, y));
                    if (precomputed.IsFull(distance)) break;

                    sum += precomputed.Get(distance);
                    x++;
                }


                sum += (-x / 2) * precomputed.Get(input.Steps - GridEntryDistance(new Int2(-2, y)))
                     + ((-x + 1) / 2) * precomputed.Get(input.Steps - GridEntryDistance(new Int2(-1, y)));
            }

            {
                sum += Precomputed(LocalEntryPoint(new Int2(0, y)))
                   .Get(input.Steps - GridEntryDistance(new Int2(0, y)));
            }

            {
                int         x           = (input.Steps - verticalDistance) / input.Grid.Size.X + 1;
                Int2        localEntry  = LocalEntryPoint(new Int2(x, y));
                Precomputed precomputed = Precomputed(localEntry);

                while (x > 0) {
                    int distance = input.Steps - GridEntryDistance(new Int2(x, y));
                    if (precomputed.IsFull(distance)) break;

                    sum += precomputed.Get(distance);
                    x--;
                }

                sum += (x / 2) * precomputed.Get(input.Steps - GridEntryDistance(new Int2(+2, y)))
                     + ((x + 1) / 2) * precomputed.Get(input.Steps - GridEntryDistance(new Int2(+1, y)));
            }
        }

        return sum;

        Precomputed Precomputed(in Int2 position) {
            if (!cache.TryGetValue(position, out Precomputed result)) {
                result = Precompute(input.Grid, position);
                cache.Add(position, result);
            }

            return result;
        }

        Int2 LocalEntryPoint(Int2 gridOffset) =>
            Int2.Eq(gridOffset, 0) * startPosition
          + (gridOffset > 0)       * (Int2)0
          + (gridOffset < 0)       * (input.Grid.Size - 1);

        int GridEntryDistance(Int2 gridOffset) =>
            Int2.CSum(
                Int2.Max(Int2.Abs(gridOffset) - 1, 0) * input.Grid.Size
              + (gridOffset > 0)                      * (input.Grid.Size - startPosition)
              + (gridOffset < 0)                      * (startPosition   + 1));
    }

    public static void Run() {
        new Day21B().Solve();
    }
}