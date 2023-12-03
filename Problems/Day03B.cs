namespace Advent_of_Code_2023;

public class Day03B : Problem<Day03B.Input, int> {
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

    protected override Input PreProcess(TextReader input) {
        List<string> lines = new();
        while (input.ReadLine() is { } line) {
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
        Dictionary<Int2, List<int>> potentialGears = new();
        {
            for (int y = 0; y < input.size.Y; y++)
            for (int x = 0; x < input.size.X; x++) {
                Int2 position = new(x, y);
                if (input.CharAt(position) == GEAR_SYMBOL)
                    potentialGears.Add(position, new List<int>());
            }
        }

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
                foreach (Int2 gearPosition in Border(startPosition - 1, endPosition + 1)
                            .Where(p => input.CharAt(p) == GEAR_SYMBOL))
                    potentialGears[gearPosition].Add(number);
            }
        }

        return potentialGears.Values.Where(numbers => numbers.Count == 2)
                             .Sum(numbers => numbers[0] * numbers[1]);
    }

    private const char GEAR_SYMBOL = '*';

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
        new Day03B().Solve();
    }
}