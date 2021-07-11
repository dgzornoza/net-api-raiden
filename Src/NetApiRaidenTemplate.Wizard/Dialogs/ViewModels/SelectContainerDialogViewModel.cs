using NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure;
using NetApiRaidenTemplate.Wizard.Models;

namespace NetApiRaidenTemplate.Wizard.Dialogs.ViewModels
{
    internal class SelectContainerDialogViewModel : ViewModelBase
    {
        protected override void CreateProject()
        {
            base.CreateProject();

            ProjectDialogResult result = new ProjectDialogResult();
            this.RaiseCreateProject(new ProjectDialogResultEventArgs(result));
        }
    }
}
