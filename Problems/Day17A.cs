namespace Advent_of_Code_2023;

public class Day17A : Problem<Day17A.Input, int> {
    public readonly record struct Input(Grid<DigitElement> Grid);

    public record struct DigitElement(byte Value) : IGridElement {
        public char DebugChar()   => (char)('0' + Value);
        public void Parse(char c) => Value = (byte)(c - '0');

        public static implicit operator byte(DigitElement element) => element.Value;
        public static implicit operator DigitElement(byte value)   => new(value);
    }

    protected override Input PreProcess(string input) =>
        new(Grid<DigitElement>.Parse(input));

    private readonly record struct State(Int2 Position, Int2 Direction, int ForwardCount);

    private State Neighbor(in State state, in Int2 direction) =>
        new(state.Position + direction,
            direction,
            direction == state.Direction
                ? state.ForwardCount + 1
                : 1);

    private void GetNeighbors(in State state, List<State> neighbors) {
        neighbors.Clear();
        if (state.ForwardCount < 3)
            neighbors.Add(Neighbor(state, state.Direction));
        neighbors.Add(Neighbor(state, IntMath.RotateLeft(state.Direction)));
        neighbors.Add(Neighbor(state, IntMath.RotateRight(state.Direction)));
    }

    protected override int Solve(Input input) {
        Int2 start = new(0, input.Grid.Size.Y - 1);
        Int2 end   = new(input.Grid.Size.X    - 1, 0);

        HashSet<State> visited = [];

        PriorityQueue<State, int> toVisit = new();
        toVisit.Enqueue(new State(start, new Int2(+1, +0), 0), 0);

        List<State> neighbors = [];

        while (toVisit.TryDequeue(out State state, out int distance)) {
            if (state.Position == end) return distance;
            if (!visited.Add(state)) continue;

            GetNeighbors(state, neighbors);
            foreach (State neighbor in neighbors) {
                if (input.Grid.IsWithin(neighbor.Position)
                 && !visited.Contains(neighbor))
                    toVisit.Enqueue(neighbor, distance + input.Grid[neighbor.Position]);
            }
        }

        return -1;
    }

    public static void Run() {
        new Day17A().Solve();
    }
}