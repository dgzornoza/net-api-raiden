using System;

namespace NetApiRaidenTemplate.Wizard.Models
{
    public class ProjectDialogResultEventArgs : EventArgs
    {
        public ProjectDialogResultEventArgs(ProjectDialogResult result)
        {
            this.Result = result;
        }

        public ProjectDialogResult Result
        {
            get;
        }
    }
}
