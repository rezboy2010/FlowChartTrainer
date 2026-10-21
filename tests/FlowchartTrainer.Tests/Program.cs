using System.Text.Json;
using FlowchartTrainer.Core.Data;
using FlowchartTrainer.Core.Logic;
using FlowchartTrainer.Core.Models;
using FlowchartTrainer.Core.State;

var results = new List<TestRecord>();
var one = TaskCatalog.All[0];
var two = TaskCatalog.All[1];
var three = TaskCatalog.All[2];

void Test(string name, string expected, Func<string> action)
{
    string actual;
    try { actual = action(); }
    catch (Exception e) { actual = $"EXCEPTION {e.GetType().Name}: {e.Message}"; }
    var passed = actual == expected;
    results.Add(new(name, expected, actual, passed));
    Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {name}: {actual}");
}

Test("Обычные данные: схема 1, x = 5", "y=8; цель=True", () =>
{
    var trace = SchemeRunner.Run(one, 5); return $"y={trace.Output}; цель={trace.GoalReached}";
});
Test("Граница строгого сравнения: схема 1, x = 4", "9 > 9: False; y=11; цель=False", () =>
{
    var trace = SchemeRunner.Run(one, 4); return $"9 > 9: {trace.Steps.Single(s => s.NodeId == "d1").BranchTaken}; y={trace.Output}; цель={trace.GoalReached}";
});
Test("Граница ≥ и обновление y: схема 2, x = 1", "первое=True; второе=True; y=9; цель=False", () =>
{
    var trace = SchemeRunner.Run(two, 1);
    return $"первое={trace.Steps.Single(s => s.NodeId == "d1").BranchTaken}; второе={trace.Steps.Single(s => s.NodeId == "d2").BranchTaken}; y={trace.Output}; цель={trace.GoalReached}";
});
Test("Граница второго условия: схема 2, x = 2", "10 < 10: False; y=6; цель=True", () =>
{
    var trace = SchemeRunner.Run(two, 2); return $"10 < 10: {trace.Steps.Single(s => s.NodeId == "d2").BranchTaken}; y={trace.Output}; цель={trace.GoalReached}";
});
Test("Все подходящие числа первой схемы", "5,6,7,8,9,10", () => string.Join(",", SchemeRunner.FindSolutions(one)));
Test("Все подходящие числа второй схемы", "2,3,4,5,6,7,8,9,10", () => string.Join(",", SchemeRunner.FindSolutions(two)));
Test("Несколько решений третьей схемы", "3,6", () => string.Join(",", SchemeRunner.FindSolutions(three)));
Test("Одинаковый результат по разным ветвям", "9;9;False;True", () =>
{
    var a = SchemeRunner.Run(three, 3); var b = SchemeRunner.Run(three, 6);
    return $"{a.Output};{b.Output};{a.Steps.Single(s => s.NodeId == "d1").BranchTaken};{b.Steps.Single(s => s.NodeId == "d1").BranchTaken}";
});
Test("Неправильный ввод: пусто, дроби, текст, диапазон, переполнение", "8 отклонено", () =>
{
    string[] inputs = ["", " ", "3.5", "3,5", "abc", "11", "-6", "99999999999999999999999999"];
    return $"{inputs.Count(x => !AnswerChecker.ValidateInput(one, x).IsValid)} отклонено";
});
Test("Допустимы обе границы и знак минуса Unicode", "-5,10,-2", () =>
    string.Join(",", new[] { "-5", "10", "−2" }.Select(x => AnswerChecker.ValidateInput(one, x)).Where(r => r.IsValid).Select(r => r.Value)));
