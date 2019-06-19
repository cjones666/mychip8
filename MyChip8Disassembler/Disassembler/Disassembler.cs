using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using MyChip8;
using MyChip8.Interpreter;

namespace MyChip8Disassembler.Disassembler
{
    public class Disassembler
    {
        private Chip8System _chip8;

        public Disassembler()
        {
            _chip8 = new Chip8System();

            /*
            for (var i = Chip8System.StartMemoryAddress; i < _chip8.Memory.TotalMemory; i += 2)
            {
                var currentByte = _chip8.Memory.ReadByteAtAddress(i);
                var nextByte = _chip8.Memory.ReadByteAtAddress(i + 1);

                var instruction = InstructionHandler.GetInstruction(currentByte,nextByte);
                if (instruction == null)
                    continue;
                Console.WriteLine("0x{0:x3} {1:x2} {2:x2}  {3}", i, currentByte, nextByte, instruction); 
            }
            */
        }

        private bool LoadProgram(string path)
        {
            var fileBytes = ProgramLoader.Load(path);
            return _chip8.LoadProgram(fileBytes);
        }

        public Dictionary<int,IInstruction<ushort>> GetProgram(string path)
        {
            var instructions = new Dictionary<int,IInstruction<ushort>>();

            var success = LoadProgram(path);
            if (!success)
                return instructions;

            for (var i = Chip8System.StartMemoryAddress; i < _chip8.Memory.TotalMemory; i += 2)
            {
                var currentByte = _chip8.Memory.ReadByteAtAddress(i);
                var nextByte = _chip8.Memory.ReadByteAtAddress(i + 1);

                var instruction = InstructionHandler.GetInstruction(currentByte,nextByte);
                if (instruction == null)
                    continue;
                instructions.Add(i,instruction);
                //Console.WriteLine("0x{0:x3} {1:x2} {2:x2}  {3}", i, currentByte, nextByte, instruction); 
            }
            return instructions;
        }
    }
}
