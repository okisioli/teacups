# Teacups

A collection of small **.NET libraries for various functionality I kept re-writing over the years**.

Even though an agent could roll any of these in seconds, I don't want to keep prompting for the same things (DRY 2.0), and prefer to grow and maintain my own, version-controlled, evolving set of narrow-focused helpers and tools.

A sibling repository, focused on helpers and controls for UI frameworks, can be found at [Teacups UI](https://github.com/okisioli/teacups-ui).

## Packages

| Package | Purpose |
| --- | --- |
| [Okisioli.Teacups.Auth](src/Okisioli.Teacups.Auth) | Provider-neutral authentication contracts. |
| [Okisioli.Teacups.Auth.Cookies](src/Okisioli.Teacups.Auth.Cookies) | Cookie-based authentication for ASP.NET Core. |
| [Okisioli.Teacups.Auth.Supabase](src/Okisioli.Teacups.Auth.Supabase) | Supabase authentication for .NET applications. |
| [Okisioli.Teacups.Media.Subs](src/Okisioli.Teacups.Media.Subs) | Framework-independent subtitle loading and playback. |
| [Okisioli.Teacups.Sharing](src/Okisioli.Teacups.Sharing) | Generation of short public IDs and readable random codes. |
| [Okisioli.Teacups.Store](src/Okisioli.Teacups.Store) | Provider-neutral contracts for secure key-value storage. |
| [Okisioli.Teacups.Store.Maui](src/Okisioli.Teacups.Store.Maui) | Secure key-value storage backed by .NET MAUI. |
| [Okisioli.Teacups.System](src/Okisioli.Teacups.System) | System and process utilities. |
| [Okisioli.Teacups.System.Windows](src/Okisioli.Teacups.System.Windows) | Native Windows window utilities for `net10.0-windows`. |
| [Okisioli.Teacups.Templates](templates) | .NET project templates, including Basic Library. |

## Templates

Install the template package with
```
dotnet new install Okisioli.Teacups.Templates
```

Inir a new project with
```
dotnet new teacups-lib -n MyLibrary
```

to create a minimal starter solution for a modern .NET 10 library, with convenient defaults such as:

- Central package management and a `Directory.Build.props` file
- New `.slnx` solution file format
- Clean folder structure predefined
- Separation of source files and build artifacts
- GitHub workflows to publish to NuGet.org and GitHub Packages
