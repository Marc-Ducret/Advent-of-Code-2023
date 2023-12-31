﻿namespace Advent_of_Code_2023;

public record struct Bool2(bool X, bool Y) {
    public readonly bool All => X && Y;
    public readonly bool Any => X || Y;

    public static Bool2 operator &(Bool2 lhs, Bool2 rhs) =>
        new(lhs.X & rhs.X,
            lhs.Y & rhs.Y);

    public static implicit operator Int2(Bool2 value) => new(value.X ? 1 : 0, value.Y ? 1 : 0);
}

public record struct Bool3(bool X, bool Y, bool Z) {
    public readonly bool All => X && Y && Z;
    public readonly bool Any => X || Y || Z;

    public static Bool3 operator &(Bool3 lhs, Bool3 rhs) =>
        new(lhs.X & rhs.X,
            lhs.Y & rhs.Y,
            lhs.Z & rhs.Z);
}

public record struct Bool4(bool X, bool Y, bool Z, bool W) {
    public readonly bool All => X && Y && Z && W;
    public readonly bool Any => X || Y || Z || W;

    public static Bool4 operator &(Bool4 lhs, Bool4 rhs) =>
        new(lhs.X & rhs.X,
            lhs.Y & rhs.Y,
            lhs.Z & rhs.Z,
            lhs.W & rhs.W);
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

    public static Int2 operator /(in Int2 lhs, in Int2 rhs) =>
        new(lhs.X / rhs.X,
            lhs.Y / rhs.Y);

    public static Int2 operator -(in Int2 value) =>
        new(-value.X,
            -value.Y);

    public static implicit operator Int2(int   value) => new(value, value);
    public static implicit operator Long2(Int2 value) => new(value.X, value.Y);

    public static Int2  Abs(in  Int2 value)            => new(Math.Abs(value.X), Math.Abs(value.Y));
    public static Int2  Sign(in Int2 value)            => new(Math.Sign(value.X), Math.Sign(value.Y));
    public static int   CSum(in Int2 value)            => value.X + value.Y;
    public static Int2  Min(in  Int2 lhs, in Int2 rhs) => new(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y));
    public static Int2  Max(in  Int2 lhs, in Int2 rhs) => new(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y));
    public static Bool2 Eq(in   Int2 lhs, in Int2 rhs) => new(lhs.X == rhs.X, lhs.Y == rhs.Y);
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

    public static Long2 operator *(in Long2 lhs, in Long2 rhs) =>
        new(lhs.X * rhs.X,
            lhs.Y * rhs.Y);

    public static Long2 operator *(in long lhs, in Long2 rhs) =>
        new(lhs * rhs.X,
            lhs * rhs.Y);

    public static Long2 operator /(in Long2 lhs, in long rhs) =>
        new(lhs.X / rhs,
            lhs.Y / rhs);

    public static implicit operator Long2(int value) => new(value, value);

    public static Long2 Abs(in  Long2 value) => new(Math.Abs(value.X), Math.Abs(value.Y));
    public static long  CSum(in Long2 value) => value.X + value.Y;
}

public record struct Int3(int X, int Y, int Z) {
    public Int2 xy => new(X, Y);
    public Int2 xz => new(X, Z);
    public Int2 yz => new(Y, Z);

    public static Bool3 operator >=(in Int3 lhs, in Int3 rhs) =>
        new(lhs.X >= rhs.X,
            lhs.Y >= rhs.Y,
            lhs.Z >= rhs.Z);

    public static Bool3 operator <=(in Int3 lhs, in Int3 rhs) =>
        new(lhs.X <= rhs.X,
            lhs.Y <= rhs.Y,
            lhs.Z <= rhs.Z);

    public static Bool3 operator <(in Int3 lhs, in Int3 rhs) =>
        new(lhs.X < rhs.X,
            lhs.Y < rhs.Y,
            lhs.Z < rhs.Z);

    public static Bool3 operator >(in Int3 lhs, in Int3 rhs) =>
        new(lhs.X > rhs.X,
            lhs.Y > rhs.Y,
            lhs.Z > rhs.Z);

    public static Int3 operator +(in Int3 lhs, in Int3 rhs) =>
        new(lhs.X + rhs.X,
            lhs.Y + rhs.Y,
            lhs.Z + rhs.Z);

    public static Int3 operator -(in Int3 lhs, in Int3 rhs) =>
        new(lhs.X - rhs.X,
            lhs.Y - rhs.Y,
            lhs.Z - rhs.Z);

    public static Int3 operator *(in Int3 lhs, in Int3 rhs) =>
        new(lhs.X * rhs.X,
            lhs.Y * rhs.Y,
            lhs.Z * rhs.Z);

    public static Int3 operator -(in Int3 value) =>
        new(-value.X,
            -value.Y,
            -value.Z);

    public static Int3 Abs(in Int3 value) => new(Math.Abs(value.X),
                                                 Math.Abs(value.Y),
                                                 Math.Abs(value.Z));

