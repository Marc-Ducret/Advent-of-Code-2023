using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day05B : Problem<Day05B.Input, long> {
    public readonly record struct Input(Input.Range[] Seeds, Input.Mapping[] Mappings) {
        public readonly struct Mapping {
            public Mapping(IEnumerable<RangeMap> rangeMaps) {
                RangeMaps = rangeMaps.OrderBy(rangeMap => rangeMap.FromStart).ToArray();
            }

            public IEnumerable<Range> Map(in Range range) {
                List<Range> ranges = new();

                long mapEnd = range.Start;
                foreach (RangeMap rangeMap in RangeMaps) {
                    Range fromIntersect = rangeMap.From.Intersect(range);

                    if (fromIntersect.Empty) continue;

                    if (fromIntersect.Start > mapEnd)
                        ranges.Add(new Range(mapEnd, fromIntersect.Start - mapEnd));

                    ranges.Add(rangeMap.Map(fromIntersect));
                    mapEnd = fromIntersect.End;
                }

                if (range.End > mapEnd)
                    ranges.Add(new Range(mapEnd, range.End - mapEnd));

                return ranges;
            }

            private RangeMap[] RangeMaps { get; }
        }

        public readonly record struct Range(long Start, long Count) {
            public long End => Start + Count;

            public bool Empty => Count == 0;

            public Range Intersect(in Range that) {
                long start = Math.Max(Start, that.Start);
                long end   = Math.Min(End, that.End);
                if (end <= start) return new Range(Start, 0);
                return new Range(start, end - start);
            }
        }

        public readonly record struct RangeMap(long ToStart, long FromStart, long Count) {
            public Range From => new(FromStart, Count);
            public Range To   => new(ToStart, Count);

            public long Map(long value) => ToStart + (value - FromStart);

            public Range Map(in Range range) => range with { Start = Map(range.Start) };
        }

        public IEnumerable<Range> Map(IEnumerable<Range> ranges) {
            foreach (Mapping mapping in Mappings) {
                ranges = ranges.SelectMany(range => mapping.Map(range));
            }

            return ranges;
        }
    }

    protected override Input PreProcess(string input) {
        string[] blocks = input.Split("\n\n");

        Input.Range[] seeds;
        {
            long[] numbers = Numbers(blocks[0]);
            seeds = new Input.Range[numbers.Length / 2];
            for (int i = 0; i < seeds.Length; i++) {
                seeds[i] = new Input.Range(numbers[2 * i + 0], numbers[2 * i + 1]);
            }
        }

        return new Input(
            seeds,
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
        return input.Map(input.Seeds).Min(range => range.Start);
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    public static void Run() {
        new Day05B().Solve();
    }
}