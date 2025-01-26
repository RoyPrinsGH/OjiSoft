using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

public interface IProjectBuilder
{
    public string ProjectName { get; }

    public bool Build(StatusContext? ctx = null);
}