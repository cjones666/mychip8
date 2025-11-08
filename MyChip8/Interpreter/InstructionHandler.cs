namespace MyChip8.Interpreter;

public static class InstructionHandler
{
    public static IInstruction<ushort> GetInstruction(byte upperByte, byte lowerByte)
    {
        var instructionBytes = (ushort)(upperByte << 8 | lowerByte);
        return GetInstruction(instructionBytes);
    }

    // The instruction ops in CHIP-8 are 2 bytes, thus we are passing in the op as a single 16-bit short integer.
    public static IInstruction<ushort>? GetInstruction(ushort instructionBytes)
    {
        var upperNib = (instructionBytes >> 12) & 0x000F;
        var register = (byte)((instructionBytes >> 8) & 0x000F);
        var lowerByte = (byte)(instructionBytes & 0x00FF);
        var addr = (ushort)(instructionBytes & 0x0FFF);
        var register1 = (ushort)((instructionBytes >> 8) & 0x000F);
        var register2 = (ushort)((instructionBytes >> 4) & 0x000F);

        return upperNib switch
        {
            0x0 => lowerByte switch
            {
                0xE0 => new Instructions.CLSInstruction(instructionBytes, 0, 0),
                0xEE => new Instructions.RETInstruction(instructionBytes, 0, 0),
                _ => null
            },
            0x1 => new Instructions.JPInstruction(instructionBytes, addr, 0),
            0x2 => new Instructions.CALLInstruction(instructionBytes, addr, 0),
            0x3 => new Instructions.SEInstruction(instructionBytes, register, lowerByte, true),
            0x4 => new Instructions.SNEInstruction(instructionBytes, register, lowerByte),
            0x5 => new Instructions.SEInstruction(instructionBytes, register1, register2, true),
            0x6 => new Instructions.LDInstruction(instructionBytes, register, lowerByte, true),
            0x7 => new Instructions.ADDInstruction(instructionBytes, register, lowerByte, true),
            0x8 => (instructionBytes & 0x000F) switch
            {
                0x0 => new Instructions.LDInstruction(instructionBytes, register1, register2, true),
                0x1 => new Instructions.ORInstruction(instructionBytes, register1, register2, true),
                0x2 => new Instructions.ANDInstruction(instructionBytes, register1, register2, true),
                0x3 => new Instructions.XORInstruction(instructionBytes, register1, register2, true),
                0x4 => new Instructions.ADDInstruction(instructionBytes, register1, register2, true),
                0x5 => new Instructions.SUBInstruction(instructionBytes, register1, register2, true),
                0x6 => new Instructions.SHRInstruction(instructionBytes, register1, register2, true),
                0x7 => new Instructions.SUBNInstruction(instructionBytes, register1, register2, true),
                0xE => new Instructions.SHLInstruction(instructionBytes, register1, register2, true),
                _ => null
            },
            0x9 => new Instructions.SNEInstruction(instructionBytes, register1, register2),
            0xA => new Instructions.LDInstruction(instructionBytes, 0, addr, true),
            0xB => new Instructions.JPInstruction(instructionBytes, addr, 0),
            0xC => new Instructions.RNDInstruction(instructionBytes, register, lowerByte, true),
            0xD => new Instructions.DRWInstruction(instructionBytes, register1, register2, (byte)(instructionBytes & 0x000F), true),
            0xE => lowerByte switch
            {
                0x9E => new Instructions.SKPInstruction(instructionBytes, register, 0, true),
                0xA1 => new Instructions.SKNPInstruction(instructionBytes, register, 0, true),
                _ => null
            },
            0xF => lowerByte switch
            {
                0x07 or 0x0A or 0x15 or 0x18 or 0x29 or 0x33 or 0x55 or 0x65 =>
                    new Instructions.LDInstruction(instructionBytes, register, 0, true),
                0x1E => new Instructions.ADDInstruction(instructionBytes, register, 0, true),
                _ => null
            },
            _ => null
        };
    }
}
