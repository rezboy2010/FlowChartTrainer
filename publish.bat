@echo off
chcp 65001 >nul
cd /d "%~dp0"
dotnet publish "src\FlowchartTrainer.Web\FlowchartTrainer.Web.csproj" -c Release -o published
if errorlevel 1 (
  echo Сборка не удалась. Изучи сообщения выше.
  pause
  exit /b 1
)
echo Файлы для размещения находятся в published\wwwroot.
echo Размещай содержимое этой папки целиком на статическом хостинге.
pause
