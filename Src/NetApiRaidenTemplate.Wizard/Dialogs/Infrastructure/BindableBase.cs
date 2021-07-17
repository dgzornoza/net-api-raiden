using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            args = args ?? throw new ArgumentNullException("args");

            this.PropertyChanged?.Invoke(this, args);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
