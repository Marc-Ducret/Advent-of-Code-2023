namespace Advent_of_Code_2023;

public class Day25A : Problem<Day25A.Input, int> {
    public readonly record struct Input(Node[] Nodes);

    public record Node(string Name) {
        public List<Node> connected = [];

        public override string ToString() => Name;
    }

    protected override Input PreProcess(string input) {
        Dictionary<string, Node> nodes = [];

        foreach (string line in input.Split('\n')) {
            string[] parts  = line.Split(": ");
            Node     source = Node(parts[0]);
            foreach (Node target in parts[1].Split(' ').Select(Node)) {
                source.connected.Add(target);
                target.connected.Add(source);
            }
        }

        return new Input(nodes.Values.ToArray());

        Node Node(string name) {
            if (!nodes.TryGetValue(name, out Node? node)) {
                node = new Node(name);
                nodes.Add(name, node);
            }

            return node;
        }
    }

    private readonly record struct Group(Node[] Nodes);

    private (Group, Group) RandomCut(Node[] nodes, (Node a, Node b)[] edges) {
        Dictionary<Node, Node> groups      = nodes.Select(n => (n, n)).ToDictionary();
        int                    groupsCount = nodes.Length;

        Random random = new();

        {
            while (groupsCount > 2)
                Contract(RandomEdge());

            Group[] remaining = groups.GroupBy(x => Find(x.Key), x => x.Key).Select(x => new Group(x.ToArray())).ToArray();
            return (remaining[0], remaining[1]);
        }

        (Node, Node) RandomEdge() {
            while (true) {
                (Node a, Node b) = edges[random.Next(edges.Length)];

                a = Find(a);
                b = Find(b);
                if (a != b) return (a, b);
            }
        }

        void Union(Node a, Node b) {
            a = Find(a);
            b = Find(b);

            if (a == b) throw new Exception("Unexpected");
            groups[b] = a;
        }

        Node Find(Node node) {
            while (node != groups[node])
                (node, groups[node]) = (groups[node], groups[groups[node]]);

            return node;
        }

        void Contract((Node a, Node b) edge) {
            Union(edge.a, edge.b);
            groupsCount--;
        }
    }

    private int ConnectionsBetween(Group a, Group b) =>
        a.Nodes.Sum(n => n.connected.Intersect(b.Nodes).Count());

    protected override int Solve(Input input) {
        Node[]         nodes = input.Nodes;
        (Node, Node)[] edges = nodes.SelectMany(n => n.connected.Select(c => (n, c))).ToArray();

        Group a, b;
        int   connectionsBetween;

        do {
            (a, b)             = RandomCut(nodes, edges);
            connectionsBetween = ConnectionsBetween(a, b);
        } while (connectionsBetween > 3);

        return a.Nodes.Length * b.Nodes.Length;
    }

    public static void Run() {
        new Day25A().Solve();
    }
}