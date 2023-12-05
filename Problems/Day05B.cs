using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day05B : Problem<Day05B.Input, uint> {
    public readonly record struct Input(Input.Range[] Seeds, Input.Mapping[] Mappings) {
        public readonly struct Mapping {
            public Mapping(IEnumerable<RangeMap> rangeMaps) {
                RangeMaps = rangeMaps.OrderBy(rangeMap => rangeMap.FromStart).ToArray();
            }

            public IEnumerable<Range> Map(in Range range) {
                List<Range> ranges = new();

                uint mapEnd = range.Start;
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

        public readonly record struct Range(uint Start, uint Count) {
            public uint End => Start + Count;

            public bool Empty => Count == 0;

            public Range Intersect(in Range that) {
                uint start = Math.Max(Start, that.Start);
                uint end   = Math.Min(End, that.End);
                if (end <= start) return new Range(Start, 0);
                return new Range(start, end - start);
            }
        }

        public readonly record struct RangeMap(uint ToStart, uint FromStart, uint Count) {
            public Range From => new(FromStart, Count);
            public Range To   => new(ToStart, Count);

            public uint Map(uint value) => ToStart + (value - FromStart);

            public Range Map(in Range range) => range with { Start = Map(range.Start) };
        }

        public IEnumerable<Range> Map(IEnumerable<Range> ranges) {
            foreach (Mapping mapping in Mappings) {
                ranges = ranges.SelectMany(range => mapping.Map(range));
            }

            return ranges;
        }
    }

    protected override Input PreProcess(TextReader input) {
        string[] blocks = input.ReadToEnd().Split("\n\n");

        Input.Range[] seeds;
        {
            uint[] numbers = Numbers(blocks[0]);
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

        static uint[] Numbers(string s) => NumberRegex().Matches(s).Select(m => uint.Parse(m.Value)).ToArray();
    }

    protected override uint Solve(Input input) {
        return input.Map(input.Seeds).Min(range => range.Start);
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    public static void Run() {
        new Day05B().Solve();
    }
}