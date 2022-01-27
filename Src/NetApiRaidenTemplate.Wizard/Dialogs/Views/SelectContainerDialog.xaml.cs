using System;
using System.ComponentModel;
using System.Windows;
using NetApiRaidenTemplate.Wizard.Dialogs.ViewModels;
using NetApiRaidenTemplate.Wizard.Models;

namespace NetApiRaidenTemplate.Wizard.Dialogs.Views
{
    public partial class SelectContainerDialog : Window
    {
        public SelectContainerDialog()
        {
            this.InitializeComponent();

            SelectContainerDialogViewModel selectContainerDialogViewModel = new SelectContainerDialogViewModel();
            selectContainerDialogViewModel.ProjectCreated = (EventHandler<ProjectDialogResultEventArgs>)Delegate.Combine(selectContainerDialogViewModel.ProjectCreated, new EventHandler<ProjectDialogResultEventArgs>(this.ProjectCreated));

            this.DataContext = selectContainerDialogViewModel;
            this.Closing += this.SelectContainerDialog_Closing;
        }

        public ProjectDialogResult Result
        {
            get;
            set;
        }

        private void ProjectCreated(object sender, ProjectDialogResultEventArgs e)
        {
            this.Result = e.Result;
            this.DialogResult = true;
        }

        private void SelectContainerDialog_Closing(object sender, CancelEventArgs e)
        {
            if (this.Result == null)
            {
                this.Result = new ProjectDialogResult();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Result = new ProjectDialogResult(true);
            this.DialogResult = false;
        }
    }
}
