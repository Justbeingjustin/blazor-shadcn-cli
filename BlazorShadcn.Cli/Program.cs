using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml.Linq;

return await BlazorShadcnCli.RunAsync(args);

internal static partial class BlazorShadcnCli
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(5),
    };

    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static readonly string[] ThemeInlineTokenLines =
    [
        "--font-sans: 'Geist', ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif;",
        "--font-mono: 'Geist Mono', ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, 'Liberation Mono', 'Courier New', monospace;",
        "--radius-sm: calc(var(--radius) - 4px);",
        "--radius-md: calc(var(--radius) - 2px);",
        "--radius-lg: var(--radius);",
        "--color-background: var(--background);",
        "--color-foreground: var(--foreground);",
        "--color-primary: var(--primary);",
        "--color-primary-foreground: var(--primary-foreground);",
        "--color-secondary: var(--secondary);",
        "--color-secondary-foreground: var(--secondary-foreground);",
        "--color-muted: var(--muted);",
        "--color-muted-foreground: var(--muted-foreground);",
        "--color-accent: var(--accent);",
        "--color-accent-foreground: var(--accent-foreground);",
        "--color-destructive: var(--destructive);",
        "--color-border: var(--border);",
        "--color-input: var(--input);",
        "--color-ring: var(--ring);",
    ];
    private static readonly string[] RootThemeTokenLines =
    [
        "--radius: 0.625rem;",
        "--background: oklch(1 0 0);",
        "--foreground: oklch(0.145 0 0);",
        "--primary: oklch(0.205 0 0);",
        "--primary-foreground: oklch(0.985 0 0);",
        "--secondary: oklch(0.97 0 0);",
        "--secondary-foreground: oklch(0.205 0 0);",
        "--muted: oklch(0.97 0 0);",
        "--muted-foreground: oklch(0.556 0 0);",
        "--accent: oklch(0.97 0 0);",
        "--accent-foreground: oklch(0.205 0 0);",
        "--destructive: oklch(0.577 0.245 27.325);",
        "--border: oklch(0.922 0 0);",
        "--input: oklch(0.922 0 0);",
        "--ring: oklch(0.708 0 0);",
    ];
    private static readonly string[] DarkThemeTokenLines =
    [
        "--background: oklch(0.145 0 0);",
        "--foreground: oklch(0.985 0 0);",
        "--primary: oklch(0.922 0 0);",
        "--primary-foreground: oklch(0.205 0 0);",
        "--secondary: oklch(0.269 0 0);",
        "--secondary-foreground: oklch(0.985 0 0);",
        "--muted: oklch(0.269 0 0);",
        "--muted-foreground: oklch(0.708 0 0);",
        "--accent: oklch(0.371 0 0);",
        "--accent-foreground: oklch(0.985 0 0);",
        "--destructive: oklch(0.704 0.191 22.216);",
        "--border: oklch(1 0 0 / 10%);",
        "--input: oklch(1 0 0 / 15%);",
        "--ring: oklch(0.556 0 0);",
    ];
    private static readonly string[] SelectInteropScriptLines =
    [
        "    <script>",
        "        window.blazorShadcnSelect = (function () {",
        "            function position(trigger, content, positionMode) {",
        "                if (!trigger || !content) return;",
        "",
        "                const rect = trigger.getBoundingClientRect();",
        "                const isPopper = positionMode === 'popper';",
        "                const minWidth = isPopper ? rect.width : Math.max(rect.width, 128);",
        "                const top = rect.bottom + (isPopper ? 4 : 2);",
        "                const left = rect.left;",
        "",
        "                content.style.top = `${top}px`;",
        "                content.style.left = `${left}px`;",
        "                content.style.minWidth = `${minWidth}px`;",
        "            }",
        "",
        "            return { position };",
        "        })();",
        "    </script>",
    ];
    private static readonly ComponentDefinition AccordionComponent = new(
        "accordion",
        [
            "Accordion.razor",
            "AccordionContent.razor",
            "AccordionContext.cs",
            "AccordionItem.razor",
            "AccordionTrigger.razor",
        ],
        "Collapsible content sections.");
    private static readonly ComponentDefinition ToggleComponent = new(
        "toggle",
        ["Toggle.razor"],
        "Two-state pressed button.");
    private static readonly ComponentDefinition ToggleGroupComponent = new(
        "toggle-group",
        [
            "ToggleGroup.razor",
            "ToggleGroupContext.cs",
            "ToggleGroupItem.razor",
        ],
        "Single or multiple selection toggle group.",
        ["toggle"]);
    private static readonly IReadOnlyDictionary<string, ComponentDefinition> Components =
        new Dictionary<string, ComponentDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["accordion"] = AccordionComponent,
            ["accordian"] = AccordionComponent,
            ["alert"] = new(
                "alert",
                [
                    "Alert.razor",
                    "AlertAction.razor",
                    "AlertDescription.razor",
                    "AlertTitle.razor",
                ],
                "Alert banner with title, description, and action."),
            ["alert-dialog"] = new(
                "alert-dialog",
                [
                    "AlertDialog.razor",
                    "AlertDialogAction.razor",
                    "AlertDialogCancel.razor",
                    "AlertDialogContent.razor",
                    "AlertDialogDescription.razor",
                    "AlertDialogFooter.razor",
                    "AlertDialogHeader.razor",
                    "AlertDialogMedia.razor",
                    "AlertDialogTitle.razor",
                    "AlertDialogTrigger.razor",
                ],
                "Modal dialog for confirming destructive or high-impact actions.",
                ["button"]),
            ["alertdialog"] = new(
                "alert-dialog",
                [
                    "AlertDialog.razor",
                    "AlertDialogAction.razor",
                    "AlertDialogCancel.razor",
                    "AlertDialogContent.razor",
                    "AlertDialogDescription.razor",
                    "AlertDialogFooter.razor",
                    "AlertDialogHeader.razor",
                    "AlertDialogMedia.razor",
                    "AlertDialogTitle.razor",
                    "AlertDialogTrigger.razor",
                ],
                "Modal dialog for confirming destructive or high-impact actions.",
                ["button"]),
            ["aspect-ratio"] = new(
                "aspect-ratio",
                ["AspectRatio.razor"],
                "Responsive aspect ratio container."),
            ["aspectratio"] = new(
                "aspect-ratio",
                ["AspectRatio.razor"],
                "Responsive aspect ratio container."),
            ["avatar"] = new(
                "avatar",
                [
                    "Avatar.razor",
                    "AvatarBadge.razor",
                    "AvatarContext.cs",
                    "AvatarFallback.razor",
                    "AvatarGroup.razor",
                    "AvatarGroupCount.razor",
                    "AvatarImage.razor",
                ],
                "Profile image with fallback, badge, and grouping support."),
            ["badge"] = new("badge", ["Badge.razor"], "Simple status badge."),
            ["button"] = new("button", ["Button.razor"], "Configurable action button."),
            ["button-group"] = new(
                "button-group",
                [
                    "ButtonGroup.razor",
                    "ButtonGroupSeparator.razor",
                    "ButtonGroupText.razor",
                ],
                "Grouped buttons with separators and inline text.",
                ["separator"]),
            ["buttongroup"] = new(
                "button-group",
                [
                    "ButtonGroup.razor",
                    "ButtonGroupSeparator.razor",
                    "ButtonGroupText.razor",
                ],
                "Grouped buttons with separators and inline text.",
                ["separator"]),
            ["card"] = new(
                "card",
                [
                    "Card.razor",
                    "CardAction.razor",
                    "CardContent.razor",
                    "CardDescription.razor",
                    "CardFooter.razor",
                    "CardHeader.razor",
                    "CardTitle.razor",
                ],
                "Structured content container."),
            ["checkbox"] = new("checkbox", ["Checkbox.razor"], "Selectable checkbox control."),
            ["dialog"] = new(
                "dialog",
                [
                    "Dialog.razor",
                    "DialogClose.razor",
                    "DialogContent.razor",
                    "DialogContext.cs",
                    "DialogDescription.razor",
                    "DialogFooter.razor",
                    "DialogHeader.razor",
                    "DialogTitle.razor",
                    "DialogTrigger.razor",
                ],
                "Modal dialog primitives with trigger, content, and close behavior."),
            ["field"] = new(
                "field",
                [
                    "Field.razor",
                    "FieldContent.razor",
                    "FieldDescription.razor",
                    "FieldError.razor",
                    "FieldGroup.razor",
                    "FieldLabel.razor",
                    "FieldLegend.razor",
                    "FieldSeparator.razor",
                    "FieldSet.razor",
                    "FieldTitle.razor",
                ],
                "Form field container.",
                SourceDirectory: "BlazorShadcn/Components/UI"),
            ["input"] = new("input", ["Input.razor"], "Text input field."),
            ["kbd"] = new(
                "kbd",
                [
                    "Kbd.razor",
                    "KbdGroup.razor",
                ],
                "Keyboard key labels and grouped shortcuts."),
            ["label"] = new("label", ["Label.razor"], "Text label for form controls."),
            ["radio-group"] = new(
                "radio-group",
                [
                    "RadioGroup.razor",
                    "RadioGroupContext.cs",
                    "RadioGroupItem.razor",
                ],
                "Radio button group with shared selection state."),
            ["radiogroup"] = new(
                "radio-group",
                [
                    "RadioGroup.razor",
                    "RadioGroupContext.cs",
                    "RadioGroupItem.razor",
                ],
                "Radio button group with shared selection state."),
            ["select"] = new(
                "select",
                [
                    "Select.razor",
                    "SelectContent.razor",
                    "SelectContext.cs",
                    "SelectGroup.razor",
                    "SelectItem.razor",
                    "SelectLabel.razor",
                    "SelectSeparator.razor",
                    "SelectTrigger.razor",
                    "SelectValue.razor",
                ],
                "Selectable listbox with trigger, content, and grouped items."),
            ["slider"] = new(
                "slider",
                [
                    "Slider.razor",
                ],
                "Single or multi-thumb range slider."),
            ["scroll-area"] = new(
                "scroll-area",
                [
                    "ScrollArea.razor",
                    "ScrollBar.razor",
                ],
                "Custom scroll container with matching scrollbar."),
            ["scrollarea"] = new(
                "scroll-area",
                [
                    "ScrollArea.razor",
                    "ScrollBar.razor",
                ],
                "Custom scroll container with matching scrollbar."),
            ["separator"] = new("separator", ["Separator.razor"], "Visual divider."),
            ["skeleton"] = new("skeleton", ["Skeleton.razor"], "Placeholder loading surface."),
            ["spinner"] = new("spinner", ["Spinner.razor"], "Loading indicator."),
            ["switch"] = new("switch", ["Switch.razor"], "Toggle switch control."),
            ["toggle"] = ToggleComponent,
            ["toggle-group"] = ToggleGroupComponent,
            ["togglegroup"] = ToggleGroupComponent,
            ["tooltip"] = new(
                "tooltip",
                [
                    "Tooltip.razor",
                    "TooltipContent.razor",
                    "TooltipProviderContext.cs",
                    "TooltipProvider.razor",
                    "TooltipTrigger.razor",
                ],
                "Hover and focus tooltip primitives."),
            ["textarea"] = new("textarea", ["Textarea.razor"], "Multi-line text input."),
            ["typography"] = new("typography", ["Typography.razor"], "Typography primitives and styles."),
        };

    private static readonly string[] BootstrapFilePatterns = ["App.razor", "*Layout*.razor", "_Layout.cshtml", "index.html"];

    private const string ToolName = "blazor-shadcn";
    private const string PackageId = "blazor-shadcn";
    private const string DefaultRepositoryOwner = "Justbeingjustin";
    private const string DefaultRepositoryName = "blazor-shadcn";
    private const string DefaultRepositoryBranch = "main";
    private const string TailwindBuildCommand = "npx @tailwindcss/cli -i ./Styles/globals.css -o ./wwwroot/tailwind.css --minify";
    private const string TailwindWatchCommand = "npx @tailwindcss/cli -i ./Styles/globals.css -o ./wwwroot/tailwind.css --watch";
    private const string UiImportsNamespace = "@using ShadcnBlazor.Components.UI";
    private const string AccordionRenderMode = "InteractiveServer";
    private const string DisableUpdateCheckEnvironmentVariable = "BLAZOR_SHADCN_DISABLE_UPDATE_CHECK";
    private const string UpdateCacheFileName = "update-check.json";
    private static readonly TimeSpan UpdateCacheDuration = TimeSpan.FromHours(12);

    public static async Task<int> RunAsync(string[] args)
    {
        var currentVersion = GetCurrentVersion();
        var parseResult = ParseCommand(args);
        if (!parseResult.Success)
        {
            Console.Error.WriteLine(parseResult.ErrorMessage);
            PrintUsage();
            return 1;
        }

        if (parseResult.ShowVersionOnly)
        {
            Console.WriteLine($"{ToolName} {currentVersion}");
            return 0;
        }

        if (parseResult.Command is null)
        {
            PrintUsage();
            return 1;
        }

        if (ShouldPerformUpdateCheck(parseResult.Command.Name))
        {
            await MaybeNotifyAboutUpdateAsync(currentVersion);
        }

        return parseResult.Command.Name switch
        {
            "new" => await CreateNewProjectAsync(parseResult.Command.Arguments),
            "list" => ListComponents(parseResult.Command.Arguments),
            "add" => await AddComponentAsync(parseResult.Command.Arguments),
            "init" => await InitializeProjectAsync(parseResult.Command.Arguments),
            "doctor" => await RunDoctorAsync(parseResult.Command.Arguments),
            "version" => await ShowVersionAsync(parseResult.Command.Arguments, currentVersion),
            "help" => ShowHelp(parseResult.Command.Arguments),
            _ => UnknownCommand(parseResult.Command.Name),
        };
    }

    private static ParsedInvocation ParseCommand(string[] args)
    {
        if (args.Length == 0)
        {
            return new(true, false, null, string.Empty);
        }

        if (args.Length == 1 && (args[0] is "--version" or "-v"))
        {
            return new(true, true, null, string.Empty);
        }

        if (args[0] is "--help" or "-h")
        {
            return new(true, false, new ParsedCommand("help", args.Skip(1).ToArray()), string.Empty);
        }

        return new(true, false, new ParsedCommand(args[0].Trim().ToLowerInvariant(), args.Skip(1).ToArray()), string.Empty);
    }

    private static int ListComponents(string[] args)
    {
        if (HasHelpFlag(args))
        {
            PrintListHelp();
            return 0;
        }

        var json = args.Any(arg => string.Equals(arg, "--json", StringComparison.OrdinalIgnoreCase));
        var unknownOption = args.FirstOrDefault(arg => arg.StartsWith("-", StringComparison.Ordinal) && !string.Equals(arg, "--json", StringComparison.OrdinalIgnoreCase));
        if (unknownOption is not null)
        {
            Console.Error.WriteLine($"Unknown option for list: {unknownOption}");
            PrintListHelp();
            return 1;
        }

        if (json)
        {
            var payload = Components.Values
                .DistinctBy(component => component.Name, StringComparer.OrdinalIgnoreCase)
                .OrderBy(component => component.Name, StringComparer.OrdinalIgnoreCase)
                .Select(component => new JsonObject
                {
                    ["name"] = component.Name,
                    ["fileName"] = component.PrimaryFileName,
                    ["fileNames"] = new JsonArray(component.FileNames.Select(fileName => JsonValue.Create(fileName)).ToArray()),
                    ["description"] = component.Description,
                })
                .ToArray();

            Console.WriteLine(new JsonArray(payload).ToJsonString(JsonOptions));
            return 0;
        }

        Console.WriteLine("Available components:");
        foreach (var component in Components.Values
                     .DistinctBy(component => component.Name, StringComparer.OrdinalIgnoreCase)
                     .OrderBy(component => component.Name, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"- {component.Name}: {component.Description}");
        }

        return 0;
    }

    private static async Task<int> AddComponentAsync(string[] args)
    {
        if (HasHelpFlag(args))
        {
            PrintAddHelp();
            return 0;
        }

        var parse = ParseAddOptions(args);
        if (!parse.Success)
        {
            Console.Error.WriteLine(parse.ErrorMessage);
            PrintAddHelp();
            return 1;
        }

        if (!Components.TryGetValue(parse.ComponentName!, out var component))
        {
            Console.Error.WriteLine($"Unknown component: {parse.ComponentName}");
            return 1;
        }

        var projectRoot = FindProjectRoot(Directory.GetCurrentDirectory());
        if (projectRoot is null)
        {
            Console.Error.WriteLine("No .csproj file found.");
            Console.Error.WriteLine($"Run this command inside a Blazor project. {GetProjectHint()}");
            return 1;
        }

        var targetDirectory = Path.Combine(projectRoot, "Components", "UI");
        var globalsCssPath = Path.Combine(projectRoot, "Styles", "globals.css");
        var globalsCssResult = await WriteGlobalsCssAsync(globalsCssPath);
        var componentsToInstall = ResolveInstallOrder(component).ToArray();

        foreach (var componentToInstall in componentsToInstall)
        {
            var installResult = await InstallComponentFilesAsync(componentToInstall, projectRoot, targetDirectory, parse.Force, parse.DryRun);
            if (!installResult.Success)
            {
                Console.Error.WriteLine(installResult.Message);
                return 1;
            }

            if (!string.IsNullOrWhiteSpace(installResult.Message))
            {
                Console.WriteLine(installResult.Message);
            }
        }

        if (globalsCssResult.Status is not FileChangeStatus.Unchanged)
        {
            Console.WriteLine(globalsCssResult.Message);
        }

        if (componentsToInstall.Any(componentToInstall => RequiresInteractiveRenderMode(componentToInstall.Name)))
        {
            var appRazorPath = Path.Combine(projectRoot, "Components", "App.razor");
            var renderModeResult = await EnsureComponentInteractivityAsync(appRazorPath, component.DisplayName);
            if (!renderModeResult.Success)
            {
                Console.Error.WriteLine(renderModeResult.Message);
                return 1;
            }

            Console.WriteLine(renderModeResult.Message);
        }

        if (componentsToInstall.Any(componentToInstall => RequiresAppRazorScript(componentToInstall.Name)))
        {
            var appRazorPath = Path.Combine(projectRoot, "Components", "App.razor");
            var scriptResult = await EnsureComponentAppRazorScriptAsync(appRazorPath, component.DisplayName, parse.DryRun);
            if (!scriptResult.Success)
            {
                Console.Error.WriteLine(scriptResult.Message);
                return 1;
            }

            if (!string.IsNullOrWhiteSpace(scriptResult.Message))
            {
                Console.WriteLine(scriptResult.Message);
            }
        }

        return 0;
    }

    private static async Task<int> InitializeProjectAsync(string[] args)
    {
        if (HasHelpFlag(args))
        {
            PrintInitHelp();
            return 0;
        }

        var options = ParseInitOptions(args);
        if (!options.Success)
        {
            Console.Error.WriteLine(options.ErrorMessage);
            PrintInitHelp();
            return 1;
        }

        var projectRoot = FindProjectRoot(Directory.GetCurrentDirectory());
        if (projectRoot is null)
        {
            Console.Error.WriteLine("No Blazor project found in the current directory.");
            Console.Error.WriteLine("Run this inside a Blazor app folder.");
            return 1;
        }

        var projectFile = FindProjectFile(projectRoot);
        var appRazorPath = Path.Combine(projectRoot, "Components", "App.razor");
        if (projectFile is null || !File.Exists(appRazorPath))
        {
            Console.Error.WriteLine("No Blazor project found in the current directory.");
            Console.Error.WriteLine("Expected a .csproj file and Components/App.razor.");
            return 1;
        }

        return await ConfigureProjectAsync(projectRoot, new ConfigureOptions(
            PromptForBootstrapRemoval: !options.Yes,
            RemoveBootstrapReferences: options.RemoveBootstrapReferences,
            RemoveDefaultAppStylesheet: false,
            SkipNpmInstall: options.SkipInstall,
            DryRun: options.DryRun));
    }

    private static async Task<int> CreateNewProjectAsync(string[] args)
    {
        if (HasHelpFlag(args))
        {
            PrintNewHelp();
            return 0;
        }

        var options = ParseNewOptions(args);
        if (!options.Success)
        {
            Console.Error.WriteLine(options.ErrorMessage);
            PrintNewHelp();
            return 1;
        }

        var projectName = options.ProjectName!;
        var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), projectName));
        if (Directory.Exists(projectRoot) && Directory.EnumerateFileSystemEntries(projectRoot).Any())
        {
            Console.Error.WriteLine($"The directory '{projectName}' already exists and is not empty.");
            return 1;
        }

        if (options.DryRun)
        {
            Console.WriteLine($"Would create Blazor project at {projectRoot}");
            Console.WriteLine(options.SkipInstall
                ? "Would skip npm install during initialization."
                : "Would run npm install during initialization.");
            return 0;
        }

        var createResult = await RunProcessAsync(
            Directory.GetCurrentDirectory(),
            GetCommandFileName("dotnet"),
            GetCommandArguments("dotnet", $"new blazor -n {QuoteArgument(projectName)} -o {QuoteArgument(projectRoot)}"),
            $"Created Blazor project {projectName}.",
            "Failed to create a new Blazor project.");
        if (!createResult.Success)
        {
            Console.Error.WriteLine(createResult.Message);
            return 1;
        }

        var cleanStarterResult = await CleanStarterProjectAsync(projectRoot);
        if (!cleanStarterResult.Success)
        {
            Console.Error.WriteLine(cleanStarterResult.Message);
            return 1;
        }

        Console.WriteLine(createResult.Message);
        foreach (var message in cleanStarterResult.Messages)
        {
            Console.WriteLine(message);
        }

        var configureResult = await ConfigureProjectAsync(projectRoot, new ConfigureOptions(
            PromptForBootstrapRemoval: false,
            RemoveBootstrapReferences: false,
            RemoveDefaultAppStylesheet: true,
            SkipNpmInstall: options.SkipInstall,
            DryRun: false));
        if (configureResult != 0)
        {
            return configureResult;
        }

        Console.WriteLine($"Starter project ready in {projectName}.");
        return 0;
    }

    private static async Task<int> ConfigureProjectAsync(string projectRoot, ConfigureOptions options)
    {
        Directory.CreateDirectory(Path.Combine(projectRoot, "Styles"));
        Directory.CreateDirectory(Path.Combine(projectRoot, "Components", "UI"));

        var projectFile = FindProjectFile(projectRoot);
        if (projectFile is null)
        {
            Console.Error.WriteLine("No .csproj file found.");
            return 1;
        }

        var appRazorPath = Path.Combine(projectRoot, "Components", "App.razor");
        var globalsCssPath = Path.Combine(projectRoot, "Styles", "globals.css");
        var bootstrapFiles = DetectBootstrapReferences(projectRoot);

        if (options.DryRun)
        {
            Console.WriteLine($"Would ensure {Path.GetRelativePath(projectRoot, globalsCssPath)} exists.");
            Console.WriteLine("Would ensure _Imports.razor contains the UI namespace.");
            Console.WriteLine("Would ensure package.json contains Tailwind scripts and dependencies.");
            Console.WriteLine(options.SkipNpmInstall
                ? "Would skip npm install."
                : "Would run npm install if dependencies are missing or package.json changed.");
            Console.WriteLine($"Would ensure {Path.GetFileName(projectFile)} builds Tailwind before dotnet build.");
            Console.WriteLine("Would patch Components/App.razor for Tailwind and fonts.");
            if (bootstrapFiles.Count > 0 && options.RemoveBootstrapReferences)
            {
                Console.WriteLine($"Would remove Bootstrap references from {bootstrapFiles.Count} file(s).");
            }

            return 0;
        }

        var globalsCssResult = await WriteGlobalsCssAsync(globalsCssPath);
        var importsRazorResult = await EnsureImportsRazorAsync(projectRoot);

        var packageJsonResult = await EnsurePackageJsonAsync(projectRoot);
        if (!packageJsonResult.Success)
        {
            Console.Error.WriteLine(packageJsonResult.Message);
            return 1;
        }

        CommandResult npmInstallResult = options.SkipNpmInstall
            ? new(true, "Skipped npm install.")
            : await EnsureNpmDependenciesInstalledAsync(projectRoot, packageJsonResult.PackageJsonChanged);
        if (!npmInstallResult.Success)
        {
            Console.Error.WriteLine(npmInstallResult.Message);
            return 1;
        }

        var csprojStatus = AddTailwindBuildTarget(projectFile);
        var appRazorResult = await PatchAppRazorAsync(appRazorPath, Path.GetFileNameWithoutExtension(projectFile), options.RemoveDefaultAppStylesheet);
        if (!appRazorResult.Success)
        {
            Console.Error.WriteLine(appRazorResult.Message);
            return 1;
        }

        var bootstrapMessage = HandleBootstrapReferences(bootstrapFiles, options);

        Console.WriteLine(globalsCssResult.Message);
        foreach (var message in importsRazorResult.Messages)
        {
            Console.WriteLine(message);
        }

        Console.WriteLine(packageJsonResult.Message);
        Console.WriteLine(npmInstallResult.Message);
        Console.WriteLine(DescribeFileChange(csprojStatus, Path.GetFileName(projectFile), "Added Tailwind build target to", "Updated Tailwind build target in"));
        Console.WriteLine(appRazorResult.Message);
        if (!string.IsNullOrWhiteSpace(bootstrapMessage))
        {
            Console.WriteLine(bootstrapMessage);
        }

        Console.WriteLine("Project initialized for blazor-shadcn.");
        return 0;
    }

    private static string HandleBootstrapReferences(IReadOnlyList<string> bootstrapFiles, ConfigureOptions options)
    {
        if (bootstrapFiles.Count == 0)
        {
            return string.Empty;
        }

        if (options.RemoveBootstrapReferences)
        {
            var removedCount = 0;
            foreach (var bootstrapFile in bootstrapFiles)
            {
                if (RemoveBootstrapReferences(bootstrapFile))
                {
                    removedCount++;
                }
            }

            return removedCount > 0
                ? "Bootstrap references removed where possible."
                : "No removable Bootstrap references were found.";
        }

        if (options.PromptForBootstrapRemoval)
        {
            Console.WriteLine("Bootstrap stylesheet detected.");
            Console.WriteLine("Blazor ShadCN works best without Bootstrap because Bootstrap styles may override Tailwind-based components.");
            Console.Write("Would you like to remove Bootstrap references? (y/n) ");
            var response = Console.ReadLine()?.Trim();
            if (string.Equals(response, "y", StringComparison.OrdinalIgnoreCase))
            {
                return HandleBootstrapReferences(bootstrapFiles, options with
                {
                    PromptForBootstrapRemoval = false,
                    RemoveBootstrapReferences = true,
                });
            }

            return "Bootstrap references were left in place.";
        }

        return "Bootstrap references detected. Re-run with --remove-bootstrap to clean them up.";
    }

    private static async Task<int> RunDoctorAsync(string[] args)
    {
        if (HasHelpFlag(args))
        {
            PrintDoctorHelp();
            return 0;
        }

        var json = args.Any(arg => string.Equals(arg, "--json", StringComparison.OrdinalIgnoreCase));
        var unknownOption = args.FirstOrDefault(arg => arg.StartsWith("-", StringComparison.Ordinal) && !string.Equals(arg, "--json", StringComparison.OrdinalIgnoreCase));
        if (unknownOption is not null)
        {
            Console.Error.WriteLine($"Unknown option for doctor: {unknownOption}");
            PrintDoctorHelp();
            return 1;
        }

        var projectRoot = FindProjectRoot(Directory.GetCurrentDirectory());
        var checks = new List<DoctorCheckResult>
        {
            await CheckCommandAvailableAsync("dotnet", "dotnet --version", "dotnet SDK"),
            await CheckCommandAvailableAsync("npm", "npm --version", "npm"),
            CheckProjectRoot(projectRoot),
            CheckAppRazor(projectRoot),
            CheckPackageJson(projectRoot),
            CheckTailwindModules(projectRoot),
            await CheckRemoteAsync("nuget-version-feed", $"https://api.nuget.org/v3-flatcontainer/{PackageId.ToLowerInvariant()}/index.json", "NuGet version feed"),
        };

        foreach (var component in Components.Values
                     .DistinctBy(component => component.Name, StringComparer.OrdinalIgnoreCase)
                     .OrderBy(component => component.Name, StringComparer.OrdinalIgnoreCase))
        {
            foreach (var fileName in component.FileNames)
            {
                checks.Add(await CheckRemoteAsync(
                    $"component-{component.Name}-{fileName.ToLowerInvariant().Replace('.', '-')}",
                    BuildComponentUrl(component, fileName),
                    $"Component source ({component.Name}/{fileName})"));
            }
        }

        var hasError = checks.Any(check => check.Status == "error");
        if (json)
        {
            var payload = checks.Select(check => new JsonObject
            {
                ["id"] = check.Id,
                ["label"] = check.Label,
                ["status"] = check.Status,
                ["message"] = check.Message,
            }).ToArray();
            Console.WriteLine(new JsonArray(payload).ToJsonString(JsonOptions));
            return hasError ? 1 : 0;
        }

        foreach (var check in checks)
        {
            Console.WriteLine($"[{check.Status}] {check.Label}: {check.Message}");
        }

        return hasError ? 1 : 0;
    }

    private static async Task<int> ShowVersionAsync(string[] args, string currentVersion)
    {
        if (HasHelpFlag(args))
        {
            PrintVersionHelp();
            return 0;
        }

        var check = args.Any(arg => string.Equals(arg, "--check", StringComparison.OrdinalIgnoreCase));
        var unknownOption = args.FirstOrDefault(arg => arg.StartsWith("-", StringComparison.Ordinal) && !string.Equals(arg, "--check", StringComparison.OrdinalIgnoreCase));
        if (unknownOption is not null)
        {
            Console.Error.WriteLine($"Unknown option for version: {unknownOption}");
            PrintVersionHelp();
            return 1;
        }

        Console.WriteLine($"{ToolName} {currentVersion}");
        if (!check)
        {
            return 0;
        }

        var update = await GetLatestVersionAsync(currentVersion, forceRefresh: true);
        if (!update.Success)
        {
            Console.Error.WriteLine(update.Message);
            return 1;
        }

        Console.WriteLine(update.Message);
        return 0;
    }

    private static int ShowHelp(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return 0;
        }

        return args[0].ToLowerInvariant() switch
        {
            "new" => PrintNewHelpAndReturn(),
            "list" => PrintListHelpAndReturn(),
            "add" => PrintAddHelpAndReturn(),
            "init" => PrintInitHelpAndReturn(),
            "doctor" => PrintDoctorHelpAndReturn(),
            "version" => PrintVersionHelpAndReturn(),
            _ => UnknownCommand(args[0]),
        };
    }

    private static int PrintNewHelpAndReturn()
    {
        PrintNewHelp();
        return 0;
    }

    private static int PrintListHelpAndReturn()
    {
        PrintListHelp();
        return 0;
    }

    private static int PrintAddHelpAndReturn()
    {
        PrintAddHelp();
        return 0;
    }

    private static int PrintInitHelpAndReturn()
    {
        PrintInitHelp();
        return 0;
    }

    private static int PrintDoctorHelpAndReturn()
    {
        PrintDoctorHelp();
        return 0;
    }

    private static int PrintVersionHelpAndReturn()
    {
        PrintVersionHelp();
        return 0;
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command: {command}");
        PrintUsage();
        return 1;
    }

    private static async Task MaybeNotifyAboutUpdateAsync(string currentVersion)
    {
        var update = await GetLatestVersionAsync(currentVersion, forceRefresh: false);
        if (!update.Success || !update.HasUpdate)
        {
            return;
        }

        Console.WriteLine($"A newer version {update.LatestVersion} is available. Run: dotnet tool update --global {PackageId}");
    }

    private static async Task<UpdateCheckResult> GetLatestVersionAsync(string currentVersion, bool forceRefresh)
    {
        try
        {
            var cachePath = GetUpdateCachePath();
            if (!forceRefresh)
            {
                var cached = await ReadCachedUpdateAsync(cachePath);
                if (cached is not null && cached.CheckedAtUtc + UpdateCacheDuration > DateTimeOffset.UtcNow)
                {
                    return BuildUpdateCheckResult(currentVersion, cached.LatestVersion);
                }
            }

            using var response = await HttpClient.GetAsync($"https://api.nuget.org/v3-flatcontainer/{PackageId.ToLowerInvariant()}/index.json");
            response.EnsureSuccessStatusCode();
            var payload = JsonNode.Parse(await response.Content.ReadAsStringAsync()) as JsonObject;
            var latestVersion = payload?["versions"]?.AsArray()
                .Select(node => node?.GetValue<string>())
                .Where(version => !string.IsNullOrWhiteSpace(version))
                .Select(version => version!)
                .LastOrDefault();

            if (string.IsNullOrWhiteSpace(latestVersion))
            {
                return new(false, false, string.Empty, "Unable to determine the latest published version.");
            }

            await WriteCachedUpdateAsync(cachePath, latestVersion);
            return BuildUpdateCheckResult(currentVersion, latestVersion);
        }
        catch (Exception exception)
        {
            return new(false, false, string.Empty, $"Update check failed: {exception.Message}");
        }
    }

    private static UpdateCheckResult BuildUpdateCheckResult(string currentVersion, string latestVersion)
    {
        var hasUpdate = TryParseVersion(latestVersion, out var latest) && TryParseVersion(currentVersion, out var current) && latest > current;
        var message = hasUpdate
            ? $"Latest available version: {latestVersion}"
            : "You are already on the latest version.";
        return new(true, hasUpdate, latestVersion, message);
    }

    private static bool ShouldPerformUpdateCheck(string commandName)
    {
        if (string.Equals(Environment.GetEnvironmentVariable(DisableUpdateCheckEnvironmentVariable), "1", StringComparison.Ordinal))
        {
            return false;
        }

        return commandName is not "version";
    }

    private static string BuildComponentUrl(ComponentDefinition component, string fileName)
    {
        var repository = Environment.GetEnvironmentVariable("BLAZOR_SHADCN_REPOSITORY");
        var sourceDirectory = component.SourceDirectory ?? $"components/{component.Name}";
        if (!string.IsNullOrWhiteSpace(repository))
        {
            return $"https://raw.githubusercontent.com/{repository.Trim().Trim('/')}/{DefaultRepositoryBranch}/{sourceDirectory}/{fileName}";
        }

        return $"https://raw.githubusercontent.com/{DefaultRepositoryOwner}/{DefaultRepositoryName}/{DefaultRepositoryBranch}/{sourceDirectory}/{fileName}";
    }

    private static IEnumerable<ComponentDefinition> ResolveInstallOrder(ComponentDefinition component)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var dependency in ResolveInstallOrderCore(component, visited))
        {
            yield return dependency;
        }
    }

    private static IEnumerable<ComponentDefinition> ResolveInstallOrderCore(ComponentDefinition component, HashSet<string> visited)
    {
        if (!visited.Add(component.Name))
        {
            yield break;
        }

        foreach (var dependencyName in component.Dependencies)
        {
            if (!Components.TryGetValue(dependencyName, out var dependency))
            {
                throw new InvalidOperationException($"Component dependency '{dependencyName}' is not registered.");
            }

            foreach (var transitiveDependency in ResolveInstallOrderCore(dependency, visited))
            {
                yield return transitiveDependency;
            }
        }

        yield return component;
    }

    private static async Task<ComponentInstallResult> InstallComponentFilesAsync(ComponentDefinition component, string projectRoot, string targetDirectory, bool force, bool dryRun)
    {
        var fileOperations = component.FileNames
            .Select(fileName => new ComponentFileOperation(
                fileName,
                Path.Combine(targetDirectory, fileName),
                BuildComponentUrl(component, fileName)))
            .ToArray();
        var existingFiles = fileOperations
            .Where(operation => File.Exists(operation.TargetPath))
            .ToArray();

        if (existingFiles.Length > 0 && !force)
        {
            return new(true, $"Component files already exist for {component.Name}: {string.Join(", ", existingFiles.Select(file => file.FileName))}. Skipping. Re-run with --force to overwrite.");
        }

        if (dryRun)
        {
            foreach (var operation in fileOperations)
            {
                Console.WriteLine($"{(File.Exists(operation.TargetPath) ? "Would overwrite" : "Would add")} {Path.GetRelativePath(projectRoot, operation.TargetPath)}");
                Console.WriteLine($"Source: {operation.SourceUrl}");
            }

            return new(true, string.Empty);
        }

        Directory.CreateDirectory(targetDirectory);

        try
        {
            foreach (var operation in fileOperations)
            {
                var componentContent = await DownloadComponentAsync(operation.SourceUrl);
                await File.WriteAllTextAsync(operation.TargetPath, componentContent, Utf8NoBom);
            }
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            var missingUrl = fileOperations
                .Select(operation => operation.SourceUrl)
                .FirstOrDefault(url => string.Equals(url, exception.Data["url"] as string, StringComparison.Ordinal))
                ?? "the requested source URL";
            return new(false, $"Component source was not found at {missingUrl}");
        }
        catch (Exception exception)
        {
            return new(false, $"Failed to download {component.Name}: {exception.Message}");
        }

        var action = existingFiles.Length > 0 ? "Updated" : "Added";
        var installedFiles = string.Join(", ", fileOperations.Select(operation => Path.GetRelativePath(projectRoot, operation.TargetPath)));
        return new(true, $"{action} {component.DisplayName} files at {installedFiles}");
    }

    private static async Task<string> DownloadComponentAsync(string url)
    {
        using var response = await HttpClient.GetAsync(url);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var exception = new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).", null, response.StatusCode);
            exception.Data["url"] = url;
            throw exception;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private static string? FindProjectRoot(string startDirectory)
    {
        var current = new DirectoryInfo(startDirectory);
        while (current is not null)
        {
            if (current.EnumerateFiles("*.csproj", SearchOption.TopDirectoryOnly).Any())
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return null;
    }

    private static string? FindProjectFile(string projectRoot)
        => Directory.EnumerateFiles(projectRoot, "*.csproj", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

    private static async Task<StepResult> WriteGlobalsCssAsync(string globalsCssPath)
    {
        var globalsCss = """
            @import "tailwindcss";

            @source "../Components/**/*.razor";

            @custom-variant dark (&:is(.dark *));

            @theme inline {
              --font-sans: 'Geist', ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif;
              --font-mono: 'Geist Mono', ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, 'Liberation Mono', 'Courier New', monospace;
              --radius-sm: calc(var(--radius) - 4px);
              --radius-md: calc(var(--radius) - 2px);
              --radius-lg: var(--radius);
              --color-background: var(--background);
              --color-foreground: var(--foreground);
              --color-primary: var(--primary);
              --color-primary-foreground: var(--primary-foreground);
              --color-secondary: var(--secondary);
              --color-secondary-foreground: var(--secondary-foreground);
              --color-muted: var(--muted);
              --color-muted-foreground: var(--muted-foreground);
              --color-accent: var(--accent);
              --color-accent-foreground: var(--accent-foreground);
              --color-destructive: var(--destructive);
              --color-border: var(--border);
              --color-input: var(--input);
              --color-ring: var(--ring);
            }

            :root {
              --radius: 0.625rem;
              --background: oklch(1 0 0);
              --foreground: oklch(0.145 0 0);
              --primary: oklch(0.205 0 0);
              --primary-foreground: oklch(0.985 0 0);
              --secondary: oklch(0.97 0 0);
              --secondary-foreground: oklch(0.205 0 0);
              --muted: oklch(0.97 0 0);
              --muted-foreground: oklch(0.556 0 0);
              --accent: oklch(0.97 0 0);
              --accent-foreground: oklch(0.205 0 0);
              --destructive: oklch(0.577 0.245 27.325);
              --border: oklch(0.922 0 0);
              --input: oklch(0.922 0 0);
              --ring: oklch(0.708 0 0);
            }

            .dark {
              --background: oklch(0.145 0 0);
              --foreground: oklch(0.985 0 0);
              --primary: oklch(0.922 0 0);
              --primary-foreground: oklch(0.205 0 0);
              --secondary: oklch(0.269 0 0);
              --secondary-foreground: oklch(0.985 0 0);
              --muted: oklch(0.269 0 0);
              --muted-foreground: oklch(0.708 0 0);
              --accent: oklch(0.371 0 0);
              --accent-foreground: oklch(0.985 0 0);
              --destructive: oklch(0.704 0.191 22.216);
              --border: oklch(1 0 0 / 10%);
              --input: oklch(1 0 0 / 15%);
              --ring: oklch(0.556 0 0);
            }

            @layer base {
              * {
                @apply border-border outline-ring/50;
              }

              body {
                @apply bg-background text-foreground;
                font-family: var(--font-sans);
              }
            }
            """;

        if (!File.Exists(globalsCssPath))
        {
            var directory = Path.GetDirectoryName(globalsCssPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(globalsCssPath, globalsCss + Environment.NewLine, Utf8NoBom);
            return new(FileChangeStatus.Created, "Created Styles/globals.css.");
        }

        var existingContent = NormalizeLineEndings(await File.ReadAllTextAsync(globalsCssPath));
        var updatedContent = existingContent;
        var changed = false;

        updatedContent = EnsureCssBlockVariables(updatedContent, "@theme inline", ThemeInlineTokenLines, out var themeChanged) ?? updatedContent;
        changed |= themeChanged;

        updatedContent = EnsureCssBlockVariables(updatedContent, ":root", RootThemeTokenLines, out var rootChanged) ?? updatedContent;
        changed |= rootChanged;

        updatedContent = EnsureCssBlockVariables(updatedContent, ".dark", DarkThemeTokenLines, out var darkChanged) ?? updatedContent;
        changed |= darkChanged;

        if (!changed)
        {
            return new(FileChangeStatus.Unchanged, "Styles/globals.css already contains the required theme tokens.");
        }

        await File.WriteAllTextAsync(globalsCssPath, updatedContent + Environment.NewLine, Utf8NoBom);
        return new(FileChangeStatus.Updated, "Updated Styles/globals.css with missing theme tokens.");
    }

    private static string? EnsureCssBlockVariables(string content, string selector, IReadOnlyList<string> variableLines, out bool changed)
    {
        changed = false;
        var blockStart = content.IndexOf(selector, StringComparison.Ordinal);
        if (blockStart < 0)
        {
            return null;
        }

        var openingBrace = content.IndexOf('{', blockStart);
        if (openingBrace < 0)
        {
            return null;
        }

        var closingBrace = FindMatchingBrace(content, openingBrace);
        if (closingBrace < 0)
        {
            return null;
        }

        var blockContent = content[(openingBrace + 1)..closingBrace];
        var missingLines = variableLines
            .Where(line => !blockContent.Contains(line[..line.IndexOf(':')], StringComparison.Ordinal))
            .Select(line => $"  {line}")
            .ToArray();
        if (missingLines.Length == 0)
        {
            return content;
        }

        changed = true;
        var insertion = Environment.NewLine + string.Join(Environment.NewLine, missingLines);
        return content.Insert(closingBrace, insertion);
    }

    private static int FindMatchingBrace(string content, int openingBraceIndex)
    {
        var depth = 0;
        for (var index = openingBraceIndex; index < content.Length; index++)
        {
            if (content[index] == '{')
            {
                depth++;
            }
            else if (content[index] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return index;
                }
            }
        }

        return -1;
    }

    private static async Task<MultiMessageStepResult> EnsureImportsRazorAsync(string directory)
    {
        var importsPath = Path.Combine(directory, "_Imports.razor");
        var messages = new List<string>();

        if (!File.Exists(importsPath))
        {
            await File.WriteAllTextAsync(importsPath, UiImportsNamespace + Environment.NewLine, Utf8NoBom);
            messages.Add("Created _Imports.razor");
            messages.Add("Added ShadcnBlazor.Components.UI to _Imports.razor");
            return new(messages);
        }

        var content = await File.ReadAllTextAsync(importsPath);
        var lines = NormalizeLineEndings(content)
            .Split('\n', StringSplitOptions.None)
            .Select(line => line.Trim())
            .ToArray();

        if (lines.Contains(UiImportsNamespace, StringComparer.Ordinal))
        {
            messages.Add("Namespace already present in _Imports.razor");
            return new(messages);
        }

        var separator = content.Length == 0 || content.EndsWith("\n", StringComparison.Ordinal) || content.EndsWith("\r", StringComparison.Ordinal)
            ? string.Empty
            : Environment.NewLine;
        await File.AppendAllTextAsync(importsPath, separator + UiImportsNamespace + Environment.NewLine, Utf8NoBom);
        messages.Add("Added ShadcnBlazor.Components.UI to _Imports.razor");
        return new(messages);
    }

    private static async Task<PackageJsonResult> EnsurePackageJsonAsync(string projectRoot)
    {
        var packageJsonPath = Path.Combine(projectRoot, "package.json");
        JsonObject packageJson;
        var created = false;

        if (File.Exists(packageJsonPath))
        {
            try
            {
                packageJson = JsonNode.Parse(await File.ReadAllTextAsync(packageJsonPath)) as JsonObject
                    ?? throw new InvalidOperationException("package.json must contain a JSON object.");
            }
            catch (Exception exception)
            {
                return new(false, false, $"package.json could not be parsed: {exception.Message}");
            }
        }
        else
        {
            packageJson = new JsonObject
            {
                ["name"] = Path.GetFileName(projectRoot).ToLowerInvariant(),
                ["version"] = "1.0.0",
            };
            created = true;
        }

        var changed = created;
        changed |= SetJsonValue(packageJson, "private", true);

        var scripts = EnsureObject(packageJson, "scripts", ref changed);
        changed |= SetJsonValue(scripts, "css:build", TailwindBuildCommand);
        changed |= SetJsonValue(scripts, "css:watch", TailwindWatchCommand);

        var devDependencies = EnsureObject(packageJson, "devDependencies", ref changed);
        changed |= SetJsonValue(devDependencies, "@tailwindcss/cli", "^4.0.0");
        changed |= SetJsonValue(devDependencies, "tailwindcss", "^4.0.0");

        if (changed)
        {
            await File.WriteAllTextAsync(packageJsonPath, packageJson.ToJsonString(JsonOptions) + Environment.NewLine, Utf8NoBom);
        }

        var message = created
            ? "Created package.json with Tailwind scripts and devDependencies."
            : changed
                ? "Updated package.json with Tailwind scripts and devDependencies."
                : "package.json already contains the required Tailwind scripts and devDependencies.";

        return new(true, changed, message);
    }

    private static async Task<CommandResult> EnsureNpmDependenciesInstalledAsync(string projectRoot, bool packageJsonChanged)
    {
        var tailwindPackagePath = Path.Combine(projectRoot, "node_modules", "tailwindcss");
        var tailwindCliPackagePath = Path.Combine(projectRoot, "node_modules", "@tailwindcss", "cli");
        if (!packageJsonChanged && Directory.Exists(tailwindPackagePath) && Directory.Exists(tailwindCliPackagePath))
        {
            return new(true, "Tailwind npm dependencies already installed.");
        }

        return await RunProcessAsync(
            projectRoot,
            GetCommandFileName("npm"),
            GetCommandArguments("npm", "install"),
            "Installed npm dependencies from package.json.",
            "Failed to install npm dependencies. Ensure Node.js and npm are installed, then run `npm install`.");
    }

    private static FileChangeStatus AddTailwindBuildTarget(string projectFilePath)
    {
        var document = XDocument.Load(projectFilePath, LoadOptions.PreserveWhitespace);
        var project = document.Root ?? throw new InvalidOperationException("The project file could not be parsed.");

        var existingTarget = project.Elements("Target")
            .FirstOrDefault(element => string.Equals((string?)element.Attribute("Name"), "BuildTailwindCSS", StringComparison.Ordinal));
        if (existingTarget is not null)
        {
            var exec = existingTarget.Element("Exec");
            var currentCommand = (string?)exec?.Attribute("Command");
            if (string.Equals(currentCommand, TailwindBuildCommand, StringComparison.Ordinal))
            {
                return FileChangeStatus.Unchanged;
            }

            if (exec is null)
            {
                existingTarget.Add(new XElement("Exec", new XAttribute("Command", TailwindBuildCommand)));
            }
            else
            {
                exec.SetAttributeValue("Command", TailwindBuildCommand);
            }

            document.Save(projectFilePath);
            return FileChangeStatus.Updated;
        }

        var propertyGroup = project.Elements("PropertyGroup").LastOrDefault();
        propertyGroup?.AddAfterSelf(
            new XText(Environment.NewLine + Environment.NewLine + "  "),
            new XElement("Target",
                new XAttribute("Name", "BuildTailwindCSS"),
                new XAttribute("BeforeTargets", "Build"),
                new XElement("Exec", new XAttribute("Command", TailwindBuildCommand))),
            new XText(Environment.NewLine));

        document.Save(projectFilePath);
        return FileChangeStatus.Created;
    }

    private static async Task<AppRazorResult> PatchAppRazorAsync(string appRazorPath, string assemblyName, bool removeDefaultAppStylesheet)
    {
        var lines = (await File.ReadAllLinesAsync(appRazorPath)).ToList();
        var originalContent = NormalizeLineEndings(string.Join(Environment.NewLine, lines));

        var headStart = FindLineIndex(lines, "<head>");
        var headEnd = FindLineIndex(lines, "</head>");
        if (headStart < 0 || headEnd <= headStart)
        {
            return new(false, "Could not patch Components/App.razor automatically because the <head> section was not found.");
        }

        var bootstrapStylesheetDetected = HasMatchingHeadLine(lines, headStart, headEnd, line =>
            line.Contains("bootstrap.min.css", StringComparison.OrdinalIgnoreCase));
        var templateStylesheetDetected = HasMatchingHeadLine(lines, headStart, headEnd, line =>
            IsDefaultTemplateStylesheetLine(line, assemblyName));

        if (removeDefaultAppStylesheet)
        {
            RemoveHeadLines(lines, headStart, headEnd, line => IsDefaultTemplateStylesheetLine(line, assemblyName));
            headStart = FindLineIndex(lines, "<head>");
            headEnd = FindLineIndex(lines, "</head>");
            templateStylesheetDetected = false;
        }

        var baseIndex = FindLineIndex(lines, "<base href=\"/\" />", headStart, headEnd);
        if (baseIndex < 0)
        {
            return new(false, "Could not patch Components/App.razor automatically because the <base href=\"/\" /> line was not found.");
        }

        var requiredHeadEntries = new (string Line, Func<string, bool> Exists)[]
        {
            ("    <ResourcePreloader />", line => line.Contains("<ResourcePreloader", StringComparison.Ordinal)),
            ("    <link rel=\"preconnect\" href=\"https://fonts.googleapis.com\" />", line => line.Contains("https://fonts.googleapis.com", StringComparison.OrdinalIgnoreCase)),
            ("    <link rel=\"preconnect\" href=\"https://fonts.gstatic.com\" crossorigin />", line => line.Contains("https://fonts.gstatic.com", StringComparison.OrdinalIgnoreCase)),
            ("    <link href=\"https://fonts.googleapis.com/css2?family=Geist:wght@100..900&family=Geist+Mono:wght@100..900&display=swap\" rel=\"stylesheet\" />", line => line.Contains("family=Geist", StringComparison.OrdinalIgnoreCase)),
            ("    <link rel=\"stylesheet\" href=\"@Assets[\"tailwind.css\"]\" />", line => line.Contains("tailwind.css", StringComparison.OrdinalIgnoreCase)),
            ($"    <link rel=\"stylesheet\" href=\"@Assets[\"{assemblyName}.styles.css\"]\" />", line => line.Contains($"{assemblyName}.styles.css", StringComparison.Ordinal)),
        };

        var insertionIndex = baseIndex + 1;
        foreach (var requiredEntry in requiredHeadEntries)
        {
            if (HasMatchingHeadLine(lines, headStart, headEnd, requiredEntry.Exists))
            {
                continue;
            }

            lines.Insert(insertionIndex, requiredEntry.Line);
            insertionIndex++;
            headEnd++;
        }

        if (FindLineIndex(lines, "<ImportMap />", headStart, headEnd) < 0)
        {
            lines.Insert(headEnd, "    <ImportMap />");
            headEnd++;
        }

        if (FindLineIndex(lines, "<HeadOutlet />", headStart, headEnd) < 0)
        {
            lines.Insert(headEnd, "    <HeadOutlet />");
        }

        var updatedContent = await UpdateBodyTagAsync(lines, appRazorPath);
        if (!updatedContent.Success)
        {
            return new(false, updatedContent.Message);
        }

        var finalContent = NormalizeLineEndings(updatedContent.Content);
        if (string.Equals(finalContent, originalContent, StringComparison.Ordinal))
        {
            return new(true, BuildAppRazorMessage(
                "Components/App.razor already contains the required head links and body classes.",
                bootstrapStylesheetDetected,
                templateStylesheetDetected));
        }

        await File.WriteAllTextAsync(appRazorPath, finalContent + Environment.NewLine, Utf8NoBom);
        return new(true, BuildAppRazorMessage(
            "Updated Components/App.razor with Tailwind, Geist fonts, and body classes.",
            bootstrapStylesheetDetected,
            templateStylesheetDetected));
    }

    private static List<string> DetectBootstrapReferences(string projectRoot)
    {
        var matches = new List<string>();
        foreach (var pattern in BootstrapFilePatterns)
        {
            foreach (var file in Directory.EnumerateFiles(projectRoot, pattern, SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(file);
                if (content.Contains("bootstrap", StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(file);
                }
            }
        }

        return matches;
    }

    private static bool RemoveBootstrapReferences(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var filteredLines = lines
            .Where(line => !line.Contains("bootstrap", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (filteredLines.Length == lines.Length)
        {
            return false;
        }

        File.WriteAllLines(filePath, filteredLines, Utf8NoBom);
        return true;
    }

    private static JsonObject EnsureObject(JsonObject parent, string propertyName, ref bool changed)
    {
        if (parent[propertyName] is JsonObject existingObject)
        {
            return existingObject;
        }

        var createdObject = new JsonObject();
        parent[propertyName] = createdObject;
        changed = true;
        return createdObject;
    }

    private static bool SetJsonValue(JsonObject obj, string propertyName, string value)
    {
        if (string.Equals((string?)obj[propertyName], value, StringComparison.Ordinal))
        {
            return false;
        }

        obj[propertyName] = value;
        return true;
    }

    private static bool SetJsonValue(JsonObject obj, string propertyName, bool value)
    {
        if (obj[propertyName]?.GetValue<bool>() == value)
        {
            return false;
        }

        obj[propertyName] = value;
        return true;
    }

    private static string DescribeFileChange(FileChangeStatus status, string path, string createdMessage, string updatedMessage)
        => status switch
        {
            FileChangeStatus.Created => $"{createdMessage} {path}.",
            FileChangeStatus.Updated => $"{updatedMessage} {path}.",
            _ => $"{path} already contains the required configuration."
        };

    private static int FindLineIndex(IReadOnlyList<string> lines, string target, int start = 0, int? end = null)
    {
        var exclusiveEnd = end ?? lines.Count;
        for (var index = start; index < exclusiveEnd; index++)
        {
            if (string.Equals(lines[index].Trim(), target, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return -1;
    }

    private static bool HasMatchingHeadLine(List<string> lines, int headStart, int headEnd, Func<string, bool> predicate)
        => lines.Skip(headStart + 1).Take(headEnd - headStart - 1).Any(predicate);

    private static void RemoveHeadLines(List<string> lines, int headStart, int headEnd, Func<string, bool> predicate)
    {
        for (var index = headEnd - 1; index > headStart; index--)
        {
            if (predicate(lines[index]))
            {
                lines.RemoveAt(index);
            }
        }
    }

    private static string BuildAppRazorMessage(string baseMessage, bool bootstrapStylesheetDetected, bool templateStylesheetDetected)
    {
        var notes = new List<string>();
        if (bootstrapStylesheetDetected)
        {
            notes.Add("Bootstrap stylesheet links were left in place until you confirm removal.");
        }

        if (templateStylesheetDetected)
        {
            notes.Add("Existing app stylesheet links were left in place; review them if unlayered CSS overrides Tailwind.");
        }

        return notes.Count == 0
            ? baseMessage
            : $"{baseMessage} {string.Join(" ", notes)}";
    }

    private static bool IsDefaultTemplateStylesheetLine(string line, string assemblyName)
    {
        if (!line.Contains("@Assets[", StringComparison.Ordinal))
        {
            return false;
        }

        if (!line.Contains(".css", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (line.Contains("tailwind.css", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (line.Contains($"{assemblyName}.styles.css", StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static Task<ContentResult> UpdateBodyTagAsync(List<string> lines, string appRazorPath)
    {
        var content = string.Join(Environment.NewLine, lines);
        var match = BodyTagRegex().Match(content);
        if (!match.Success)
        {
            return Task.FromResult(new ContentResult(false, string.Empty, $"Could not patch {Path.GetFileName(appRazorPath)} automatically because the <body> tag was not found."));
        }

        var existingTag = match.Value;
        var attrs = match.Groups["attrs"].Value;
        string newTag;

        var classMatch = BodyClassRegex().Match(attrs);
        if (classMatch.Success)
        {
            var classNames = classMatch.Groups["value"].Value
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet(StringComparer.Ordinal);
            classNames.Add("min-h-screen");
            classNames.Add("font-sans");
            classNames.Add("antialiased");

            var updatedClasses = string.Join(" ", classNames);
            var updatedAttrs = BodyClassRegex().Replace(attrs, $"class=\"{updatedClasses}\"", 1);
            newTag = $"<body{updatedAttrs}>";
        }
        else
        {
            newTag = $"<body{attrs} class=\"min-h-screen font-sans antialiased\">";
        }

        return Task.FromResult(new ContentResult(true, content.Replace(existingTag, newTag, StringComparison.Ordinal), string.Empty));
    }

    private static bool RequiresInteractiveRenderMode(string componentName)
        => string.Equals(componentName, "accordion", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "alert-dialog", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "dialog", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "select", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "slider", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "switch", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "toggle", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "toggle-group", StringComparison.OrdinalIgnoreCase)
            || string.Equals(componentName, "tooltip", StringComparison.OrdinalIgnoreCase);

    private static bool RequiresAppRazorScript(string componentName)
        => string.Equals(componentName, "select", StringComparison.OrdinalIgnoreCase);

    private static async Task<CommandResult> EnsureComponentInteractivityAsync(string appRazorPath, string componentDisplayName)
    {
        if (!File.Exists(appRazorPath))
        {
            return new(false, $"{componentDisplayName} requires Components/App.razor so interactive render mode can be configured.");
        }

        var originalContent = NormalizeLineEndings(await File.ReadAllTextAsync(appRazorPath));
        if (!HeadOutletTagRegex().IsMatch(originalContent))
        {
            return new(false, $"{componentDisplayName} requires a <HeadOutlet /> entry in Components/App.razor.");
        }

        if (!RoutesTagRegex().IsMatch(originalContent))
        {
            return new(false, $"{componentDisplayName} requires a <Routes /> entry in Components/App.razor.");
        }

        var updatedContent = AddInteractiveRenderModeIfMissing(originalContent, HeadOutletTagRegex(), out var headOutletChanged);
        updatedContent = AddInteractiveRenderModeIfMissing(updatedContent, RoutesTagRegex(), out var routesChanged);

        if (!headOutletChanged && !routesChanged)
        {
            return new(true, $"Components/App.razor already contains interactive render mode for {componentDisplayName.ToLowerInvariant()}.");
        }

        await File.WriteAllTextAsync(appRazorPath, updatedContent + Environment.NewLine, Utf8NoBom);
        return new(true, $"Updated Components/App.razor with interactive render mode for {componentDisplayName.ToLowerInvariant()}.");
    }

    private static async Task<CommandResult> EnsureComponentAppRazorScriptAsync(string appRazorPath, string componentDisplayName, bool dryRun)
    {
        if (!File.Exists(appRazorPath))
        {
            return new(false, $"{componentDisplayName} requires Components/App.razor so the required client-side script can be configured.");
        }

        var lines = (await File.ReadAllLinesAsync(appRazorPath)).ToList();
        if (lines.Any(line => line.Contains("window.blazorShadcnSelect", StringComparison.Ordinal)))
        {
            return new(true, $"Components/App.razor already contains the required client-side script for {componentDisplayName.ToLowerInvariant()}.");
        }

        var bodyEnd = FindLineIndex(lines, "</body>");
        if (bodyEnd < 0)
        {
            return new(false, $"{componentDisplayName} requires a </body> entry in Components/App.razor.");
        }

        if (dryRun)
        {
            return new(true, "Would update Components/App.razor with the required client-side script for select.");
        }

        lines.InsertRange(bodyEnd, SelectInteropScriptLines);
        var updatedContent = NormalizeLineEndings(string.Join(Environment.NewLine, lines));
        await File.WriteAllTextAsync(appRazorPath, updatedContent + Environment.NewLine, Utf8NoBom);
        return new(true, $"Updated Components/App.razor with the required client-side script for {componentDisplayName.ToLowerInvariant()}.");
    }

    private static string AddInteractiveRenderModeIfMissing(string content, Regex tagRegex, out bool changed)
    {
        var didChange = false;
        var updatedContent = tagRegex.Replace(content, match =>
        {
            var attrs = match.Groups["attrs"].Value;
            if (attrs.Contains("@rendermode", StringComparison.OrdinalIgnoreCase))
            {
                return match.Value;
            }

            didChange = true;
            return $"<{match.Groups["tag"].Value}{attrs} @rendermode=\"{AccordionRenderMode}\" />";
        }, 1);
        changed = didChange;
        return updatedContent;
    }

    private static async Task<CommandResult> RunProcessAsync(string workingDirectory, string fileName, string arguments, string successMessage, string failureMessage)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                }
            };

            process.Start();
            var standardOutput = await process.StandardOutput.ReadToEndAsync();
            var standardError = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                return new(true, successMessage);
            }

            var error = string.IsNullOrWhiteSpace(standardError) ? standardOutput.Trim() : standardError.Trim();
            return new(false, $"{failureMessage} {error}".Trim());
        }
        catch (Exception exception)
        {
            return new(false, $"{failureMessage} {exception.Message}".Trim());
        }
    }

    private static async Task<DoctorCheckResult> CheckCommandAvailableAsync(string id, string commandText, string label)
    {
        var splitIndex = commandText.IndexOf(' ');
        var command = splitIndex > 0 ? commandText[..splitIndex] : commandText;
        var arguments = splitIndex > 0 ? commandText[(splitIndex + 1)..] : string.Empty;
        var result = await RunProcessAsync(
            Directory.GetCurrentDirectory(),
            GetCommandFileName(command),
            GetCommandArguments(command, arguments),
            $"{label} is available.",
            $"{label} is not available.");

        return new(id, label, result.Success ? "ok" : "error", result.Message);
    }

    private static DoctorCheckResult CheckProjectRoot(string? projectRoot)
        => projectRoot is null
            ? new("project-root", "Blazor project", "warn", $"No project detected from {Directory.GetCurrentDirectory()}.")
            : new("project-root", "Blazor project", "ok", $"Detected project at {projectRoot}");

    private static DoctorCheckResult CheckAppRazor(string? projectRoot)
    {
        if (projectRoot is null)
        {
            return new("app-razor", "Components/App.razor", "warn", "Project not detected.");
        }

        var path = Path.Combine(projectRoot, "Components", "App.razor");
        return File.Exists(path)
            ? new("app-razor", "Components/App.razor", "ok", "Found Components/App.razor.")
            : new("app-razor", "Components/App.razor", "error", "Missing Components/App.razor.");
    }

    private static DoctorCheckResult CheckPackageJson(string? projectRoot)
    {
        if (projectRoot is null)
        {
            return new("package-json", "package.json", "warn", "Project not detected.");
        }

        var path = Path.Combine(projectRoot, "package.json");
        return File.Exists(path)
            ? new("package-json", "package.json", "ok", "Found package.json.")
            : new("package-json", "package.json", "warn", "package.json not found. `init` can create it.");
    }

    private static DoctorCheckResult CheckTailwindModules(string? projectRoot)
    {
        if (projectRoot is null)
        {
            return new("tailwind-modules", "Tailwind npm packages", "warn", "Project not detected.");
        }

        var tailwindPackagePath = Path.Combine(projectRoot, "node_modules", "tailwindcss");
        var tailwindCliPackagePath = Path.Combine(projectRoot, "node_modules", "@tailwindcss", "cli");
        return Directory.Exists(tailwindPackagePath) && Directory.Exists(tailwindCliPackagePath)
            ? new("tailwind-modules", "Tailwind npm packages", "ok", "Tailwind packages are installed.")
            : new("tailwind-modules", "Tailwind npm packages", "warn", "Tailwind packages are not installed yet.");
    }

    private static async Task<DoctorCheckResult> CheckRemoteAsync(string id, string url, string label)
    {
        try
        {
            using var response = await HttpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return new(id, label, "ok", $"Reachable: {url}");
            }

            return new(id, label, "error", $"Request failed with status {(int)response.StatusCode}.");
        }
        catch (Exception exception)
        {
            return new(id, label, "error", $"Request failed: {exception.Message}");
        }
    }

    private static string GetCommandFileName(string command)
        => OperatingSystem.IsWindows() ? "cmd" : command;

    private static string GetCommandArguments(string command, string arguments)
        => OperatingSystem.IsWindows() ? $"/c {command} {arguments}".TrimEnd() : arguments;

    private static string QuoteArgument(string value)
        => $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";

    private static async Task<CleanStarterResult> CleanStarterProjectAsync(string projectRoot)
    {
        try
        {
            var messages = new List<string>();
            Directory.CreateDirectory(Path.Combine(projectRoot, "Components", "UI"));

            var homePagePath = Path.Combine(projectRoot, "Components", "Pages", "Home.razor");
            if (File.Exists(homePagePath))
            {
                const string homePageContent = """
                    @page "/"

                    <PageTitle>Home</PageTitle>
                    """;
                await File.WriteAllTextAsync(homePagePath, homePageContent + Environment.NewLine, Utf8NoBom);
                messages.Add("Cleaned Components/Pages/Home.razor.");
            }

            var mainLayoutPath = Path.Combine(projectRoot, "Components", "Layout", "MainLayout.razor");
            if (File.Exists(mainLayoutPath))
            {
                const string mainLayoutContent = """
                    @inherits LayoutComponentBase

                    <main class="min-h-screen">
                        @Body
                    </main>
                    """;
                await File.WriteAllTextAsync(mainLayoutPath, mainLayoutContent + Environment.NewLine, Utf8NoBom);
                messages.Add("Simplified Components/Layout/MainLayout.razor.");
            }

            DeleteIfExists(projectRoot, Path.Combine(projectRoot, "Components", "Layout", "NavMenu.razor"), messages);
            DeleteIfExists(projectRoot, Path.Combine(projectRoot, "Components", "Layout", "NavMenu.razor.css"), messages);
            DeleteIfExists(projectRoot, Path.Combine(projectRoot, "Components", "Pages", "Counter.razor"), messages);
            DeleteIfExists(projectRoot, Path.Combine(projectRoot, "Components", "Pages", "Counter.razor.css"), messages);
            DeleteIfExists(projectRoot, Path.Combine(projectRoot, "Components", "Pages", "Weather.razor"), messages);
            DeleteIfExists(projectRoot, Path.Combine(projectRoot, "Components", "Pages", "Weather.razor.css"), messages);

            var appCssPath = Path.Combine(projectRoot, "wwwroot", "app.css");
            if (File.Exists(appCssPath))
            {
                await File.WriteAllTextAsync(appCssPath, string.Empty, Utf8NoBom);
                messages.Add("Cleared wwwroot/app.css.");
            }

            messages.Add("Starter cleanup complete.");
            return new(true, messages, string.Empty);
        }
        catch (Exception exception)
        {
            return new(false, Array.Empty<string>(), $"Failed to clean the starter project: {exception.Message}");
        }
    }

    private static void DeleteIfExists(string projectRoot, string path, List<string> messages)
    {
        if (!File.Exists(path))
        {
            return;
        }

        File.Delete(path);
        messages.Add($"Removed {Path.GetRelativePath(projectRoot, path)}.");
    }

    private static async Task<CachedUpdateState?> ReadCachedUpdateAsync(string cachePath)
    {
        try
        {
            if (!File.Exists(cachePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(cachePath);
            return JsonSerializer.Deserialize<CachedUpdateState>(json);
        }
        catch
        {
            return null;
        }
    }

    private static async Task WriteCachedUpdateAsync(string cachePath, string latestVersion)
    {
        var directory = Path.GetDirectoryName(cachePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var state = new CachedUpdateState(DateTimeOffset.UtcNow, latestVersion);
        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(state, JsonOptions) + Environment.NewLine, Utf8NoBom);
    }

    private static string GetUpdateCachePath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(root))
        {
            root = Path.GetTempPath();
        }

        return Path.Combine(root, ToolName, UpdateCacheFileName);
    }

    private static string GetCurrentVersion()
    {
        var assembly = typeof(BlazorShadcnCli).Assembly;
        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            var plusIndex = informationalVersion.IndexOf('+');
            return plusIndex >= 0 ? informationalVersion[..plusIndex] : informationalVersion;
        }

        return assembly.GetName().Version?.ToString(3) ?? "0.0.0";
    }

    private static bool TryParseVersion(string version, out Version parsed)
    {
        var sanitized = version;
        var dashIndex = sanitized.IndexOf('-');
        if (dashIndex >= 0)
        {
            sanitized = sanitized[..dashIndex];
        }

        return Version.TryParse(sanitized, out parsed!);
    }

    private static bool HasHelpFlag(IEnumerable<string> args)
        => args.Any(arg => arg is "--help" or "-h");

    private static string NormalizeLineEndings(string content)
        => content.Replace("\r\n", "\n").Replace('\r', '\n');

    private static string GetProjectHint()
        => "Use `blazor-shadcn doctor` to inspect the current environment.";

    private static AddOptions ParseAddOptions(string[] args)
    {
        string? componentName = null;
        var force = false;
        var dryRun = false;

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "--force":
                    force = true;
                    break;
                case "--dry-run":
                    dryRun = true;
                    break;
                default:
                    if (arg.StartsWith("-", StringComparison.Ordinal))
                    {
                        return new(false, null, false, false, $"Unknown option for add: {arg}");
                    }

                    if (componentName is not null)
                    {
                        return new(false, null, false, false, "Only one component can be added at a time.");
                    }

                    componentName = arg.Trim().ToLowerInvariant();
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(componentName))
        {
            return new(false, null, false, false, "Missing component name.");
        }

        return new(true, componentName, force, dryRun, string.Empty);
    }

    private static InitOptions ParseInitOptions(string[] args)
    {
        var yes = false;
        var removeBootstrap = false;
        var skipInstall = false;
        var dryRun = false;

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "--yes":
                    yes = true;
                    removeBootstrap = true;
                    break;
                case "--remove-bootstrap":
                    removeBootstrap = true;
                    break;
                case "--no-bootstrap-removal":
                    removeBootstrap = false;
                    yes = true;
                    break;
                case "--skip-install":
                    skipInstall = true;
                    break;
                case "--dry-run":
                    dryRun = true;
                    break;
                default:
                    if (arg.StartsWith("-", StringComparison.Ordinal))
                    {
                        return new(false, false, false, false, false, $"Unknown option for init: {arg}");
                    }

                    return new(false, false, false, false, false, $"Unexpected argument for init: {arg}");
            }
        }

        return new(true, yes, removeBootstrap, skipInstall, dryRun, string.Empty);
    }

    private static NewOptions ParseNewOptions(string[] args)
    {
        string? projectName = null;
        var skipInstall = false;
        var dryRun = false;

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "--skip-install":
                case "--no-install":
                    skipInstall = true;
                    break;
                case "--dry-run":
                    dryRun = true;
                    break;
                default:
                    if (arg.StartsWith("-", StringComparison.Ordinal))
                    {
                        return new(false, null, false, false, $"Unknown option for new: {arg}");
                    }

                    if (projectName is not null)
                    {
                        return new(false, null, false, false, "Only one project name can be provided.");
                    }

                    projectName = arg.Trim();
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(projectName))
        {
            return new(false, null, false, false, "Missing project name.");
        }

        return new(true, projectName, skipInstall, dryRun, string.Empty);
    }

    private static void PrintUsage()
    {
        Console.WriteLine($"Usage: {ToolName} <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  new <project-name>   Create a new Blazor app configured for blazor-shadcn");
        Console.WriteLine("  init                 Configure the current Blazor project");
        Console.WriteLine("  add <component>      Add a component into Components/UI");
        Console.WriteLine("  list                 List available components");
        Console.WriteLine("  doctor               Validate local prerequisites and project shape");
        Console.WriteLine("  version              Show the current CLI version");
        Console.WriteLine("  help [command]       Show general or command-specific help");
        Console.WriteLine();
        Console.WriteLine("Global options:");
        Console.WriteLine("  -h, --help           Show help");
        Console.WriteLine("  -v, --version        Show version");
    }

    private static void PrintNewHelp()
    {
        Console.WriteLine($"Usage: {ToolName} new <project-name> [--skip-install|--no-install] [--dry-run]");
        Console.WriteLine("Create a new Blazor project, remove the template demo content, and configure Tailwind.");
    }

    private static void PrintInitHelp()
    {
        Console.WriteLine($"Usage: {ToolName} init [--yes] [--remove-bootstrap] [--no-bootstrap-removal] [--skip-install] [--dry-run]");
        Console.WriteLine("Configure the current Blazor project for blazor-shadcn.");
        Console.WriteLine("--yes removes interactive prompting and accepts the default bootstrap cleanup action.");
    }

    private static void PrintAddHelp()
    {
        Console.WriteLine($"Usage: {ToolName} add <component> [--force] [--dry-run]");
        Console.WriteLine("Download a component into Components/UI.");
    }

    private static void PrintListHelp()
    {
        Console.WriteLine($"Usage: {ToolName} list [--json]");
        Console.WriteLine("List available components.");
    }

    private static void PrintDoctorHelp()
    {
        Console.WriteLine($"Usage: {ToolName} doctor [--json]");
        Console.WriteLine("Validate dotnet, npm, project detection, and remote availability.");
    }

    private static void PrintVersionHelp()
    {
        Console.WriteLine($"Usage: {ToolName} version [--check]");
        Console.WriteLine("Show the current CLI version and optionally check for a newer published package.");
    }

    [GeneratedRegex("<body(?<attrs>[^>]*)>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BodyTagRegex();

    [GeneratedRegex("class\\s*=\\s*\"(?<value>[^\"]*)\"", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BodyClassRegex();

    [GeneratedRegex("<(?<tag>HeadOutlet)(?<attrs>[^>]*)\\s*/>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex HeadOutletTagRegex();

    [GeneratedRegex("<(?<tag>Routes)(?<attrs>[^>]*)\\s*/>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex RoutesTagRegex();

    private sealed record ParsedInvocation(bool Success, bool ShowVersionOnly, ParsedCommand? Command, string ErrorMessage);
    private sealed record ParsedCommand(string Name, string[] Arguments);
    private sealed record ComponentDefinition(
        string Name,
        string[] FileNames,
        string Description,
        string[]? DependencyNames = null,
        string? SourceDirectory = null)
    {
        public string PrimaryFileName => FileNames[0];
        public string DisplayName => Name[..1].ToUpperInvariant() + Name[1..];
        public string[] Dependencies => DependencyNames ?? [];
    }
    private sealed record ComponentFileOperation(string FileName, string TargetPath, string SourceUrl);
    private sealed record ComponentInstallResult(bool Success, string Message);

    private sealed record AddOptions(bool Success, string? ComponentName, bool Force, bool DryRun, string ErrorMessage);
    private sealed record InitOptions(bool Success, bool Yes, bool RemoveBootstrapReferences, bool SkipInstall, bool DryRun, string ErrorMessage);
    private sealed record NewOptions(bool Success, string? ProjectName, bool SkipInstall, bool DryRun, string ErrorMessage);
    private sealed record ConfigureOptions(bool PromptForBootstrapRemoval, bool RemoveBootstrapReferences, bool RemoveDefaultAppStylesheet, bool SkipNpmInstall, bool DryRun);
    private sealed record PackageJsonResult(bool Success, bool PackageJsonChanged, string Message);
    private sealed record CommandResult(bool Success, string Message);
    private sealed record AppRazorResult(bool Success, string Message);
    private sealed record ContentResult(bool Success, string Content, string Message);
    private sealed record StepResult(FileChangeStatus Status, string Message);
    private sealed record MultiMessageStepResult(IReadOnlyList<string> Messages);
    private sealed record CleanStarterResult(bool Success, IReadOnlyList<string> Messages, string Message);
    private sealed record DoctorCheckResult(string Id, string Label, string Status, string Message);
    private sealed record CachedUpdateState(DateTimeOffset CheckedAtUtc, string LatestVersion);
    private sealed record UpdateCheckResult(bool Success, bool HasUpdate, string LatestVersion, string Message);

    private enum FileChangeStatus
    {
        Unchanged,
        Created,
        Updated,
    }
}
