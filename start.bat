@echo off
chcp 65001 >nul
setlocal
cd /d "%~dp0"
where dotnet >nul 2>nul
if errorlevel 1 (
  echo Установи .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
  echo После установки снова запусти этот файл.
  pause
  exit /b 1
)
dotnet --list-sdks | findstr /B "10\." >nul
if errorlevel 1 (
  echo Для проекта нужен .NET 10 SDK. Установи его и повтори запуск.
  pause
  exit /b 1
)
echo Чтение блок-схем — Резяпкин Александр
echo При первом запуске загружаются зависимости. Нужен интернет.
echo После сообщения Now listening on открой http://localhost:5080
echo Не закрывай это окно во время работы. Остановка: Ctrl+C.
echo.
dotnet run --project "src\FlowchartTrainer.Web\FlowchartTrainer.Web.csproj" --no-launch-profile --urls http://localhost:5080
if errorlevel 1 (
  echo.
  echo Запуск не удался. Скопируй сообщение выше для разбора ошибки.
  pause
  exit /b 1
)
endlocal
