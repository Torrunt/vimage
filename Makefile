-include .env

## publish: creates a 64bit release build
.PHONY: publish
publish:
	@sed -i.bak "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"$(SENTRY_DSN)\";|" vimage/Program.cs
	dotnet publish vimage -c Release -r win-x64
	dotnet publish vimage_settings -c Release -r win-x64
	@sed -i "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"\";|" vimage/Program.cs
	@rm -f vimage/Program.cs.bak

## publish-x86: creates a 32bit release build
.PHONY: publish-x86
publish-x86:
	@sed -i.bak "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"$(SENTRY_DSN)\";|" vimage/Program.cs
	dotnet publish vimage -c Release -r win-x86
	dotnet publish vimage_settings -c Release -r win-x86
	@sed -i "s|public const string SENTRY_DSN = \".*\";|public const string SENTRY_DSN = \"\";|" vimage/Program.cs
	@rm -f vimage/Program.cs.bak
