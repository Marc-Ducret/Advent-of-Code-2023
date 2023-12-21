using System.Text.RegularExpressions;

namespace Advent_of_Code_2023;

public partial class Day20A : Problem<Day20A.Input, int> {
    public readonly record struct Input(Machine Broadcast);

    public abstract class Machine(string name) {
        public string        Name    { get; } = name;
        public List<Machine> Inputs  { get; } = [];
        public List<Machine> Outputs { get; } = [];

        public abstract void  Init();
        public abstract bool? ReceivePulse(Machine input, bool pulse);

        public class Broadcast(string name) : Machine(name) {
            public override void Init() { }

            public override bool? ReceivePulse(Machine input, bool pulse) =>
                pulse;
        }

        public class Toggle(string name) : Machine(name) {
            private bool state = false;

            public override void Init() { }

            public override bool? ReceivePulse(Machine input, bool pulse) =>
                pulse ? null : state = !state;
        }

        public class NAnd(string name) : Machine(name) {
            private Dictionary<Machine, bool> memory = [];

            public override void Init() {
                memory = Inputs.ToDictionary(m => m, _ => false);
            }

            public override bool? ReceivePulse(Machine input, bool pulse) {
                memory[input] = pulse;
                return !memory.Values.All(x => x);
            }
        }

        public class Null(string name) : Machine(name) {
            public override void Init() { }

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

        return new Input(machinesByName["broadcaster"]);
    }

    protected override int Solve(Input input) {
        int lowCount  = 0;
        int highCount = 0;

        Queue<(Machine inputMachine, bool pulse, Machine outputMachine)> queue = [];
        for (int i = 0; i < 1_000; i++) {
            queue.Enqueue((input.Broadcast, false, input.Broadcast));

            while (queue.TryDequeue(out var signal)) {
                if (signal.pulse) highCount++;
                else lowCount++;
                if (signal.outputMachine.ReceivePulse(signal.inputMachine, signal.pulse) is { } response)
                    foreach (Machine output in signal.outputMachine.Outputs) {
                        queue.Enqueue((signal.outputMachine, response, output));
                    }
            }
        }

        return lowCount * highCount;
    }

    public static void Run() {
        new Day20A().Solve();
    }

    [GeneratedRegex(@"(?<type>[%&])?(?<name>[\w]+) -> (?<outputs>[\w\s,]+)")]
    private static partial Regex WiringRegex();
}