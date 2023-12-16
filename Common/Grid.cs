using System.Text;

namespace Advent_of_Code_2023;

public readonly struct Grid<T> : IEquatable<Grid<T>>
    where T : struct, IEquatable<T>, IGridElement {
    public  Int2 Size   { get; }
    private T[,] Values { get; }

    public Grid(Int2 size) {
        Size   = size;
        Values = new T[size.X, size.Y];
    }

    public bool IsWithin(in Int2 position) =>
        ((position >= 0) & (position < Size)).All;

    public T this[in Int2 position] {
        get =>
            IsWithin(position)
                ? Values[position.X, position.Y]
                : default;
        set {
            if (IsWithin(position))
                Values[position.X, position.Y] = value;
        }
    }

    public IEnumerable<Int2> Positions() {
        Int2 size = Size;
        return Enumerable.Range(0, size.Y)
                         .SelectMany(
                              y => Enumerable.Range(0, size.X)
                                             .Select(x => new Int2(x, y)));
    }

    public string DebugString {
        get {
            StringBuilder builder = new();
            for (int y = Size.Y - 1; y >= 0; y--) {
                for (int x = 0; x < Size.X; x++) {
                    builder.Append(this[new Int2(x, y)].DebugChar());
                }

                builder.Append('\n');
            }

            return builder.ToString();
        }
    }

    public Grid<T> Copy() {
        Grid<T> grid = new(Size);
        foreach (Int2 position in Positions()) {
            grid[position] = this[position];
        }

        return grid;
    }

    public override int GetHashCode() {
        HashCode hash = new();
        foreach (Int2 position in Positions()) {
            hash.Add(this[position]);
        }

        return hash.ToHashCode();
    }

    public bool Equals(Grid<T> other) {
        foreach (Int2 position in Positions()) {
            if (!this[position].Equals(other[position]))
                return false;
        }

        return true;
    }

    public override bool Equals(object?      obj)                 => obj is Grid<T> other && Equals(other);
    public static   bool operator ==(Grid<T> left, Grid<T> right) => left.Equals(right);
    public static   bool operator !=(Grid<T> left, Grid<T> right) => !(left == right);

    public static Grid<T> Parse(string input) {
        string[] lines = input.Split('\n');
        Grid<T>  grid  = new(new Int2(lines.Min(l => l.Length), lines.Length));

        foreach (Int2 position in grid.Positions()) {
            T value = default;
            value.Parse(lines[grid.Size.Y - position.Y - 1][position.X]);
            grid[position] = value;
        }

        return grid;
    }
}