using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day19A : Problem<Day19A.Input, int> {
    public readonly record struct Input(Workflow In, Int4[] Gears);

    public interface ITarget;

    public class Reject : ITarget {
        public static readonly Reject INSTANCE = new();
    }

    public class Accept : ITarget {
        public static readonly Accept INSTANCE = new();
    }

    public record Workflow(string Name) : ITarget {
        public List<Rule> Rules { get; } = [];

        public bool Accepts(Int4 value) {
            foreach (Rule rule in Rules) {
                if (rule.Matches(value)) {
                    return rule.Target switch {
                        Accept            => true,
                        Reject            => false,
                        Workflow workflow => workflow.Accepts(value),
                        _                 => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            throw new Exception("No matching rule");
        }
    }

    public readonly record struct Rule(Int4 Factors, int Threshold, ITarget Target) {
        public bool Matches(Int4 value) =>
            Int4.CSum(value * Factors) > Threshold;
    }

    protected override Input PreProcess(string input) {
        Dictionary<string, Workflow> workflowByName = [];

        string[] parts = input.Split("\n\n");
        foreach (string line in parts[0].Split('\n')) {
            ParseWorkflow(line);
        }

        return new Input(workflowByName["in"], parts[1].Split('\n').Select(ParseGear).ToArray());

        Workflow Workflow(string name) {
            if (workflowByName.TryGetValue(name, out Workflow? workflow))
                return workflow;
            workflow = new Workflow(name);
            workflowByName.Add(name, workflow);
            return workflow;
        }

        Rule ParseRule(string rule) {
            Match match = RuleRegex().Match(rule);
            if (!match.Success) throw new ArgumentException("Invalid rule.");

            Int4 factors;
            int  threshold;
            if (match.Groups["selector"].Success) {
                factors = match.Groups["selector"].Value switch {
                    "x" => new Int4 { X = 1 },
                    "m" => new Int4 { Y = 1 },
                    "a" => new Int4 { Z = 1 },
                    "s" => new Int4 { W = 1 },
                    _   => throw new ArgumentOutOfRangeException()
                };
                threshold = int.Parse(match.Groups["threshold"].Value);

                if (match.Groups["operator"].Value == "<") {
                    factors   = -factors;
                    threshold = -threshold;
                }
            } else {
                factors   = 0;
                threshold = -1;
            }

            return new Rule(factors, threshold,
                            match.Groups["target"].Value switch {
                                "A"          => Accept.INSTANCE,
                                "R"          => Reject.INSTANCE,
                                { } workflow => Workflow(workflow)
                            });
        }

        Workflow ParseWorkflow(string line) {
            Match match = WorkflowRegex().Match(line);
            if (!match.Success) throw new ArgumentException("Invalid workflow.");

            Workflow workflow = Workflow(match.Groups["name"].Value);

            foreach (string rule in match.Groups["rules"].Value.Split(',')) {
                workflow.Rules.Add(ParseRule(rule));
            }

            return workflow;
        }

        Int4 ParseGear(string line) {
            Match match = GearRegex().Match(line);
            if (!match.Success) throw new ArgumentException("Invalid gear.");

            return new Int4(
                int.Parse(match.Groups["x"].Value),
                int.Parse(match.Groups["m"].Value),
                int.Parse(match.Groups["a"].Value),
                int.Parse(match.Groups["s"].Value)
            );
        }
    }

    protected override int Solve(Input input) =>
        input.Gears
             .Where(input.In.Accepts)
             .Sum(gear => Int4.CSum(gear));

    public static void Run() {
        new Day19A().Solve();
    }

    [GeneratedRegex(@"(?<name>[\w]+){(?<rules>[^}]+)}")]
    private static partial Regex WorkflowRegex();

    [GeneratedRegex(@"((?<selector>[xmas])(?<operator>[<>])(?<threshold>[\d]+):)?(?<target>.+)")]
    private static partial Regex RuleRegex();

    [GeneratedRegex(@"{x=(?<x>[\d]+),m=(?<m>[\d]+),a=(?<a>[\d]+),s=(?<s>[\d]+)}")]
    private static partial Regex GearRegex();
}