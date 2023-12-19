using System.Globalization;
using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day18B : Problem<Day18B.Input, long> {
    public readonly record struct Input(Instruction[] Instructions);

    public readonly record struct Instruction(Direction Direction, int Count);

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

    private static Dictionary<Direction, Direction> directionOpposite = new() {
        [Direction.EAST]  = Direction.WEST,
        [Direction.NORTH] = Direction.SOUTH,
        [Direction.WEST]  = Direction.EAST,
        [Direction.SOUTH] = Direction.NORTH
    };

    protected override Input PreProcess(string input) {
        return new Input(input.Split('\n').Select(ParseInstruction).ToArray());

        Instruction ParseInstruction(string line) {
            Match match = InstructionRegex().Match(line);
            if (!match.Success) throw new Exception("Invalid instruction");

            uint color = uint.Parse(match.Groups["color"].Value, NumberStyles.HexNumber);

            return new Instruction(
                (color & 0xF) switch {
                    0 => Direction.EAST,
                    1 => Direction.SOUTH,
                    2 => Direction.WEST,
                    3 => Direction.NORTH,
                    _ => throw new ArgumentOutOfRangeException()
                },
                (int)(color >> 4)
            );
        }
    }

    private IEnumerable<Int2> Vertices(IEnumerable<Instruction> instructions) {
        Int2 position = 0;
        foreach (Instruction instruction in instructions) {
            yield return position += instruction.Count * directionOffset[instruction.Direction];
        }
    }

    private readonly record struct Edge(Int2 Start, Int2 End, Direction Direction);

    private IEnumerable<Edge> Edges(IEnumerable<Instruction> instructions) {
        Int2 position = 0;
        foreach (Instruction instruction in instructions) {
            Edge edge = new(position,
                            position + instruction.Count * directionOffset[instruction.Direction],
                            instruction.Direction);
            yield return edge;
            position = edge.End;
        }
    }

    private record struct GridElement(bool Dug) : IGridElement {
        public char DebugChar() => Dug ? '#' : '.';

        public void Parse(char c) {
            Dug = c == '#';
        }
    }

    private record struct DirectionElement(Direction Value) : IGridElement {
        public char DebugChar() => Value switch {
            Direction.EMPTY                   => ' ',
            Direction.NORTH | Direction.SOUTH => '║',
            Direction.NORTH | Direction.EAST  => '╚',
            Direction.NORTH | Direction.WEST  => '╝',
            Direction.WEST  | Direction.EAST  => '═',
            Direction.SOUTH | Direction.EAST  => '╔',
            Direction.SOUTH | Direction.WEST  => '╗',
            _                                 => '?'
        };

        public void Parse(char c) {
            throw new NotImplementedException();
        }

        public static implicit operator Direction(DirectionElement element) => element.Value;
        public static implicit operator DirectionElement(Direction value)   => new(value);
    }

    protected override long Solve(Input input) {
        int[] xSteps = Vertices(input.Instructions).Select(p => p.X).Distinct().Order().ToArray();
        int[] ySteps = Vertices(input.Instructions).Select(p => p.Y).Distinct().Order().ToArray();

        Dictionary<int, int> xToIndex = xSteps.Select((x, index) => (x, index)).ToDictionary();
        Dictionary<int, int> yToIndex = ySteps.Select((y, index) => (y, index)).ToDictionary();

        Grid<DirectionElement> pathGrid = new(new Int2(xSteps.Length, ySteps.Length));

        List<Int2> stepIndexes = [];

        foreach (Edge edge in Edges(input.Instructions)) {
            GetStepIndexes(edge);
            for (int i = 0; i < stepIndexes.Count - 1; i++)
                pathGrid[stepIndexes[i]] |= edge.Direction;
            Direction oppositeDirection = directionOpposite[edge.Direction];
            for (int i = 1; i < stepIndexes.Count; i++)
                pathGrid[stepIndexes[i]] |= oppositeDirection;
        }

        long sum = 0;

        // Inside
        for (int y = 0; y < pathGrid.Size.Y - 1; y++) {
            bool inside = false;
            for (int x = 0; x < pathGrid.Size.X - 1; x++) {
                Int2 index = new(x, y);

                if ((pathGrid[index] & Direction.NORTH) != Direction.EMPTY)
                    inside = !inside;


                if (inside) {
                    Int2 cellSize = GetStep(index + 1) - GetStep(index);

                    sum += (cellSize.X - 1) * (long)(cellSize.Y - 1);
                }
            }
        }

        // Horizontal edges
        for (int y = 0; y < pathGrid.Size.Y; y++) {
            bool northInside = false;
            bool southInside = false;
            bool inside      = false;
            for (int x = 0; x < pathGrid.Size.X; x++) {
                Int2 index = new(x, y);

                bool wasInside = inside; 

                if ((pathGrid[index] & Direction.NORTH) != Direction.EMPTY)
                    northInside = !northInside;
                if ((pathGrid[index] & Direction.SOUTH) != Direction.EMPTY)
                    southInside = !southInside;

                inside = northInside || southInside;
                if (wasInside && !inside)
                    sum += 1;

                if (inside) {
                    sum += xSteps[x + 1] - xSteps[x];
                }
            }
        }

        // Vertical edges
        for (int x = 0; x < pathGrid.Size.X; x++) {
            bool eastInside = false;
            bool westInside = false;
            bool inside      = false;
            for (int y = 0; y < pathGrid.Size.Y; y++) {
                Int2 index = new(x, y);

                bool wasInside = inside; 

                if ((pathGrid[index] & Direction.EAST) != Direction.EMPTY)
                    eastInside = !eastInside;
                if ((pathGrid[index] & Direction.WEST) != Direction.EMPTY)
                    westInside = !westInside;

                inside = eastInside || westInside;

                if (inside) {
                    sum += ySteps[y + 1] - ySteps[y] - 1;
                }
            }
        }

        return sum;

        Int2 GetStepIndex(Int2 position) => new(xToIndex[position.X], yToIndex[position.Y]);
        Int2 GetStep(Int2      index)    => new(xSteps[index.X], ySteps[index.Y]);

        void GetStepIndexes(Edge edge) {
            stepIndexes.Clear();
            Int2 startIndex = GetStepIndex(edge.Start);
            Int2 endIndex   = GetStepIndex(edge.End);

            Int2 offset = directionOffset[edge.Direction];

            Int2 index = startIndex;
            stepIndexes.Add(index);
            while (index != endIndex) {
                index += offset;
                stepIndexes.Add(index);
            }
        }
    }

    public static void Run() {
        new Day18B().Solve();
    }

    [GeneratedRegex(@"(?<direction>[UDRL]) (?<count>\d+) \(#(?<color>[a-f0-9]+)\)")]
    private static partial Regex InstructionRegex();
}