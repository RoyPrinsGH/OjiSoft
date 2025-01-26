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
            .AddChoices(["Run in development mode", "Build for production", "Test", "Exit"]);

        bool shouldExit = false;

        while (!shouldExit)
        {
            var action = AnsiConsole.Prompt(mainOptions);

            // Based on the user's selection, we will call the appropriate method.
            switch (action)
            {
                case "Build for production":
                    BuildForProduction();
                    break;
                case "Run in development mode":
                    // Placeholder for development mode logic.
                    break;
                case "Test":
                    // Placeholder for test logic.
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
        // Get project names via Reflection
        var projectTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IProjectBuilder)))
            .ToList();

        var projects = projectTypes.Select(t => (IProjectBuilder)Activator.CreateInstance(t)!).ToList();

        // Select project to build.
        var projectsToBuild = new MultiSelectionPrompt<string>()
            .Title("Select project(s) to build")
            .AddChoices(projects.Select(p => p.ProjectName));

        var selectedProjects = AnsiConsole.Prompt(projectsToBuild);

        bool buildSuccess = false;

        AnsiConsole.Status()
            .Start("Starting...", ctx => 
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("green"));
                foreach (var project in selectedProjects)
                {
                    buildSuccess = projects.First(p => p.ProjectName == project).Build(ctx);
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
}