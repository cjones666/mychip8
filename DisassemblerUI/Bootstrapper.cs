using System.Windows;
using Caliburn.Micro;
using DisassemblerUI.ViewModels;

namespace DisassemblerUI;

public class Bootstrapper : BootstrapperBase
{
    public Bootstrapper()
    {
        Initialize();
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        DisplayRootViewForAsync<DisassemblerShellViewModel>();
    }
}
