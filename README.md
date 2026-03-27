# blazor-shadcn-cli

CLI for installing shadcn-style Blazor components directly into an app as source files.

## Commands

```bash
blazor-shadcn new MyApp
blazor-shadcn list
blazor-shadcn add accordion
blazor-shadcn add alert
blazor-shadcn add alert-dialog
blazor-shadcn add aspect-ratio
blazor-shadcn add avatar
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn add button-group
blazor-shadcn add checkbox
blazor-shadcn add dialog
blazor-shadcn add field
blazor-shadcn add dropdown-menu
blazor-shadcn add kbd
blazor-shadcn add radio-group
blazor-shadcn add popover
blazor-shadcn add select
blazor-shadcn add slider
blazor-shadcn add scroll-area
blazor-shadcn add separator
blazor-shadcn add skeleton
blazor-shadcn add spinner
blazor-shadcn add switch
blazor-shadcn add tabs
blazor-shadcn add toggle
blazor-shadcn add toggle-group
blazor-shadcn add tooltip
blazor-shadcn add textarea
blazor-shadcn add typography
blazor-shadcn init
blazor-shadcn doctor
blazor-shadcn version --check
```

Run `blazor-shadcn help <command>` for command-specific options.

## Current MVP Components

- `accordion`
- `alert`
- `alert-dialog`
- `aspect-ratio`
- `avatar`
- `badge`
- `button`
- `button-group`
- `card`
- `carousel`
- `checkbox`
- `dialog`
- `empty`
- `field`
- `dropdown-menu`
- `kbd`
- `popover`
- `radio-group`
- `select`
- `slider`
- `scroll-area`
- `input`
- `label`
- `separator`
- `skeleton`
- `spinner`
- `switch`
- `tabs`
- `toggle`
- `toggle-group`
- `tooltip`
- `textarea`
- `typography`

## Example

Create a new app and add your first component:

```bash
blazor-shadcn new MyApp
cd MyApp
blazor-shadcn add accordion
blazor-shadcn add alert
blazor-shadcn add alert-dialog
blazor-shadcn add aspect-ratio
blazor-shadcn add avatar
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn add button-group
blazor-shadcn add carousel
blazor-shadcn add checkbox
blazor-shadcn add dialog
blazor-shadcn add empty
blazor-shadcn add field
blazor-shadcn add dropdown-menu
blazor-shadcn add kbd
blazor-shadcn add label
blazor-shadcn add popover
blazor-shadcn add radio-group
blazor-shadcn add select
blazor-shadcn add slider
blazor-shadcn add scroll-area
blazor-shadcn add separator
blazor-shadcn add skeleton
blazor-shadcn add spinner
blazor-shadcn add switch
blazor-shadcn add tabs
blazor-shadcn add toggle
blazor-shadcn add toggle-group
blazor-shadcn add tooltip
blazor-shadcn add textarea
blazor-shadcn add typography
```

## Notes