    public static int CSum(in Int3 value) => value.X + value.Y + value.Z;

    public static Int3 Min(in Int3 lhs, in Int3 rhs) => new(Math.Min(lhs.X, rhs.X),
                                                            Math.Min(lhs.Y, rhs.Y),
                                                            Math.Min(lhs.Z, rhs.Z));

    public static Int3 Max(in Int3 lhs, in Int3 rhs) => new(Math.Max(lhs.X, rhs.X),
                                                            Math.Max(lhs.Y, rhs.Y),
                                                            Math.Max(lhs.Z, rhs.Z));

    public static implicit operator Int3(int value) => new(value, value, value);
}

public record struct Long3(long X, long Y, long Z) {
    public Long2 xy => new(X, Y);
    public Long2 xz => new(X, Z);
    public Long2 yz => new(Y, Z);

    public static Bool3 operator >=(in Long3 lhs, in Long3 rhs) =>
        new(lhs.X >= rhs.X,
            lhs.Y >= rhs.Y,
            lhs.Z >= rhs.Z);

    public static Bool3 operator <=(in Long3 lhs, in Long3 rhs) =>
        new(lhs.X <= rhs.X,
            lhs.Y <= rhs.Y,
            lhs.Z <= rhs.Z);

    public static Bool3 operator <(in Long3 lhs, in Long3 rhs) =>
        new(lhs.X < rhs.X,
            lhs.Y < rhs.Y,
            lhs.Z < rhs.Z);

    public static Bool3 operator >(in Long3 lhs, in Long3 rhs) =>
        new(lhs.X > rhs.X,
            lhs.Y > rhs.Y,
            lhs.Z > rhs.Z);

    public static Long3 operator +(in Long3 lhs, in Long3 rhs) =>
        new(lhs.X + rhs.X,
            lhs.Y + rhs.Y,
            lhs.Z + rhs.Z);

    public static Long3 operator -(in Long3 lhs, in Long3 rhs) =>
        new(lhs.X - rhs.X,
            lhs.Y - rhs.Y,
            lhs.Z - rhs.Z);

    public static Long3 operator *(in Long3 lhs, in Long3 rhs) =>
        new(lhs.X * rhs.X,
            lhs.Y * rhs.Y,
            lhs.Z * rhs.Z);

    public static Long3 operator /(in Long3 lhs, in long rhs) =>
        new(lhs.X / rhs,
            lhs.Y / rhs,
            lhs.Z / rhs);

    public static Long3 operator -(in Long3 value) =>
        new(-value.X,
            -value.Y,
            -value.Z);

    public static Long3 Abs(in Long3 value) => new(Math.Abs(value.X),
                                                   Math.Abs(value.Y),
                                                   Math.Abs(value.Z));

    public static long CSum(in Long3 value) => value.X + value.Y + value.Z;

    public static Long3 Min(in Long3 lhs, in Long3 rhs) => new(Math.Min(lhs.X, rhs.X),
                                                               Math.Min(lhs.Y, rhs.Y),
                                                               Math.Min(lhs.Z, rhs.Z));

    public static Long3 Max(in Long3 lhs, in Long3 rhs) => new(Math.Max(lhs.X, rhs.X),
                                                               Math.Max(lhs.Y, rhs.Y),
                                                               Math.Max(lhs.Z, rhs.Z));

    public static Long3 Cross(in Long3 lhs, in Long3 rhs) => new(lhs.Y * rhs.Z - lhs.Z * rhs.Y,
                                                                 lhs.Z * rhs.X - lhs.X * rhs.Z,
                                                                 lhs.X * rhs.Y - lhs.Y * rhs.X);

    public static long Dot(in Long3 lhs, in Long3 rhs) => CSum(lhs * rhs);

    public static long LengthSq(in Long3 value) => Dot(value, value);

    public static implicit operator Long3(long value) => new(value, value, value);
}

public record struct Int4(int X, int Y, int Z, int W) {
    public static Bool4 operator >=(in Int4 lhs, in Int4 rhs) =>
        new(lhs.X >= rhs.X,
            lhs.Y >= rhs.Y,
            lhs.Z >= rhs.Z,
            lhs.W >= rhs.W);

    public static Bool4 operator <=(in Int4 lhs, in Int4 rhs) =>
        new(lhs.X <= rhs.X,
            lhs.Y <= rhs.Y,
            lhs.Z <= rhs.Z,
            lhs.W <= rhs.W);

    public static Bool4 operator <(in Int4 lhs, in Int4 rhs) =>
        new(lhs.X < rhs.X,
            lhs.Y < rhs.Y,
            lhs.Z < rhs.Z,
            lhs.W < rhs.W);

    public static Bool4 operator >(in Int4 lhs, in Int4 rhs) =>
        new(lhs.X > rhs.X,
            lhs.Y > rhs.Y,
            lhs.Z > rhs.Z,
            lhs.W > rhs.W);

