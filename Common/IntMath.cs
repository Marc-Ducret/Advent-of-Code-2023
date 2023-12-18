namespace Advent_of_Code_2023;

public static class IntMath {
    public static ulong GCD(ulong a, ulong b) {
        while (a != 0 && b != 0) {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a | b;
    }

    public static ulong LCM(ulong a, ulong b) => a * b / GCD(a, b);

    public static Int2 RotateRight(in Int2 value) => new(+value.Y, -value.X);
    public static Int2 RotateLeft(in  Int2 value) => new(-value.Y, +value.X);
}