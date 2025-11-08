using System.Windows;
using System.Windows.Input;
using DisassemblerUI.ViewModels;

namespace DisassemblerUI.Views;

public partial class DisassemblerShellView : Window
{
    public DisassemblerShellView()
    {
        InitializeComponent();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            if (DataContext is DisassemblerShellViewModel viewModel)
            {
                viewModel.OnLoadFileButton();
                e.Handled = true;
            }
        }
    }
}
