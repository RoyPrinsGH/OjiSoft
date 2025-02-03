using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

/// <summary>
/// This class holds the build logic for the Account Portal UI project.
/// </summary>
public sealed class OjiSoftIdentityServer : IProject, IBuildableProject, IDeployableProject
{
    public string ProjectName { get; } = "OjiSoft Identity Server";

    public string ProjectFolderName { get; } = "OjiSoft-IdentityProvider";

    public string ProductionBranchName { get; } = "ojisoft-identity-provider-production";

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

        AnsiConsole.MarkupLine($"{ProjectName} - [yellow]Running dotnet publish...[/]");
        ctx?.Status("Running dotnet publish...");

        var process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -o ../../Build/" + ProjectFolderName + " && exit\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] dotnet publish failed with exit code " + process.ExitCode);
            return false;
        }

        Directory.SetCurrentDirectory("../../");

        return true;
    }

    public bool Deploy(StatusContext? ctx = null)
    {
        var doDeployment = AnsiConsole.Confirm("Are you sure you want to deploy the OjiSoft Identity Server to production?", false);
        if (doDeployment == false)
        {
            AnsiConsole.MarkupLine("[yellow]Deployment cancelled.[/]");
            return false;
        }

        AnsiConsole.MarkupLine($"{ProjectName} - [yellow]Deploying to production...[/]");
        ctx?.Status("Deploying to production...");

        var process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"git checkout " + ProductionBranchName + " && git pull origin " + ProductionBranchName + " && git merge master && git push origin " + ProductionBranchName + " && exit\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Deployment failed with exit code " + process.ExitCode);
            return false;
        }

        // Return to master
        process = System.Diagnostics.Process.Start("CMD.exe", "/C start /WAIT cmd /C \"git checkout master && exit\"");
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            AnsiConsole.MarkupLine("[yellow]Warning:[/] Failed to return to master branch with exit code " + process.ExitCode);
            return true;
        }

        return true;
    }
}