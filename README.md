# blazor-shadcn-cli

CLI for installing shadcn-style Blazor components directly into an app as source files.

## Commands

```bash
blazor-shadcn new MyApp
blazor-shadcn list
blazor-shadcn add accordion
blazor-shadcn add avatar
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn add checkbox
blazor-shadcn add separator
blazor-shadcn add spinner
blazor-shadcn add typography
blazor-shadcn init
blazor-shadcn doctor
blazor-shadcn version --check
```

Run `blazor-shadcn help <command>` for command-specific options.

## Current MVP Components

- `accordion`
- `avatar`
- `badge`
- `button`
- `card`
- `checkbox`
- `input`
- `label`
- `separator`
- `spinner`
- `typography`

## Example

Create a new app and add your first component:

```bash
blazor-shadcn new MyApp
cd MyApp
blazor-shadcn add accordion
blazor-shadcn add avatar
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn add checkbox
blazor-shadcn add label
blazor-shadcn add separator
blazor-shadcn add spinner
blazor-shadcn add typography
```

## Notes

- `add` installs component source files into `Components/UI` and ensures required theme tokens exist in `Styles/globals.css`.
- `accordion` installs `Accordion.razor`, `AccordionContent.razor`, `AccordionContext.cs`, `AccordionItem.razor`, and `AccordionTrigger.razor`.
- `avatar` installs `Avatar.razor`, `AvatarBadge.razor`, `AvatarContext.cs`, `AvatarFallback.razor`, `AvatarGroup.razor`, `AvatarGroupCount.razor`, and `AvatarImage.razor`.
- `card` installs `Card.razor`, `CardAction.razor`, `CardContent.razor`, `CardDescription.razor`, `CardFooter.razor`, `CardHeader.razor`, and `CardTitle.razor`.
- `separator` installs `Separator.razor`.
- `add accordion` also ensures `Components/App.razor` has interactive render mode on both `HeadOutlet` and `Routes`; other components do not change render mode configuration.
- `add --force` overwrites existing component files, and `add --dry-run` previews the action.
- `new` creates a fresh Blazor app, removes the template demo content, configures Tailwind/fonts/imports, and leaves the project ready for `blazor-shadcn add <component>`.
- `new --skip-install` skips `npm install`, and `new --dry-run` previews the action.
- `init` prepares an existing Blazor project by creating or updating `Styles/globals.css`, ensuring `package.json` contains the required Tailwind scripts and devDependencies, optionally installing npm packages, adding the Tailwind build target to the project file, patching `Components/App.razor`, and optionally removing Bootstrap references.
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
dotnet tool install --global blazor-shadcn --version 0.2.12 --configfile .\NuGet.Local.config
```

Run it:

```powershell
blazor-shadcn new MyApp
blazor-shadcn list
blazor-shadcn add accordion
blazor-shadcn add avatar
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn add checkbox
blazor-shadcn add label
blazor-shadcn add separator
blazor-shadcn add spinner
blazor-shadcn add typography
blazor-shadcn init
blazor-shadcn doctor
blazor-shadcn version --check
```

Update the installed tool after making changes:

```powershell
dotnet pack BlazorShadcn.Cli\BlazorShadcn.Cli.csproj -c Release
dotnet tool update --global blazor-shadcn --version 0.2.12 --configfile .\NuGet.Local.config
```

`add` depends on the component files existing in the source GitHub repository. By default the CLI uses `Justbeingjustin/blazor-shadcn` on the `main` branch. Set `BLAZOR_SHADCN_REPOSITORY` to use a different `<owner>/<repo>`.

If your machine has private or unavailable NuGet feeds configured, using [`NuGet.Local.config`](C:\Development\GithubRepositories\blazor-shadcn-cli\NuGet.Local.config) avoids those feeds entirely for local tool install/update.
