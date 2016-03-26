using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChip8.Interpreter;
using MyChip8.SystemComponents;

namespace MyChip8.Chip8Disassembler
{
    public class Disassembler
    {
        private Chip8System _chip8;

        public Disassembler()
        {
            var fileBytes = ProgramLoader.Load("D:\\Dev\\Chip8\\CHIP8\\GAMES\\PONG");
            //var memory = new Memory(0x200 + fileBytes.Length);
            _chip8.LoadProgram(fileBytes);

            for (var i = 0x0200; i < _chip8.Memory.TotalMemory; i = i + 2)
            {
                var currentByte = _chip8.Memory.ReadByteAtAddress(i);
                var nextByte = _chip8.Memory.ReadByteAtAddress(i + 1);

                var instruction = InstructionHandler.GetInstruction(currentByte,nextByte);
                Console.WriteLine("0x{0:x3} {1:x2} {2:x2}  {3}", i, currentByte, nextByte, instruction); 
            }
        }
    }
}
