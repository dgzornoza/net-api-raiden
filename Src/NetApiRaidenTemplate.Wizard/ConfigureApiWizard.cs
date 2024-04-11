using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using NetApiRaidenTemplate.Wizard.Dialogs.Views;
using NetApiRaidenTemplate.Wizard.FeaturesManagers;
using NetApiRaidenTemplate.Wizard.Helpers;
using NetApiRaidenTemplate.Wizard.Models;

namespace NetApiRaidenTemplate.Wizard
{
    public class ConfigureApiWizard : IWizard
    {
        private DTE dte;
        private IDictionary<string, string> replacementsDictionary;
        private ProjectDialogResult projectDialogResult;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
            // Actualmente no necesario
        }

        public void ProjectFinishedGenerating(Project project)
        {
            // Actualmente no necesario
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            // Actualmente no necesario
        }

        public void RunFinished()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (dte == null)
            {
                return;
            }

            // copy resource template files
            if (replacementsDictionary.ContainsKey(Configuration.TemplateParams.DestinationDirectoryKey))
            {
                var filePath = Path.Combine(replacementsDictionary[Configuration.TemplateParams.DestinationDirectoryKey], ".editorconfig");
                var fileBytes = ResourceHelpers.GetEmbeddedResource("Resources.TemplateFiles.editorconfig");
                File.WriteAllBytes(filePath, fileBytes);

                // copy readme html in solution folder and show in VS
                filePath = Path.Combine(replacementsDictionary[Configuration.TemplateParams.DestinationDirectoryKey], "Readme.html");
                fileBytes = ResourceHelpers.GetEmbeddedResource("Resources.TemplateFiles.Readme.html");
                File.WriteAllBytes(filePath, fileBytes);

                ////_ = dte.ItemOperations.Navigate(filePath, vsNavigateOptions.vsNavigateOptionsNewWindow);
            }

            // execute features managers           
            ExecuteFeaturesManagers();
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                // store DTE and template params dictionary
                if (automationObject is DTE automationObjectAsDte)
                {
                    this.dte = automationObjectAsDte;
                }

                this.replacementsDictionary = replacementsDictionary;

                var selectContainerDialog = new SelectContainerDialog();
                _ = selectContainerDialog.ShowDialog();

                projectDialogResult = selectContainerDialog.Result;
                if (projectDialogResult.Cancelled)
                {
                    throw new WizardBackoutException();
                }

                if (replacementsDictionary.ContainsKey("$targetframeworkversion$"))
                {
                    replacementsDictionary.Add("passthrough:TargetFrameworkVersion", replacementsDictionary["$targetframeworkversion$"] ?? string.Empty);
                }
            }
            catch
            {
                DeleteSolutionFolders();
                throw;
            }
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private void DeleteSolutionFolders()
        {
            // delete solution folders
            IEnumerable<string> folders = new string[]
            {
                replacementsDictionary["$destinationdirectory$"],
                replacementsDictionary["$solutiondirectory$"],
            };

            foreach (var folder in folders)
            {
                try
                {
                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder);
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine($"Error deleting folder {folder}");
                }
            }
        }

        private void ExecuteFeaturesManagers()
        {
            var includedFeatures = GetIncludedFeaturesFromDialogResult();

            var types = typeof(IFeatureManager).Assembly.GetTypes()
                .Where(item => typeof(IFeatureManager).IsAssignableFrom(item) && item.IsClass && !item.IsAbstract);

            foreach (var type in types)
            {
                var FeatureManager = Activator.CreateInstance(type, dte.Solution) as IFeatureManager;
                if (includedFeatures.Contains(type))
                {
                    FeatureManager.Include();
                }
                else
                {
                    FeatureManager.Exclude();
                }
            }
        }

        private IEnumerable<Type> GetIncludedFeaturesFromDialogResult()
        {
            var featureTypes = new List<Type>();
            if (projectDialogResult.SelectedIdentityOption == IdentityOption.IdentityServer)
            {
                featureTypes.Add(typeof(IdentityFeatureManager));
            }

            // Add more features ...

            return featureTypes;
        }
    }
}