Test("Модель повторно отклоняет число вне диапазона", "ArgumentOutOfRangeException", () =>
{
    try { SchemeRunner.Run(one, 11); return "accepted"; }
    catch (ArgumentOutOfRangeException) { return "ArgumentOutOfRangeException"; }
});
Test("Неверный шаг не изменяет видимые переменные", "False;False;False", () =>
{
    var s = new TrainingSession(one); s.SetInput("5"); s.Begin(); s.Next(); s.Next();
    s.StepAnswer = "999"; s.Check();
    return $"{s.IsChecked};{s.CanAdvance};{s.VisibleVariables.ContainsKey("y")}";
});
Test("Проверка арифметики, обновление состояния и снимок до/после", "True;10;False;10", () =>
{
    var s = new TrainingSession(one); s.SetInput("5"); s.Begin(); s.Next(); s.Next(); s.StepAnswer="10"; s.Check();
    var checkedStep=s.CurrentStep!;
    return $"{s.IsChecked};{s.VisibleVariables["y"]};{checkedStep.Before.ContainsKey("y")};{checkedStep.After["y"]}";
});
Test("Нельзя перейти дальше до правильного ответа", "2", () =>
{
    var s = new TrainingSession(one); s.SetInput("5"); s.Begin(); s.Next(); s.Next(); s.Next(); return s.StepIndex.ToString();
});
Test("Проверка ветви учитывает выбор ученика", "False;True", () =>
{
    var step=SchemeRunner.Run(one, 5).Steps.Single(s => s.NodeId == "d1");
    return $"{AnswerChecker.CheckStep(step, null, false).IsCorrect};{AnswerChecker.CheckStep(step, null, true).IsCorrect}";
});
Test("Изменение параметра сбрасывает разбор и подсказки", "True;0;False;;6", () =>
{
    var s=new TrainingSession(three); s.SetInput("3"); s.Begin(); s.Next(); s.ShowSolutions=true; s.SetInput("6");
    return $"{s.Trace is null};{s.StepIndex};{s.ShowSolutions};{s.StepAnswer};{s.InputText}";
});
Test("Смена задачи очищает прежние данные", "two;True;;0", () =>
{
    var s=new TrainingSession(one); s.SetInput("5"); s.Begin(); s.Next(); s.SelectTask(two);
    return $"{s.Task.Id};{s.Trace is null};{s.InputText};{s.StepIndex}";
});
Test("Повторный запуск даёт тот же результат и первый шаг", "True;0;8", () =>
{
    var s=new TrainingSession(one); s.SetInput("5"); s.Begin(); var original=s.Trace!.Output; s.Next(); s.Begin();
    return $"{s.Trace!.Output == original};{s.StepIndex};{s.Trace.Output}";
});
Test("Новая сессия после перезагрузки пуста", "True;;0", () =>
{
    var fresh=new TrainingSession(one); return $"{fresh.Trace is null};{fresh.InputText};{fresh.StepIndex}";
});
Test("Возврат к прошлому шагу не портит снимки", "False;10;5", () =>
{
    var s=new TrainingSession(one); s.SetInput("5"); s.Begin(); s.Next(); s.Next(); s.StepAnswer="10"; s.Check(); s.Next(); s.Back();
    return $"{s.CurrentStep!.Before.ContainsKey("y")};{s.CurrentStep.After["y"]};{s.CurrentStep.Before["x"]}";
});
Test("Целочисленная арифметика обнаруживает переполнение", "OverflowException", () =>
{
    try { new Expression(Operand.N(long.MaxValue), ArithmeticOperator.Add, Operand.N(1)).Evaluate(new Dictionary<string,long>()); return "wrapped"; }
    catch(OverflowException){return "OverflowException";}
});
Test("Защита от цикла в ошибочной схеме", "InvalidOperationException", () =>
{
    var bad=one with { Nodes=[new StartNode("start", "start", 0, 0)] };
    try { SchemeRunner.Run(bad, 5); return "accepted"; }
    catch(InvalidOperationException){return "InvalidOperationException";}
});
Test("Защита от отсутствующего блока", "InvalidOperationException", () =>
{
    var bad=one with { Nodes=[new StartNode("start", "missing", 0, 0)] };
    try { SchemeRunner.Run(bad, 5); return "accepted"; }
    catch(InvalidOperationException){return "InvalidOperationException";}
});
Test("Все входные числа сверены с независимыми формулами", "45 совпадений", () =>
{
    var count=0;
    foreach(var task in TaskCatalog.All)
    for(var x=task.MinInput;x<=task.MaxInput;x++)
    {
        long y; bool goal;
        if(task.Id=="one") { y=2*x+1; goal=y>9; y=goal?y-3:y+2; }
        else if(task.Id=="two") { y=x+3; y=y>=4?2*y:y-2; goal=y>=10; y=y<10?y+1:y-4; }
        else { y=2*x+1; y=y>10?y-4:y+2; goal=y==9; }
        var trace=SchemeRunner.Run(task,x);
        if(trace.Output!=y || trace.GoalReached!=goal) return $"ошибка: {task.Id}, {x}";
        if(trace.Steps.Any(step=>step.BranchTaken is not null && !step.Before.OrderBy(k=>k.Key).SequenceEqual(step.After.OrderBy(k=>k.Key)))) return "условие изменило значения";
        count++;
    }
    return $"{count} совпадений";
});
Test("Тест: двойное нажатие не увеличивает оценку", "1;True", () =>
{
    var q=new QuizSession(); q.SelectedOption=q.Current.CorrectIndex; q.Check(); q.Check(); return $"{q.Score};{q.IsChecked}";
});
Test("Тест: переход требует проверенного ответа", "0", () => {var q=new QuizSession();q.Next();return q.Index.ToString();});
Test("Тест: завершение и повторный запуск", "5;True;0;False", () =>
{
    var q=new QuizSession(); for(var i=0;i<5;i++){q.SelectedOption=q.Current.CorrectIndex;q.Check();q.Next();}
    var score=q.Score;var completed=q.IsCompleted;q.Reset();return $"{score};{completed};{q.Score};{q.IsCompleted}";
});

Test("Практика начинается с вычисления, x уже задан", "a1;1;6;9;False", () =>
{
    var s=new TrainingSession(one); s.SetInput("9"); s.BeginAtFirstAction();
    return $"{s.CurrentStep!.NodeId};{s.DisplayStepNumber};{s.DisplayStepCount};{s.VisibleVariables["x"]};{s.CanAdvance}";
});
Test("Новый запуск не пропускает ошибку ввода", "False;True", () =>
{
    var s=new TrainingSession(one); s.SetInput("3.5");
    return $"{s.BeginAtFirstAction()};{s.Trace is null}";
});
Test("Обозначения присваивания одинаковы в схеме и подстановке", "y = 2 × x;y = 2 × 9", () =>
{
    var step=SchemeRunner.Run(one,9).Steps.Single(s=>s.NodeId=="a1");
    return $"{step.Formula};{step.Substitution}";
});

var failed=results.Count(r=>!r.Passed);
Console.WriteLine($"\n{results.Count-failed}/{results.Count} checks passed.");
if(args.Length>0) File.WriteAllText(args[0], JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
return failed==0?0:1;

internal sealed record TestRecord(string Name,string Expected,string Actual,bool Passed);
