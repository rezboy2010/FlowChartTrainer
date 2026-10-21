using FlowchartTrainer.Core.Data;

namespace FlowchartTrainer.Core.State;

public sealed class QuizSession
{
    public int Index { get; private set; }
    public int? SelectedOption { get; set; }
    public bool IsChecked { get; private set; }
    public int Score { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool IsCorrect => SelectedOption == Current.CorrectIndex;
    public QuizQuestion Current => QuizCatalog.All[Index];
    public void Check()
    {
        if (IsChecked || IsCompleted || SelectedOption is null || SelectedOption < 0 || SelectedOption >= Current.Options.Count) return;
        IsChecked = true;
        if (IsCorrect) Score++;
    }
    public void Next()
    {
        if (!IsChecked) return;
        if (Index == QuizCatalog.All.Count - 1) { IsCompleted = true; return; }
        Index++; SelectedOption = null; IsChecked = false;
    }
    public void Reset() { Index = 0; Score = 0; SelectedOption = null; IsChecked = false; IsCompleted = false; }
}
