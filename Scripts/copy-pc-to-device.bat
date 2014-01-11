@echo off

rem ts - device to pc
rem rs - ps to device

rem de - wp device
rem xd - default emulator

echo Albite READER
"c:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\Tools\IsolatedStorageExplorerTool\ISETool.exe" rs de 95b37562-175c-46d9-a063-36c2d4d9b280 ..\IsoTemp\AlbiteREADER\IsolatedStore

echo Albite Tests
"c:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\Tools\IsolatedStorageExplorerTool\ISETool.exe" rs de 779d5de6-661c-49c6-99aa-81bec84004d1 ..\IsoTemp\AlbiteTests\IsolatedStore