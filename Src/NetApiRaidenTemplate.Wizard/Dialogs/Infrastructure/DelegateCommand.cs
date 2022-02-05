using System;
using System.Diagnostics;
using System.Windows.Input;

namespace NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure
{
    public sealed class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecuteMethod;
        private readonly Action executeMethod;

        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod ?? throw new ArgumentNullException("executeMethod");
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (this.canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (this.canExecuteMethod != null)
            {
                return this.canExecuteMethod();
            }

            return true;
        }

        public void CanExecuteChanged_RaiseEvent(object sender, EventArgs e)
        {
            if (this.canExecuteMethod != null)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            this.executeMethod?.Invoke();
        }
    }

    public sealed class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> canExecuteMethod;
        private readonly Action<T> executeMethod;

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Predicate<T> canExecuteMethod)
        {
            executeMethod = executeMethod ?? throw new ArgumentNullException("executeMethod");

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (this.canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (this.canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (this.canExecuteMethod != null)
            {
                return this.canExecuteMethod((T)parameter);
            }

            return true;
        }

        public void CanExecuteChanged_RaiseEvent(object sender, EventArgs e)
        {
            if (this.canExecuteMethod != null)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        [DebuggerStepThrough]
        public void Execute(object parameter)
        {
            this.executeMethod((T)parameter);
        }
    }
}
