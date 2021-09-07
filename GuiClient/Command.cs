using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiClient
{
    internal class Command : ICommand
    {
        private bool _isExecuting;
        private bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                if (value == _isExecuting)
                {
                    return;
                }

                _isExecuting = value;
                NotifyCanExecuteChanged();
            }
        }

        private readonly Func<object, Task> _execute;
        private readonly Func<object, bool> _canExecute;

        public Command(Func<object, Task> execute) 
            : this(execute, _ => true)
        {
        }

        public Command(
            Func<object, Task> execute,
            Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                IsExecuting = true;
                await _execute(parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        public bool CanExecute(object parameter) => !IsExecuting && _canExecute(parameter);

        public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public event EventHandler CanExecuteChanged;
    }
}