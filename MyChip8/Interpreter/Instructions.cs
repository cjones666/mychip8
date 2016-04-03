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
            protected ushort _parameter1;
            protected ushort _parameter2;

            protected Instruction(ushort originalOp, ushort parameter1, ushort parameter2)
            {
                _originalOp = originalOp;
                _parameter1 = parameter1;
                _parameter2 = parameter2;
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
                throw new NotImplementedException();
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
                return ("CLS");
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
                return ("RET");
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
                        cpu.PC = _parameter1;
                        break;
                    case 0xB:
                        cpu.PC = (byte)(_parameter1 + cpu.VRegisters[0]);
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
                cpu.Stack.Add(_parameter1);
                cpu.SP++;
                cpu.PC = _parameter1;
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
                        return (string.Format("SE V{0:x1}, {1:x2}", _parameter1, _parameter2));
                    case 0x5:
                        return (string.Format("SE V{0:x1}, V{1:x1}", _parameter1, _parameter2));
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x3:
                        if (cpu.VRegisters[_parameter1] == _parameter2)
                            cpu.PC += 0x2;
                        break;
                    case 0x5:
                        if (cpu.VRegisters[_parameter1] == cpu.VRegisters[_parameter2])
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
                        return (string.Format("SNE V{0:x1}, {1:x2}", _parameter1, _parameter2));
                    case 0x9:
                        return (string.Format("SNE V{0:x1}, V{1:x1}", _parameter1, _parameter2));
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x4:
                    if (cpu.VRegisters[_parameter1] != _parameter2)
                        cpu.PC += 0x2;
                        break;
                    case 0x9:
                        if (cpu.VRegisters[_parameter1] != cpu.VRegisters[_parameter2])
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
                        return (string.Format("LD V{0:x1}, {1:x2}", _parameter1, _parameter2));
                    case 0x8:
                        return (string.Format("LD V{0:x1},V{1:x1}", _parameter1,_parameter2));
                    case 0xA:
                        return (string.Format("LD I, {0:x3}", _parameter1));
                    case 0xF:
                        switch (_lowerNib)
                        {
                            case 0x07:
                                return (string.Format("LD V{0:x1},DT", _parameter1));
                            case 0x0A:
                                return (string.Format("LD V{0:x1},K", _parameter1));
                            case 0x15:
                                return (string.Format("LD DT,V{0:x1}", _parameter1));
                            case 0x18:
                                return (string.Format("LD ST,V{0:x1}", _parameter1));
                            case 0x29:
                                return (string.Format("LD F,V{0:x1}", _parameter1));
                            case 0x33:
                                return (string.Format("LD B,V{0:x1}", _parameter1));
                            case 0x55:
                                return (string.Format("LD [I],V{0:x1}", _parameter1));
                            case 0x65:
                                return (string.Format("LD V{0:x1}, [I]", _parameter1));
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
                        cpu.VRegisters[_parameter1] = (byte)_parameter2;
                        break;
                    case 0x8:
                        cpu.VRegisters[_parameter1] = cpu.VRegisters[_parameter2];
                        break;
                    case 0xA:
                        cpu.I = _parameter1;
                        break;
                    case 0xF:
                        switch (_lowerNib)
                        {
                            case 0x07:
                                cpu.VRegisters[_parameter1] = cpu.DT;
                                break;
                            case 0x0A:
                                // Do some input check here
                                break;
                            case 0x15:
                                cpu.DT = cpu.VRegisters[_parameter1];
                                break;
                            case 0x18:
                                cpu.ST = cpu.VRegisters[_parameter1];
                                break;
                            case 0x29:
                                // sprite shit
                                break;
                            case 0x33:
                                var hundredDigit = Math.Abs(_parameter1/100%10);
                                var tensDigit = Math.Abs(_parameter1/10%10);
                                var onesDigit = Math.Abs(_parameter1%10);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I,(byte)hundredDigit);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I+1, (byte)tensDigit);
                                cpu.SystemMemory.SetByteAtAddress(cpu.I+2, (byte)onesDigit);
                                break;
                            case 0x55:
                                var address = cpu.I;
                                for (var i = 0; i < _parameter1; i++)
                                {
                                    cpu.SystemMemory.SetByteAtAddress(address,cpu.VRegisters[i]);
                                    address++;
                                }
                                break;
                            case 0x65:
                                var addressToRead = cpu.I;
                                for (var i = 0; i < _parameter1; i++)
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
                        return (string.Format("ADD V{0:x1}, {1:x2}", _parameter1, _parameter2));
                    case 0x8:
                        return (string.Format("ADD V{0:x1},V{1:x1}", _parameter1, _parameter2));
                    case 0xF:
                        return (string.Format("ADD I,V{0:x1}", _parameter1));    
                    default:
                        return string.Empty;
                }
            }

            public override void Execute(CPU cpu)
            {
                switch (_upperNib)
                {
                    case 0x7:
                        cpu.VRegisters[_parameter1] += (byte) _parameter2;
                        break;
                    case 0x8:
                        ushort value = (ushort)(cpu.VRegisters[_parameter1] + cpu.VRegisters[_parameter2]);
                        if (value > 0x255)
                            cpu.VRegisters[0xF] = 1;
                        cpu.VRegisters[_parameter1] = (byte) value;
                        break;
                    case 0xF:
                        cpu.I = (byte)(cpu.I + cpu.VRegisters[_parameter1]);
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
                cpu.VRegisters[_parameter1] = (byte)(cpu.VRegisters[_parameter1] | cpu.VRegisters[_parameter2]);
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
                cpu.VRegisters[_parameter1] = (byte)(cpu.VRegisters[_parameter1] & cpu.VRegisters[_parameter2]);                    
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
                return (string.Format("XOR V{0:x1},V{1:x1}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                cpu.VRegisters[_parameter1] = (byte)(cpu.VRegisters[_parameter1] ^ cpu.VRegisters[_parameter2]);                    
            }
        }

        /// <summary>
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// </summary>
        public class SUBInstruction : Instruction
        {
            public SUBInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("SUB V{0:x1},V{1:x1}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[_parameter1] > cpu.VRegisters[_parameter2])
                    cpu.VRegisters[0xF] = 1;
                cpu.VRegisters[_parameter1] = (byte)(cpu.VRegisters[_parameter1] - cpu.VRegisters[_parameter2]);
            }
        }
        /// <summary>
        /// Set Vx = Vx SHR 1.
        /// </summary>
        public class SHRInstruction : Instruction
        {
            public SHRInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("SHR V{0:x1},V{1:x1}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                if ((cpu.VRegisters[_parameter1] & 1) == 1)
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[_parameter1] = (byte)(cpu.VRegisters[_parameter1] >> 1);
            }
        }
        /// <summary>
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// </summary>
        public class SUBNInstruction : Instruction
        {
            public SUBNInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("SUBN V{0:x1},V{1:x1}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[_parameter2] > cpu.VRegisters[_parameter1])
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[_parameter1] = (byte)(cpu.VRegisters[_parameter2] - cpu.VRegisters[_parameter1]);
            }
        }
        /// <summary>
        /// Set Vx = Vx SHL 1.
        /// </summary>
        public class SHLInstruction : Instruction
        {
            public SHLInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("SHL V{0:x1},V{1:x1}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                if (cpu.VRegisters[_parameter1] >> 3 == 1)
                {
                    cpu.VRegisters[0xF] = 1;
                }
                else
                {
                    cpu.VRegisters[0xF] = 0;
                }
                cpu.VRegisters[_parameter1] = (byte)(cpu.VRegisters[_parameter1] << 1);                    
            }
        }

        public class RNDInstruction : Instruction
        {
            public RNDInstruction(ushort originalOp, ushort parameter1, ushort parameter2) : base(originalOp, parameter1, parameter2)
            {
            }

            public override string ToString()
            {
                return (string.Format("RND V{0:x1}, {1:x2}", _parameter1, _parameter2));
            }

            public override void Execute(CPU cpu)
            {
                var random = (byte)new Random((int)DateTime.UtcNow.Ticks).Next(0,255);
                cpu.VRegisters[_parameter1] = (byte) (random & _parameter2);
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
                return (string.Format("DRW V{0:x1}, V{1:x1}, {2:x1}", _parameter1, _parameter2, _nibble));
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
            }

            public override string ToString()
            {
                return (string.Format("SKP V{0:x1}", _parameter1));
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
            }

            public override string ToString()
            {
                return (string.Format("SKNP V{0:x1}", _parameter1));
            }

            public override void Execute(CPU cpu)
            {
                // Do some keyboard check here
            }
        }
    }
}
