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
            if (!File.Exists(fileName))
                return new byte[4096];            
            return File.ReadAllBytes(fileName);
        }
    }
}
