using System;
using System.Windows.Input;

namespace NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure
{
    internal abstract class DelegateCommandBase<TExecute, TCanExecute> : ICommand
        where TExecute : Delegate
        where TCanExecute : Delegate
    {
        protected readonly TCanExecute canExecuteMethod;

        protected readonly TExecute executeMethod;

        protected DelegateCommandBase(TExecute executeMethod) : this(executeMethod, null)
        {
        }

        protected DelegateCommandBase(TExecute executeMethod, TCanExecute canExecuteMethod)
        {
            this.executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
            this.canExecuteMethod = canExecuteMethod;
        }

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void CanExecuteChanged_RaiseEvent(object sender, EventArgs e)
        {
            if (canExecuteMethod != null)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
