using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ytdlpMA.Commands
{
    internal abstract class AsyncCommandBase : ICommand
    {
        private bool _isExecuting;
        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object? parameter)
        {
            return !IsExecuting;
        }

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public async void Execute(object? parameter)
        {
            IsExecuting = true;
            await ExecuteAsync(parameter);
            IsExecuting = false;
        }

        protected abstract Task ExecuteAsync(object? parameter);
    }
}
