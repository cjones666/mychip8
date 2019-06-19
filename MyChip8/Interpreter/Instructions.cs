using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemComponents;

namespace MyChip8.Interpreter
{
    public class Instructions
    {
        public abstract class Instruction<T> : IInstruction<T>
        {
            public string Name { get; set; }
            public List<Parameter<T>> Parameters { get; set; }
         
            protected ushort _originalOp;

            protected Instruction(string name, ushort originalOp, params T[] parameters)
            {
                Name = name;
                _originalOp = originalOp;
                Parameters = new List<Parameter<T>>();
                foreach (var param in parameters)
                {
                    var newParameter = new Parameter<T>() {Value = param};
                    Parameters.Add(newParameter);
                }
            }

            public abstract override string ToString();

            public abstract void Execute(CPU cpu);
            public virtual void Finalize(CPU cpu)
            {
                cpu.PC += 0x2;
            }

            public Parameter<T> GetParameter(int index)
            {
                if (Parameters == null || index < 0 || index >= Parameters.Count)
                    return null;
                return Parameters[index];
            }
        }

        public class SYSInstruction : Instruction<ushort>
        {
            public SYSInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SYS,originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return "SYS";
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class CLSInstruction : Instruction<ushort>
        {
            public CLSInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.CLS,originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return "CLS";
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class RETInstruction : Instruction<ushort>
        {
            public RETInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.RET,originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return "RET";
            }

            public override void Execute(CPU cpu)
            {
                cpu.PC = cpu.Stack[cpu.SP];
                cpu.SP--;
            }
        }
        
        /// <summary>
        /// Jump to location nnn.
        /// </summary>
        public class JPInstruction : Instruction<ushort>
        {
            private byte _upperNib;

            public JPInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.JP,originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x1:
                        return ($"JP {GetParameter(0).Value:x3}");
                    case 0xB:
                        return ($"JP V0, {GetParameter(0).Value:x3}");
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x1:
                        cpu.PC = GetParameter(0).Value;
                        break;
                    case 0xB:
                        cpu.PC = (byte)(GetParameter(0).Value + cpu.VRegisters[0]);
                        break;
                    default:
                        break;
                }
            }

            public override void Finalize(CPU cpu)
            {
            }
        }

        /// <summary>
        /// Call subroutine at nnn.
        /// </summary>
        public class CALLInstruction : Instruction<ushort>
        {
            public CALLInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.CALL,originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return ($"CALL {GetParameter(0).Value:x3}");
            }

