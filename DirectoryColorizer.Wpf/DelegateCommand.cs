using System.Windows.Input;

namespace DirectoryColorizer.Wpf;

public class DelegateCommand: ICommand
{
    private readonly Func<object?, bool> _canExecute;
    private readonly Action<object?> _execute;

    public DelegateCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (_ => true);
    }

    public bool CanExecute(object? parameter) => _canExecute.Invoke(parameter);

    public void Execute(object? parameter) => _execute.Invoke(parameter);

    public void UpdateCanExecute() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public event EventHandler? CanExecuteChanged;
}