using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using MyChip8;

namespace EmulatorUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private string _romName = "No ROM loaded";

    public Chip8System? Chip8 { get; set; }

    public Action? RequestRenderUpdate { get; set; }
    public Action? RequestOpenRom { get; set; }

    [RelayCommand]
    private void Play()
    {
        if (Chip8 == null)
        {
            StatusText = "No ROM loaded";
            return;
        }

        Chip8.Start();
        IsRunning = true;
        StatusText = "Running";
    }

    [RelayCommand]
    private void Pause()
    {
        if (Chip8 == null)
            return;

        Chip8.Stop();
        IsRunning = false;
        StatusText = "Paused";
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (IsRunning)
            Pause();
        else
            Play();
    }

    [RelayCommand]
    private void Reset()
    {
        if (Chip8 == null)
            return;

        bool wasRunning = IsRunning;
        if (wasRunning)
            Chip8.Stop();

        Chip8.Reset();

        if (wasRunning)
            Chip8.Start();

        StatusText = "Reset";
    }

    [RelayCommand]
    private void OpenRom()
    {
        RequestOpenRom?.Invoke();
    }

    [RelayCommand]
    private void Exit()
    {
        if (Chip8 != null && IsRunning)
            Chip8.Stop();

        Environment.Exit(0);
    }

    public void HandleKeyDown(byte chipKey)
    {
        Chip8?.Input.SetKeyState(chipKey, true);
    }

    public void HandleKeyUp(byte chipKey)
    {
        Chip8?.Input.SetKeyState(chipKey, false);
    }
}
