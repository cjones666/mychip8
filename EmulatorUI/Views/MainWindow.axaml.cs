using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using EmulatorUI.ViewModels;
using MyChip8;
using MyChip8.SystemComponents;

namespace EmulatorUI.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _viewModel;
    private Timer? _renderTimer;
    private Canvas? _displayCanvas;

    // CHIP-8 keyboard mapping (modern keyboard -> CHIP-8 hex key)
    // Original layout:     Modern keyboard mapping:
    // 1 2 3 C              1 2 3 4
    // 4 5 6 D              Q W E R
    // 7 8 9 E              A S D F
    // A 0 B F              Z X C V
    private readonly Dictionary<Key, byte> _keyMap = new()
    {
        { Key.D1, 0x1 }, { Key.D2, 0x2 }, { Key.D3, 0x3 }, { Key.D4, 0xC },
        { Key.Q, 0x4 },  { Key.W, 0x5 },  { Key.E, 0x6 },  { Key.R, 0xD },
        { Key.A, 0x7 },  { Key.S, 0x8 },  { Key.D, 0x9 },  { Key.F, 0xE },
        { Key.Z, 0xA },  { Key.X, 0x0 },  { Key.C, 0xB },  { Key.V, 0xF }
    };

    public MainWindow()
    {
        InitializeComponent();

        _displayCanvas = this.FindControl<Canvas>("DisplayCanvas");

        this.DataContextChanged += (s, e) =>
        {
            _viewModel = DataContext as MainWindowViewModel;
            if (_viewModel != null)
            {
                _viewModel.RequestRenderUpdate = () => Dispatcher.UIThread.Post(RenderDisplay);
                _viewModel.RequestOpenRom = () => Dispatcher.UIThread.InvokeAsync(OnOpenRom);

                // Start render timer (60 FPS)
                _renderTimer = new Timer(_ => _viewModel.RequestRenderUpdate?.Invoke(), null, 0, 1000 / 60);
            }
        };
    }

    private async void OnOpenRom()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open CHIP-8 ROM",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("CHIP-8 ROM Files") { Patterns = new[] { "*.ch8", "*.c8" } },
                new FilePickerFileType("All Files") { Patterns = new[] { "*" } }
            }
        });

        if (files.Count > 0)
        {
            var file = files[0];
            await using var stream = await file.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var romData = ms.ToArray();

            if (_viewModel != null)
            {
                // Stop current emulation if running
                if (_viewModel.IsRunning)
                    _viewModel.Chip8?.Stop();

                // Create new CHIP-8 system
                _viewModel.Chip8 = new Chip8System();

                // Load ROM
                if (_viewModel.Chip8.LoadProgram(romData))
                {
                    _viewModel.RomName = Path.GetFileName(file.Name);
                    _viewModel.StatusText = "ROM loaded successfully";

                    // Auto-start
                    _viewModel.PlayCommand.Execute(null);
                }
                else
                {
                    _viewModel.StatusText = "Failed to load ROM";
                }
            }
        }
    }

    private void RenderDisplay()
    {
        if (_displayCanvas == null || _viewModel?.Chip8 == null)
            return;

        // Only redraw if we haven't already rendered this exact display state
        var displayBuffer = _viewModel.Chip8.Display.GetBuffer();

        _displayCanvas.Children.Clear();

        const int pixelWidth = 10;  // Each CHIP-8 pixel = 10x10 screen pixels
        const int pixelHeight = 10;

        var whiteBrush = Brushes.White;

        for (int y = 0; y < Display.Height; y++)
        {
            for (int x = 0; x < Display.Width; x++)
            {
                if (displayBuffer[x, y])
                {
                    var rect = new Avalonia.Controls.Shapes.Rectangle
                    {
                        Width = pixelWidth,
                        Height = pixelHeight,
                        Fill = whiteBrush
                    };
                    Canvas.SetLeft(rect, x * pixelWidth);
                    Canvas.SetTop(rect, y * pixelHeight);
                    _displayCanvas.Children.Add(rect);
                }
            }
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_keyMap.TryGetValue(e.Key, out byte chipKey))
        {
            _viewModel?.HandleKeyDown(chipKey);
            e.Handled = true;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (_keyMap.TryGetValue(e.Key, out byte chipKey))
        {
            _viewModel?.HandleKeyUp(chipKey);
            e.Handled = true;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _renderTimer?.Dispose();
        _viewModel?.Chip8?.Stop();
        base.OnClosed(e);
    }
}