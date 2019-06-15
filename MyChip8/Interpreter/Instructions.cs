using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemComponents;

namespace MyChip8.Interpreter
{
    public class Instructions
    {
        public abstract class Instruction : IInstruction
        {
            protected ushort _originalOp;
            protected Parameter<ushort> _parameter1;
            protected Parameter<ushort> _parameter2;

            protected Instruction(ushort originalOp, ushort parameter1, ushort parameter2)
            {
                _originalOp = originalOp;
                _parameter1 = new Parameter<ushort>() {Value = parameter1};
                _parameter2 = new Parameter<ushort>() {Value = parameter2};
            }

            public abstract override string ToString();

            public abstract void Execute(CPU cpu);
            public virtual void Finalize(CPU cpu)
            {
                cpu.PC += 0x2;
            }
        }

        public class SYSInstruction : Instruction
        {
            public SYSInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
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

        public class CLSInstruction : Instruction
        {
            public CLSInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
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

        public class RETInstruction : Instruction
        {
            public RETInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
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
        public class JPInstruction : Instruction
        {
            private byte _upperNib;

            public JPInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x1:
                        return (string.Format("JP {0:x3}", _parameter1));
                    case 0xB:
                        return (string.Format("JP V0, {0:x3}", _parameter1));
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x1:
                        cpu.PC = _parameter1.Value;
                        break;
                    case 0xB:
                        cpu.PC = (byte)(_parameter1.Value + cpu.VRegisters[0]);
                        break;
                        //return (string.Format("JP V0, {0:x3}", _parameter1));
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
        public class CALLInstruction : Instruction
        {
            public CALLInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("CALL {0:x3}", _parameter1));
            }

            public override void Execute(CPU cpu)
            {
                cpu.Stack.Add(_parameter1.Value);
                cpu.SP++;
                cpu.PC = _parameter1.Value;
            }
            public override void Finalize(CPU cpu)
            {
            }
        }
        /// <summary>
        /// Skip next instruction if Vx = kk.
        /// </summary>
        public class SEInstruction : Instruction
        {
            private byte _upperNib;

            public SEInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x3:
                        return ($"SE V{_parameter1:x1}, {_parameter2:x2}");
                    case 0x5:
                        return ($"SE V{_parameter1:x1}, V{_parameter2:x1}");
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x3:
                        if (cpu.VRegisters[_parameter1.Value] == _parameter2.Value)
                            cpu.PC += 0x2;
                        break;
                    case 0x5:
                        if (cpu.VRegisters[_parameter1.Value] == cpu.VRegisters[_parameter2.Value])
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
        public class SNEInstruction : Instruction
        {
            private byte _upperNib;
            public SNEInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _upperNib = (byte) ((originalOp >> 12) & 0x000F);
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x4:
                        return ($"SNE V{_parameter1:x1}, {_parameter2:x2}");
                    case 0x9:
                        return ($"SNE V{_parameter1:x1}, V{_parameter2:x1}");
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x4:
                    if (cpu.VRegisters[_parameter1.Value] != _parameter2.Value)
                        cpu.PC += 0x2;
                    break;
                    case 0x9:
                        if (cpu.VRegisters[_parameter1.Value] != cpu.VRegisters[_parameter2.Value])
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
        public class LDInstruction : Instruction
        {
            private byte _upperNib;
            private byte _lowerNib;

