using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

public interface IProject
{
    public string ProjectName { get; }

    public string ProjectFolderName { get; }

    public bool Build(StatusContext? ctx = null);
}