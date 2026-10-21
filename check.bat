@echo off
chcp 65001 >nul
cd /d "%~dp0"
dotnet run --project "tests\FlowchartTrainer.Tests\FlowchartTrainer.Tests.csproj" -- "%~dp0test-results.json"
if errorlevel 1 (
  echo Есть ошибка проверки или запуска. Изучи сообщения выше.
  pause
  exit /b 1
)
echo Проверки пройдены. Подробности записаны в test-results.json.
pause
