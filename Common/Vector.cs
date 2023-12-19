namespace Advent_of_Code_2023;

public record struct Bool2(bool X, bool Y) {
    public readonly bool All => X && Y;
    public readonly bool Any => X || Y;

    public static Bool2 operator &(Bool2 lhs, Bool2 rhs) =>
        new(lhs.X & rhs.X,
            lhs.Y & rhs.Y);
}

public record struct Int2(int X, int Y) {
    public static Bool2 operator >=(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X >= rhs.X,
            lhs.Y >= rhs.Y);

    public static Bool2 operator <=(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X <= rhs.X,
            lhs.Y <= rhs.Y);

    public static Bool2 operator <(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X < rhs.X,
            lhs.Y < rhs.Y);

    public static Bool2 operator >(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X > rhs.X,
            lhs.Y > rhs.Y);

    public static Int2 operator +(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X + rhs.X,
            lhs.Y + rhs.Y);

    public static Int2 operator -(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X - rhs.X,
            lhs.Y - rhs.Y);

    public static Int2 operator *(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X * rhs.X,
            lhs.Y * rhs.Y);

    public static Int2 operator -(in Int2 value) =>
        new(-value.X,
            -value.Y);

    public static implicit operator Int2(int   value) => new(value, value);
    public static implicit operator Long2(Int2 value) => new(value.X, value.Y);

    public static Int2 Abs(in  Int2 value)            => new(Math.Abs(value.X), Math.Abs(value.Y));
    public static int  CSum(in Int2 value)            => value.X + value.Y;
    public static Int2 Min(in  Int2 lhs, in Int2 rhs) => new(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y));
    public static Int2 Max(in  Int2 lhs, in Int2 rhs) => new(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y));
}

public record struct Long2(long X, long Y) {
    public static Bool2 operator >=(in Long2 lhs, in Long2 rhs) =>
        new(lhs.X >= rhs.X,
            lhs.Y >= rhs.Y);

    public static Bool2 operator <=(in Long2 lhs, in Long2 rhs) =>
        new(lhs.X <= rhs.X,
            lhs.Y <= rhs.Y);

    public static Bool2 operator <(in Long2 lhs, in Long2 rhs) =>
        new(lhs.X < rhs.X,
            lhs.Y < rhs.Y);

    public static Bool2 operator >(in Long2 lhs, in Long2 rhs) =>
        new(lhs.X > rhs.X,
            lhs.Y > rhs.Y);

    public static Long2 operator +(in Long2 lhs, in Long2 rhs) =>
        new(lhs.X + rhs.X,
            lhs.Y + rhs.Y);

    public static Long2 operator -(in Long2 lhs, in Long2 rhs) =>
        new(lhs.X - rhs.X,
            lhs.Y - rhs.Y);

    public static Long2 operator *(in long lhs, in Long2 rhs) =>
        new(lhs * rhs.X,
            lhs * rhs.Y);

    public static implicit operator Long2(int value) => new(value, value);

    public static Long2 Abs(in  Long2 value) => new(Math.Abs(value.X), Math.Abs(value.Y));
    public static long  CSum(in Long2 value) => value.X + value.Y;
}