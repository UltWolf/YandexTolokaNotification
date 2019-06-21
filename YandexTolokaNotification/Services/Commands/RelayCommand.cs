using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace YandexTolokaNotification.Services.Commands
{
    public class RelayCommand : ICommand
    {
        Action<object> execute;
        Func<object, bool> canExecute;
        public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
            {
                return canExecute.Invoke(parameter);
            }
            return true;
        }

        public void Execute(object parameter)
        {
            if (execute != null)
                execute.Invoke(parameter);
        } 
        public RelayCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.canExecute = canExecute;
            this.execute = executeAction;
        }
    }
}
