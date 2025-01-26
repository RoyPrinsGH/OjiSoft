using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

public interface IProjectDefinition
{
    public string ProjectName { get; }

    public bool Build(StatusContext? ctx = null);
}