            public override void Execute(CPU cpu)
            {
                cpu.Stack.Add(GetParameter(0).Value);
                cpu.SP++;
                cpu.PC = GetParameter(0).Value;
            }
            public override void Finalize(CPU cpu)
            {
            }
        }
        /// <summary>
        /// Skip next instruction if Vx = kk.
        /// </summary>
        public class SEInstruction : Instruction<ushort>
        {
            private byte _upperNib;

            public SEInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SE,originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
                switch (_upperNib)
                {
                    case 0x3:
                        GetParameter(0).RegisterType = RegisterTypes.V;
                        break;
                    case 0x5:
                        GetParameter(0).RegisterType = RegisterTypes.V;
                        GetParameter(1).RegisterType = RegisterTypes.V;
                        break;
                }
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x3:
                        return ($"SE V{GetParameter(0).Value:x1}, {GetParameter(1).Value:x2}");
                    case 0x5:
                        return ($"SE V{GetParameter(0).Value:x1}, V{GetParameter(1).Value:x1}");
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x3:
                        if (cpu.VRegisters[GetParameter(0).Value] == GetParameter(1).Value)
                            cpu.PC += 0x2;
                        break;
                    case 0x5:
                        if (cpu.VRegisters[GetParameter(0).Value] == cpu.VRegisters[GetParameter(1).Value])
                            cpu.PC += 0x2;
                        break;
                    default:
                        return;
                }
            }
        }
        /// <summary>
        /// Skip next instruction if Vx != kk.
        /// </summary>
        public class SNEInstruction : Instruction<ushort>
        {
            private byte _upperNib;
            public SNEInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SNE,originalOp, parameter1, parameter2)
            {
                _upperNib = (byte) ((originalOp >> 12) & 0x000F);
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x4:
                        return ($"SNE V{GetParameter(0).Value:x1}, {GetParameter(1).Value:x2}");
                    case 0x9:
                        return ($"SNE V{GetParameter(0).Value:x1}, V{GetParameter(1).Value:x1}");
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x4:
                    if (cpu.VRegisters[GetParameter(0).Value] != GetParameter(1).Value)
                        cpu.PC += 0x2;
                    break;
                    case 0x9:
                        if (cpu.VRegisters[GetParameter(0).Value] != cpu.VRegisters[GetParameter(1).Value])
                            cpu.PC += 0x2;
                        break;
                    default:
                        break;
                }
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Set Vx = kk.
        /// </summary>
        public class LDInstruction : Instruction<ushort>
        {
            private byte _upperNib;
            private byte _lowerNib;

            public LDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.LD,originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
                _lowerNib = (byte) (originalOp & 0x00FF);

                switch (_upperNib)
                {
                    case 0x6:
                        GetParameter(0).RegisterType = RegisterTypes.V;
                        break;
                    case 0x8:
                        GetParameter(0).RegisterType = RegisterTypes.V;
                        GetParameter(1).RegisterType = RegisterTypes.V;
                        break;
                    case 0xA:
                        GetParameter(0).RegisterType = RegisterTypes.I;
                        break;
                    case 0xF:
                        switch (_lowerNib)
                        {
                            case 0x07:
                                GetParameter(0).RegisterType = RegisterTypes.V;
                                GetParameter(1).RegisterType = RegisterTypes.DT;
                                break;
                            case 0x0A:
                                GetParameter(0).RegisterType = RegisterTypes.V;
                                break;
                            case 0x15:
                                GetParameter(0).RegisterType = RegisterTypes.DT;
                                GetParameter(1).RegisterType = RegisterTypes.V;
                                break;
                            case 0x18:
                                GetParameter(1).RegisterType = RegisterTypes.V;
                                break;
                            case 0x29:
                                GetParameter(1).RegisterType = RegisterTypes.V;
                                break;
                            case 0x33:
                                GetParameter(1).RegisterType = RegisterTypes.V;
                                break;
                            case 0x55:
                                GetParameter(0).RegisterType = RegisterTypes.I;
                                GetParameter(1).RegisterType = RegisterTypes.V;
                                break;
                            case 0x65:
                                GetParameter(0).RegisterType = RegisterTypes.V;
                                GetParameter(1).RegisterType = RegisterTypes.I;
                                break;
                        }
                        break;
                }
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x6:
                        return ($"LD V{GetParameter(0).Value:x1}, {GetParameter(1).Value:x2}");
                    case 0x8:
                        return ($"LD V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
                    case 0xA:
                        return ($"LD I, {GetParameter(0).Value:x3}");
                    case 0xF:
                        switch (_lowerNib)
                        {
                            case 0x07:
                                return ($"LD V{GetParameter(0).Value:x1},DT");
                            case 0x0A:
                                return ($"LD V{GetParameter(0).Value:x1},K");
                            case 0x15:
                                return ($"LD DT,V{GetParameter(0).Value:x1}");
                            case 0x18:
                                return ($"LD ST,V{GetParameter(0).Value:x1}");
                            case 0x29:
                                return ($"LD F,V{GetParameter(0).Value:x1}");
                            case 0x33:
                                return ($"LD B,V{GetParameter(0).Value:x1}");
                            case 0x55:
                                return ($"LD [I],V{GetParameter(0).Value:x1}");
                            case 0x65:
                                return ($"LD V{GetParameter(0).Value:x1}, [I]");
                            default:
                                return string.Empty;
                        }
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x6:
                        cpu.VRegisters[GetParameter(0).Value] = (byte)GetParameter(1).Value;
                        break;
                    case 0x8:
                        cpu.VRegisters[GetParameter(0).Value] = cpu.VRegisters[GetParameter(1).Value];
                        break;
                    case 0xA:
                        cpu.I = GetParameter(0).Value;
                        break;
                    case 0xF:
                        switch (_lowerNib)
                        {
                            case 0x07:
                                cpu.VRegisters[GetParameter(0).Value] = cpu.DT;
                                break;
                            case 0x0A:
                                // Do some input check here
                                break;
                            case 0x15:
                                cpu.DT = cpu.VRegisters[GetParameter(0).Value];
                                break;
                            case 0x18:
                                cpu.ST = cpu.VRegisters[GetParameter(0).Value];
                                break;
                            case 0x29:
                                // sprite shit
                                break;
                            case 0x33:
                                var hundredDigit = Math.Abs(GetParameter(0).Value/100%10);
                                var tensDigit = Math.Abs(GetParameter(0).Value/10%10);
                                var onesDigit = Math.Abs(GetParameter(0).Value%10);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I,(byte)hundredDigit);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I+1, (byte)tensDigit);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I+2, (byte)onesDigit);
                                break;
                            case 0x55:
                                var address = cpu.I;
                                for (var i = 0; i < GetParameter(0).Value; i++)
                                {
                                    cpu.SystemMemory.SetByteAtAddress(address,cpu.VRegisters[i]);
                                    address++;
                                }
                                break;
                            case 0x65:
                                var addressToRead = cpu.I;
                                for (var i = 0; i < GetParameter(0).Value; i++)
                                {
                                    cpu.VRegisters[i] = cpu.SystemMemory.ReadByteAtAddress(addressToRead);
                                    addressToRead++;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Set Vx = Vx + kk.
        /// </summary>
        public class ADDInstruction : Instruction<ushort>
        {
            private byte _upperNib;

            public ADDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.ADD,originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
                switch (_upperNib)
                {
                    case 0x7:
                        GetParameter(0).RegisterType = RegisterTypes.V;
                        break;
                    case 0x8:
                        GetParameter(1).RegisterType = RegisterTypes.V;
                        break;
                    case 0xF:
                        GetParameter(0).RegisterType = RegisterTypes.I;
                        break;
                }
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x7:
                        return ($"ADD V{GetParameter(0).Value:x1}, {GetParameter(1).Value:x2}");
                    case 0x8:
                        return ($"ADD V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
                    case 0xF:
                        return ($"ADD I,V{GetParameter(0).Value:x1}");    
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x7:
                        cpu.VRegisters[GetParameter(0).Value] += (byte) GetParameter(1).Value;
                        break;
                    case 0x8:
                        ushort value = (ushort)(cpu.VRegisters[GetParameter(0).Value] + cpu.VRegisters[GetParameter(1).Value]);
                        if (value > 0x255)
                            cpu.VRegisters[0xF] = 1;
                        cpu.VRegisters[GetParameter(0).Value] = (byte) value;
                        break;
                    case 0xF:
                        cpu.I = (byte)(cpu.I + cpu.VRegisters[GetParameter(0).Value]);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Set Vx = Vx OR Vy.
        /// </summary>
        public class ORInstruction : Instruction<ushort>
        {
            public ORInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.OR,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"OR V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                cpu.VRegisters[GetParameter(0).Value] = (byte)(cpu.VRegisters[GetParameter(0).Value] | cpu.VRegisters[GetParameter(1).Value]);
            }
        }

        public class ANDInstruction : Instruction<ushort>
        {
            public ANDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.AND,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"AND V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                cpu.VRegisters[GetParameter(0).Value] = (byte)(cpu.VRegisters[GetParameter(0).Value] & cpu.VRegisters[GetParameter(1).Value]);                    
            }
        }

        /// <summary>
        /// Set Vx = Vx XOR Vy.
        /// </summary>
        public class XORInstruction : Instruction<ushort>
        {
            public XORInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.XOR,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"XOR V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                cpu.VRegisters[GetParameter(0).Value] = (byte)(cpu.VRegisters[GetParameter(0).Value] ^ cpu.VRegisters[GetParameter(1).Value]);                    
            }
        }

        /// <summary>
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// </summary>
        public class SUBInstruction : Instruction<ushort>
        {
            public SUBInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SUB,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SUB V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[GetParameter(0).Value] > cpu.VRegisters[GetParameter(1).Value])
                    cpu.VRegisters[0xF] = 1;
                cpu.VRegisters[GetParameter(0).Value] = (byte)(cpu.VRegisters[GetParameter(0).Value] - cpu.VRegisters[GetParameter(1).Value]);
            }
        }
        /// <summary>
        /// Set Vx = Vx SHR 1.
        /// </summary>
        public class SHRInstruction : Instruction<ushort>
        {
            public SHRInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SHR,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SHR V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if ((cpu.VRegisters[GetParameter(0).Value] & 1) == 1)
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[GetParameter(0).Value] = (byte)(cpu.VRegisters[GetParameter(0).Value] >> 1);
            }
        }
        /// <summary>
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// </summary>
        public class SUBNInstruction : Instruction<ushort>
        {
            public SUBNInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SUBN,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SUBN V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[GetParameter(1).Value] > cpu.VRegisters[GetParameter(0).Value])
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[GetParameter(0).Value] = (byte)(cpu.VRegisters[GetParameter(1).Value] - cpu.VRegisters[GetParameter(0).Value]);
            }
        }
        /// <summary>
        /// Set Vx = Vx SHL 1.
        /// </summary>
        public class SHLInstruction : Instruction<ushort>
        {
            public SHLInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SHL,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SHL V{GetParameter(0).Value:x1},V{GetParameter(1).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[GetParameter(0).Value] >> 3 == 1)
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[GetParameter(0).Value] = (byte)(cpu.VRegisters[GetParameter(0).Value] << 1);                    
            }
        }

        public class RNDInstruction : Instruction<ushort>
        {
            public RNDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.RND,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"RND V{GetParameter(0).Value:x1}, {GetParameter(1).Value:x2}");
            }

            public override void Execute(CPU cpu)
            {
                var random = (byte)new Random((int)DateTime.UtcNow.Ticks).Next(0,255);
                cpu.VRegisters[GetParameter(0).Value] = (byte) (random & GetParameter(1).Value);
            }
        }

        public class DRWInstruction : Instruction<ushort>
        {
            private byte _nibble;

            public DRWInstruction(ushort originalOp, ushort parameter1, ushort parameter2,byte nibble) : base(OpNames.DRW,originalOp, parameter1, parameter2)
            {
                _nibble = nibble;
                GetParameter(0).RegisterType = RegisterTypes.V;
                GetParameter(1).RegisterType = RegisterTypes.V;
                Parameters.Add(new Parameter<ushort>() {Value = nibble});
            }

            public override string ToString()
            {
                return ($"DRW V{GetParameter(0).Value:x1}, V{GetParameter(1).Value:x1}, {_nibble:x1}");
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class SKPInstruction : Instruction<ushort>
        {
            public SKPInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SKP,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SKP V{GetParameter(0).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                // Do some keyboard check here

            }
        }

        public class SKNPInstruction : Instruction<ushort>
        {
            public SKNPInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(OpNames.SKNP,originalOp, parameter1, parameter2)
            {
                GetParameter(0).RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SKNP V{GetParameter(0).Value:x1}");
            }

            public override void Execute(CPU cpu)
            {
                // Do some keyboard check here
            }
        }
    }
}
