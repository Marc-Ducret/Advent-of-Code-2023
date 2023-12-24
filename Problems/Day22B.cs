namespace Advent_of_Code_2023;

public class Day22B : Problem<Day22B.Input, int> {
    public readonly record struct Input(Brick[] Bricks);

    public readonly record struct Brick(Int3 Start, Int3 Size) {
        public Int3 End => Start + Size - 1;

        public IEnumerable<Int2> StackPositions() {
            Int2 start = Start.xy;
            Int2 size  = Size.xy;
            return Enumerable.Range(start.Y, size.Y)
                             .SelectMany(
                                  y => Enumerable.Range(start.X, size.X)
                                                 .Select(x => new Int2(x, y)));
        }

        public static Brick FromStartEnd(Int3 start, Int3 end) => new(Int3.Min(start, end), Int3.Abs(start - end) + 1);
    }

    public readonly record struct Stack(int Height, int BrickId) : IGridElement {
        public char DebugChar()   => (char)('A' + BrickId);
        public void Parse(char c) => throw new NotImplementedException();
    }

    protected override Input PreProcess(string input) {
        return new Input(input.Split('\n').Select(ParseBrick).ToArray());

        Brick ParseBrick(string line) {
            string[] parts = line.Split('~');
            return Brick.FromStartEnd(ParseInt3(parts[0]), ParseInt3(parts[1]));
        }

        Int3 ParseInt3(string value) {
            string[] parts = value.Split(',');
            return new Int3(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
    }

    protected override int Solve(Input input) {
        Int2 gridSize = 0;
        foreach (Brick brick in input.Bricks)
            gridSize = Int2.Max(gridSize, Int2.Max(brick.Start.xy, brick.End.xy) + 1);

        Grid<Stack> stacks = new(gridSize);

        foreach (Int2 position in stacks.Positions()) {
            stacks[position] = new Stack(0, -1);
        }

        HashSet<int>[] supportsOf  = Enumerable.Range(0, input.Bricks.Length).Select(_ => new HashSet<int>()).ToArray();
        HashSet<int>[] supportedBy = Enumerable.Range(0, input.Bricks.Length).Select(_ => new HashSet<int>()).ToArray();
        int[] sortedIds = Enumerable.Range(0, input.Bricks.Length)
                                    .OrderBy(id => input.Bricks[id].Start.Z)
                                    .ToArray();

        int count = 0;

        foreach (int brickId in sortedIds) {
            int   zStart = 0;
            Brick brick  = input.Bricks[brickId];
            foreach (Int2 stackPosition in brick.StackPositions()) {
                zStart = Math.Max(zStart, stacks[stackPosition].Height);
            }

            foreach (Int2 stackPosition in brick.StackPositions()) {
                if (stacks[stackPosition].Height  == zStart
                 && stacks[stackPosition].BrickId >= 0) {
                    supportsOf[brickId].Add(stacks[stackPosition].BrickId);
                    supportedBy[stacks[stackPosition].BrickId].Add(brickId);
                }

                stacks[stackPosition] = new Stack(zStart + brick.Size.Z, brickId);
            }
        }

        int sum = 0;
        foreach (int brickId in sortedIds) {
            HashSet<int> fallen = [];
            Fall(fallen, brickId);
            sum += fallen.Count - 1;
        }

        return sum;

        void Fall(ISet<int> fallen, int brickId) {
            if (!fallen.Add(brickId)) return;

            foreach (int supported in supportedBy[brickId]) {
                if (supportsOf[supported].IsSubsetOf(fallen))
                    Fall(fallen, supported);
            }
        }
    }

    public static void Run() {
        new Day22B().Solve();
    }
}