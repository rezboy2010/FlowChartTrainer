using System.Globalization;

namespace FlowchartTrainer.Core.Models;

public enum ArithmeticOperator { Add, Subtract, Multiply }
public enum ComparisonOperator { Greater, GreaterOrEqual, Less }

// An operand is either a variable or an integer. Expressions are never executed as text.
public sealed record Operand(string? Variable, long Constant)
{
    public static Operand V(string name) => new(name, 0);
    public static Operand N(long value) => new(null, value);
    public long Evaluate(IReadOnlyDictionary<string, long> values) => Variable is null
        ? Constant : values.TryGetValue(Variable, out var value) ? value
        : throw new InvalidOperationException($"Переменная {Variable} ещё не задана.");
    public string Label => Variable ?? Constant.ToString(CultureInfo.InvariantCulture);
    public string Substitute(IReadOnlyDictionary<string, long> values)
    {
        var value = Evaluate(values);
        return value < 0 ? $"({value})" : value.ToString(CultureInfo.InvariantCulture);
    }
}

public sealed record Expression(Operand Left, ArithmeticOperator Operation, Operand Right)
{
    public long Evaluate(IReadOnlyDictionary<string, long> values)
    {
        var a = Left.Evaluate(values);
        var b = Right.Evaluate(values);
        return Operation switch
        {
            ArithmeticOperator.Add => checked(a + b),
            ArithmeticOperator.Subtract => checked(a - b),
            ArithmeticOperator.Multiply => checked(a * b),
            _ => throw new InvalidOperationException("Неизвестное действие.")
        };
    }
    private string Symbol => Operation switch
    {
        ArithmeticOperator.Add => "+", ArithmeticOperator.Subtract => "−", _ => "×"
    };
    public string Label => $"{Left.Label} {Symbol} {Right.Label}";
    public string Substitute(IReadOnlyDictionary<string, long> values) =>
        $"{Left.Substitute(values)} {Symbol} {Right.Substitute(values)}";
}

public sealed record Condition(Operand Left, ComparisonOperator Operation, Operand Right)
{
    public bool Evaluate(IReadOnlyDictionary<string, long> values) => Operation switch
    {
        ComparisonOperator.Greater => Left.Evaluate(values) > Right.Evaluate(values),
        ComparisonOperator.GreaterOrEqual => Left.Evaluate(values) >= Right.Evaluate(values),
        ComparisonOperator.Less => Left.Evaluate(values) < Right.Evaluate(values),
        _ => throw new InvalidOperationException("Неизвестное сравнение.")
    };
    private string Symbol => Operation switch
    {
        ComparisonOperator.Greater => ">", ComparisonOperator.GreaterOrEqual => "≥", _ => "<"
    };
    public string Label => $"{Left.Label} {Symbol} {Right.Label}";
    public string Substitute(IReadOnlyDictionary<string, long> values) =>
        $"{Left.Substitute(values)} {Symbol} {Right.Substitute(values)}";
}
