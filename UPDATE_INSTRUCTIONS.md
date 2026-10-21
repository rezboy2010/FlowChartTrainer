# Как установить обновление

1. Останови приложение: в терминале VS Code нажми Ctrl+C.
2. Открой папку проекта, где находятся start.bat, src и docs.
3. Распакуй содержимое этого архива прямо в эту папку, разрешив замену одноимённых файлов. Не создавай ещё одну вложенную папку src.
4. Запусти .\start.bat в терминале VS Code.
5. Открой http://localhost:5080 и нажми Ctrl+F5.

В архиве полные версии изменённых файлов: вручную искать и менять отдельные строки не нужно. Исходный проект должен быть уже распакован — это обновление, а не отдельный проект.

## Что изменилось

- Начальное x и ответ y имеют разные подписи и инструкции.
- Сразу после ввода x открывается первое вычисление; поле ответа получает фокус.
- Кнопки «Проверить вычисление» и «Проверить ветвь» стоят рядом с ответом.
- До верного ответа «Дальше» недоступно.
- В блоках, подстановках, теории и тесте присваивание обозначено знаком =.
- Пример для первого задания: x = 9; последовательно вводи y = 18, затем 19; выбери «Да»; затем введи 16.

## Какие файлы заменяются

- README.md
- docs/ARCHITECTURE.md
- docs/TEST_RESULTS.json
- docs/TEST_RESULTS.md
- src/FlowchartTrainer.Core/Data/QuizCatalog.cs
- src/FlowchartTrainer.Core/Logic/SchemeRunner.cs
- src/FlowchartTrainer.Core/Models/Scheme.cs
- src/FlowchartTrainer.Core/State/TrainingSession.cs
- src/FlowchartTrainer.Web/Components/Practice.razor
- src/FlowchartTrainer.Web/Components/Theory.razor
- src/FlowchartTrainer.Web/wwwroot/css/app.css
- tests/FlowchartTrainer.Tests/Program.cs

Старую папку published для запуска исходников менять не требуется. Если используешь готовые опубликованные файлы, запусти publish.bat: он пересоберёт published/wwwroot с изменениями.
