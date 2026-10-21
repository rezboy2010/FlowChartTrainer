namespace FlowchartTrainer.Core.Models;

public abstract record SchemeNode(string Id, string Label, int X, int Y)
{
    public virtual int Width => 200;
    public virtual int Height => 54;
}
public sealed record StartNode(string Id, string NextId, int X, int Y)
    : SchemeNode(Id, "Начало", X, Y);
public sealed record InputNode(string Id, string Variable, string NextId, int X, int Y)
    : SchemeNode(Id, $"Ввод {Variable}", X, Y);
public sealed record AssignmentNode(string Id, string Variable, Expression Expression, string NextId, int X, int Y)
    : SchemeNode(Id, $"{Variable} = {Expression.Label}", X, Y);
public sealed record DecisionNode(string Id, Condition Condition, string YesId, string NoId, int X, int Y)
    : SchemeNode(Id, Condition.Label, X, Y)
{
    public override int Width => 184;
    public override int Height => 104;
}
public sealed record OutputNode(string Id, string Variable, string NextId, int X, int Y)
    : SchemeNode(Id, $"Вывод {Variable}", X, Y);
public sealed record EndNode(string Id, int X, int Y) : SchemeNode(Id, "Конец", X, Y);

public sealed record SchemeEdge(string FromId, string ToId, string Path, string? Label = null,
    int LabelX = 0, int LabelY = 0);

public abstract record TaskGoal(string Description);
public sealed record BranchGoal(string DecisionId, bool ExpectedBranch, string Description)
    : TaskGoal(Description);
public sealed record OutputGoal(long ExpectedOutput, string Description) : TaskGoal(Description);

public sealed record TrainingTask(string Id, string Title, string Subtitle, int MinInput, int MaxInput,
    TaskGoal Goal, IReadOnlyList<SchemeNode> Nodes, IReadOnlyList<SchemeEdge> Edges, int DiagramHeight)
{
    public string RangeDescription => $"Целое x от {MinInput} до {MaxInput} включительно";
}

public enum AnswerKind { None, Number, Branch }
public sealed record ExecutionStep(string NodeId, string Title, string Formula, string Substitution,
    string Explanation, IReadOnlyDictionary<string, long> Before, IReadOnlyDictionary<string, long> After,
    string? NextNodeId, AnswerKind AnswerKind = AnswerKind.None, long? ExpectedNumber = null,
    bool? BranchTaken = null);
public sealed record ExecutionTrace(IReadOnlyList<ExecutionStep> Steps, long Output, bool GoalReached);
public sealed record ValidationResult(bool IsValid, long Value, string Message);
public sealed record CheckResult(bool IsCorrect, string Message);
