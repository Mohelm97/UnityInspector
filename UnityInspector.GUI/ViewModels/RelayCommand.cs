using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UnityInspector.GUI.ViewModels
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _execute;

        private readonly Func<object, bool> _canExecute;

        public RelayCommand (Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute (object parameter) => _canExecute == null || _canExecute (parameter);

        public void Execute (object parameter) => _execute (parameter);

        public void RaiseCanExecuteChanged () => CanExecuteChanged?.Invoke (this, EventArgs.Empty);
    }
}
