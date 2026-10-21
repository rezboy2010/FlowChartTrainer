using FlowchartTrainer.Core.Logic;
using FlowchartTrainer.Core.Models;

namespace FlowchartTrainer.Core.State;

public sealed class TrainingSession(TrainingTask task)
{
    public TrainingTask Task { get; private set; } = task;
    public string InputText { get; private set; } = "";
    public ExecutionTrace? Trace { get; private set; }
    public int StepIndex { get; private set; }
    public int FirstActionIndex { get; private set; }
    public int DisplayStepNumber => Math.Max(1, StepIndex - FirstActionIndex + 1);
    public int DisplayStepCount => Math.Max(1, (Trace?.Steps.Count ?? 0) - FirstActionIndex);
    public string InputError { get; private set; } = "";
    public string StepAnswer { get; set; } = "";
    public bool? SelectedBranch { get; set; }
    public CheckResult? Feedback { get; private set; }
    public bool ShowSolutions { get; set; }
    private readonly HashSet<int> _checked = [];
    public ExecutionStep? CurrentStep => Trace?.Steps[StepIndex];
    public bool IsChecked => _checked.Contains(StepIndex);
    public bool CanAdvance => CurrentStep is not null && (CurrentStep.AnswerKind == AnswerKind.None || IsChecked);
    public bool IsCompleted => Trace is not null && StepIndex == Trace.Steps.Count - 1;
    public IReadOnlyDictionary<string, long> VisibleVariables => CurrentStep is null
        ? new Dictionary<string, long>() : CanAdvance ? CurrentStep.After : CurrentStep.Before;

    public void SelectTask(TrainingTask task)
    {
        Task = task; InputText = ""; ClearTrace();
    }
    public void SetInput(string text)
    {
        if (InputText == text) return;
        InputText = text; ClearTrace();
    }
    public bool Begin()
    {
        ClearTrace();
        var validation = AnswerChecker.ValidateInput(Task, InputText);
        if (!validation.IsValid) { InputError = validation.Message; return false; }
        try
        {
            Trace = SchemeRunner.Run(Task, validation.Value);
            FirstActionIndex = Trace.Steps.TakeWhile(step => step.AnswerKind == AnswerKind.None).Count();
            return true;
        }
        catch (Exception error) when (error is OverflowException or InvalidOperationException)
        { InputError = "Не удалось выполнить схему. Проверь данные или начни заново."; return false; }
    }
    // Keep start/input in the trace, but open practice on its first question.
    public bool BeginAtFirstAction()
    {
        if (!Begin()) return false;
        StepIndex = Math.Min(FirstActionIndex, Trace!.Steps.Count - 1);
        return true;
    }
    public void Check()
    {
        if (CurrentStep is null || IsChecked) return;
        Feedback = AnswerChecker.CheckStep(CurrentStep, StepAnswer, SelectedBranch);
        if (Feedback.IsCorrect) _checked.Add(StepIndex);
    }
    public void Next()
    {
        if (!CanAdvance || IsCompleted) return;
        StepIndex++; ClearStepAnswer();
    }
    public void Back()
    {
        if (StepIndex == 0) return;
        StepIndex--; ClearStepAnswer();
    }
    public void Restart() => ClearTrace();
    private void ClearStepAnswer() { StepAnswer = ""; SelectedBranch = null; Feedback = null; }
    private void ClearTrace()
    {
        Trace = null; StepIndex = 0; FirstActionIndex = 0; InputError = ""; ShowSolutions = false;
        _checked.Clear(); ClearStepAnswer();
    }
}
