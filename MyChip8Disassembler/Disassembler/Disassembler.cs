using System;
using System.Collections.Generic;
using System.IO;
using MyChip8;
using MyChip8.Interpreter;

namespace MyChip8Disassembler.Disassembler
{
    public class Disassembler
    {
        private Chip8System _chip8;
        public string LastError { get; private set; }

        public Disassembler()
        {
            _chip8 = new Chip8System();
        }

        private bool LoadProgram(string path)
        {
            try
            {
                var fileBytes = ProgramLoader.Load(path);
                if (fileBytes.Length == 0)
                {
                    LastError = "ROM file is empty.";
                    return false;
                }

                if (fileBytes.Length > 4096 - Chip8System.StartMemoryAddress)
                {
                    LastError = $"ROM file is too large ({fileBytes.Length} bytes). Maximum is {4096 - Chip8System.StartMemoryAddress} bytes.";
                    return false;
                }

                return _chip8.LoadProgram(fileBytes);
            }
            catch (FileNotFoundException)
            {
                LastError = "ROM file not found.";
                return false;
            }
            catch (Exception ex)
            {
                LastError = $"Error loading ROM: {ex.Message}";
                return false;
            }
        }

        public Dictionary<int, IInstruction<ushort>> GetProgram(string path)
        {
            var instructions = new Dictionary<int, IInstruction<ushort>>();
            LastError = null;

            if (string.IsNullOrWhiteSpace(path))
            {
                LastError = "No file path provided.";
                return instructions;
            }

            var success = LoadProgram(path);
            if (!success)
                return instructions;

            try
            {
                for (var i = Chip8System.StartMemoryAddress; i < _chip8.Memory.TotalMemory; i += 2)
                {
                    var currentByte = _chip8.Memory.ReadByteAtAddress(i);
                    var nextByte = _chip8.Memory.ReadByteAtAddress(i + 1);

                    var instruction = InstructionHandler.GetInstruction(currentByte, nextByte);
                    if (instruction == null)
                        continue;
                    instructions.Add(i, instruction);
                }
            }
            catch (Exception ex)
            {
                LastError = $"Error parsing instructions: {ex.Message}";
                return new Dictionary<int, IInstruction<ushort>>();
            }

            return instructions;
        }
    }
}
