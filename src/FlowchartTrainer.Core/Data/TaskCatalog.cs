using FlowchartTrainer.Core.Models;
using static FlowchartTrainer.Core.Models.Operand;

namespace FlowchartTrainer.Core.Data;

public static class TaskCatalog
{
    private static Expression Add(Operand a, Operand b) => new(a, ArithmeticOperator.Add, b);
    private static Expression Sub(Operand a, Operand b) => new(a, ArithmeticOperator.Subtract, b);
    private static Expression Mul(Operand a, Operand b) => new(a, ArithmeticOperator.Multiply, b);

    public static IReadOnlyList<TrainingTask> All { get; } = [CreateOne(), CreateTwo(), CreateThree()];

    private static TrainingTask CreateOne() => new(
        "one", "Одна проверка", "Разберись с границей условия", -5, 10,
        new BranchGoal("d1", true, "Подбери x, чтобы в условии y > 9 выполнилась ветвь «Да»."),
        [new StartNode("start", "input", 360, 38),
         new InputNode("input", "x", "a1", 360, 120),
         new AssignmentNode("a1", "y", Mul(N(2), V("x")), "a2", 360, 202),
         new AssignmentNode("a2", "y", Add(V("y"), N(1)), "d1", 360, 284),
         new DecisionNode("d1", new(V("y"), ComparisonOperator.Greater, N(9)), "yes1", "no1", 360, 390),
         new AssignmentNode("yes1", "y", Sub(V("y"), N(3)), "output", 172, 510),
         new AssignmentNode("no1", "y", Add(V("y"), N(2)), "output", 548, 510),
         new OutputNode("output", "y", "end", 360, 612), new EndNode("end", 360, 694)],
        OneEdges(), 740);

    private static TrainingTask CreateThree() => new(
        "three", "Заданный результат", "У задачи может быть несколько решений", 0, 12,
        new OutputGoal(9, "Подбери x, чтобы в конце схема вывела y = 9."),
        [new StartNode("start", "input", 360, 38),
         new InputNode("input", "x", "a1", 360, 120),
         new AssignmentNode("a1", "y", Mul(N(2), V("x")), "a2", 360, 202),
         new AssignmentNode("a2", "y", Add(V("y"), N(1)), "d1", 360, 284),
         new DecisionNode("d1", new(V("y"), ComparisonOperator.Greater, N(10)), "yes1", "no1", 360, 390),
         new AssignmentNode("yes1", "y", Sub(V("y"), N(4)), "output", 172, 510),
         new AssignmentNode("no1", "y", Add(V("y"), N(2)), "output", 548, 510),
         new OutputNode("output", "y", "end", 360, 612), new EndNode("end", 360, 694)],
        OneEdges(), 740);

    private static TrainingTask CreateTwo() => new(
        "two", "Два условия", "Следи за изменением y между проверками", -5, 10,
        new BranchGoal("d2", false, "Подбери x, чтобы во втором условии y < 10 выполнилась ветвь «Нет»."),
        [new StartNode("start", "input", 360, 38),
         new InputNode("input", "x", "a1", 360, 120),
         new AssignmentNode("a1", "y", Add(V("x"), N(3)), "d1", 360, 202),
         new DecisionNode("d1", new(V("y"), ComparisonOperator.GreaterOrEqual, N(4)), "yes1", "no1", 360, 308),
         new AssignmentNode("yes1", "y", Mul(V("y"), N(2)), "d2", 172, 428),
         new AssignmentNode("no1", "y", Sub(V("y"), N(2)), "d2", 548, 428),
         new DecisionNode("d2", new(V("y"), ComparisonOperator.Less, N(10)), "yes2", "no2", 360, 556),
         new AssignmentNode("yes2", "y", Add(V("y"), N(1)), "output", 172, 676),
         new AssignmentNode("no2", "y", Sub(V("y"), N(4)), "output", 548, 676),
         new OutputNode("output", "y", "end", 360, 778), new EndNode("end", 360, 860)],
        [new("start", "input", "M360 65 V93"), new("input", "a1", "M360 147 V175"),
         new("a1", "d1", "M360 229 V256"),
         new("d1", "yes1", "M268 308 H172 V401", "Да", 205, 295),
         new("d1", "no1", "M452 308 H548 V401", "Нет", 505, 295),
         new("yes1", "d2", "M172 455 V490 H360 V504"),
         new("no1", "d2", "M548 455 V490 H360 V504"),
         new("d2", "yes2", "M268 556 H172 V649", "Да", 205, 543),
         new("d2", "no2", "M452 556 H548 V649", "Нет", 505, 543),
         new("yes2", "output", "M172 703 V737 H360 V751"),
         new("no2", "output", "M548 703 V737 H360 V751"),
         new("output", "end", "M360 805 V833")], 906);

    private static IReadOnlyList<SchemeEdge> OneEdges() => [
        new("start", "input", "M360 65 V93"), new("input", "a1", "M360 147 V175"),
        new("a1", "a2", "M360 229 V257"), new("a2", "d1", "M360 311 V338"),
        new("d1", "yes1", "M268 390 H172 V483", "Да", 205, 377),
        new("d1", "no1", "M452 390 H548 V483", "Нет", 505, 377),
        new("yes1", "output", "M172 537 V571 H360 V585"),
        new("no1", "output", "M548 537 V571 H360 V585"),
        new("output", "end", "M360 639 V667")];
}
