# Источники и ИИ

Дата подготовки: 21 октября 2026 года.

## Источники

1. Задание учителя: «Тема 10. Чтение блок-схем», второй тип задачи; предоставленные пользователем требования к визуализации, архитектуре, проверкам и защите.
2. Макет Александра в Figma, предоставленный изображением: разделы «Главная», «Теория», «Тест», «Практика», схема слева, управление и разбор справа.
3. [Microsoft Learn: типы C#](https://learn.microsoft.com/ru-ru/dotnet/csharp/fundamentals/types/).
4. [Microsoft Learn: присваивание](https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/operators/assignment-operator).
5. [Microsoft Learn: сравнение](https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/operators/comparison-operators).
6. [Microsoft Learn: проверяемая арифметика](https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/statements/checked-and-unchecked).
7. [Microsoft Learn: Blazor](https://learn.microsoft.com/ru-ru/aspnet/core/blazor/?view=aspnetcore-10.0).
8. [Microsoft Learn: размещение Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly/?view=aspnetcore-10.0).
9. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

Учебные формулировки и три схемы составлены для этого проекта. Изображения блок-схем строятся кодом SVG; внешние фотографии, шрифты и библиотеки диаграмм не используются.

## Использование ИИ

ChatGPT использован для обсуждения технологии, проектирования модели, написания исходников C#/Razor/CSS, подготовки задач, автоматических проверок и черновиков документации.

Приняты предложения: C# и Blazor WebAssembly; разделение расчётов и интерфейса; единая трасса для подсветки, пояснений и проверки; точная целочисленная арифметика; сброс разбора при изменении входа.

Исправлены предложения и неточности макета: Blazor и C# не являются отдельными серверными слоями; условия обозначаются ромбами; две выходящие ветви имеют разные подписи; запуск выполняется через HTTP, а не открытием HTML-файла; несколько правильных входных чисел проверяются выполнением схемы.

Проверка результата: сборка с ошибками на предупреждениях; автоматические проверки на C#; независимые формулы для всех 45 допустимых входов трёх схем; проверка интерфейса в браузере в объёме, указанном в TEST_RESULTS.md.

Автору нужно самостоятельно изучить исходники и пройти сценарий защиты. Проверка одноклассником и личная проверка запуска на Windows не заявлены выполненными. Их фактические результаты необходимо записать после проведения.

## Дополнение: GitHub Pages

- Microsoft: размещение Blazor WebAssembly на GitHub Pages — https://learn.microsoft.com/aspnet/core/blazor/host-and-deploy/webassembly/github-pages?view=aspnetcore-10.0
- GitHub: настройка источника публикации — https://docs.github.com/pages/getting-started-with-github-pages/configuring-a-publishing-source-for-your-github-pages-site
- GitHub: пользовательские процессы сборки Pages — https://docs.github.com/pages/getting-started-with-github-pages/using-custom-workflows-with-github-pages

ИИ использован для подготовки процесса GitHub Actions и инструкции по загрузке с сохранением папок. Принято решение собирать исходники автоматически, а путь приложения определять из настроек Pages. Ссылки перезагрузки исправлены на относительные для работы в подпапке. Проверены сборка и автоматические проверки; запуск процесса на GitHub и доступность публичной ссылки должны проверяться отдельно после загрузки.
