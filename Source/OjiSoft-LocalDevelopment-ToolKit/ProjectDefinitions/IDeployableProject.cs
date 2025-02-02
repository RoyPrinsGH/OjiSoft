using Spectre.Console;

namespace OjiSoft.LocalDevelopmentToolKit.ProjectDefinitions;

public interface IDeployableProject : IProject
{
    public string ProductionBranchName { get; }
    public bool Deploy(StatusContext? ctx = null);
}