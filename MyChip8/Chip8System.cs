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
        public const int MaxMemorySize = 4096;
        public const int StartMemoryAddress = 0x200;

        public Memory Memory { get; private set; }

        private CPU _cpu;

        public Chip8System()
        {
            Memory = new Memory(MaxMemorySize);
            _cpu = new CPU(Memory);
        }

        public bool LoadProgram(byte[] programData)
        {
            if (Memory == null)
                return false;
            if (programData.Length > Memory.TotalMemory)
                return false;
            Memory.Clear();
            for (var i = 0; i < programData.Length; i++)
            {
                Memory.SetByteAtAddress(i + StartMemoryAddress,programData[i]);
            }
            return true;
        }
    }
}
