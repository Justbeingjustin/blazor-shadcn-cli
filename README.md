# blazor-shadcn-cli

CLI for installing shadcn-style Blazor components directly into an app as source files.

## Commands

```bash
blazor-shadcn new MyApp
blazor-shadcn list
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn init
blazor-shadcn doctor
blazor-shadcn version --check
```

Run `blazor-shadcn help <command>` for command-specific options.

## Current MVP Components

- `badge`
- `button`
- `card`
- `input`
- `separator`

## Example

Create a new app and add your first component:

```bash
blazor-shadcn new MyApp
cd MyApp
blazor-shadcn add badge
blazor-shadcn add button
```

## Notes

- `add` installs components into `Components/UI`.
- `add --force` overwrites an existing component file, and `add --dry-run` previews the action.
- `new` creates a fresh Blazor app, removes the template demo content, configures Tailwind/fonts/imports, and leaves the project ready for `blazor-shadcn add <component>`.
- `new --skip-install` skips `npm install`, and `new --dry-run` previews the action.
- `init` prepares an existing Blazor project by creating `Styles/globals.css`, ensuring `package.json` contains the required Tailwind scripts and devDependencies, optionally installing npm packages, adding the Tailwind build target to the project file, patching `Components/App.razor`, and optionally removing Bootstrap references.
- `init --yes` runs non-interactively, `init --remove-bootstrap` removes Bootstrap references, `init --no-bootstrap-removal` keeps them, `init --skip-install` skips `npm install`, and `init --dry-run` previews the action.
- `list --json` and `doctor --json` emit machine-readable output.
- `doctor` validates local prerequisites, project detection, the NuGet version feed, and every registered component source URL.
- `version --check` forces an update check, and normal commands show a cached upgrade notice when a newer NuGet package is available.
- The default component source repo is `Justbeingjustin/blazor-shadcn`. Set `BLAZOR_SHADCN_REPOSITORY` to override it.
- Set `BLAZOR_SHADCN_DISABLE_UPDATE_CHECK=1` to disable automatic update notices.

## Local Testing

Pack the tool:

```powershell
dotnet pack BlazorShadcn.Cli\BlazorShadcn.Cli.csproj -c Release
```

Install it from the local package source:

```powershell
dotnet tool install --global blazor-shadcn --version 0.2.1 --configfile .\NuGet.Local.config
```

Run it:

```powershell
blazor-shadcn new MyApp
blazor-shadcn list
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn init
blazor-shadcn doctor
blazor-shadcn version --check
```

Update the installed tool after making changes:

```powershell
dotnet pack BlazorShadcn.Cli\BlazorShadcn.Cli.csproj -c Release
dotnet tool update --global blazor-shadcn --version 0.2.1 --configfile .\NuGet.Local.config
```

`add` depends on the component files existing in the source GitHub repository. By default the CLI uses `Justbeingjustin/blazor-shadcn` on the `main` branch. Set `BLAZOR_SHADCN_REPOSITORY` to use a different `<owner>/<repo>`.

If your machine has private or unavailable NuGet feeds configured, using [`NuGet.Local.config`](C:\Development\GithubRepositories\blazor-shadcn-cli\NuGet.Local.config) avoids those feeds entirely for local tool install/update.
