using System.Collections.Generic;
using EnvDTE;
using NetApiRaidenTemplate.Wizard.Helpers;
using System.Linq;

namespace NetApiRaidenTemplate.Wizard.FeaturesManagers
{
    public abstract class FeatureManagerBase : IFeatureManager
    {
        private readonly Solution solution;
        private readonly IDictionary<Project, IEnumerable<string>> projectsFilesPaths;

        protected FeatureManagerBase(Solution solution)
        {
            this.solution = solution;
            projectsFilesPaths = solution.GetAllProjects()
                .ToDictionary(keySelector => keySelector, valueSelector => GetAllProjectItemsFilesPaths(valueSelector.ProjectItems.Cast<ProjectItem>()));
        }

        protected abstract string FeatureName { get; }

        protected abstract IEnumerable<string> FeatureFilesNames { get; }

        protected abstract IDictionary<string, string> FeatureFileNamesMappers { get; }

        public virtual void Exclude()
        {
            RemoveFiles();
            RemoveFeatureCode();
        }

        public virtual void Include()
        {
            ReplaceFileNames();
        }

        private void ReplaceFileNames()
        {
        }

        private void RemoveFiles()
        {

        }

        private void RemoveFeatureCode()
        {
        }

        private IEnumerable<string> GetAllProjectItemsFilesPaths(IEnumerable<ProjectItem> projectItems)
        {
            var filenames = new List<string>();
            foreach (var projectItem in projectItems)
            {
                var nestedProjectItems = projectItem.ProjectItems.Cast<ProjectItem>();
                if (!nestedProjectItems.Any())
                {
                    filenames.Add(projectItem.FileNames[0]);
                }

                filenames.AddRange(GetAllProjectItemsFilesPaths(nestedProjectItems));
            }

            return filenames;
        }
    }
}
