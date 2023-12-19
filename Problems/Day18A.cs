using System.Globalization;
using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day18A : Problem<Day18A.Input, int> {
    public readonly record struct Input(Instruction[] Instructions);

    public readonly record struct Instruction(Direction Direction, int Count, uint Color);

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

    protected override Input PreProcess(string input) {
        return new Input(input.Split('\n').Select(ParseInstruction).ToArray());

        Instruction ParseInstruction(string line) {
            Match match = InstructionRegex().Match(line);
            if (!match.Success) throw new Exception("Invalid instruction");

            return new Instruction(
                match.Groups["direction"].Value switch {
                    "U" => Direction.NORTH,
                    "D" => Direction.SOUTH,
                    "R" => Direction.EAST,
                    "L" => Direction.WEST,
                    _   => throw new ArgumentOutOfRangeException()
                },
                int.Parse(match.Groups["count"].Value),
                uint.Parse(match.Groups["color"].Value, NumberStyles.HexNumber)
            );
        }
    }

    private IEnumerable<Int2> FollowInstructions(Int2 position, IEnumerable<Instruction> instructions) {
        foreach (Instruction instruction in instructions) {
            Int2 offset = directionOffset[instruction.Direction];
            for (int i = 0; i < instruction.Count; i++) {
                yield return position += offset;
            }
        }
    }

    private record struct GridElement(bool Dug) : IGridElement {
        public char DebugChar() => Dug ? '#' : '.';

        public void Parse(char c) {
            Dug = c == '#';
        }
    }

    private record struct Int2Element(Int2 Value) : IGridElement {
        public char DebugChar() => (char)('!' + Value.GetHashCode() % ('~' - '!'));

        public void Parse(char c) {
            throw new NotImplementedException();
        }

        public static implicit operator Int2(Int2Element element) => element.Value;
        public static implicit operator Int2Element(Int2 value)   => new(value);
    }

    private Grid<Int2Element> ConnectedComponents(Grid<GridElement> grid) {
        Grid<Int2Element> connectedComponents = new(grid.Size);
        Int2              unset               = -1;
        foreach (Int2 position in connectedComponents.Positions())
            connectedComponents[position] = unset;

        foreach (Int2 position in connectedComponents.Positions())
            Visit(position);

        return connectedComponents;

        void Visit(Int2 component) {
            if (connectedComponents[component].Value != unset) return;
            Stack<Int2> toVisit = new();
            toVisit.Push(component);

            while (toVisit.TryPop(out Int2 p)) {
                if (connectedComponents[p].Value != unset) continue;
                connectedComponents[p] = component;

                if (grid[p].Dug) continue;
                foreach (Int2 offset in directionOffset.Values) {
                    Int2 neighbor = p + offset;
                    if (!grid.IsWithin(neighbor)) continue;
                    if (grid[neighbor].Dug) continue;
                    toVisit.Push(neighbor);
                }
            }
        }
    }

    protected override int Solve(Input input) {
        Int2 start = 0;
        Int2 min   = start, max = start;

        foreach (Int2 position in FollowInstructions(start, input.Instructions)) {
            min = Int2.Min(min, position);
            max = Int2.Max(max, position);
        }

        Int2 size = max - min + 1;

        Grid<GridElement> grid = new(size);

        foreach (Int2 position in FollowInstructions(start, input.Instructions).Prepend(start))
            grid[position - min] = new GridElement(true);

        Grid<Int2Element> connectedComponents = ConnectedComponents(grid);

        HashSet<Int2> exteriorComponents = [];
        foreach (Int2 position in connectedComponents.Positions())
            if ((position <= 0).Any || (position >= connectedComponents.Size - 1).Any)
                exteriorComponents.Add(connectedComponents[position]);

        foreach (Int2 position in grid.Positions()) {
            if (grid[position].Dug) continue;
            if (exteriorComponents.Contains(connectedComponents[position])) continue;
            grid[position] = new GridElement(true);
        }

        return grid.Positions().Count(p => grid[p].Dug);
    }

    public static void Run() {
        new Day18A().Solve();
    }

    [GeneratedRegex(@"(?<direction>[UDRL]) (?<count>\d+) \(#(?<color>[a-f0-9]+)\)")]
    private static partial Regex InstructionRegex();
}