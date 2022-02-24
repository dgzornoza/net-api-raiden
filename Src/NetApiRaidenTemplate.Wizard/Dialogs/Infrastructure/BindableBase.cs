using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetApiRaidenTemplate.Wizard.Dialogs.Infrastructure
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IEnumerable<PropertyInfo> canProperties;

        public BindableBase()
        {
            // obtener cancommands para reevaluarse en notificaciones (por convencion seran usados CanXXXCmd)
            canProperties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(item => item.Name.StartsWith("Can") && item.Name.EndsWith("Cmd"));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));

            PropertyChanged?.Invoke(this, args);
        }

        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            RaisePropertyChanged((property.Body as MemberExpression).Member.Name);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChangedEventArgs = new PropertyChangedEventArgs(propertyName);

            OnPropertyChanged(propertyChangedEventArgs);

            // notificar cancommands en todas las propiedades
            for (int i = 0; i < canProperties.Count(); i++)
            {
                OnPropertyChanged(propertyChangedEventArgs);
            }
        }
    }
}
