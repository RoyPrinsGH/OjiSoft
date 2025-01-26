using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

/// <summary>
/// This class holds the build logic for the Account Portal UI project.
/// </summary>
public sealed class OjiSoftAccountPortalUIBuilder : IProjectBuilder
{
    public string ProjectName { get; } = "OjiSoft Account Portal UI";

    public bool Build(StatusContext? ctx = null)
    {
        // Check if Source directory exists
        if (!Directory.Exists("Source"))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] ./Source/ directory not found.");
            return false;
        }

        // Check if Source\OjiSoft-AccountPortal-UI directory exists
        if (!Directory.Exists("Source/OjiSoft-AccountPortal-UI"))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] ./Source/OjiSoft-AccountPortal-UI/ directory not found.");
            return false;
        }

        // CD into Source\OjiSoft-AccountPortal-UI
        Directory.SetCurrentDirectory("Source/OjiSoft-AccountPortal-UI");

        // Run npm install'
        AnsiConsole.MarkupLine($"[purple]{ProjectName}[/] - [yellow]Running npm install...[/]");
        ctx?.Status("Running npm install...");

        var process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"npm install && exit\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] npm install failed with exit code " + process.ExitCode);
            return false;
        }

        // Run npm run build
        AnsiConsole.MarkupLine($"[purple]{ProjectName}[/] - [yellow]Running npm run build...[/]");
        ctx?.Status("Running npm run build...");

        process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"npm run build && exit\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] npm run build failed with exit code " + process.ExitCode);
            return false;
        }

        // CD back to the root directory
        Directory.SetCurrentDirectory("../../");

        // Xcopy the build output to the appropriate directory
        AnsiConsole.MarkupLine($"[purple]{ProjectName}[/] - [yellow]Copying build output...[/]");
        ctx?.Status("Copying build output...");

        process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"xcopy /E /Y /I \"./Source/OjiSoft-AccountPortal-UI/dist\" \"./Build/AccountPortalUI\"\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] xcopy failed with exit code " + process.ExitCode);
            return false;
        }

        return true;
    }
}