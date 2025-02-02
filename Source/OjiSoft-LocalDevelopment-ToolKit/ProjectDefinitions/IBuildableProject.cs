using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

public interface IBuildableProject : IProject
{
    public bool Build(StatusContext? ctx = null);
}