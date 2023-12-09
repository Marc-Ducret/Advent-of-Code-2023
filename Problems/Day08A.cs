using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day08A : Problem<Day08A.Input, int> {
    public readonly record struct Input(
        Input.Instruction[] Instructions,
        Input.Node[]        Nodes,
        Input.Node          Start,
        Input.Node          End
    ) {
        public class Node {
            public Node? Left  { get; private set; }
            public Node? Right { get; private set; }

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

        return new Input(instructions, nodesByName.Values.ToArray(), nodesByName["AAA"], nodesByName["ZZZ"]);

        Input.Node Node(string name) {
            if (!nodesByName.TryGetValue(name, out Input.Node? node)) {
                node = new Input.Node();
                nodesByName.Add(name, node);
            }

            return node;
        }
    }

    protected override int Solve(Input input) {
        Input.Node node  = input.Start;
        int        steps = 0;
        while (node != input.End) {
            node = node.Follow(input.Instructions[steps % input.Instructions.Length]);
            steps++;
        }

        return steps;
    }

    public static void Run() {
        new Day08A().Solve();
    }

    [GeneratedRegex(@"([A-Z]+) = \(([A-Z]+), ([A-Z]+)\)")]
    private static partial Regex RowRegex();
}