using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ninlabs.attachables.Util
{

    public class RelayCommand : ICommand
    {
        Action m_execute;
        Func<bool> m_canExecute;
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            m_execute = execute;
            m_canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return m_canExecute();
        }

        public void Execute(object parameter)
        {
            m_execute();
        }

        public event EventHandler CanExecuteChanged;
    }

    public class RelayCommand<T> : ICommand where T : class
    {
        Action<T> m_execute;
        Func<bool> m_canExecute;
        public RelayCommand(Action<T> execute, Func<bool> canExecute)
        {
            m_execute = execute;
            m_canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return m_canExecute();
        }

        public void Execute(object parameter)
        {
            m_execute(parameter as T);
        }

        public event EventHandler CanExecuteChanged;
    }
}
