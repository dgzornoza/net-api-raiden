using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using NetApiRaidenTemplate.Wizard.Dialogs.Views;
using NetApiRaidenTemplate.Wizard.Helpers;
using NetApiRaidenTemplate.Wizard.Models;

namespace NetApiRaidenTemplate.Wizard
{
    public class ConfigureApiWizard : IWizard
    {
        private DTE dte;
        private Dictionary<string, string> replacementsDictionary;

        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
        }

        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        public void RunFinished()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (this.dte == null)
            {
                return;
            }

            // copy readme html in solution folder and show in VS
            if (this.replacementsDictionary.ContainsKey(Configuration.TemplateParams.DestinationDirectoryKey))
            {
                string filePath = Path.Combine(this.replacementsDictionary[Configuration.TemplateParams.DestinationDirectoryKey], "Readme.html");
                byte[] fileBytes = ResourceHelpers.GetEmbeddedResource("Resources.TemplateFiles.Readme.html");
                File.WriteAllBytes(filePath, fileBytes);

                this.dte.ItemOperations.Navigate(filePath, vsNavigateOptions.vsNavigateOptionsNewWindow);
            }

            //// read projects and items
            ////Array activeProjects = (Array)dte.ActiveSolutionProjects;

            ////if (activeProjects.Length > 0)
            ////{
            ////    Project activeProj = (Project)activeProjects.GetValue(0);

            ////    foreach (ProjectItem pi in activeProj.ProjectItems)
            ////    {
            ////        // Do something for the project items like filename checks etc.
            ////    }
            ////}
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                // store DTE and template params dictionary
                if (automationObject is DTE dte)
                {
                    this.dte = dte;
                }

                this.replacementsDictionary = replacementsDictionary;

                SelectContainerDialog selectContainerDialog = new SelectContainerDialog();
                selectContainerDialog.ShowDialog();

                ProjectDialogResult result = selectContainerDialog.Result;
                if (result.Cancelled)
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
                this.DeleteSolutionFolders();
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
                this.replacementsDictionary["$destinationdirectory$"],
                this.replacementsDictionary["$solutiondirectory$"],
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
                }
            }
        }
    }
}
