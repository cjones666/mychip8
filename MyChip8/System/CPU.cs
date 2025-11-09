using MyChip8.Interpreter;

namespace MyChip8.SystemComponents;

public class CPU
{
    private Timer? _cpuThread;
    private Timer? _timerThread;
    private long _lastTimerUpdate;
    private const int CpuFrequencyHz = 500; // CHIP-8 runs at ~500-700 Hz
    private const int CpuIntervalMs = 2; // ~500 Hz
    private const int TimerFrequencyHz = 60;
    private const int TimerIntervalMs = 1000 / TimerFrequencyHz; // ~16.67ms

    public Memory SystemMemory { get; }
    public Display Display { get; }
    public Input Input { get; }

    public byte[] VRegisters { get; } = new byte[16];

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

    public bool WaitingForKey { get; set; }
    public byte? WaitingKeyRegister { get; set; }

    public CPU(Memory systemMemory, Display display, Input input)
    {
        SystemMemory = systemMemory;
        Display = display;
        Input = input;

        // Load font data into memory
        FontData.LoadIntoMemory(systemMemory);

        // Set PC to program start address
        PC = 0x200;
    }

    public void Start()
    {
        _cpuThread = new Timer(Update, this, 0, CpuIntervalMs);
        _timerThread = new Timer(UpdateTimers, this, 0, TimerIntervalMs);
        _lastTimerUpdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void Stop()
    {
        _cpuThread?.Dispose();
        _timerThread?.Dispose();
    }

    private void UpdateTimers(object? state)
    {
        // Decrement delay timer
        if (DT > 0)
        {
            DT--;
        }

        // Decrement sound timer
        if (ST > 0)
        {
            ST--;
            // TODO: Play sound while ST > 0
        }
    }

    public void Update(object? sender)
    {
        if (PC == 0)
            return;

        // Handle waiting for key press
        if (WaitingForKey && WaitingKeyRegister.HasValue)
        {
            var key = Input.WaitForKeyPress();
            if (key.HasValue)
            {
                VRegisters[WaitingKeyRegister.Value] = key.Value;
                WaitingForKey = false;
                WaitingKeyRegister = null;
            }
            else
            {
                return; // Keep waiting
            }
        }

        // Fetch instruction (2 bytes, big-endian)
        var upperByte = SystemMemory.ReadByteAtAddress(PC);
        var lowerByte = SystemMemory.ReadByteAtAddress(PC + 1);
        var instruction = InstructionHandler.GetInstruction(upperByte, lowerByte);

        if (instruction == null)
        {
            // Unknown instruction, skip it
            PC += 2;
            return;
        }

        // Execute instruction
        instruction.Execute(this);
        instruction.Finalize(this);
    }
}