    public static Int4 operator +(in Int4 lhs, in Int4 rhs) =>
        new(lhs.X + rhs.X,
            lhs.Y + rhs.Y,
            lhs.Z + rhs.Z,
            lhs.W + rhs.W);

    public static Int4 operator -(in Int4 lhs, in Int4 rhs) =>
        new(lhs.X - rhs.X,
            lhs.Y - rhs.Y,
            lhs.Z - rhs.Z,
            lhs.W - rhs.W);

    public static Int4 operator *(in Int4 lhs, in Int4 rhs) =>
        new(lhs.X * rhs.X,
            lhs.Y * rhs.Y,
            lhs.Z * rhs.Z,
            lhs.W * rhs.W);

    public static Int4 operator -(in Int4 value) =>
        new(-value.X,
            -value.Y,
            -value.Z,
            -value.W);

    public static Int4 Abs(in Int4 value) => new(Math.Abs(value.X),
                                                 Math.Abs(value.Y),
                                                 Math.Abs(value.Z),
                                                 Math.Abs(value.W));

    public static int CSum(in Int4 value) => value.X + value.Y + value.Z + value.W;

    public static Int4 Min(in Int4 lhs, in Int4 rhs) => new(Math.Min(lhs.X, rhs.X),
                                                            Math.Min(lhs.Y, rhs.Y),
                                                            Math.Min(lhs.Z, rhs.Z),
                                                            Math.Min(lhs.W, rhs.W));

    public static Int4 Max(in Int4 lhs, in Int4 rhs) => new(Math.Max(lhs.X, rhs.X),
                                                            Math.Max(lhs.Y, rhs.Y),
                                                            Math.Max(lhs.Z, rhs.Z),
                                                            Math.Max(lhs.W, rhs.W));

    public static implicit operator Int4(int   value) => new(value, value, value, value);
    public static implicit operator Long4(Int4 value) => new(value.X, value.Y, value.Z, value.W);
}

public record struct Long4(long X, long Y, long Z, long W) {
    public static Bool4 operator >=(in Long4 lhs, in Long4 rhs) =>
        new(lhs.X >= rhs.X,
            lhs.Y >= rhs.Y,
            lhs.Z >= rhs.Z,
            lhs.W >= rhs.W);

    public static Bool4 operator <=(in Long4 lhs, in Long4 rhs) =>
        new(lhs.X <= rhs.X,
            lhs.Y <= rhs.Y,
            lhs.Z <= rhs.Z,
            lhs.W <= rhs.W);

    public static Bool4 operator <(in Long4 lhs, in Long4 rhs) =>
        new(lhs.X < rhs.X,
            lhs.Y < rhs.Y,
            lhs.Z < rhs.Z,
            lhs.W < rhs.W);

    public static Bool4 operator >(in Long4 lhs, in Long4 rhs) =>
        new(lhs.X > rhs.X,
            lhs.Y > rhs.Y,
            lhs.Z > rhs.Z,
            lhs.W > rhs.W);

    public static Long4 operator +(in Long4 lhs, in Long4 rhs) =>
        new(lhs.X + rhs.X,
            lhs.Y + rhs.Y,
            lhs.Z + rhs.Z,
            lhs.W + rhs.W);

    public static Long4 operator -(in Long4 lhs, in Long4 rhs) =>
        new(lhs.X - rhs.X,
            lhs.Y - rhs.Y,
            lhs.Z - rhs.Z,
            lhs.W - rhs.W);

    public static Long4 operator *(in Long4 lhs, in Long4 rhs) =>
        new(lhs.X * rhs.X,
            lhs.Y * rhs.Y,
            lhs.Z * rhs.Z,
            lhs.W * rhs.W);

    public static Long4 operator -(in Long4 value) =>
        new(-value.X,
            -value.Y,
            -value.Z,
            -value.W);

    public static Long4 Abs(in Long4 value) => new(Math.Abs(value.X),
                                                   Math.Abs(value.Y),
                                                   Math.Abs(value.Z),
                                                   Math.Abs(value.W));

    public static long CSum(in Long4 value) => value.X + value.Y + value.Z + value.W;
    public static long CMul(in Long4 value) => value.X * value.Y * value.Z * value.W;

    public static Long4 Min(in Long4 lhs, in Long4 rhs) => new(Math.Min(lhs.X, rhs.X),
                                                               Math.Min(lhs.Y, rhs.Y),
                                                               Math.Min(lhs.Z, rhs.Z),
                                                               Math.Min(lhs.W, rhs.W));

    public static Long4 Max(in Long4 lhs, in Long4 rhs) => new(Math.Max(lhs.X, rhs.X),
                                                               Math.Max(lhs.Y, rhs.Y),
                                                               Math.Max(lhs.Z, rhs.Z),
                                                               Math.Max(lhs.W, rhs.W));

    public static implicit operator Long4(long value) => new(value, value, value, value);
}