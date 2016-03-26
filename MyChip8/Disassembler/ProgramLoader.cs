using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChip8.Chip8Disassembler
{
    public class ProgramLoader
    {
        public static byte[] Load(string fileName)
        {
            return !File.Exists(fileName) ? new byte[4096] : File.ReadAllBytes(fileName);
        }
    }
}
