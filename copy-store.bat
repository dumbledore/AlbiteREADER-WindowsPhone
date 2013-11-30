@echo Albite READER
@rmdir /s /q IsoTemp\AlbiteREADER
@"c:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\Tools\IsolatedStorageExplorerTool\ISETool.exe" ts xd 95b37562-175c-46d9-a063-36c2d4d9b280 IsoTemp\AlbiteREADER

@echo Albite Tests
@rmdir /s /q IsoTemp\AlbiteTests
@"c:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\Tools\IsolatedStorageExplorerTool\ISETool.exe" ts xd 779d5de6-661c-49c6-99aa-81bec84004d1 IsoTemp\AlbiteTests
