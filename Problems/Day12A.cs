namespace Advent_of_Code_2023;

public class Day12A : Problem<Day12A.Input, int> {
    public readonly record struct Input(Record[] Records);

    public struct Record(ArraySegment<Condition> conditions, ArraySegment<int> damagedGroups) {
        public ArraySegment<Condition> conditions    = conditions;
        public ArraySegment<int>       damagedGroups = damagedGroups;

        public override string ToString() =>
            string.Join("", conditions.Select(c => c switch {
                Condition.OPERATIONAL => '.',
                Condition.DAMAGED     => '#',
                Condition.UNKNOWN     => '?',
                _                     => throw new ArgumentOutOfRangeException(nameof(c), c, null)
            })) + " " + string.Join(',', damagedGroups);
    }

    public enum Condition {
        OPERATIONAL,
        DAMAGED,
        UNKNOWN
    }

    protected override Input PreProcess(string input) {
        Dictionary<char, Condition> parseCondition = new() {
            ['.'] = Condition.OPERATIONAL,
            ['#'] = Condition.DAMAGED,
            ['?'] = Condition.UNKNOWN
        };

        string[] lines = input.Split('\n');

        return new Input(
            lines.Select(line => {
                string[] elements = line.Split(' ');
                return new Record(
                    elements[0].Select(c => parseCondition[c]).ToArray(),
                    elements[1].Split(',').Select(int.Parse).ToArray()
                );
            }).ToArray()
        );
    }

    private int SolveRecord(Record record) {
        int possibilities = 0;

        if (record.damagedGroups.Count > 0) {
            int nextGroupSize = record.damagedGroups[0];

            int maxGroupStartIndex = record.conditions.Count - nextGroupSize;
            for (int i = 0; i <= maxGroupStartIndex; i++) {
                if (record.conditions[i] is Condition.DAMAGED)
                    maxGroupStartIndex = i;

                if (CanBeDamagedGroup(i, nextGroupSize)) {
                    possibilities += SolveRecord(record with {
                        conditions = record.conditions[Math.Min(i + nextGroupSize + 1, record.conditions.Count)..],
                        damagedGroups = record.damagedGroups[1..]
                    });
                }
            }
        } else {
            possibilities = record.conditions.Contains(Condition.DAMAGED) ? 0 : 1;
        }

        return possibilities;

        bool CanBeDamagedGroup(int startIndex, int groupSize) {
            for (int i = 0; i < groupSize; i++) {
                if (record.conditions[startIndex + i] is Condition.OPERATIONAL)
                    return false;
            }

            if (startIndex + groupSize < record.conditions.Count
             && record.conditions[startIndex + groupSize] is Condition.DAMAGED)
                return false;

            return true;
        }
    }

    protected override int Solve(Input input) {
        return input.Records.Select(SolveRecord).Sum();
    }

    public static void Run() {
        new Day12A().Solve();
    }
}