using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChip8.SystemMemory
{
    public class Memory
    {
        public int TotalMemory;

        // Memory space for Chip 8
        // 0x000 to 0x1FF - Reserved for interpreter
        // 0x200 to 0xFFF - Program / Data Space

        private static byte[] _memory;


        public Memory(int memorySize)
        {
            _memory = new byte[memorySize];
            TotalMemory = memorySize;
        }

        public bool LoadProgram(byte[] programData)
        {
            if (programData.Length > TotalMemory)
                return false;
            for (var i = 0; i < programData.Length; i++)
            {
                _memory[i + 0x200] = programData[i];
            }
            return true;
        }

        public byte ReadByteAtAddress(int address)
        {
            return _memory[address];
        }

    }
}