@echo off
set "MONSTERDAY_UNITY=C:\Program Files\Unity\Hub\Editor\6000.5.4f1\Editor\Unity.exe"
if not exist "%MONSTERDAY_UNITY%" (
  echo Unity 6000.5.4f1 wurde nicht gefunden. Bitte oeffne das Projekt ueber Unity Hub.
  pause
  exit /b 1
)
start "Monsterday Unity" "%MONSTERDAY_UNITY%" -projectPath "%~dp0"
