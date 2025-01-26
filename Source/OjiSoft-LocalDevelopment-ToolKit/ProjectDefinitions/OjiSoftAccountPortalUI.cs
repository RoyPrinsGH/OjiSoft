using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

/// <summary>
/// This class holds the build logic for the Account Portal UI project.
/// </summary>
public sealed class OjiSoftAccountPortalUI : IProject
{
    public string ProjectName { get; } = "OjiSoft Account Portal UI";

    public string ProjectFolderName { get; } = "OjiSoft-AccountPortal-UI";

    public bool Build(StatusContext? ctx = null)
    {
        if (!Directory.Exists("Source"))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] ./Source/ directory not found.");
            return false;
        }

        if (!Directory.Exists($"Source/{ProjectFolderName}"))
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] ./Source/{ProjectFolderName}/ directory not found.");
            return false;
        }

        // Clear the build subdirectory if it exists
        if (Directory.Exists($"Build/{ProjectFolderName}"))
        {
            Directory.Delete($"Build/{ProjectFolderName}", true);
        }

        Directory.SetCurrentDirectory($"Source/{ProjectFolderName}");

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

        process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"xcopy /E /Y /I \"./Source/" + ProjectFolderName + "/dist\" \"./Build/" + ProjectFolderName + "\"\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] xcopy failed with exit code " + process.ExitCode);
            return false;
        }

        return true;
    }
}