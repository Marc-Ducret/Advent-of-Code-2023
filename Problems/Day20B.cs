using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day20B : Problem<Day20B.Input, Day20B.Output> {
    public readonly record struct Input(Machine Broadcast, Machine Output, Machine[] Machines);

    public readonly record struct Output(int[] Periods) {
        public ulong Result => Periods.Aggregate(1UL, (a, b) => IntMath.LCM(a, (ulong)b));
        
        public override string ToString() => $"{string.Join(",", Periods)}->{Result}";
    }

    public abstract class Machine(string name) {
        public string        Name    { get; } = name;
        public List<Machine> Inputs  { get; } = [];
        public List<Machine> Outputs { get; } = [];

        public override    string ToString() => TypePrefix() + Name;
        protected abstract string TypePrefix();
        public abstract    void   Init();
        public abstract    void   Reset();
        public abstract    bool?  ReceivePulse(Machine input, bool pulse);

        public class Broadcast(string name) : Machine(name) {
            protected override string TypePrefix() => "";

            public override void Init()  { }
            public override void Reset() { }

            public override bool? ReceivePulse(Machine input, bool pulse) =>
                pulse;
        }

        public class Toggle(string name) : Machine(name) {
            private bool state;

            protected override string TypePrefix() => "%";

            public override void Init() { }

            public override void Reset() {
                state = false;
            }

            public override bool? ReceivePulse(Machine input, bool pulse) =>
                pulse ? null : state = !state;
        }

        public class NAnd(string name) : Machine(name) {
            private Dictionary<Machine, bool> memory = [];

            protected override string TypePrefix() => "&";

            public override void Init() {
                memory = Inputs.ToDictionary(m => m, _ => false);
            }

            public override void Reset() {
                foreach (Machine input in Inputs) {
                    memory[input] = false;
                }
            }

            public override bool? ReceivePulse(Machine input, bool pulse) {
                memory[input] = pulse;
                return !memory.Values.All(x => x);
            }
        }

        public class Null(string name) : Machine(name) {
            protected override string TypePrefix() => "";

            public override void Init()  { }
            public override void Reset() { }

            public override bool? ReceivePulse(Machine input, bool pulse) => null;
        }
    }

    protected override Input PreProcess(string input) {
        Dictionary<string, Machine> machinesByName;

        {
            Dictionary<string, Type> machineTypes = [];
            foreach (string line in input.Split('\n')) {
                Match match = WiringRegex().Match(line);
                if (!match.Success) throw new ArgumentException("Invalid wiring.");

                machineTypes[match.Groups["name"].Value] =
                    match.Groups["name"].Value == "broadcaster"
                        ? typeof(Machine.Broadcast)
                        : match.Groups["type"].Success
                            ? match.Groups["type"].Value switch {
                                "%" => typeof(Machine.Toggle),
                                "&" => typeof(Machine.NAnd),
                                _   => throw new ArgumentOutOfRangeException()
                            }
                            : typeof(Machine.Null);

                foreach (string outputName in match.Groups["outputs"].Value.Split(", ")) {
                    machineTypes.TryAdd(outputName, typeof(Machine.Null));
                }
            }

            machinesByName = machineTypes.ToDictionary(entry => entry.Key,
                                                       entry => (Machine)entry.Value
                                                                              .GetConstructor(new[] { typeof(string) })!
                                                                              .Invoke(new object?[] { entry.Key }));
        }

        foreach (string line in input.Split('\n')) {
            Match match = WiringRegex().Match(line);
            if (!match.Success) throw new ArgumentException("Invalid wiring.");

            Machine inputMachine = machinesByName[match.Groups["name"].Value];

            foreach (string outputName in match.Groups["outputs"].Value.Split(", ")) {
                Machine outputMachine = machinesByName[outputName];
                inputMachine.Outputs.Add(outputMachine);
                outputMachine.Inputs.Add(inputMachine);
            }
        }

        foreach (Machine machine in machinesByName.Values) {
            machine.Init();
        }

        return new Input(machinesByName["broadcaster"], machinesByName["rx"], machinesByName.Values.ToArray());
    }

    protected override Output Solve(Input input) {
        foreach (Machine machine in input.Machines) {
            machine.Reset();
        }
        
        int       pressCount  = 0;
        const int sampleCount = 16;

        Dictionary<Machine, List<(int pressCount, bool value)>> monitor =
            input.Output.Inputs.SelectMany(m => m.Inputs)
                 .ToDictionary(m => m, _ => new List<(int, bool)>());

        Queue<(Machine inputMachine, bool pulse, Machine outputMachine)> queue = [];
        while (monitor.Values.Min(e => e.Count < sampleCount)) {
            pressCount++;
            queue.Enqueue((input.Broadcast, false, input.Broadcast));

            while (queue.TryDequeue(out var signal)) {
                if (monitor.TryGetValue(signal.inputMachine, out var events)) {
                    if (signal.pulse)
                        events.Add((pressCount, signal.pulse));
                }

                if (signal.outputMachine.ReceivePulse(signal.inputMachine, signal.pulse) is { } response)
                    foreach (Machine output in signal.outputMachine.Outputs) {
                        queue.Enqueue((signal.outputMachine, response, output));
                    }
            }
        }

        int[] periods = monitor.Values.Select(e => e[0].pressCount).ToArray();
        foreach (var ((machine, events), period) in monitor.Zip(periods)) {
            int expected = period;
            foreach (var @event in events.Distinct()) {
                if (@event.pressCount != expected)
                    throw new ArgumentException("Aperiodic machine.");
                expected += period;
            }
        }

        return new Output(periods);
    }

    public static void Run() {
        new Day20B().Solve();
    }

    [GeneratedRegex(@"(?<type>[%&])?(?<name>[\w]+) -> (?<outputs>[\w\s,]+)")]
    private static partial Regex WiringRegex();
}