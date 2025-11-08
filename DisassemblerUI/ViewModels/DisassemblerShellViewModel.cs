using System;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using DisassemblerUI.Models;
using Microsoft.Win32;
using MyChip8Disassembler.Disassembler;

namespace DisassemblerUI.ViewModels
{
    public class DisassemblerShellViewModel : Screen
    {
        private BindableCollection<InstructionModel> _programInstructions;
        public BindableCollection<InstructionModel> ProgramInstructions
        {
            get { return _programInstructions; }
            set
            {
                _programInstructions = value;
                NotifyOfPropertyChange(() => ProgramInstructions);
            }
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }

        private bool _showHex;
        public bool ShowHex
        {
            get => _showHex;
            set
            {
                _showHex = value;
                NotifyOfPropertyChange(() => ShowHex);
                UpdateParameterDisplay();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                NotifyOfPropertyChange(() => StatusMessage);
            }
        }

        private Disassembler _disassembler;

        public DisassemblerShellViewModel()
        {
            _disassembler = new Disassembler();
            ProgramInstructions = new BindableCollection<InstructionModel>();
            StatusMessage = "Ready. Load a ROM file to begin.";
            _showHex = false;
        }

        public void OnLoadFileButton()
        {
            try
            {
                if (string.IsNullOrEmpty(FileName))
                {
                    // Show the file picker
                    var fileDialog = new OpenFileDialog
                    {
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Filter = "CHIP-8 ROM files (*.ch8;*.bin)|*.ch8;*.bin|All files (*.*)|*.*",
                        Title = "Open CHIP-8 ROM"
                    };

                    if (fileDialog.ShowDialog() != true)
                        return;

                    FileName = fileDialog.FileName;
                }
                else
                {
                    if (!File.Exists(FileName))
                    {
                        MessageBox.Show("File does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        FileName = string.Empty;
                        return;
                    }
                }

                PopulateInstructionModel(FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Error loading file.";
            }
        }

        private void PopulateInstructionModel(string fileName)
        {
            StatusMessage = "Loading ROM...";

            var instructions = _disassembler.GetProgram(fileName);

            if (!string.IsNullOrEmpty(_disassembler.LastError))
            {
                MessageBox.Show(_disassembler.LastError, "Disassembly Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void OnShowHexCheckBox()
        {
            // This method is called by Caliburn when the checkbox changes
            // The ShowHex property setter handles the update
        }
    }
}