- `add` installs component source files into `Components/UI`, writes any required JavaScript assets into `wwwroot/blazor-shadcn`, and ensures required theme tokens exist in `Styles/globals.css`.
- `accordion` installs `Accordion.razor`, `AccordionContent.razor`, `AccordionContext.cs`, `AccordionItem.razor`, and `AccordionTrigger.razor`.
- `alert` installs `Alert.razor`, `AlertAction.razor`, `AlertDescription.razor`, and `AlertTitle.razor`.
- `alert-dialog` installs `Button.razor`, `AlertDialog.razor`, `AlertDialogAction.razor`, `AlertDialogCancel.razor`, `AlertDialogContent.razor`, `AlertDialogDescription.razor`, `AlertDialogFooter.razor`, `AlertDialogHeader.razor`, `AlertDialogMedia.razor`, `AlertDialogTitle.razor`, and `AlertDialogTrigger.razor`.
- `add alert-dialog` installs `button` first so the shared button primitive is available.
- `aspect-ratio` installs `AspectRatio.razor`.
- `avatar` installs `Avatar.razor`, `AvatarBadge.razor`, `AvatarContext.cs`, `AvatarFallback.razor`, `AvatarGroup.razor`, `AvatarGroupCount.razor`, and `AvatarImage.razor`.
- `card` installs `Card.razor`, `CardAction.razor`, `CardContent.razor`, `CardDescription.razor`, `CardFooter.razor`, `CardHeader.razor`, and `CardTitle.razor`.
- `button-group` installs `Separator.razor`, `ButtonGroup.razor`, `ButtonGroupSeparator.razor`, and `ButtonGroupText.razor`.
- `add button-group` installs `separator` first so the shared `Separator.razor` primitive is available.
- `carousel` installs `Button.razor`, `Carousel.razor`, `CarouselApi.cs`, `CarouselContent.razor`, `CarouselContext.cs`, `CarouselItem.razor`, `CarouselNext.razor`, `CarouselPrevious.razor`, and `carousel.js`.
- `add carousel` also copies `carousel.js` to `wwwroot/blazor-shadcn` and ensures `Components/App.razor` loads it.
- `add carousel` installs `button` first so the shared button primitive is available.
- `dialog` installs `Dialog.razor`, `DialogClose.razor`, `DialogContent.razor`, `DialogContext.cs`, `DialogDescription.razor`, `DialogFooter.razor`, `DialogHeader.razor`, `DialogTitle.razor`, and `DialogTrigger.razor`.
- `empty` installs `Empty.razor`, `EmptyContent.razor`, `EmptyDescription.razor`, `EmptyHeader.razor`, `EmptyMedia.razor`, and `EmptyTitle.razor`.
- `field` installs `Field.razor`, `FieldContent.razor`, `FieldDescription.razor`, `FieldError.razor`, `FieldGroup.razor`, `FieldLabel.razor`, `FieldLegend.razor`, `FieldSeparator.razor`, `FieldSet.razor`, and `FieldTitle.razor`.
- `dropdown-menu` installs `DropdownMenu.razor`, `DropdownMenuContent.razor`, `DropdownMenuContext.cs`, `DropdownMenuGroup.razor`, `DropdownMenuItem.razor`, `DropdownMenuLabel.razor`, `DropdownMenuPortal.razor`, `DropdownMenuSeparator.razor`, `DropdownMenuShortcut.razor`, `DropdownMenuSub.razor`, `DropdownMenuSubContent.razor`, `DropdownMenuSubContext.cs`, `DropdownMenuSubGroupContext.cs`, `DropdownMenuSubTrigger.razor`, and `DropdownMenuTrigger.razor`.
- `kbd` installs `Kbd.razor` and `KbdGroup.razor`.
- `popover` installs `Popover.razor`, `PopoverAnchor.razor`, `PopoverContent.razor`, `PopoverContext.cs`, `PopoverDescription.razor`, `PopoverHeader.razor`, `PopoverTitle.razor`, and `PopoverTrigger.razor`.
- `radio-group` installs `RadioGroup.razor`, `RadioGroupContext.cs`, and `RadioGroupItem.razor`.
- `select` installs `Select.razor`, `SelectContent.razor`, `SelectContext.cs`, `SelectGroup.razor`, `SelectItem.razor`, `SelectLabel.razor`, `SelectSeparator.razor`, `SelectTrigger.razor`, and `SelectValue.razor`.
- `add select` also ensures `Components/App.razor` contains the `window.blazorShadcnSelect.position` script used to position the dropdown content.
- `slider` installs `Slider.razor`.
- `scroll-area` installs `ScrollArea.razor` and `ScrollBar.razor`.
- `separator` installs `Separator.razor`.
- `skeleton` installs `Skeleton.razor`.
- `switch` installs `Switch.razor`.
- `tabs` installs `Tabs.razor`, `TabsContent.razor`, `TabsContext.cs`, `TabsList.razor`, and `TabsTrigger.razor`.
- `toggle` installs `Toggle.razor`.
- `toggle-group` installs `Toggle.razor`, `ToggleGroup.razor`, `ToggleGroupContext.cs`, and `ToggleGroupItem.razor`.
- `add toggle-group` installs `toggle` first so shared toggle primitives come from the `toggle` component source.
- `tooltip` installs `Tooltip.razor`, `TooltipContent.razor`, `TooltipProviderContext.cs`, `TooltipProvider.razor`, and `TooltipTrigger.razor`.
- `textarea` installs `Textarea.razor`.
- `add accordion`, `add alert-dialog`, `add carousel`, `add dialog`, `add dropdown-menu`, `add popover`, `add select`, `add slider`, `add switch`, `add tabs`, `add toggle`, `add toggle-group`, and `add tooltip` also ensure `Components/App.razor` has interactive render mode on both `HeadOutlet` and `Routes`; other components do not change render mode configuration.
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
dotnet tool install --global blazor-shadcn --version 0.2.46 --configfile .\NuGet.Local.config
```

Run it:

```powershell
blazor-shadcn new MyApp
blazor-shadcn list
blazor-shadcn add accordion
blazor-shadcn add alert
blazor-shadcn add alert-dialog
blazor-shadcn add aspect-ratio
blazor-shadcn add avatar
blazor-shadcn add badge
blazor-shadcn add button
blazor-shadcn add button-group
blazor-shadcn add checkbox
blazor-shadcn add dialog
blazor-shadcn add field
blazor-shadcn add dropdown-menu
blazor-shadcn add kbd
blazor-shadcn add label
blazor-shadcn add popover
blazor-shadcn add radio-group
blazor-shadcn add select
blazor-shadcn add slider
blazor-shadcn add scroll-area
blazor-shadcn add separator
blazor-shadcn add skeleton
blazor-shadcn add spinner
blazor-shadcn add switch
blazor-shadcn add tabs
blazor-shadcn add toggle
blazor-shadcn add toggle-group
blazor-shadcn add tooltip
blazor-shadcn add textarea
blazor-shadcn add typography
blazor-shadcn init
blazor-shadcn doctor
blazor-shadcn version --check
```

Update the installed tool after making changes:

```powershell
dotnet pack BlazorShadcn.Cli\BlazorShadcn.Cli.csproj -c Release
dotnet tool update --global blazor-shadcn --version 0.2.46 --configfile .\NuGet.Local.config
```

`add` depends on the component files existing in the source GitHub repository. By default the CLI uses `Justbeingjustin/blazor-shadcn` on the `main` branch. Set `BLAZOR_SHADCN_REPOSITORY` to use a different `<owner>/<repo>`.

If your machine has private or unavailable NuGet feeds configured, using [`NuGet.Local.config`](C:\Development\GithubRepositories\blazor-shadcn-cli\NuGet.Local.config) avoids those feeds entirely for local tool install/update.
