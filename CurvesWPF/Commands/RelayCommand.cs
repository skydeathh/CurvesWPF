using System.Windows.Input;

namespace CurvesWPF.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeWithParameter;
        private readonly Action _executeWithoutParameter;
        private readonly Func<bool> _canExecute;


        public RelayCommand(Action<object> executeWithParameter, Func<bool> canExecute = null)
        {
            _executeWithParameter = executeWithParameter ?? throw new ArgumentNullException(nameof(executeWithParameter));
            _canExecute = canExecute;
        }

        public RelayCommand(Action executeWithoutParameter, Func<bool> canExecute = null)
        {
            _executeWithoutParameter = executeWithoutParameter ?? throw new ArgumentNullException(nameof(executeWithoutParameter));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            if (_executeWithParameter != null)
                _executeWithParameter(parameter);
            else
                _executeWithoutParameter?.Invoke();
        }
    }
}