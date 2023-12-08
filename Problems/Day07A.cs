namespace Advent_of_Code_2023;

public class Day07A : Problem<Day07A.Input, int> {
    public readonly record struct Input(Input.Hand[] Hands) {
        public readonly record struct Hand(byte[] CardRanks, int Bid) : IComparable<Hand> {
            private readonly HandType _type = TypeOf(CardRanks);
            
            public int CompareTo(Hand other) {
                int compare = ((int)_type).CompareTo((int)other._type);
                if (compare != 0) return compare;
                for (int i = 0; i < CardRanks.Length; i++) {
                    compare = CardRanks[i].CompareTo(other.CardRanks[i]);
                    if (compare != 0) return compare;
                }

                return compare;
            }
        }
    }

    protected override Input PreProcess(TextReader input) {
        return new Input(
            input.ReadToEnd()
                 .Split("\n")
                 .Select(line => {
                      string[] elements = line.Split(" ");
                      return new Input.Hand(
                          elements[0].Select(c => cardRanks[c])
                                     .ToArray(),
                          int.Parse(elements[1])
                      );
                  })
                 .ToArray()
        );
    }

    public enum HandType {
        FIVE_OF_A_KIND = 00005,
        FOUR_OF_A_KIND = 00014,
        FULL_HOUSE     = 00032,
        TREE_OF_A_KIND = 00113,
        TWO_PAIRS      = 00122,
        ONE_PAIR       = 01112,
        HIGH_CARD      = 11111
    }

    private const string CARD_RANKS = "AKQJT98765432";
    private static readonly Dictionary<char, byte> cardRanks =
        CARD_RANKS.Select((c, index) => (c, index))
                  .ToDictionary(
                       entry => entry.c, entry => (byte)entry.index);

    private static HandType TypeOf(byte[] ranks) {
        int handType = 0;
        foreach (int count in ranks.Distinct()
                                   .Select(r => ranks.Count(x => x == r))
                                   .Order()) {
            handType *= 10;
            handType += count;
        }

        return (HandType)handType;
    }

    protected override int Solve(Input input) =>
        input.Hands.OrderDescending()
             .Select((hand, index) => hand.Bid * (index + 1))
             .Sum();

    public static void Run() {
        new Day07A().Solve();
    }
}