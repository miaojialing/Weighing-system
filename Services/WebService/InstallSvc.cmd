cd /d %~dp0
sc create "EntranceGuardWebSvc" binpath="%cd%\WebService.exe" start=auto DisplayName="EntranceGuard Web&MQTT Service"
pause