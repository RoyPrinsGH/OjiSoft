using System.Reflection;
using OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;
using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit;

/// <summary>
/// The main entry point for the application. This application is a toolkit for local development,
/// allowing developers to easily manage their local development environment.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        // Change working directory to the given argument, if it exists.
        if (args.Length > 0)
        {
            if (Directory.Exists(args[0]))
            {
                Directory.SetCurrentDirectory(args[0]);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Error:[/] The specified directory does not exist.");
            }
        }

        // We expect to be running in the OjiSoft root directory, so we check for the existence of the .git directory, and warn the user if it is not found.
        if (!Directory.Exists(".git"))
        {
            AnsiConsole.MarkupLine("[yellow]Warning:[/] This application is intended to be run from the root of the OjiSoft repository. Please ensure you are in the correct directory before proceeding.");
        }

        var mainOptions = new SelectionPrompt<string>()
            .Title("[purple]OjiSoft Local Development Toolkit[/]")
            .AddChoices(["Test", "Build", "Deploy", "Exit"]);

        bool shouldExit = false;

        while (!shouldExit)
        {
            var action = AnsiConsole.Prompt(mainOptions);

            // Based on the user's selection, we will call the appropriate method.
            switch (action)
            {
                case "Test":
                    // Placeholder for test logic.
                    break;
                case "Build":
                    BuildForProduction();
                    break;
                case "Deploy":
                    DeployToProduction();
                    break;
                case "Exit":
                    AnsiConsole.MarkupLine("[yellow]Exiting...[/]");
                    shouldExit = true;
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Error:[/] Invalid selection.");
                    break;
            }
        }
    }

    private static bool BuildForProduction()
    {
        var projectTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IBuildableProject)))
            .ToList();

        if (projectTypes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] No buildable projects found.");
            return false;
        }

        var projects = projectTypes.Select(t => (IBuildableProject)Activator.CreateInstance(t)!).ToList();

        var projectsToBuild = new MultiSelectionPrompt<IBuildableProject>()
            .Title("Select project(s) to build")
            .AddChoices(projects)
            .UseConverter(p => p.ProjectName)
            .NotRequired();

        var selectedProjects = AnsiConsole.Prompt(projectsToBuild);

        if (selectedProjects.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] No projects selected.");
            return false;
        }

        bool buildSuccess = false;

        AnsiConsole.Status()
            .Start("Starting...", ctx => 
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("green"));
                foreach (var project in selectedProjects)
                {
                    buildSuccess = project.Build(ctx);
                    if (!buildSuccess)
                    {
                        AnsiConsole.MarkupLine($"[red]Error:[/] Build failed for {project}.");
                        return;
                    }
                }
            });

        if (!buildSuccess)
        {
            return false;
        }

        AnsiConsole.MarkupLine("[green]Build complete![/]");
        return true;
    }

    private static bool DeployToProduction()
    {
        var projectTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IDeployableProject)))
            .ToList();

        if (projectTypes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] No deployable projects found.");
            return false;
        }

        var projects = projectTypes.Select(t => (IDeployableProject)Activator.CreateInstance(t)!).ToList();

        var projectsToDeploy = new MultiSelectionPrompt<IDeployableProject>()
            .Title("Select project(s) to deploy, the selected projects will go through the build & test process on the runner before being deployed to the production server.")
            .AddChoices(projects)
            .UseConverter(p => p.ProjectName);

        var selectedProjects = AnsiConsole.Prompt(projectsToDeploy);

        if (selectedProjects.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] No projects selected.");
            return false;
        }

        bool deploySuccess = false;

        AnsiConsole.Status()
            .Start("Starting...", ctx => 
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("green"));
                foreach (var project in selectedProjects)
                {
                    deploySuccess = project.Deploy(ctx);
                    if (!deploySuccess)
                    {
                        AnsiConsole.MarkupLine($"[red]Error:[/] Deployment failed for {project}.");
                        return;
                    }
                }
            });

        if (!deploySuccess)
        {
            return false;
        }

        AnsiConsole.MarkupLine("[green]Deployment complete![/]");
        return true;
    }
}