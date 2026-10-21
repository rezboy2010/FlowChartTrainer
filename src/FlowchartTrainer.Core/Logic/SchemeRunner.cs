using FlowchartTrainer.Core.Models;

namespace FlowchartTrainer.Core.Logic;

public static class SchemeRunner
{
    // The same trace drives the diagram, variable table, explanations and answer checks.
    public static ExecutionTrace Run(TrainingTask task, long input)
    {
        if (input < task.MinInput || input > task.MaxInput)
            throw new ArgumentOutOfRangeException(nameof(input), "Число вне диапазона задачи.");
        var nodes = task.Nodes.ToDictionary(node => node.Id);
        var visited = new HashSet<string>();
        var values = new Dictionary<string, long>();
        var steps = new List<ExecutionStep>();
        string? current = "start";
        long? output = null;
        while (current is not null)
        {
            if (!visited.Add(current)) throw new InvalidOperationException("В схеме обнаружен цикл.");
            if (!nodes.TryGetValue(current, out var node)) throw new InvalidOperationException("Переход ведёт в отсутствующий блок.");
            var before = new Dictionary<string, long>(values);
            string? next;
            string title, formula = node.Label, substitution = "", explanation;
            var answerKind = AnswerKind.None;
            long? expected = null;
            bool? branch = null;
            switch (node)
            {
                case StartNode start:
                    next = start.NextId; title = "Начинаем выполнение";
                    explanation = "Двигаемся по стрелке к вводу. Значения переменных пока не заданы.";
                    break;
                case InputNode enter:
                    values[enter.Variable] = input; next = enter.NextId; title = "Считываем входное число";
                    substitution = $"{enter.Variable} = {input}";
                    explanation = $"Сохраняем выбранное число {input} в переменной {enter.Variable}.";
                    break;
                case AssignmentNode assignment:
                    expected = assignment.Expression.Evaluate(values);
                    substitution = $"{assignment.Variable} = {assignment.Expression.Substitute(values)}";
                    values[assignment.Variable] = expected.Value;
                    next = assignment.NextId; title = $"Вычисляем {assignment.Variable}";
                    answerKind = AnswerKind.Number;
                    explanation = $"Сначала считаем правую часть, затем записываем {expected} в {assignment.Variable}. Старое значение заменяется новым.";
                    break;
                case DecisionNode decision:
                    branch = decision.Condition.Evaluate(values);
                    next = branch.Value ? decision.YesId : decision.NoId;
                    title = "Выбираем следующую ветвь";
                    substitution = decision.Condition.Substitute(values);
                    answerKind = AnswerKind.Branch;
                    explanation = $"{substitution} — {(branch.Value ? "истина" : "ложь")}. Идём по ветви «{(branch.Value ? "Да" : "Нет")}». Значения переменных не меняются.";
                    break;
                case OutputNode show:
                    output = values[show.Variable]; next = show.NextId;
                    title = "Получаем результат"; substitution = $"{show.Variable} = {output}";
                    explanation = $"Схема выводит текущее значение {show.Variable}: {output}.";
                    break;
                case EndNode:
                    next = null; title = "Выполнение завершено";
                    explanation = "Все действия на выбранном пути выполнены. Сравниваем пройденную ветвь или результат с целью задачи.";
                    break;
                default: throw new InvalidOperationException("Неизвестный тип блока.");
            }
            steps.Add(new(current, title, formula, substitution, explanation, before,
                new Dictionary<string, long>(values), next, answerKind, expected, branch));
            current = next;
        }
        if (output is null || steps[^1].NodeId != "end")
            throw new InvalidOperationException("Схема должна завершиться выводом и блоком «Конец».");
        var goalReached = task.Goal switch
        {
            BranchGoal goal => steps.Any(step => step.NodeId == goal.DecisionId && step.BranchTaken == goal.ExpectedBranch),
            OutputGoal goal => output == goal.ExpectedOutput,
            _ => false
        };
        return new(steps.AsReadOnly(), output.Value, goalReached);
    }

    public static IReadOnlyList<long> FindSolutions(TrainingTask task) =>
        Enumerable.Range(task.MinInput, task.MaxInput - task.MinInput + 1)
            .Select(value => (long)value).Where(value => Run(task, value).GoalReached).ToArray();
}
