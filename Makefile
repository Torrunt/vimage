## publish: creates a 64bit release build
.PHONY: publish
publish:
	dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true

## publish-x86: creates a 32bit release build
.PHONY: publish-x86
publish-x86:
	dotnet publish -c Release -r win-x86 --self-contained false /p:PublishSingleFile=true