            public LDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
                _lowerNib = (byte) (originalOp & 0x00FF);
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x6:
                        return ($"LD V{_parameter1:x1}, {_parameter2:x2}");
                    case 0x8:
                        return ($"LD V{_parameter1:x1},V{_parameter2:x1}");
                    case 0xA:
                        return ($"LD I, {_parameter1:x3}");
                    case 0xF:
                        switch (_lowerNib)
                        {
                            case 0x07:
                                return ($"LD V{_parameter1:x1},DT");
                            case 0x0A:
                                return ($"LD V{_parameter1:x1},K");
                            case 0x15:
                                return ($"LD DT,V{_parameter1:x1}");
                            case 0x18:
                                return ($"LD ST,V{_parameter1:x1}");
                            case 0x29:
                                return ($"LD F,V{_parameter1:x1}");
                            case 0x33:
                                return ($"LD B,V{_parameter1:x1}");
                            case 0x55:
                                return ($"LD [I],V{_parameter1:x1}");
                            case 0x65:
                                return ($"LD V{_parameter1:x1}, [I]");
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
                        cpu.VRegisters[_parameter1.Value] = (byte)_parameter2.Value;
                        break;
                    case 0x8:
                        cpu.VRegisters[_parameter1.Value] = cpu.VRegisters[_parameter2.Value];
                        break;
                    case 0xA:
                        cpu.I = _parameter1.Value;
                        break;
                    case 0xF:
                        switch (_lowerNib)
                        {
                            case 0x07:
                                cpu.VRegisters[_parameter1.Value] = cpu.DT;
                                break;
                            case 0x0A:
                                // Do some input check here
                                break;
                            case 0x15:
                                cpu.DT = cpu.VRegisters[_parameter1.Value];
                                break;
                            case 0x18:
                                cpu.ST = cpu.VRegisters[_parameter1.Value];
                                break;
                            case 0x29:
                                // sprite shit
                                break;
                            case 0x33:
                                var hundredDigit = Math.Abs(_parameter1.Value/100%10);
                                var tensDigit = Math.Abs(_parameter1.Value/10%10);
                                var onesDigit = Math.Abs(_parameter1.Value%10);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I,(byte)hundredDigit);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I+1, (byte)tensDigit);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I+2, (byte)onesDigit);
                                break;
                            case 0x55:
                                var address = cpu.I;
                                for (var i = 0; i < _parameter1.Value; i++)
                                {
                                    cpu.SystemMemory.SetByteAtAddress(address,cpu.VRegisters[i]);
                                    address++;
                                }
                                break;
                            case 0x65:
                                var addressToRead = cpu.I;
                                for (var i = 0; i < _parameter1.Value; i++)
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
        public class ADDInstruction : Instruction
        {
            private byte _upperNib;

            public ADDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _upperNib = (byte)((originalOp >> 12) & 0x000F);
            }

