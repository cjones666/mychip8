using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using DisassemblerUI.Models;
using Microsoft.Win32;
using MyChip8.Interpreter;
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

        public string _fileName;
        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                NotifyOfPropertyChange(() => FileName);
            }
        }

        private Disassembler _disassembler;

        public DisassemblerShellViewModel()
        {
            _disassembler = new Disassembler();
            ProgramInstructions = new BindableCollection<InstructionModel>();
        }

        public void OnLoadFileButton()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                // Show the file picker
                var fileDialog = new OpenFileDialog {CheckFileExists = true, CheckPathExists = true};
                if (fileDialog.ShowDialog() != true) 
                    return;
                // Load the file
                FileName = fileDialog.FileName;
                PopulateInstructionModel(FileName);
            }
            else
            {
                if (File.Exists(FileName))
                {
                    // Do a check here with the disassemble to determine that this is a valid chip 8 file
                    PopulateInstructionModel(FileName);
                }
                else
                {   
                    // Show the error message box and clear the box
                    MessageBox.Show("File does not exit");
                    FileName = string.Empty;
                }
            }
        }

        private void PopulateInstructionModel(string fileName)
        {
            var instructions = _disassembler.GetProgram(fileName);
            ProgramInstructions.Clear();
            foreach (var instruction in instructions)
            {
                var instructionModel = InstructionModel.GetModel(instruction.Key, instruction.Value);
                ProgramInstructions.Add(instructionModel);
            }
        }

        public void OnShowHexCheckBox()
        {
            // Update the instruction data to convert all hexadecimal to decimal or vice versa.
        }
    }
}
