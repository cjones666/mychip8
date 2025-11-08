namespace MyChip8Disassembler.Disassembler;

public static class ProgramLoader
{
    public static byte[] Load(string fileName) =>
        !File.Exists(fileName) ? new byte[4096] : File.ReadAllBytes(fileName);
}
