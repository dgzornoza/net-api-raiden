using System.Collections.Generic;
using EnvDTE;
using NetApiRaidenTemplate.Wizard.Helpers;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace NetApiRaidenTemplate.Wizard.FeaturesManagers
{
    public abstract class FeatureManagerBase : IFeatureManager
    {
        protected FeatureManagerBase(Solution solution)
        {
            this.solution = solution;
            ProjectsFilesPaths = solution.GetAllProjects()
                .ToDictionary(keySelector => keySelector, valueSelector => GetAllProjectItemsFilesPaths(valueSelector.ProjectItems.Cast<ProjectItem>()));
        }

        protected Solution solution { get; private set; }

        public IReadOnlyDictionary<Project, IEnumerable<string>> ProjectsFilesPaths { get; private set; }

        protected abstract string FeatureName { get; }

        protected abstract IEnumerable<string> FeatureFilesNames { get; }

        protected abstract IDictionary<string, string> FeatureFileNamesMappers { get; }

        public virtual void Exclude()
        {
            RemoveFiles();

            foreach (var filePath in ProjectsFilesPaths.SelectMany(item => item.Value))
            {
                RemoveFeatureContentFromFile(filePath);
                RemoveFeatureTagsFromFile(filePath);
            }
        }

        public virtual void Include()
        {
            ReplaceFileNames();

            foreach (var filePath in ProjectsFilesPaths.SelectMany(item => item.Value))
            {
                RemoveFeatureTagsFromFile(filePath);
            }
        }

        private void ReplaceFileNames()
        {
            var filesToRename = ProjectsFilesPaths.Values.SelectMany(item => item)
                .Where(item => FeatureFileNamesMappers.Any(featureFileNamesMappers => item.EndsWith(featureFileNamesMappers.Key)));

            foreach (var file in filesToRename)
            {
                var fileNameMap = FeatureFileNamesMappers.First(item => file.EndsWith(item.Key));

                File.Move(file, file.Replace(fileNameMap.Key, fileNameMap.Value));
            }
        }

        private void RemoveFiles()
        {
            var filesToRemove = ProjectsFilesPaths.Values.SelectMany(item => item)
                .Where(item => FeatureFilesNames.Any(featureFile => item.EndsWith(featureFile)));

            foreach (var file in filesToRemove)
            {
                File.Delete(file);
            }
        }

        private void RemoveFeatureContentFromFile(string filePath)
        {
            bool isInFeatureCode = false;
            var outputFileLines = File.ReadAllLines(filePath).Select(item =>
            {
                string line = null;
                if (item.Contains($"${FeatureName}$ start"))
                {
                    isInFeatureCode = true;
                    line = item;
                }
                else if (item.Contains($"${FeatureName}$ end"))
                {
                    isInFeatureCode = false;
                }

                if (!isInFeatureCode)
                {
                    line = item;
                }

                return line;
            }).OfType<string>();

            File.WriteAllLines(filePath, outputFileLines);
        }

        private void RemoveFeatureTagsFromFile(string filePath)
        {
            var outputFileLines = File.ReadAllLines(filePath).Select(item =>
            {
                string line = null;
                if (item.Contains($"${FeatureName}$ start |"))
                {
                    line = item.Replace($"${FeatureName}$ start |", string.Empty);
                }
                else if (item.Contains($"${FeatureName}$ end |"))
                {
                    line = item.Replace($"${FeatureName}$ end |", string.Empty);
                }
                else
                {
                    line = item;
                }

                return line;
            });

            File.WriteAllLines(filePath, outputFileLines);
        }

        private IEnumerable<string> GetAllProjectItemsFilesPaths(IEnumerable<ProjectItem> projectItems)
        {
            var filenames = new List<string>();
            foreach (var projectItem in projectItems)
            {
                var nestedProjectItems = projectItem.ProjectItems.Cast<ProjectItem>();
                if (!nestedProjectItems.Any() && projectItem.Kind != ProjectKinds.FolderItem)
                {
                    filenames.Add(projectItem.FileNames[0]);
                }

                filenames.AddRange(GetAllProjectItemsFilesPaths(nestedProjectItems));
            }

            return filenames;
        }
    }
}
