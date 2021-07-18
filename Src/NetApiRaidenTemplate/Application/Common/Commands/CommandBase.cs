using System;

namespace $safeprojectname$.Common.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            this.Id = Guid.NewGuid();
        }

        protected CommandBase(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; init; }
    }

    public abstract class CommandBase<TResult> : ICommand<TResult>
    {
        protected CommandBase()
        {
            this.Id = Guid.NewGuid();
        }

        protected CommandBase(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; init; }
    }
}
