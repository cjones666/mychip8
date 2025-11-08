using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DisassemblerUI.ViewModels;
using DisassemblerUI.Views;

namespace DisassemblerUI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = new DisassemblerShellViewModel();
            var mainWindow = new DisassemblerShellView
            {
                DataContext = viewModel
            };

            // Pass window reference to ViewModel for dialogs
            viewModel.SetWindow(mainWindow);

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
