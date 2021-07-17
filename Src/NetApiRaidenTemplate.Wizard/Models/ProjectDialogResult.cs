namespace NetApiRaidenTemplate.Wizard.Models
{
    public class ProjectDialogResult
    {
        public ProjectDialogResult(bool cancelled = true)
        {
            this.Cancelled = cancelled;
        }

        public ProjectDialogResult()
        {
            this.Cancelled = false;
        }

        public bool Cancelled
        {
            get;
            set;
        }
    }
}
