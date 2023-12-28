using System.Collections;

namespace Advent_of_Code_2023;

public class Day23B : Problem<Day23B.Input, int> {
    public readonly record struct Input(Grid<TileElement> Grid);

    public enum Tile {
        FOREST,
        PATH,
        EAST,
        NORTH,
        WEST,
        SOUTH
    }

    public record struct TileElement(Tile Value) : IGridElement {
        private static readonly (char @char, Tile tile)[] charsPairings = [
            ('#', Tile.FOREST),
            ('.', Tile.PATH),
            ('>', Tile.EAST),
            ('^', Tile.NORTH),
            ('<', Tile.WEST),
            ('v', Tile.SOUTH)
        ];

        private static readonly Dictionary<char, Tile> parse =
            charsPairings.ToDictionary();

        private static readonly Dictionary<Tile, char> debug =
            charsPairings.ToDictionary(pair => pair.tile, pair => pair.@char);

        public char DebugChar()   => debug.GetValueOrDefault(Value, '?');
        public void Parse(char c) => Value = parse[c];

        public static implicit operator Tile(TileElement element) => element.Value;
        public static implicit operator TileElement(Tile value)   => new(value);
    }

    protected override Input PreProcess(string input) {
        return new Input(Grid<TileElement>.Parse(input));
    }

    private static Int2[] pathOffsets = [
        new Int2(+1, +0),
        new Int2(+0, +1),
        new Int2(-1, +0),
        new Int2(+0, -1)
    ];

    private static Int2[] eastOffsets  = [new Int2(+1, +0)];
    private static Int2[] northOffsets = [new Int2(+0, +1)];
    private static Int2[] westOffsets  = [new Int2(-1, +0)];
    private static Int2[] southOffsets = [new Int2(+0, -1)];

    private Int2[] Offsets(Tile tile) =>
        tile switch {
            Tile.FOREST => Array.Empty<Int2>(),
            Tile.PATH   => pathOffsets,
            Tile.EAST   => pathOffsets,
            Tile.NORTH  => pathOffsets,
            Tile.WEST   => pathOffsets,
            Tile.SOUTH  => pathOffsets,
            _           => throw new ArgumentOutOfRangeException()
        };

    private enum Action {
        START_VISIT,
        END_VISIT
    }

    private class Node(Int2 position) {
        public Int2                          position = position;
        public List<(Node node, int length)> edges    = [];

        public override string ToString() => position.ToString();
    }

    protected override int Solve(Input input) {
        Int2 start = default;
        Int2 end   = default;

        for (int x = 0; x < input.Grid.Size.X; x++) {
            if (input.Grid[new Int2(x, input.Grid.Size.Y - 1)] == Tile.PATH)
                start = new Int2(x, input.Grid.Size.Y - 1);
            if (input.Grid[new Int2(x, 0)] == Tile.PATH)
                end = new Int2(x, 0);
        }

        Dictionary<Int2, Node> nodes = input.Grid.Positions()
                                            .Where(p => input.Grid[p] != Tile.FOREST)
                                            .Where(p => Offsets(input.Grid[p])
                                                       .Select(o => p + o)
                                                       .Count(n => input.Grid[n] != Tile.FOREST) != 2)
                                            .Select(p => (p, new Node(p)))
                                            .ToDictionary();

        foreach (Node node in nodes.Values) {
            foreach (Int2 offset in pathOffsets) {
                Int2 neighbor = node.position + offset;
                if (input.Grid[neighbor] != Tile.FOREST)
                    node.edges.Add(FollowEdge(neighbor, node.position, 1));
            }
        }
        
        return Visit(new HashSet<Node>(), nodes[start]) ?? -1;

        int? Visit(ISet<Node> visited, Node node) {
            if (!visited.Add(node)) return null;

            int? length;
            if (node.position == end) length = 0;
            else {
                length = null;
                foreach (var edge in node.edges) {
                    if (Visit(visited, edge.node) is { } offsetLength) {
                        int lengthUsingThisEdge = edge.length + offsetLength;
                        if (length is not { } currentLength || currentLength < lengthUsingThisEdge)
                            length = lengthUsingThisEdge;
                    }
                }
            }

            visited.Remove(node);

            return length;
        }

        (Node node, int length) FollowEdge(Int2 position, Int2 from, int currentDistance) {
            if (nodes.TryGetValue(position, out Node? node)) return (node, currentDistance);
            foreach (Int2 offset in pathOffsets) {
                Int2 neighbor = position + offset;
                if (neighbor             != from
                 && input.Grid[neighbor] != Tile.FOREST)
                    return FollowEdge(neighbor, position, currentDistance + 1);
            }

            throw new Exception("Unexpected");
        }
    }

    public static void Run() {
        new Day23B().Solve();
    }
}