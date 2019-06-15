namespace MyChip8.SystemComponents
{
    public class Memory
    {
        public int TotalMemory;

        // Memory space for Chip 8
        // 0x000 to 0x1FF - Reserved for interpreter
        // 0x200 to 0xFFF - Program / Data Space

        private static byte[] _memory;
        private readonly int _memorySize;

        public Memory(int memorySize)
        {
            _memorySize = memorySize;
            _memory = new byte[memorySize];
            TotalMemory = memorySize;
        }

        public byte ReadByteAtAddress(int address)
        {
            return _memory[address];
        }

        public void SetByteAtAddress(int address, byte content)
        {
            _memory[address] = content;
        }

        public void Clear()
        {
            _memory = new byte[_memorySize];
        }
    }
}