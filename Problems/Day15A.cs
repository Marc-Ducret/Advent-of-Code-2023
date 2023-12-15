namespace Advent_of_Code_2023;

public class Day15A : Problem<Day15A.Input, int> {
    public readonly record struct Input(Instruction[] Instructions);

    public readonly record struct Instruction(char[] Chars);

    protected override Input PreProcess(string input) {
        return new Input(
            input.Split(',')
                 .Select(x => new Instruction(x.ToCharArray()))
                 .ToArray()
        );
    }

    private byte Hash(Instruction instruction) {
        byte hash = 0;
        unchecked {
            foreach (char c in instruction.Chars) {
                hash += (byte)c;
                hash *= 17;
            }
        }

        return hash;
    }

    protected override int Solve(Input input) {
        return input.Instructions.Select(Hash)
                    .Sum(hash => hash);
    }

    public static void Run() {
        new Day15A().Solve();
    }
}