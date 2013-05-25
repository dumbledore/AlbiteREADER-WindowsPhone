@echo Albite READER
@rmdir /s /q IsoTemp\AlbiteREADER
@"c:\Program Files (x86)\Microsoft SDKs\Windows Phone\v7.1\Tools\IsolatedStorageExplorerTool\ISETool.exe" ts xd bdf349d4-69ac-4450-a13f-ca351b98353c IsoTemp\AlbiteREADER

@echo Albite Tests
@rmdir /s /q IsoTemp\AlbiteTests
@"c:\Program Files (x86)\Microsoft SDKs\Windows Phone\v7.1\Tools\IsolatedStorageExplorerTool\ISETool.exe" ts xd 9b45c86a-6522-4d1c-8cd8-3d79d377b8d9 IsoTemp\AlbiteTests