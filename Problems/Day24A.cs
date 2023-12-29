using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public class Day24A : Problem<Day24A.Input, int> {
    public readonly record struct Input(Bounds TestArea, Hailstone[] Hailstones);

    public readonly record struct Bounds(Long2 Min, Long2 Max);

    public readonly record struct Hailstone(Long3 Position, Long3 Velocity);

    protected override Input PreProcess(string input) {
        string[] parts = input.Split("\n\n");
        return new Input(ParseBounds(parts[0]), parts[1].Split('\n').Select(ParseHailstone).ToArray());

        Long2 ParseLong2(string value) {
            string[] parts = Regex.Split(value, @"\s*,\s*");
            return new Long2(long.Parse(parts[0]), long.Parse(parts[1]));
        }

        Bounds ParseBounds(string value) {
            string[] parts = value.Split(" ~ ");
            return new Bounds(ParseLong2(parts[0]), ParseLong2(parts[1]));
        }

        Long3 ParseLong3(string value) {
            string[] parts = Regex.Split(value, @"\s*,\s*");
            return new Long3(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
        }

        Hailstone ParseHailstone(string value) {
            string[] parts = value.Split(" @ ");
            return new Hailstone(ParseLong3(parts[0]), ParseLong3(parts[1]));
        }
    }

    private const bool DEBUG = false;

    private bool IntersectWithin(Bounds bounds, Long2 posA, Long2 dirA, Long2 posB, Long2 dirB) {
        // posA + tA * dirA = posB + tB * dirB
        // posA - posB = tB * dirB - tA * dirA
        // posA - posB = Dir * T
        //   with T = [-tA, tB]
        //        Dir = [[dirA.X, dirB.X], [dirA.Y, dirB.Y]]
        // T = Dir^-1 * (posA - posB)
        // Dir^-1 = [[dirB.Y, -dirB.X], [-dirA.Y, dirA.X]] / determinant

        if (DEBUG) {
            Console.WriteLine($"A: {posA} @ {dirA}");
            Console.WriteLine($"B: {posB} @ {dirB}");
        }

        Long2 posDiff = posA - posB;

        long determinant = dirA.X * dirB.Y - dirB.X * dirA.Y;
        int  sign        = Math.Sign(determinant);
        if (sign == 0) {
            if (DEBUG) Console.WriteLine("Never");
            return false;
        }

        determinant *= sign;

        long tNumeratorA = sign * -Long2.CSum(new Long2(dirB.Y, -dirB.X) * posDiff);
        long tNumeratorB = sign * Long2.CSum(new Long2(-dirA.Y, dirA.X) * posDiff);

        if (tNumeratorA < 0 || tNumeratorB < 0) {
            if (DEBUG) Console.WriteLine("In the past");
            return false;
        }

        if (DEBUG)
            Console.WriteLine(
                $"At x={posA.X + (tNumeratorA * dirA).X / (float)determinant}, y={posA.Y + (tNumeratorA * dirA).Y / (float)determinant}");

        double intersectX = posA.X + tNumeratorA / (double)determinant * dirA.X; 
        double intersectY = posA.Y + tNumeratorA / (double)determinant * dirA.Y;

        return intersectX >= bounds.Min.X
                           && intersectX <= bounds.Max.X
                           && intersectY >= bounds.Min.Y
                           && intersectY <= bounds.Max.Y;
    }

    protected override int Solve(Input input) {
        int count = 0;

        for (int i = 0; i < input.Hailstones.Length; i++)
        for (int j = i + 1; j < input.Hailstones.Length; j++) {
            if (IntersectWithin(input.TestArea,
                                input.Hailstones[i].Position.xy,
                                input.Hailstones[i].Velocity.xy,
                                input.Hailstones[j].Position.xy,
                                input.Hailstones[j].Velocity.xy
                ))
                count++;
        }

        return count;
    }

    public static void Run() {
        new Day24A().Solve();
    }
}