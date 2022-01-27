using System.Collections.Generic;
using NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure;
using NetApiRaidenTemplate.Wizard.Models;

namespace NetApiRaidenTemplate.Wizard.Dialogs.ViewModels
{
    internal class SelectContainerDialogViewModel : ViewModelBase
    {
        private readonly Dictionary<IdentityOption, string> identityOptions;
        private IdentityOption selectedIdentityOption;

        public SelectContainerDialogViewModel()
        {
            identityOptions = CreateIdentityOptions();
        }

        public IDictionary<IdentityOption, string> IdentityOptions => identityOptions;

        public IdentityOption SelectedIdentityOption
        {
            get => selectedIdentityOption;
            set => SetProperty(ref selectedIdentityOption, value);
        }

        protected override void CreateProject()
        {
            base.CreateProject();

            var result = new ProjectDialogResult();
            result.SelectedIdentityOption = SelectedIdentityOption;

            RaiseCreateProject(new ProjectDialogResultEventArgs(result));
        }

        private Dictionary<IdentityOption, string> CreateIdentityOptions()
        {
            return new Dictionary<IdentityOption, string>()
            {
                { IdentityOption.None, "" },
                { IdentityOption.IdentityServer, "IdentityServer4" },
                { IdentityOption.IdentityCore, "Identity Core" },
            };
        }
    }
}
