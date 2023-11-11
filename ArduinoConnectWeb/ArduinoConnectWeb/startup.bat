@echo off
title ArduinoConnectWeb

set "HOST="
set "PORT=5000"

@for /f "delims=[] tokens=2" %%a in ('ping -4 -n 1 %ComputerName% ^| findstr [') DO (
    set "HOST=%%a"
)

if %HOST%=="" (
	set "HOST=127.0.0.1"
	echo Host address has been set to default value: %HOST%.
)

echo Starting ArduinoConnectWeb on address %HOST% and port %PORT% ...
ArduinoConnectWeb.exe /custom /host %HOST% /port %PORT%
