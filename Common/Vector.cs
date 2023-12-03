namespace Advent_of_Code_2023;

public record struct Bool2(bool X, bool Y) {
    public readonly bool All => X && Y;

    public static Bool2 operator &(Bool2 lhs, Bool2 rhs) =>
        new(lhs.X & rhs.X,
            lhs.Y & rhs.Y);
}

public record struct Int2(int X, int Y) {
    public static Bool2 operator >=(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X >= rhs.X,
            lhs.Y >= rhs.Y);

    public static Bool2 operator <=(Int2 lhs, Int2 rhs) =>
        new(lhs.X <= rhs.X,
            lhs.Y <= rhs.Y);

    public static Bool2 operator <(Int2 lhs, Int2 rhs) =>
        new(lhs.X < rhs.X,
            lhs.Y < rhs.Y);

    public static Bool2 operator >(Int2 lhs, Int2 rhs) =>
        new(lhs.X > rhs.X,
            lhs.Y > rhs.Y);

    public static Int2 operator +(Int2 lhs, Int2 rhs) =>
        new(lhs.X + rhs.X,
            lhs.Y + rhs.Y);

    public static Int2 operator -(Int2 lhs, Int2 rhs) =>
        new(lhs.X - rhs.X,
            lhs.Y - rhs.Y);

    public static implicit operator Int2(int value) => new(value, value);
}