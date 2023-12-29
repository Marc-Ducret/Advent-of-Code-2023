using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day05A : Problem<Day05A.Input, long> {
    public readonly record struct Input(long[] Seeds, Input.Mapping[] Mappings) {
        public readonly record struct Mapping(RangeMap[] RangeMaps) {
            public long Map(long value) {
                foreach (RangeMap rangeMap in RangeMaps) {
                    if (rangeMap.ContainsFrom(value))
                        return rangeMap.Map(value);
                }

                return value;
            }
        }

        public readonly record struct RangeMap(long ToStart, long FromStart, long Count) {
            public bool ContainsFrom(long value) => value >= FromStart && value < FromStart + Count;
            public long Map(long          value) => ToStart + (value                        - FromStart);
        }

        public long Map(long value) {
            foreach (Mapping mapping in Mappings) {
                value = mapping.Map(value);
            }

            return value;
        }
    }

    protected override Input PreProcess(string input) {
        string[] blocks = input.Split("\n\n");

        return new Input(
            Numbers(blocks[0]),
            blocks[1..]
               .Select(block =>
                           new Input.Mapping(
                               block.Split('\n').Select(Numbers)
                                    .Where(numbers => numbers.Length > 0)
                                    .Select(numbers => new Input.RangeMap(numbers[0], numbers[1], numbers[2]))
                                    .ToArray()
                           ))
               .ToArray()
        );

        static long[] Numbers(string s) => NumberRegex().Matches(s).Select(m => long.Parse(m.Value)).ToArray();
    }

    protected override long Solve(Input input) {
        return input.Seeds.Min(input.Map);
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    public static void Run() {
        new Day05A().Solve();
    }
}