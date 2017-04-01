using System;
using System.Windows.Input;

namespace SieveofEratosthenes.Commands
{
    public class RelayCommand : ICommand
    {
        readonly Action execute;
        private bool canExecute;

        public RelayCommand(Action execute)
        {
            this.execute = execute;
            this.canExecute = true;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute;
        }

        public void SetCanExecute(bool canExecute)
        {
            this.canExecute = canExecute;
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute();
        }
    }
}
