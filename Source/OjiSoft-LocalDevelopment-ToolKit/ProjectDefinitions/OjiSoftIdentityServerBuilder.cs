using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

/// <summary>
/// This class holds the build logic for the Account Portal UI project.
/// </summary>
public sealed class OjiSoftIdentityServerBuilder : IProjectBuilder
{
    public string ProjectName { get; } = "OjiSoft Identity Server";

    public bool Build(StatusContext? ctx = null)
    {
        // Check if Source directory exists
        if (!Directory.Exists("Source"))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] ./Source/ directory not found.");
            return false;
        }

        // Check if Source\IdentityProvider directory exists
        if (!Directory.Exists("Source/OjiSoft-IdentityProvider"))
        {
            AnsiConsole.MarkupLine("[red]Error:[/] ./Source/OjiSoft-IdentityProvider/ directory not found.");
            return false;
        }

        // CD into Source\IdentityProvider
        Directory.SetCurrentDirectory("Source/OjiSoft-IdentityProvider");

        AnsiConsole.MarkupLine($"[purple]{ProjectName}[/] - [yellow]Running dotnet publish...[/]");
        ctx?.Status("Running dotnet publish...");

        // publish into the ../../Build/IdentityProvider directory
        var process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"dotnet publish -c Release -o ../../Build/IdentityProvider && exit\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] dotnet publish failed with exit code " + process.ExitCode);
            return false;
        }

        // CD back to the root directory
        Directory.SetCurrentDirectory("../../");

        return true;
    }
}