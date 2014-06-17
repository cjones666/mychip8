using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChip8.Interpreter;
using MyChip8.SystemMemory;

namespace MyChip8.Chip8Disassembler
{
    public class Disassembler
    {

        public Disassembler()
        {
            var fileBytes = ProgramLoader.Load("D:\\Users\\Christopher\\Desktop\\Fishie [Hap, 2005].ch8");
            var memory = new Memory(0x200 + fileBytes.Length);
            memory.LoadProgram(fileBytes);

            for (var i = 0x200; i < memory.TotalMemory; i = i + 2)
            {
                var currentByte = memory.ReadByteAtAddress(i);
                var nextByte = memory.ReadByteAtAddress(i + 1);

                var instruction = InstructionHandler.GetInstruction(currentByte,nextByte);
                System.Console.WriteLine(String.Format("0x{0:x3} {1:x2} {2:x2}  {3}", i, currentByte, nextByte, instruction));
               
            }
        }
    }
}
