using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DisassemblerUI.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MyChip8Disassembler.Disassembler;

namespace DisassemblerUI.ViewModels;

public class DisassemblerShellViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<InstructionModel> _programInstructions = [];
    public ObservableCollection<InstructionModel> ProgramInstructions
    {
        get => _programInstructions;
        set
        {
            if (_programInstructions != value)
            {
                _programInstructions = value;
                OnPropertyChanged();
            }
        }
    }

    private string _fileName = string.Empty;
    public string FileName
    {
        get => _fileName;
        set
        {
            if (_fileName != value)
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _showHex;
    public bool ShowHex
    {
        get => _showHex;
        set
        {
            if (_showHex != value)
            {
                _showHex = value;
                OnPropertyChanged();
                UpdateParameterDisplay();
            }
        }
    }

    private string _statusMessage = "Ready. Load a ROM file to begin.";
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    private readonly Disassembler _disassembler = new();
    private Window? _window;

    public DisassemblerShellViewModel()
    {
    }

    public void SetWindow(Window window)
    {
        _window = window;
    }

    public async void OnLoadFileButton()
    {
        try
        {
            if (string.IsNullOrEmpty(FileName))
            {
                // Show the file picker
                if (_window == null)
                    return;

                var files = await _window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open CHIP-8 ROM",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("CHIP-8 ROM files")
                        {
                            Patterns = new[] { "*.ch8", "*.bin" }
                        },
                        FilePickerFileTypes.All
                    }
                });

                if (files.Count == 0)
                    return;

                FileName = files[0].Path.LocalPath;
            }
            else
            {
                if (!File.Exists(FileName))
                {
                    await ShowError("File does not exist.", "Error");
                    FileName = string.Empty;
                    return;
                }
            }

            PopulateInstructionModel(FileName);
        }
        catch (Exception ex)
        {
            await ShowError($"Failed to load file: {ex.Message}", "Error");
            StatusMessage = "Error loading file.";
        }
    }

    private async void PopulateInstructionModel(string fileName)
    {
        StatusMessage = "Loading ROM...";

        var instructions = _disassembler.GetProgram(fileName);

        if (!string.IsNullOrEmpty(_disassembler.LastError))
        {
            await ShowError(_disassembler.LastError, "Disassembly Error");
            StatusMessage = "Error disassembling ROM.";
            return;
        }

        ProgramInstructions.Clear();
        foreach (var instruction in instructions)
        {
            var instructionModel = InstructionModel.GetModel(instruction.Key, instruction.Value);
            ProgramInstructions.Add(instructionModel);
        }

        StatusMessage = $"Loaded {instructions.Count} instructions from {Path.GetFileName(fileName)}";
    }

    private void UpdateParameterDisplay()
    {
        foreach (var instruction in ProgramInstructions)
        {
            instruction.UpdateParameterDisplay(_showHex);
        }
    }

    private async Task ShowError(string message, string title)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, Icon.Error);
        await box.ShowAsync();
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
