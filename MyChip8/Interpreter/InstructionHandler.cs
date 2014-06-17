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

        private Dictionary<InstructionType, IInstruction> _opMap;

        public InstructionHandler()
        {
            _opMap = new Dictionary<InstructionType, IInstruction>();
            var opTypes = Enum.GetValues(typeof (InstructionType.Op));
            foreach (var instructionType in opTypes)
            {
                var type = Type.GetType(instructionType + "Instruction");
                if (type != null)
                {
                    var instructionInstance = Activator.CreateInstance(type) as IInstruction;
                    _opMap.Add((InstructionType)instructionType,instructionInstance);
                }
            }
        }

        public static string GetInstruction(byte upperByte, byte lowerByte)
        {
            var instructionBytes = (ushort)((upperByte) << 8 | (lowerByte));
            return GetInstruction(instructionBytes);
        }

        // The instruction ops in CHIP-8 are 2 bytes, thus we are passing in the op as a single 16-bit short integer.
        public static string GetInstruction(ushort instructionBytes)
        {
            
            var upperNib = (instructionBytes >> 12) & 0x000F;

            int addr;
            int lowerByte;
            int register;
            switch (upperNib)
            {
                case 0x0:
                {
                    // Get the lower byte
                    lowerByte = instructionBytes & 0x00FF;
                    if (lowerByte == 0xE0)
                        return ("CLS");
                    if (lowerByte == 0xEE)
                        return ("RET");
                    return ("OpCode 0 not implemented");
                }
                case 0x1:
                    addr = instructionBytes & 0x0FFF;
                    return (string.Format("JP {0:x3}", addr));
                case 0x2:
                    addr = instructionBytes & 0x0FFF;
                    return (string.Format("CALL {0:x3}", addr));
                case 0x3:
                    register = (instructionBytes >> 8) & 0x000F;
                    lowerByte = instructionBytes & 0x00Ff;
                    return (string.Format("SE V{0:x1}, {1:x2}", register,lowerByte));
                case 0x4:
                    register = (instructionBytes >> 8) & 0x000F;
                    lowerByte = instructionBytes & 0x00FF;
                    return (string.Format("SNE V{0:x1}, {1:x2}", register,lowerByte));
                case 0x5:
                    var register1 = (instructionBytes >> 8) & 0x000F;
                    var register2 = (instructionBytes >> 4) & 0x000F;
                    return (string.Format("SE V{0:x1}, V{1:x1}", register1, register2));
                case 0x6:
                    register = (instructionBytes >> 8) & 0x000F;
                    lowerByte = instructionBytes & 0x00Ff;
                    return (string.Format("LD V{0:x1}, {1:x2}", register, lowerByte));
                case 0x7:
                    register = (instructionBytes >> 8) & 0x000F;
                    lowerByte = instructionBytes & 0x00Ff;
                    return (string.Format("ADD V{0:x1}, {1:x2}", register, lowerByte));
                case 0x8:
                    register1 = (instructionBytes >> 8) & 0x000F;
                    register2 = (instructionBytes >> 4) & 0x000F;
                    var op = instructionBytes & 0x000F;
                    string opCode = String.Empty;
                    if (op == 0x0)
                        opCode = "LD";
                    else if (op == 0x1)
                        opCode = "OR";
                    else if (op == 0x2)
                        opCode = "AND";
                    else if (op == 0x3)
                        opCode = "XOR";
                    else if (op == 0x4)
                        opCode = "ADD";
                    else if (op == 0x5)
                        opCode = "SUB";
                    else if (op == 0x6)
                        opCode = "SHR";
                    else if (op == 0x7)
                        opCode = "SUBN";
                    else if (op == 0xE)
                        opCode = "SHL";
                    return (string.Format("{0} V{1:x1},V{2:x1}", opCode, register1, register2));
                case 0x9:
                    register1 = (instructionBytes >> 8) & 0x000F;
                    register2 = (instructionBytes >> 4) & 0x000F;
                    return (string.Format("SNE V{0:x1}, {1:x2}", register1, register2));
                case 0xA:
                    addr = instructionBytes & 0x0FFF;
                    return (string.Format("LD I, {0:x3}", addr));
                case 0xB:
                    addr = instructionBytes & 0x0FFF;
                    return (string.Format("JP V0, {0:x3}", addr));
                case 0xC: 
                    register = (instructionBytes >> 8) & 0x000F;
                    lowerByte = instructionBytes & 0x00Ff;
                    return (string.Format("RND V{0:x1}, {1:x2}", register, lowerByte));
                case 0xD:
                    register1 = (instructionBytes >> 8) & 0x000F;
                    register2 = (instructionBytes >> 4) & 0x000F;
                    var lowerNibble = instructionBytes & 0x000F;
                    return (string.Format("DRW V{0:x1}, V{1:x1}, {2:x1}", register1, register2,lowerNibble));
                case 0xE:
                    register = (instructionBytes >> 8) & 0x000F;
                    op = instructionBytes & 0x00FF;
                    opCode = string.Empty;
                    if (op == 0x9E)
                        opCode = "SKP";
                    else if (op == 0xA1)
                        opCode = "SKNP";
                    return (string.Format("{0} V{1:x1}", opCode, register));
                case 0xF:
                    register = (instructionBytes >> 8) & 0x000F;
                    op = instructionBytes & 0x00FF;
                    opCode = string.Empty;
                    if (op == 0x07)
                    {
                        opCode = "LD";
                        return (string.Format("{0} V{1:x1},DT", opCode, register));
                    }
                    else if (op == 0x0A)
                    {
                        opCode = "LD";
                        return (string.Format("{0} V{1:x1},K", opCode, register));
                    }
                    else if (op == 0x15)
                    {
                        opCode = "LD";
                        return (string.Format("{0} DT,V{1:x1}", opCode, register));
                    }
                    else if (op == 0x18)
                    {
                        opCode = "LD";
                        return (string.Format("{0} ST,V{1:x1}", opCode, register));
                    }
                    else if (op == 0x1E)
                    {
                        opCode = "ADD";
                        return (string.Format("{0} I,V{1:x1}", opCode, register));    
                    }
                    else if (op == 0x29)
                    {
                        opCode = "LD";
                        return (string.Format("{0} F,V{1:x1}", opCode, register));
                    }
                    else if (op == 0x33)
                    {
                        opCode = "LD";
                        return (string.Format("{0} B,V{1:x1}", opCode, register));
                    }
                    else if (op == 0x55)
                    {
                        opCode = "LD";
                        return (string.Format("{0} [I],V{1:x1}", opCode, register));
                    }
                    else if (op == 0x65)
                    {
                        opCode = "LD";
                        return (string.Format("{0} V{1:x1}, [I]", opCode, register));
                    }
                    return "OpCode not recognized";
                default:
                    return "OpCode not recognized";
            }
        }
    }
}
