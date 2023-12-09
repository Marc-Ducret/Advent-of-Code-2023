using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day08B : Problem<Day08B.Input, ulong> {
    public readonly record struct Input(
        Input.Instruction[] Instructions,
        Input.Node[]        Nodes
    ) {
        public class Node {
            public Node(bool isStart, bool isEnd, string name) {
                IsStart = isStart;
                IsEnd   = isEnd;
                Name    = name;
            }

            public bool   IsStart { get; }
            public bool   IsEnd   { get; }
            public string Name    { get; }
            public Node?  Left    { get; private set; }
            public Node?  Right   { get; private set; }

            public void Set(Node left, Node right) {
                Left  = left;
                Right = right;
            }

            public Node Follow(Instruction instruction) =>
                instruction switch {
                    Instruction.R => Right!,
                    Instruction.L => Left!,
                    _             => throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null)
                };

            public override string ToString() => Name;
        }

        public enum Instruction {
            R,
            L
        }
    }

    protected override Input PreProcess(string input) {
        Dictionary<string, Input.Node> nodesByName = new();
        
        string[] lines = input.Split('\n');

        Input.Instruction[] instructions =
            lines[0].Select(c => Enum.Parse<Input.Instruction>(c.ToString())).ToArray();

        foreach (string line in lines[1..]) {
            if (line.Length == 0) continue;
            Match match = RowRegex().Match(line);
            if (!match.Success) {
                throw new ArgumentException($"Could not parse: {line}");
            }

            Node(match.Groups[1].Value)
               .Set(Node(match.Groups[2].Value), Node(match.Groups[3].Value));
        }

        return new Input(instructions, nodesByName.Values.ToArray());

        Input.Node Node(string name) {
            if (!nodesByName.TryGetValue(name, out Input.Node? node)) {
                node = new Input.Node(name.EndsWith('A'), name.EndsWith('Z'), name);
                nodesByName.Add(name, node);
            }

            return node;
        }
    }

    private readonly record struct PathPattern(ulong[] Ends, ulong CycleStart, ulong CycleLength);

    private struct EndsEnumerator {
        private PathPattern pattern;

        private bool  foundFirstCyclicEnd;
        private int   firstCyclicEndIndex;
        private ulong cycle;
        private int   endIndex;

        public EndsEnumerator(PathPattern pattern) {
            this.pattern        = pattern;
            foundFirstCyclicEnd = Current >= pattern.CycleStart;
        }

        public bool NextGreaterOrEqual(ulong threshold) {
            while (!foundFirstCyclicEnd) {
                if (Current >= threshold)
                    return true;
                if (++endIndex >= pattern.Ends.Length)
                    return false;

                if (Current >= pattern.CycleStart) {
                    firstCyclicEndIndex = endIndex;
                    foundFirstCyclicEnd = true;
                }
            }

            if (Current >= threshold)
                return true;

            cycle += (threshold - Current) / pattern.CycleLength;

            while (Current < threshold) {
                if (++endIndex >= pattern.Ends.Length) {
                    endIndex = firstCyclicEndIndex;
                    cycle++;
                }
            }

            return true;
        }

        public ulong Current => pattern.Ends[endIndex] + pattern.CycleLength * cycle;
    }

    private PathPattern FindPattern(Input.Node startNode, Input.Instruction[] instructions) {
        List<ulong>                            ends = new();
        Dictionary<(Input.Node, ulong), ulong> seen = new();
        Input.Node                             node = startNode;
        ulong                                  step = 0;

        while (true) {
            ulong instructionsIndex = step % (ulong)instructions.Length;

            var state = (node, instructionsIndex);
            if (seen.TryGetValue(state, out ulong seenStep)) {
                return new PathPattern(ends.ToArray(), seenStep, step - seenStep);
            }

            if (node.IsEnd) ends.Add(step);
            seen.Add(state, step);

            node = node.Follow(instructions[instructionsIndex]);
            step++;
        }
    }

    private PathPattern Intersect(PathPattern patternA, PathPattern patternB) {
        ulong cycleStart  = Math.Max(patternA.CycleStart, patternB.CycleStart);
        ulong cycleLength = IntMath.LCM(patternA.CycleLength, patternB.CycleLength);

        List<ulong> ends = new();

        {
            EndsEnumerator endsA = new(patternA);
            EndsEnumerator endsB = new(patternB);

            endsA.NextGreaterOrEqual(0);
            endsB.NextGreaterOrEqual(0);

            while (Math.Min(endsA.Current, endsB.Current) < cycleStart + cycleLength) {
                if (endsA.Current == endsB.Current) {
                    ends.Add(endsA.Current);
                    if (!endsA.NextGreaterOrEqual(endsA.Current + 1)) break;
                    if (!endsB.NextGreaterOrEqual(endsA.Current)) break;
                } else if (endsA.Current < endsB.Current) {
                    if (!endsA.NextGreaterOrEqual(endsB.Current)) break;
                } else {
                    if (!endsB.NextGreaterOrEqual(endsA.Current)) break;
                }
            }
        }

        return new PathPattern(ends.ToArray(), cycleStart, cycleLength);
    }

    protected override ulong Solve(Input input) {
        PathPattern[] patterns = input.Nodes.Where(n => n.IsStart)
                                      .Select(startNode => FindPattern(startNode, input.Instructions))
                                      .ToArray();

        PathPattern pattern = patterns.Aggregate(Intersect);

        return pattern.Ends.First();
    }

    public static void Run() {
        new Day08B().Solve();
    }

    [GeneratedRegex(@"([\dA-Z]+) = \(([\dA-Z]+), ([\dA-Z]+)\)")]
    private static partial Regex RowRegex();
}