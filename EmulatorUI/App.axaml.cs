using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using EmulatorUI.ViewModels;
using EmulatorUI.Views;

namespace EmulatorUI;

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
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            var viewModel = new MainWindowViewModel();
            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };

            // Load ROM from command-line argument if provided
            if (desktop.Args != null && desktop.Args.Length > 0)
            {
                // Check for --disassemble flag
                bool disassemble = desktop.Args.Length > 1 && desktop.Args[0] == "--disassemble";
                var romPath = disassemble ? desktop.Args[1] : desktop.Args[0];

                if (System.IO.File.Exists(romPath))
                {
                    if (disassemble)
                    {
                        // Disassemble and exit
                        var disasm = new MyChip8Disassembler.Disassembler.Disassembler();
                        var instructions = disasm.GetProgram(romPath);

                        if (disasm.LastError != null)
                        {
                            System.Console.WriteLine($"Error: {disasm.LastError}");
                        }
                        else
                        {
                            System.Console.WriteLine($"Disassembly of {System.IO.Path.GetFileName(romPath)}:");
                            System.Console.WriteLine("=====================================");

                            foreach (var kvp in instructions.OrderBy(x => x.Key))
                            {
                                System.Console.WriteLine($"{kvp.Key:X4}: {kvp.Value}");
                            }

                            System.Console.WriteLine($"\nTotal instructions: {instructions.Count}");
                        }

                        System.Environment.Exit(0);
                        return;
                    }

                    var romData = System.IO.File.ReadAllBytes(romPath);
                    viewModel.Chip8 = new MyChip8.Chip8System();
                    if (viewModel.Chip8.LoadProgram(romData))
                    {
                        viewModel.RomName = System.IO.Path.GetFileName(romPath);
                        viewModel.StatusText = "ROM loaded from command line";
                        // Auto-start
                        viewModel.PlayCommand.Execute(null);
                    }
                }
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}