using System;
using MyChip8;
using MyChip8.Interpreter;

namespace MyChip8Disassembler.Disassembler
{
    public class Disassembler
    {
        private Chip8System _chip8;

        public Disassembler()
        {
            var fileBytes = ProgramLoader.Load("C:\\Dev\\mychip8\\MyChip8\\GAMES\\PONG");
            _chip8 = new Chip8System();
            _chip8.LoadProgram(fileBytes);

            for (var i = 0x0200; i < _chip8.Memory.TotalMemory; i += 2)
            {
                var currentByte = _chip8.Memory.ReadByteAtAddress(i);
                var nextByte = _chip8.Memory.ReadByteAtAddress(i + 1);

                var instruction = InstructionHandler.GetInstruction(currentByte,nextByte);
                if (instruction == null)
                    continue;
                Console.WriteLine("0x{0:x3} {1:x2} {2:x2}  {3}", i, currentByte, nextByte, instruction); 
            }
        }
    }
}
