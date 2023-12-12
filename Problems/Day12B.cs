namespace Advent_of_Code_2023;

public class Day12B : Problem<Day12B.Input, long> {
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

        public Key GetKey => new(conditions.Array!.GetHashCode(), conditions.Offset, damagedGroups.Offset);

        public readonly record struct Key(int InstanceHash, int ConditionsStart, int DamagedGroupsStart);
    }

    public enum Condition {
        OPERATIONAL,
        DAMAGED,
        UNKNOWN
    }

    protected override Input PreProcess(string input) {
        const int duplicateCount = 5;

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
                    Duplicate(elements[0], '?').Select(c => parseCondition[c]).ToArray(),
                    Duplicate(elements[1], ',').Split(',').Select(int.Parse).ToArray()
                );
            }).ToArray()
        );

        string Duplicate(string initial, char separator) =>
            string.Join(separator,
                        Enumerable.Range(0, duplicateCount).Select(_ => initial));
    }

    private void NormalizeRecord(ref Record record) {
        int firstNonOperationalIndex = 0;
        while (firstNonOperationalIndex < record.conditions.Count
            && record.conditions[firstNonOperationalIndex] is Condition.OPERATIONAL)
            firstNonOperationalIndex++;
        record = record with { conditions = record.conditions[firstNonOperationalIndex..] };
    }

    private long SolveRecord(Record record, Dictionary<Record.Key, long> cache) {
        NormalizeRecord(ref record);
        Record.Key key = record.GetKey;

        if (!cache.TryGetValue(key, out long result))
            cache.Add(key, result = SolveRecordWithoutCache(record, cache));
        return result;
    }

    private long SolveRecordWithoutCache(Record record, Dictionary<Record.Key, long> cache) {
        long possibilities = 0;

        if (record.damagedGroups.Count > 0) {
            int nextGroupSize = record.damagedGroups[0];

            int maxGroupStartIndex       = record.conditions.Count - nextGroupSize;
            for (int i = 0; i <= maxGroupStartIndex; i++) {
                if (record.conditions[i] is Condition.DAMAGED)
                    maxGroupStartIndex = i;

                if (CanBeDamagedGroup(i, nextGroupSize)) {
                    possibilities += SolveRecord(record with {
                        conditions = record.conditions[Math.Min(i + nextGroupSize + 1, record.conditions.Count)..],
                        damagedGroups = record.damagedGroups[1..]
                    }, cache);
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

    protected override long Solve(Input input) {
        Dictionary<Record.Key, long> cache = new();
        return input.Records.Select(r => SolveRecord(r, cache)).Sum();
    }

    public static void Run() {
        new Day12B().Solve();
    }
}