using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public class Day24B : Problem<Day24B.Input, long> {
    public readonly record struct Input(Hailstone[] Hailstones);

    public readonly record struct Hailstone(Long3 Position, Long3 Velocity);

    protected override Input PreProcess(string input) {
        return new Input(input.Split('\n').Select(ParseHailstone).ToArray());

        Long3 ParseLong3(string value) {
            string[] parts = Regex.Split(value, @"\s*,\s*");
            return new Long3(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
        }

        Hailstone ParseHailstone(string value) {
            string[] parts = value.Split(" @ ");
            return new Hailstone(ParseLong3(parts[0]), ParseLong3(parts[1]));
        }
    }

    private bool Intersect(Long2 posA, Long2 dirA, Long2 posB, Long2 dirB, out long timeA, out long timeB) {
        // posA + tA * dirA = posB + tB * dirB
        // posA - posB = tB * dirB - tA * dirA
        // posA - posB = Dir * T
        //   with T = [-tA, tB]
        //        Dir = [[dirA.X, dirB.X], [dirA.Y, dirB.Y]]
        // T = Dir^-1 * (posA - posB)
        // Dir^-1 = [[dirB.Y, -dirB.X], [-dirA.Y, dirA.X]] / determinant

        Long2 posDiff = posA - posB;

        long determinant = dirA.X * dirB.Y - dirB.X * dirA.Y;
        int  sign        = Math.Sign(determinant);
        if (sign == 0) {
            timeA = 0;
            timeB = 0;
            return false;
        }

        determinant *= sign;

        long tNumeratorA = sign * -Long2.CSum(new Long2(dirB.Y, -dirB.X) * posDiff);
        long tNumeratorB = sign * Long2.CSum(new Long2(-dirA.Y, dirA.X) * posDiff);

        if (tNumeratorA % determinant != 0
         || tNumeratorB % determinant != 0)
            throw new Exception("Non integer intersect.");

        timeA = tNumeratorA / determinant;
        timeB = tNumeratorB / determinant;
        return true;
    }

    private bool Intersect(Long3 posA, Long3 dirA, Long3 posB, Long3 dirB, out long timeA, out long timeB) =>
        Intersect(posA.xy, dirA.xy, posB.xy, dirB.xy, out timeA, out timeB)
     || Intersect(posA.yz, dirA.yz, posB.yz, dirB.yz, out timeA, out timeB)
     || Intersect(posA.xz, dirA.xz, posB.xz, dirB.xz, out timeA, out timeB);

    private Long3 SimplifyDirection(in Long3 direction) =>
        direction == 0
            ? 0
            : direction / (long)IntMath.GCD((ulong)Math.Abs(direction.X),
                                            (ulong)Math.Abs(direction.Y),
                                            (ulong)Math.Abs(direction.Z));

    private long SolveThroughZero(Hailstone[] hailstones) {
        {
            Long3 direction = FindDirection();

            // rock_position(t) = (initial + speed * t) * direction
            (Long3 position, long time)[] positions = hailstones
                                                     .Select(
                                                          hailstone =>
                                                              IntersectPosition(direction, hailstone))
                                                     .ToArray();

            // rock_position(t1) - rock_position(t0) = (speed * (t1 - t0)) * direction

            long speed = Long3.Dot(positions[1].position - positions[0].position, direction)
                       / Long3.LengthSq(direction)
                       / (positions[1].time - positions[0].time);

            long initial = Long3.Dot(positions[0].position, direction)
                         / Long3.LengthSq(direction)
                         - speed * positions[0].time;

            return Long3.CSum(initial * direction);
        }

        (Long3 position, long time) IntersectPosition(Long3 direction, Hailstone hailstone) {
            if (!Intersect(0, direction, hailstone.Position, hailstone.Velocity, out _, out long time))
                throw new Exception("No intersect.");

            return (hailstone.Position + time * hailstone.Velocity, time);
        }

        Long3 FindDirection() {
            Long3[] planeNormals = hailstones.Select(PlaneNormal).ToArray();

            for (int i = 0; i < planeNormals.Length; i++)
            for (int j = i + 1; j < planeNormals.Length; j++) {
                Long3 direction = SimplifyDirection(Long3.Cross(planeNormals[i], planeNormals[j]));
                if (direction != 0)
                    return direction;
            }

            return 0;
        }

        Long3 PlaneNormal(Hailstone hailstone) =>
            SimplifyDirection(
                Long3.Cross(
                    SimplifyDirection(hailstone.Position),
                    SimplifyDirection(hailstone.Velocity)
                )
            );
    }

    protected override long Solve(Input input) {
        Hailstone last = input.Hailstones[^1];
        return SolveThroughZero(input.Hailstones[..^1]
                                     .Select(h => new Hailstone(h.Position - last.Position,
                                                                h.Velocity - last.Velocity))
                                     .ToArray())
             + Long3.CSum(last.Position);
    }

    public static void Run() {
        new Day24B().Solve();
    }
}