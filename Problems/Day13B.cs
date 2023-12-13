namespace Advent_of_Code_2023;

public class Day13B : Problem<Day13B.Input, long> {
    public readonly record struct Input(Pattern[] Patterns);

    public record struct Pattern(Line[] Rows, Line[] Columns) {
        public Int2 Size => new(Columns.Length, Rows.Length);

        public void Swap(Int2 position) {
            Rows[position.Y][position.X]    = !Rows[position.Y][position.X];
            Columns[position.X][position.Y] = !Columns[position.X][position.Y];
        }
    }

    public record struct Line(ulong Value) {
        public bool this[int index] {
            get {
                ulong mask = 1UL << index;
                return (Value & mask) == mask;
            }
            set {
                ulong mask = 1UL << index;
                Value = (Value & ~mask) | (value ? mask : 0UL);
            }
        }

        public static Line FromCollection(IEnumerable<bool> values) {
            Line line = default;
            foreach ((bool value, int index) in values.Select((value, index) => (value, index))) {
                line[index] = value;
            }

            return line;
        }
    }

    protected override Input PreProcess(string input) {
        Pattern ParsePattern(string pattern) {
            string[] lines = pattern.Split('\n');
            Int2     size  = new(lines.Min(l => l.Length), lines.Length);
            bool[,]  grid  = new bool[size.X, size.Y];

            for (int y = 0; y < size.Y; y++)
            for (int x = 0; x < size.X; x++) {
                grid[x, y] = lines[y][x] != '.';
            }

            return new Pattern(
                Enumerable.Range(0, size.Y)
                          .Select(y =>
                                      Line.FromCollection(
                                          Enumerable.Range(0, size.X).Select(x => grid[x, y])
                                      )
                           ).ToArray(),
                Enumerable.Range(0, size.X)
                          .Select(x =>
                                      Line.FromCollection(
                                          Enumerable.Range(0, size.Y).Select(y => grid[x, y])
                                      )
                           ).ToArray()
            );
        }

        return new Input(
            input.Split("\n\n").Select(ParsePattern).ToArray()
        );
    }

    private int SolvePattern(Pattern pattern) {
        if (FindPatternReflection(pattern) is not { } unfixedReflection)
            throw new Exception("No reflection found before fixing.");

        for (int y = 0; y < pattern.Size.Y; y++)
        for (int x = 0; x < pattern.Size.X; x++) {
            Int2 position = new(x, y);
            pattern.Swap(position);
            int? reflectionOpt = FindPatternReflection(pattern, differentOf: unfixedReflection);
            pattern.Swap(position);

            if (reflectionOpt is { } reflection)
                return reflection;
        }

        throw new Exception("No reflection found.");

        static int? FindPatternReflection(Pattern pattern, int differentOf = 0) {
            if (FindReflection(pattern.Rows, differentOf / 100) is { } rowReflection) {
                return 100 * rowReflection;
            }

            if (FindReflection(pattern.Columns, differentOf) is { } columnReflection) {
                return columnReflection;
            }

            return null;
        }

        static int? FindReflection(Line[] lines, int differentOf = 0) {
            for (int i = 1; i < lines.Length; i++) {
                if (i == differentOf) continue;
                bool reflects = true;
                int  offset   = 1;
                while (true) {
                    int before = i          - offset;
                    int after  = i + offset - 1;

                    if (before < 0) break;
                    if (after  >= lines.Length) break;

                    if (lines[before] != lines[after]) {
                        reflects = false;
                        break;
                    }

                    offset++;
                }

                if (reflects)
                    return i;
            }

            return null;
        }
    }

    protected override long Solve(Input input) {
        return input.Patterns.Select(SolvePattern).Sum();
    }

    public static void Run() {
        new Day13B().Solve();
    }
}