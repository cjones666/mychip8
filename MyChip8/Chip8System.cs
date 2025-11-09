using MyChip8.SystemComponents;

namespace MyChip8;

public class Chip8System
{
    public const int MaxMemorySize = 4096;
    public const int StartMemoryAddress = 0x200;

    public Memory Memory { get; }
    public Display Display { get; }
    public Input Input { get; }
    public CPU CPU { get; }

    public Chip8System()
    {
        Memory = new Memory(MaxMemorySize);
        Display = new Display();
        Input = new Input();
        CPU = new CPU(Memory, Display, Input);
    }

    public bool LoadProgram(byte[] programData)
    {
        if (programData.Length > Memory.TotalMemory - StartMemoryAddress)
            return false;

        // Don't clear memory completely - font data is already loaded at 0x000
        // Just clear program space
        for (var i = StartMemoryAddress; i < MaxMemorySize; i++)
        {
            Memory.SetByteAtAddress(i, 0);
        }

        // Load program into memory starting at 0x200
        for (var i = 0; i < programData.Length; i++)
        {
            Memory.SetByteAtAddress(i + StartMemoryAddress, programData[i]);
        }

        return true;
    }

    public void Start()
    {
        CPU.Start();
    }

    public void Stop()
    {
        CPU.Stop();
    }

    public void Reset()
    {
        // Clear display
        Display.Clear();

        // Clear input
        Input.Clear();

        // Reset CPU state
        CPU.PC = StartMemoryAddress;
        CPU.I = 0;
        CPU.SP = 0;
        CPU.DT = 0;
        CPU.ST = 0;
        CPU.Stack.Clear();
        for (int i = 0; i < 16; i++)
        {
            CPU.VRegisters[i] = 0;
        }

        // Reload font data
        FontData.LoadIntoMemory(Memory);
    }
}
