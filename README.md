# Okisioli.Teacups

A collection of small, reusable .NET libraries for authentication, secure storage, subtitles, and system utilities.

## Packages

| Package | Purpose |
| --- | --- |
| `Okisioli.Teacups.Auth` | Provider-neutral authentication contracts. |
| `Okisioli.Teacups.Auth.Cookies` | Cookie-based authentication for ASP.NET Core. |
| `Okisioli.Teacups.Auth.Supabase` | Supabase authentication for .NET applications. |
| `Okisioli.Teacups.Media.Subs` | Framework-independent subtitle loading and playback. |
| `Okisioli.Teacups.Store` | Provider-neutral contracts for secure key-value storage. |
| `Okisioli.Teacups.Store.Maui` | Secure key-value storage backed by .NET MAUI. |
| `Okisioli.Teacups.System` | System and process utilities. |
| `Okisioli.Teacups.System.Windows` | Native Windows window utilities for `net10.0-windows`. |
| `Okisioli.Teacups.Templates` | .NET project templates, including Basic Library. |

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
