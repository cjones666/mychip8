using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemComponents;

namespace MyChip8
{
    public class Chip8System
    {
        private CPU _cpu;

        public Memory Memory { get; private set; }

        public Chip8System()
        {
            Memory = new Memory(4096);
            _cpu = new CPU(Memory);
        }

        public bool LoadProgram(byte[] programData)
        {
            if (Memory == null)
                return false;
            if (programData.Length > Memory.TotalMemory)
                return false;
            for (var i = 0; i < programData.Length; i++)
            {
                Memory.SetByteAtAddress(i + 0x200,programData[i]);
            }
            return true;
        }
    }
}
