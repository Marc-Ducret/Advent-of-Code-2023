using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day19B : Problem<Day19B.Input, long> {
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
                if (rule.Interval.Contains(value)) {
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

    public readonly record struct Rule(Interval Interval, ITarget Target) {
        public bool Expandable() => Target is Workflow;

        public IEnumerable<Rule> ExpandOnce() {
            Rule source = this;
            return ((Workflow)Target).Rules
                                     .Select(r => Combine(source, r))
                                     .Where(r => !r.Interval.Empty);
        }

        public IEnumerable<Rule> FullyExpand() =>
            Expandable()
                ? ExpandOnce().SelectMany(r => r.FullyExpand())
                : new[] { this };

        private static Rule Combine(Rule source, Rule target) =>
            target with { Interval = Interval.Intersect(source.Interval, target.Interval) };
    }

    public readonly record struct Interval(Int4 Min, Int4 Max) {
        public bool Contains(Int4 value) =>
            (value >= Min & value <= Max).All;

        public bool Contains(in Interval interval) =>
            (interval.Min >= Min & interval.Max <= Max).All;

        public bool Empty => (Max < Min).Any;

        public long Size => Empty
                                ? 0
                                : Long4.CMul(Max - Min + 1);

        public static Interval Intersect(in Interval lhs, in Interval rhs) =>
            new(Int4.Max(lhs.Min, rhs.Min),
                Int4.Min(lhs.Max, rhs.Max));
    }

    public class SetCount {
        private Dictionary<Interval, long> Positive { get; } = [];
        private Dictionary<Interval, long> Negative { get; } = [];

        public void Add(Interval interval) {
            if (Positive.Keys.Any(i => i.Contains(interval)))
                return;
            (long, Interval)[] newPositive = Negative.Select(i => (i.Value, Interval.Intersect(i.Key, interval)))
                                                     .Where(i => !i.Item2.Empty)
                                                     .Append((1, interval))
                                                     .ToArray();
            (long, Interval)[] newNegative = Positive.Select(i => (i.Value, Interval.Intersect(i.Key, interval)))
                                                     .Where(i => !i.Item2.Empty)
                                                     .ToArray();
            foreach (var (count, i) in newPositive) {
                if (Positive.TryGetValue(i, out long actualCount)) {
                    Positive[i] = actualCount + count;
                } else {
                    Positive.Add(i, count);
                }
            }

            foreach (var (count, i) in newNegative) {
                if (Negative.TryGetValue(i, out long actualCount)) {
                    Negative[i] = actualCount + count;
                } else {
                    Negative.Add(i, count);
                }
            }

            foreach (Interval key in Negative.Keys) {
                if (Positive.TryGetValue(key, out long positive)) {
                    long negative = Negative[key];
                    long common   = Math.Min(positive, negative);
                    if (positive == common) Positive.Remove(key);
                    else Positive[key] -= common;
                    if (negative == common) Negative.Remove(key);
                    else Negative[key] -= common;
                }
            }
        }

        public long CountIn(Interval interval) =>
            Positive.Sum(i => i.Value * Interval.Intersect(i.Key, interval).Size)
          - Negative.Sum(i => i.Value * Interval.Intersect(i.Key, interval).Size);
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

            const int minValue = 1;
            const int maxValue = 4_000;

            Int4 min = minValue;
            Int4 max = maxValue;
            if (match.Groups["selector"].Success) {
                Int4 selector = match.Groups["selector"].Value switch {
                    "x" => new Int4 { X = 1 },
                    "m" => new Int4 { Y = 1 },
                    "a" => new Int4 { Z = 1 },
                    "s" => new Int4 { W = 1 },
                    _   => throw new ArgumentOutOfRangeException()
                };
                int threshold = int.Parse(match.Groups["threshold"].Value);

                if (match.Groups["operator"].Value == "<") {
                    max = (threshold - 1) * selector
                        + maxValue        * (1 - selector);
                } else {
                    min = (threshold + 1) * selector
                        + minValue        * (1 - selector);
                }
            }

            return new Rule(new Interval(min, max),
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

    protected override long Solve(Input input) {
        Rule[] rules = input.In.Rules.SelectMany(r => r.FullyExpand()).ToArray();

        SetCount alreadyMapped = new();

        long sum = 0;

        foreach (Rule rule in rules) {
            if (rule.Target is Accept) {
                sum += rule.Interval.Size - alreadyMapped.CountIn(rule.Interval);
            }

            alreadyMapped.Add(rule.Interval);
        }

        return sum;
    }

    public static void Run() {
        new Day19B().Solve();
    }

    [GeneratedRegex(@"(?<name>[\w]+){(?<rules>[^}]+)}")]
    private static partial Regex WorkflowRegex();

    [GeneratedRegex(@"((?<selector>[xmas])(?<operator>[<>])(?<threshold>[\d]+):)?(?<target>.+)")]
    private static partial Regex RuleRegex();

    [GeneratedRegex(@"{x=(?<x>[\d]+),m=(?<m>[\d]+),a=(?<a>[\d]+),s=(?<s>[\d]+)}")]
    private static partial Regex GearRegex();
}