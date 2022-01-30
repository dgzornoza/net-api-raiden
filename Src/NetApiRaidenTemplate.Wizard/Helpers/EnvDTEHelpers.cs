using System.Collections.Generic;
using System.Linq;
using EnvDTE;

namespace NetApiRaidenTemplate.Wizard.Helpers
{
    public static class EnvDTEHelpers
    {
        public static IEnumerable<Project> GetAllProjects(this Solution sln)
        {
            return sln.Projects.Cast<Project>()
                .Where(item => item.Kind == ProjectKinds.ProjectKindSolutionFolder && item.ProjectItems != null)
                .SelectMany(item => item.ProjectItems.Cast<ProjectItem>())
                .Where(item => item.Object is Project)
                .Select(item => item.Object as Project);
        }
    }
}
