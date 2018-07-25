using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DeveloperTest.Commands
{
    public class CommandHandler : ICommand
    {
        private Action _action;

        public event EventHandler CanExecuteChanged;

        public CommandHandler(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return _action != null;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }
}
