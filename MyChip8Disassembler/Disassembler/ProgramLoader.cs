using System.IO;

namespace MyChip8Disassembler.Disassembler
{
    public class ProgramLoader
    {
        public static byte[] Load(string fileName)
        {
            return !File.Exists(fileName) ? new byte[4096] : File.ReadAllBytes(fileName);
        }
    }
}
