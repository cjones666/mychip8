using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using DisassemblerUI.ViewModels;

namespace DisassemblerUI.Views;

public partial class DisassemblerShellView : Window
{
    public DisassemblerShellView()
    {
        InitializeComponent();

        // Handle keyboard shortcuts
        KeyDown += Window_KeyDown;
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.O && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            if (DataContext is DisassemblerShellViewModel viewModel)
            {
                viewModel.OnLoadFileButton();
                e.Handled = true;
            }
        }
    }

    private void OnLoadFileButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DisassemblerShellViewModel viewModel)
        {
            viewModel.OnLoadFileButton();
        }
    }
}
