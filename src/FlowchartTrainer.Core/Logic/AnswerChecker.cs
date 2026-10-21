using System.Globalization;
using FlowchartTrainer.Core.Models;

namespace FlowchartTrainer.Core.Logic;

public static class AnswerChecker
{
    public static ValidationResult ValidateInput(TrainingTask task, string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return new(false, 0, "Введи целое число x.");
        if (!TryInteger(text, out var value)) return new(false, 0, "Нужно целое число: например, 3 или −2. Дроби и буквы не подходят.");
        if (value < task.MinInput || value > task.MaxInput)
            return new(false, 0, $"Допустимы числа от {task.MinInput} до {task.MaxInput} включительно.");
        return new(true, value, "");
    }

    public static bool TryInteger(string? text, out long value) => long.TryParse(
        text?.Trim().Replace("−", "-"), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);

    public static CheckResult CheckStep(ExecutionStep step, string? answer, bool? branch) => step.AnswerKind switch
    {
        AnswerKind.Number when !TryInteger(answer, out _) => new(false, "Введи целое значение переменной."),
        AnswerKind.Number when TryInteger(answer, out var value) && value == step.ExpectedNumber
            => new(true, "Верно. Значение переменной обновлено."),
        AnswerKind.Number => new(false, "Пока неверно. Пересчитай выражение с указанными значениями."),
        AnswerKind.Branch when branch is null => new(false, "Сначала выбери «Да» или «Нет»."),
        AnswerKind.Branch when branch == step.BranchTaken => new(true, "Верно. Следуем по выбранной ветви."),
        AnswerKind.Branch => new(false, "Проверь знак сравнения. «Да» выбирают, когда условие истинно."),
        _ => new(true, "Можно перейти к следующему шагу.")
    };
}
