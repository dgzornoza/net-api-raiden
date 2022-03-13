using System.Collections.Generic;
using EnvDTE;
using NetApiRaidenTemplate.Wizard.Helpers;
using System.Linq;
using System.IO;

namespace NetApiRaidenTemplate.Wizard.FeaturesManagers
{
    public abstract class FeatureManagerBase : IFeatureManager
    {
        private static IEnumerable<string> CommonFiles { get; } = new List<string>()
        {
            @"Readme.html",
        };

        protected FeatureManagerBase(Solution solution)
        {
            this.Solution = solution;
            ProjectsFilesPaths = solution.GetAllProjects().ToDictionary(
                keySelector => keySelector,
                valueSelector => (IList<string>)GetAllProjectItemsFilesPaths(valueSelector.ProjectItems.Cast<ProjectItem>()).ToList());

            this.SolutionPath = Directory.GetParent(Path.GetDirectoryName(ProjectsFilesPaths.First().Key.FileName)).FullName;
        }

        protected string SolutionPath { get; set; }

        protected Solution Solution { get; private set; }

        protected IDictionary<Project, IList<string>> ProjectsFilesPaths { get; private set; }

        protected abstract string FeatureName { get; }

        protected abstract IEnumerable<string> ExclusiveFeatureFiles { get; }

        protected abstract IDictionary<string, string> FeatureFileNamesMappers { get; }

        public virtual void Exclude()
        {
            // common files
            foreach (var commonFile in CommonFiles)
            {
                var filename = Path.Combine(this.SolutionPath, commonFile);
                RemoveFeatureContentFromFile(filename);
                RemoveFeatureTagsFromFile(filename);
            }

            foreach (var projectFilesPaths in ProjectsFilesPaths)
            {
                // project
                RemoveFiles(projectFilesPaths);
                RemoveFeatureContentFromFile(projectFilesPaths.Key.FileName);
                RemoveFeatureTagsFromFile(projectFilesPaths.Key.FileName);

                // project items
                foreach (var filePath in projectFilesPaths.Value)
                {
                    RemoveFeatureContentFromFile(filePath);
                    RemoveFeatureTagsFromFile(filePath);
                }
            }
        }

        public virtual void Include()
        {
            // common files
            foreach (var commonFile in CommonFiles)
            {
                var filename = Path.Combine(this.SolutionPath, commonFile);
                RemoveFeatureContentFromFile(filename);
                RemoveFeatureTagsFromFile(filename);
            }

            foreach (var projectFilesPaths in ProjectsFilesPaths)
            {
                // project
                ReplaceProjectFileNames(projectFilesPaths);
                RemoveFeatureTagsFromFile(projectFilesPaths.Key.FileName);

                // project items
                foreach (var filePath in projectFilesPaths.Value)
                {
                    RemoveFeatureTagsFromFile(filePath);
                }
            }
        }

        private void ReplaceProjectFileNames(KeyValuePair<Project, IList<string>> projectFilesPaths)
        {
            for (var i = 0; i < projectFilesPaths.Value.Count; i++)
            {
                var file = projectFilesPaths.Value[i];

                if (FeatureFileNamesMappers.Any(item => file.EndsWith(item.Key)))
                {
                    var fileNameMap = FeatureFileNamesMappers.First(item => file.EndsWith(item.Key));

                    var newFile = file.Replace(fileNameMap.Key, fileNameMap.Value);
                    File.Move(file, newFile);

                    projectFilesPaths.Value[i] = newFile;
                }
            }
        }

        private void RemoveFiles(KeyValuePair<Project, IList<string>> projectFilesPaths)
        {
            var filesToRemove = projectFilesPaths.Value
                .Where(item => ExclusiveFeatureFiles.Any(featureFile => item.EndsWith(featureFile))).ToList();

            foreach (var file in filesToRemove)
            {
                File.Delete(file);
                _ = projectFilesPaths.Value.Remove(file);
            }
        }

        /// <summary>
        /// Remove feature content, available tags:
        /// 
        /// Multiline
        /// $feature_name$ start
        /// content ...
        /// $feature_name$ end
        /// 
        /// inline
        /// // $feature_name$ conent ...
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void RemoveFeatureContentFromFile(string filePath)
        {
            bool isInFeatureCode = false;
            var outputFileLines = File.ReadAllLines(filePath).Select(item =>
            {
                // multiline
                string line = null;
                if (item.Contains($"${FeatureName}$ start"))
                {
                    isInFeatureCode = true;
                    line = item;    // mantain tag, should be remove later
                }
                else if (item.Contains($"${FeatureName}$ end"))
                {
                    isInFeatureCode = false;
                }

                if (!isInFeatureCode)
                {
                    // verify feature inline
                    line = item.Contains($"// ${FeatureName}$") ? null : item;
                }

                return line;
            }).OfType<string>();

            File.WriteAllLines(filePath, outputFileLines);
        }

        /// <summary>
        /// Remove feature tags, available tags:
        /// 
        /// Multiline
        /// $feature_name$ start
        /// content ...
        /// $feature_name$ end
        /// 
        /// inline
        /// // $feature_name$ conent ...
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        private void RemoveFeatureTagsFromFile(string filePath)
        {
            var outputFileLines = File.ReadAllLines(filePath).Select(item =>
            {
                string line = null;
                // multiline
                if (item.Contains($"${FeatureName}$ start") || item.Contains($"${FeatureName}$ end"))
                {
                    line = null;
                }
                // inline
                else if (item.Contains($"// ${FeatureName}$"))
                {
                    line = item.Replace($"// ${FeatureName}$ ", string.Empty);
                }
                else
                {
                    line = item;
                }

                return line;
            }).OfType<string>();

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