            public override string ToString()
            {
                switch (_upperNib)
                {
                    case 0x7:
                        return ($"ADD V{_parameter1:x1}, {_parameter2:x2}");
                    case 0x8:
                        return ($"ADD V{_parameter1:x1},V{_parameter2:x1}");
                    case 0xF:
                        return ($"ADD I,V{_parameter1:x1}");    
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x7:
                        cpu.VRegisters[_parameter1.Value] += (byte) _parameter2.Value;
                        break;
                    case 0x8:
                        ushort value = (ushort)(cpu.VRegisters[_parameter1.Value] + cpu.VRegisters[_parameter2.Value]);
                        if (value > 0x255)
                            cpu.VRegisters[0xF] = 1;
                        cpu.VRegisters[_parameter1.Value] = (byte) value;
                        break;
                    case 0xF:
                        cpu.I = (byte)(cpu.I + cpu.VRegisters[_parameter1.Value]);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Set Vx = Vx OR Vy.
        /// </summary>
        public class ORInstruction : Instruction
        {
            public ORInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("OR V{0:x1},V{1:x1}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                cpu.VRegisters[_parameter1.Value] = (byte)(cpu.VRegisters[_parameter1.Value] | cpu.VRegisters[_parameter2.Value]);
            }
        }

        public class ANDInstruction : Instruction
        {
            public ANDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("AND V{0:x1},V{1:x1}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                cpu.VRegisters[_parameter1.Value] = (byte)(cpu.VRegisters[_parameter1.Value] & cpu.VRegisters[_parameter2.Value]);                    
            }
        }

        /// <summary>
        /// Set Vx = Vx XOR Vy.
        /// </summary>
        public class XORInstruction : Instruction
        {
            public XORInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return ($"XOR V{_parameter1:x1},V{_parameter2:x1}");
            }

            public override void Execute(CPU cpu)
            {
                cpu.VRegisters[_parameter1.Value] = (byte)(cpu.VRegisters[_parameter1.Value] ^ cpu.VRegisters[_parameter2.Value]);                    
            }
        }

        /// <summary>
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// </summary>
        public class SUBInstruction : Instruction
        {
            public SUBInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _parameter1.IsRegister = true;
                _parameter1.RegisterType = RegisterTypes.V;
                _parameter2.IsRegister = true;
                _parameter2.RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SUB V{_parameter1:x1},V{_parameter2:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[_parameter1.Value] > cpu.VRegisters[_parameter2.Value])
                    cpu.VRegisters[0xF] = 1;
                cpu.VRegisters[_parameter1.Value] = (byte)(cpu.VRegisters[_parameter1.Value] - cpu.VRegisters[_parameter2.Value]);
            }
        }
        /// <summary>
        /// Set Vx = Vx SHR 1.
        /// </summary>
        public class SHRInstruction : Instruction
        {
            public SHRInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _parameter1.IsRegister = true;
                _parameter1.RegisterType = RegisterTypes.V;
                _parameter2.IsRegister = true;
                _parameter2.RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SHR V{_parameter1:x1},V{_parameter2:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if ((cpu.VRegisters[_parameter1.Value] & 1) == 1)
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[_parameter1.Value] = (byte)(cpu.VRegisters[_parameter1.Value] >> 1);
            }
        }
        /// <summary>
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// </summary>
        public class SUBNInstruction : Instruction
        {
            public SUBNInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _parameter1.IsRegister = true;
                _parameter1.RegisterType = RegisterTypes.V;
                _parameter2.IsRegister = true;
                _parameter2.RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SUBN V{_parameter1:x1},V{_parameter2:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[_parameter2.Value] > cpu.VRegisters[_parameter1.Value])
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[_parameter1.Value] = (byte)(cpu.VRegisters[_parameter2.Value] - cpu.VRegisters[_parameter1.Value]);
            }
        }
        /// <summary>
        /// Set Vx = Vx SHL 1.
        /// </summary>
        public class SHLInstruction : Instruction
        {
            public SHLInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _parameter1.IsRegister = true;
                _parameter1.RegisterType = RegisterTypes.V;
                _parameter2.IsRegister = true;
                _parameter2.RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SHL V{_parameter1:x1},V{_parameter2:x1}");
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[_parameter1.Value] >> 3 == 1)
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[_parameter1.Value] = (byte)(cpu.VRegisters[_parameter1.Value] << 1);                    
            }
        }

        public class RNDInstruction : Instruction
        {
            public RNDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _parameter1.IsRegister = true;
                _parameter1.RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"RND V{_parameter1:x1}, {_parameter2:x2}");
            }

            public override void Execute(CPU cpu)
            {
                var random = (byte)new Random((int)DateTime.UtcNow.Ticks).Next(0,255);
                cpu.VRegisters[_parameter1.Value] = (byte) (random & _parameter2.Value);
            }
        }

        public class DRWInstruction : Instruction
        {

            private byte _nibble;

            public DRWInstruction(ushort originalOp, ushort parameter1, ushort parameter2,byte nibble) : base(originalOp, parameter1, parameter2)
            {
                _nibble = nibble;
            }

            public override string ToString()
            {
                return ($"DRW V{_parameter1:x1}, V{_parameter2:x1}, {_nibble:x1}");
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class SKPInstruction : Instruction
        {
            public SKPInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _parameter1.IsRegister = true;
                _parameter1.RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SKP V{_parameter1:x1}");
            }

            public override void Execute(CPU cpu)
            {
                // Do some keyboard check here

            }
        }

        public class SKNPInstruction : Instruction
        {
            public SKNPInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
                _parameter1.IsRegister = true;
                _parameter1.RegisterType = RegisterTypes.V;
            }

            public override string ToString()
            {
                return ($"SKNP V{_parameter1:x1}");
            }

            public override void Execute(CPU cpu)
            {
                // Do some keyboard check here
            }
        }
    }
}
