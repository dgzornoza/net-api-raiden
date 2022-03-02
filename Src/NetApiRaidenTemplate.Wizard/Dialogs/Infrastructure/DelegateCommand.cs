using System;
using System.Diagnostics;

namespace NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure
{
    public sealed class DelegateCommand : DelegateCommandBase<Action, Func<bool>>
    {
        public DelegateCommand(Action executeMethod) : base(executeMethod, null)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
        }

        [DebuggerStepThrough]
        public override bool CanExecute(object parameter) => canExecuteMethod == null || canExecuteMethod();

        [DebuggerStepThrough]
        public override void Execute(object parameter) => executeMethod?.Invoke();
    }

    public sealed class DelegateCommand<T> : DelegateCommandBase<Action<T>, Predicate<T>>
    {
        public DelegateCommand(Action<T> executeMethod) : base(executeMethod, null)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Predicate<T> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
        }

        [DebuggerStepThrough]
        public override bool CanExecute(object parameter) => canExecuteMethod == null || canExecuteMethod((T)parameter);

        [DebuggerStepThrough]
        public override void Execute(object parameter) => executeMethod((T)parameter);
    }
}
