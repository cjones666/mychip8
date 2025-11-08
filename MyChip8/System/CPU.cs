using MyChip8.Interpreter;

namespace MyChip8.SystemComponents;

public class CPU
{
    private Timer? _cpuThread;

    public Memory SystemMemory { get; }

    public byte[] VRegisters { get; } = new byte[0xF];

    public ushort I { get; set; }

    public List<ushort> Stack { get; } = [];

    private byte _stackPointer;
    public byte SP
    {
        get => _stackPointer;
        set => _stackPointer = value switch
        {
            >= 16 => 15,
            _ => value
        };
    }

    public ushort PC { get; set; }

    public byte DT { get; set; }
    public byte ST { get; set; }

    public CPU(Memory systemMemory)
    {
        SystemMemory = systemMemory;
    }

    public void Start()
    {
        _cpuThread = new Timer(Update, this, 0, 1000);
    }

    public void Update(object? sender)
    {
        if (PC == 0)
            return;

        // Fetch instruction
        var instructionBytes = SystemMemory.ReadByteAtAddress(PC);
        var instruction = InstructionHandler.GetInstruction(instructionBytes);

        // Do instruction
        instruction.Execute(this);
        instruction.Finalize(this);
        // Update timers / registers / etc.
    }
}
