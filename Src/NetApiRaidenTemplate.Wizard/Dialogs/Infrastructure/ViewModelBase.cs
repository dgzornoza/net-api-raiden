using System;
using System.Windows.Input;
using NetApiRaidenTemplate.Wizard.Models;

namespace NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure
{
    internal class ViewModelBase : ViewModelBase<ProjectDialogResultEventArgs>
    {
        private ICommand createProjectCommand;

        public ViewModelBase()
        {
        }

        public EventHandler<ProjectDialogResultEventArgs> ProjectCreated { get; set; }

        public ICommand CreateProjectCommand => this.createProjectCommand ?? (this.createProjectCommand = new DelegateCommand(this.CreateProject, this.CanCreateProject));

        protected virtual void CreateProject()
        {
        }

        protected virtual bool CanCreateProject()
        {
            return true;
        }

        protected void RaiseCreateProject(ProjectDialogResultEventArgs e)
        {
            this.OnProjectCreated(e);
        }

        protected virtual void OnProjectCreated(ProjectDialogResultEventArgs e)
        {
            this.ProjectCreated?.Invoke(this, e);
        }
    }

    internal class ViewModelBase<T> : BindableBase where T : ProjectDialogResultEventArgs
    {
        protected bool isBusy;

        public ViewModelBase()
        {
        }

        public EventHandler<T> DialogResult;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                if (value == isBusy)
                {
                    return;
                }

                isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }
    }
}
