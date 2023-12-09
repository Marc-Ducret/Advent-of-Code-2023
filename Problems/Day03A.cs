namespace Advent_of_Code_2023;

public class Day03A : Problem<Day03A.Input, int> {
    public readonly struct Input {
        public readonly Int2   size;
        public readonly char[] schematic;

        public Input(Int2 size, char[] schematic) {
            this.size      = size;
            this.schematic = schematic;
        }

        public bool IsWithin(in Int2 position) =>
            ((position >= 0) & (position < size)).All;

        public char CharAt(in Int2 position) =>
            IsWithin(position)
                ? schematic[size.X * position.Y + position.X]
                : '.';
    }

    public enum CharType {
        EMPTY,
        DIGIT,
        SYMBOL
    }

    protected override Input PreProcess(string input) {
        List<string> lines = new();
        foreach (string line in input.Split('\n')) {
            if (line.Length == 0) continue;
            lines.Add(line);
        }

        Int2   size      = new(lines[0].Length, lines.Count);
        char[] schematic = new char[size.X * size.Y];
        for (int y = 0; y < size.Y; y++)
        for (int x = 0; x < size.X; x++) {
            schematic[size.X * y + x] = lines[y][x];
        }

        return new Input(size, schematic);
    }

    protected override int Solve(Input input) {
        int sum = 0;
        for (int y = 0; y < input.size.Y; y++)
        for (int x = 0; x < input.size.X; x++) {
            Int2 startPosition = new(x, y);

            int  number = 0;
            char c;
            while (TypeOf(c = input.CharAt(new Int2(x, y))) == CharType.DIGIT) {
                number *= 10;
                number += c - '0';
                x++;
            }

            Int2 endPosition = new(x - 1, y);
            if (number > 0) {
                if (Border(startPosition - 1, endPosition + 1).Any(p => TypeOf(input.CharAt(p)) == CharType.SYMBOL))
                    sum += number;
            }
        }

        return sum;
    }

    private static CharType TypeOf(char c) => c switch {
        '.'               => CharType.EMPTY,
        >= '0' and <= '9' => CharType.DIGIT,
        _                 => CharType.SYMBOL
    };

    private static IEnumerable<Int2> Border(Int2 from, Int2 to) {
        Int2 current = from;
        while (current.X != to.X) {
            current.X += 1;
            yield return current;
        }

        while (current.Y != to.Y) {
            current.Y += 1;
            yield return current;
        }

        while (current.X != from.X) {
            current.X -= 1;
            yield return current;
        }

        while (current.Y != from.Y) {
            current.Y -= 1;
            yield return current;
        }
    }

    public static void Run() {
        new Day03A().Solve();
    }
}