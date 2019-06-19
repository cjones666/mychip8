using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyChip8.Interpreter
{
    public class InstructionHandler
    {
        private Dictionary<InstructionType, IInstruction<ushort>> _opMap;

        public InstructionHandler()
        {
            _opMap = new Dictionary<InstructionType, IInstruction<ushort>>();
            var opTypes = Enum.GetValues(typeof (InstructionType.Op));
            foreach (var instructionType in opTypes)
            {
                var type = Type.GetType(instructionType + "Instruction");
                if (type != null)
                {
                    var instructionInstance = Activator.CreateInstance(type) as IInstruction<ushort>;
                    _opMap.Add((InstructionType)instructionType,instructionInstance);
                }
            }
        }

        public static IInstruction<ushort> GetInstruction(byte upperByte, byte lowerByte)
        {
            var instructionBytes = (ushort)((upperByte) << 8 | (lowerByte));
            return GetInstruction(instructionBytes);
        }

        // The instruction ops in CHIP-8 are 2 bytes, thus we are passing in the op as a single 16-bit short integer.
        public static IInstruction<ushort> GetInstruction(ushort instructionBytes)
        {
            var upperNib = (instructionBytes >> 12) & 0x000F;

            ushort addr;
            byte lowerByte;
            byte register;
            switch (upperNib)
            {
                case 0x0:
                {
                    // Get the lower byte
                    lowerByte = (byte)(instructionBytes & 0x00FF);
                    if (lowerByte == 0xE0)
                        return new Instructions.CLSInstruction(instructionBytes,0,0);
                    if (lowerByte == 0xEE)
                        return new Instructions.RETInstruction(instructionBytes, 0, 0);
                    return null;
                }
                case 0x1:
                    addr = (ushort)(instructionBytes & 0x0FFF);
                    return new Instructions.JPInstruction(instructionBytes,addr,0);
                case 0x2:
                    addr = (ushort) (instructionBytes & 0x0FFF);
                    return new Instructions.CALLInstruction(instructionBytes,addr,0);
                case 0x3:
                    register = (byte) ((instructionBytes >> 8) & 0x000F);
                    lowerByte = (byte) (instructionBytes & 0x00Ff);
                    return new Instructions.SEInstruction(instructionBytes,register,lowerByte);
                case 0x4:
                    register = (byte) ((instructionBytes >> 8) & 0x000F);
                    lowerByte = (byte) (instructionBytes & 0x00FF);
                    return new Instructions.SNEInstruction(instructionBytes,register,lowerByte);
                case 0x5:
                    var register1 = (ushort) ((instructionBytes >> 8) & 0x000F);
                    var register2 = (ushort) ((instructionBytes >> 4) & 0x000F);
                    return new Instructions.SEInstruction(instructionBytes,register1,register2);
                case 0x6:
                    register = (byte) ((instructionBytes >> 8) & 0x000F);
                    lowerByte = (byte) (instructionBytes & 0x00Ff);
                    return new Instructions.LDInstruction(instructionBytes,register,lowerByte);
                case 0x7:
                    register = (byte) ((instructionBytes >> 8) & 0x000F);
                    lowerByte = (byte) (instructionBytes & 0x00Ff);
                    return new Instructions.ADDInstruction(instructionBytes,register,lowerByte);
                case 0x8:
                    register1 = (ushort) ((instructionBytes >> 8) & 0x000F);
                    register2 = (ushort) ((instructionBytes >> 4) & 0x000F);
                    var op = instructionBytes & 0x000F;
                    //var opCode = string.Empty;
                    switch (op)
                    {
                        case 0x0:
                            return new Instructions.LDInstruction(instructionBytes,register1,register2);
                        case 0x1:
                            return new Instructions.ORInstruction(instructionBytes,register1,register2);
                        case 0x2:
                            return new Instructions.ANDInstruction(instructionBytes,register1,register2);
                        case 0x3:
                            return new Instructions.XORInstruction(instructionBytes, register1, register2);
                        case 0x4:
                            return new Instructions.ADDInstruction(instructionBytes, register1, register2);
                        case 0x5:
                            return new Instructions.SUBInstruction(instructionBytes, register1, register2);
                        case 0x6:
                            return new Instructions.SHRInstruction(instructionBytes, register1, register2);
                        case 0x7:
                            return new Instructions.SUBNInstruction(instructionBytes, register1, register2);
                        case 0xE:
                            return new Instructions.SHLInstruction(instructionBytes, register1, register2);
                        default:
                            return null;
                    }
                case 0x9:
                    register1 = (ushort) ((instructionBytes >> 8) & 0x000F);
                    register2 = (ushort) ((instructionBytes >> 4) & 0x000F);
                    return new Instructions.SNEInstruction(instructionBytes, register1, register2);
                case 0xA:
                    addr = (ushort) (instructionBytes & 0x0FFF);
                    return new Instructions.LDInstruction(instructionBytes,0,addr);
                case 0xB:
                    addr = (ushort) (instructionBytes & 0x0FFF);
                    return new Instructions.JPInstruction(instructionBytes,addr,0);
                case 0xC: 
                    register = (byte) ((instructionBytes >> 8) & 0x000F);
                    lowerByte = (byte) (instructionBytes & 0x00Ff);
                    return new Instructions.RNDInstruction(instructionBytes,register,lowerByte);
                case 0xD:
                    register1 = (ushort) ((instructionBytes >> 8) & 0x000F);
                    register2 = (ushort) ((instructionBytes >> 4) & 0x000F);
                    var lowerNibble = (byte)(instructionBytes & 0x000F);
                    return new Instructions.DRWInstruction(instructionBytes,register1,register2,lowerNibble);
                case 0xE:
                    register = (byte) ((instructionBytes >> 8) & 0x000F);
                    op = instructionBytes & 0x00FF;
                    switch (op)
                    {
                        case 0x9E:
                            return new Instructions.SKPInstruction(instructionBytes,register,0);
                        case 0xA1:
                            return new Instructions.SKNPInstruction(instructionBytes,register,0);
                        default:
                            return null;
                    }
                case 0xF:
                    register = (byte) ((instructionBytes >> 8) & 0x000F);
                    op = instructionBytes & 0x00FF;
                    switch (op)
                    {
                        case 0x07:
                        case 0x0A:
                        case 0x15:
                        case 0x18:
                        case 0x29:
                        case 0x33:
                        case 0x55:
                        case 0x65:
                            return new Instructions.LDInstruction(instructionBytes,register,0);
                        case 0x1E:
                            return new Instructions.ADDInstruction(instructionBytes,register,0);
                        default:
                            return null;
                    }

                default:
                    return null;
            }
        }
    }
}
