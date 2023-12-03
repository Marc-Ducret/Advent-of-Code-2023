namespace Advent_of_Code_2023;

public static class RunAll {
    public static void Main() {
        foreach (Type type in typeof(Problem<,>).Assembly.GetTypes()
                                                .Where(IsProblemInstance)
                                                .OrderBy(t => t.Name)
                ) {
            object instance = type.GetConstructor(new Type[] { })!.Invoke(new object[] { });
            type.GetMethod(nameof(Problem<string, string>.Solve))!.Invoke(instance, new object[] { });
        }
    }

    private static bool IsProblemInstance(Type type) =>
        type.BaseType is { IsGenericType: true } baseType
     && baseType.GetGenericTypeDefinition() == typeof(Problem<,>);
}