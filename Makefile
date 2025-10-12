-include .env

## publish: creates a 64bit release build
.PHONY: publish
publish:
	@sed -i.bak "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"$(SENTRY_DSN)\";|" vimage/Source/Program.cs
	dotnet publish vimage -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
	dotnet publish vimage_settings -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
	@sed -i "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"\";|" vimage/Source/Program.cs
	@rm -f vimage/Source/Program.cs.bak

## publish-x86: creates a 32bit release build
.PHONY: publish-x86
publish-x86:
	@sed -i.bak "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"$(SENTRY_DSN)\";|" vimage/Source/Program.cs
	dotnet publish vimage -c Release -r win-x86 --self-contained false /p:PublishSingleFile=true
	dotnet publish vimage_settings -c Release -r win-x86 --self-contained false /p:PublishSingleFile=true
	@sed -i "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"\";|" vimage/Source/Program.cs
	@rm -f vimage/Source/Program.cs.bak
