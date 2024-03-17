using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DirectoryColorizer.Tools;
using Microsoft.Win32;

namespace DirectoryColorizer.Wpf;

public class MainWindowVM: INotifyPropertyChanged
{
    private ImageSource? _icon = null;
    private Bitmap? _source = null;
    private Bitmap? _current = null;
    private string _path = "";
    private float _shift;

    public MainWindowVM()
    {
        SetNewCmd = new DelegateCommand(
            _ =>
            {
                if (_current is null) throw new ArgumentNullException(nameof(_current));
                IconTool.SetNewIcon(Path, _current);
                _source = _current;
            }, _ => _current is not null && _source != _current);
        ResetCmd = new DelegateCommand(
            _ =>
            {
                _current = null;
                Icon = _source?.ToBitmapImage();
                Shift = 0;
                SetNewCmd.UpdateCanExecute();
                ResetCmd!.UpdateCanExecute();
            }, _ => _current is not null && _source != _current);
        SetDefaultCmd = new DelegateCommand(
            _ =>
            {
                IconTool.SetOriginalIcon(Path);
                _current = _source = IconTool.GetCurrentIcon(Path);
                Icon = _current?.ToBitmapImage();
                SetNewCmd.UpdateCanExecute();
                ResetCmd.UpdateCanExecute();
                SetDefaultCmd!.UpdateCanExecute();
            }, _ => Directory.Exists(Path));
    }

    public ImageSource? Icon
    {
        get => _icon;
        private set => SetField(ref _icon, value);
    }

    public string Path
    {
        get => _path;
        set
        {
            if(!SetField(ref _path, value)) return;
            _current = _source = IconTool.GetCurrentIcon(value);
            Icon = _current?.ToBitmapImage();
            ResetCmd.UpdateCanExecute();
            SetNewCmd.UpdateCanExecute();
            SetDefaultCmd.UpdateCanExecute();
        }
    }

    public float Shift
    {
        get => _shift;
        set
        {
            if(!SetField(ref _shift, value)) return;
            if (_source is null) return;
            _current = IconTool.ShiftColors(_source, value);
            Icon = _current.ToBitmapImage();
            ResetCmd.UpdateCanExecute();
            SetNewCmd.UpdateCanExecute();
        }
    }

    public DelegateCommand SetNewCmd { get; }
    public DelegateCommand ResetCmd { get; }
    public DelegateCommand SetDefaultCmd { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public partial class MainWindow : Window
{
    private readonly MainWindowVM _dataVM;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = _dataVM = new MainWindowVM();
    }

    private void FolderPathClick(object sender, MouseButtonEventArgs e)
    {
        var dialog = new OpenFolderDialog();
        if (!(dialog.ShowDialog() ?? false)) return;
        var folder = dialog.FolderName;
        _dataVM.Path = folder;
    }
}