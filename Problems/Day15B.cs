namespace Advent_of_Code_2023;

public class Day15B : Problem<Day15B.Input, int> {
    public readonly record struct Input(Instruction[] Instructions);

    public readonly record struct Instruction(string Label, int FocusLength = -1) {
        public bool Remove => FocusLength < 0;
    }

    protected override Input PreProcess(string input) {
        return new Input(
            input.Split(',')
                 .Select(ParseInstruction)
                 .ToArray()
        );

        Instruction ParseInstruction(string instruction) {
            if (instruction.EndsWith('-'))
                return new Instruction(instruction[..^1]);
            int equalsIndex = instruction.IndexOf('=');
            if (equalsIndex < 0)
                throw new ArgumentException("Invalid instruction.");
            return new Instruction(instruction[..equalsIndex], int.Parse(instruction[(equalsIndex + 1)..]));
        }
    }

    private readonly record struct Box(List<Instruction> Lenses) {
        public void Process(Instruction instruction) {
            if (instruction.Remove)
                Remove(instruction);
            else
                Add(instruction);
        }

        private void Remove(Instruction instruction) {
            Lenses.RemoveAll(x => x.Label == instruction.Label);
        }

        private void Add(Instruction instruction) {
            int existingIndex = Lenses.FindIndex(x => x.Label == instruction.Label);
            if (existingIndex < 0)
                Lenses.Add(instruction);
            else
                Lenses[existingIndex] = instruction;
        }
    }

    private byte Hash(string label) {
        byte hash = 0;
        unchecked {
            foreach (char c in label) {
                hash += (byte)c;
                hash *= 17;
            }
        }

        return hash;
    }

    protected override int Solve(Input input) {
        Box[] boxes = new Box[0x100];
        for (int i = 0; i < boxes.Length; i++) {
            boxes[i] = new Box([]);
        }

        foreach (Instruction instruction in input.Instructions) {
            boxes[Hash(instruction.Label)].Process(instruction);
        }

        return boxes.SelectMany(
                         (box, boxIndex) =>
                             box.Lenses.Select(
                                 (lens, lensIndex) =>
                                     FocusingPower(boxIndex, lensIndex, lens.FocusLength)))
                    .Sum();

        int FocusingPower(int boxIndex, int lensIndex, int focalLength) =>
            (boxIndex + 1) * (lensIndex + 1) * focalLength;
    }

    public static void Run() {
        new Day15B().Solve();
    }
}