using MyChip8.SystemComponents;

namespace MyChip8.Interpreter;

public static class Instructions
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
            Parameters = [];
            foreach (var param in parameters)
            {
                Parameters.Add(new Parameter<T> { Value = param });
            }
        }

        public abstract override string ToString();

        public abstract void Execute(CPU cpu);

        public virtual void Finalize(CPU cpu) => cpu.PC += 0x2;

        public Parameter<T>? GetParameter(int index) =>
            Parameters == null || index < 0 || index >= Parameters.Count
                ? null
                : Parameters[index];
    }

    public class SYSInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SYS, originalOp, parameter1, parameter2)
    {
        public override string ToString() => "SYS";

        public override void Execute(CPU cpu) => throw new NotImplementedException();
    }

    public class CLSInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.CLS, originalOp, parameter1, parameter2)
    {
        public override string ToString() => "CLS";

        public override void Execute(CPU cpu) => cpu.Display.Clear();
    }

    public class RETInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.RET, originalOp, parameter1, parameter2)
    {
        public override string ToString() => "RET";

        public override void Execute(CPU cpu)
        {
            cpu.SP--;
            cpu.PC = cpu.Stack[cpu.SP];
        }
    }

    /// <summary>
    /// Jump to location nnn.
    /// </summary>
    public class JPInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.JP, originalOp, parameter1, parameter2)
    {
        private readonly byte _upperNib = (byte)((originalOp >> 12) & 0x000F);

        public override string ToString() => _upperNib switch
        {
            0x1 => $"JP {GetParameter(0)!.Value:x3}",
            0xB => $"JP V0, {GetParameter(0)!.Value:x3}",
            _ => string.Empty
        };

        public override void Execute(CPU cpu)
        {
            switch (_upperNib)
            {
                case 0x1:
                    cpu.PC = GetParameter(0)!.Value;
                    break;
                case 0xB:
                    cpu.PC = (ushort)(GetParameter(0)!.Value + cpu.VRegisters[0]);
                    break;
            }
        }

        public override void Finalize(CPU cpu) { }
    }

    /// <summary>
    /// Call subroutine at nnn.
    /// </summary>
    public class CALLInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.CALL, originalOp, parameter1, parameter2)
    {
        public override string ToString() => $"CALL {GetParameter(0)!.Value:x3}";

        public override void Execute(CPU cpu)
        {
            cpu.Stack.Add(cpu.PC);
            cpu.SP++;
            cpu.PC = GetParameter(0)!.Value;
        }

        public override void Finalize(CPU cpu) { }
    }

    /// <summary>
    /// Skip next instruction if Vx = kk.
    /// </summary>
    public class SEInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SE, originalOp, parameter1, parameter2)
    {
        private readonly byte _upperNib = (byte)((originalOp >> 12) & 0x000F);

        public SEInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            switch (_upperNib)
            {
                case 0x3:
                    GetParameter(0)!.RegisterType = RegisterTypes.V;
                    break;
                case 0x5:
                    GetParameter(0)!.RegisterType = RegisterTypes.V;
                    GetParameter(1)!.RegisterType = RegisterTypes.V;
                    break;
            }
        }

        public override string ToString() => _upperNib switch
        {
            0x3 => $"SE V{GetParameter(0)!.Value:x1}, {GetParameter(1)!.Value:x2}",
            0x5 => $"SE V{GetParameter(0)!.Value:x1}, V{GetParameter(1)!.Value:x1}",
            _ => string.Empty
        };

        public override void Execute(CPU cpu)
        {
            switch (_upperNib)
            {
                case 0x3:
                    if (cpu.VRegisters[GetParameter(0)!.Value] == GetParameter(1)!.Value)
                        cpu.PC += 0x2;
                    break;
                case 0x5:
                    if (cpu.VRegisters[GetParameter(0)!.Value] == cpu.VRegisters[GetParameter(1)!.Value])
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
    public class SNEInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SNE, originalOp, parameter1, parameter2)
    {
        private readonly byte _upperNib = (byte)((originalOp >> 12) & 0x000F);

        public override string ToString() => _upperNib switch
        {
            0x4 => $"SNE V{GetParameter(0)!.Value:x1}, {GetParameter(1)!.Value:x2}",
            0x9 => $"SNE V{GetParameter(0)!.Value:x1}, V{GetParameter(1)!.Value:x1}",
            _ => string.Empty
        };

        public override void Execute(CPU cpu)
        {
            switch (_upperNib)
            {
                case 0x4:
                if (cpu.VRegisters[GetParameter(0)!.Value] != GetParameter(1)!.Value)
                    cpu.PC += 0x2;
                break;
                case 0x9:
                    if (cpu.VRegisters[GetParameter(0)!.Value] != cpu.VRegisters[GetParameter(1)!.Value])
                        cpu.PC += 0x2;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Set Vx = kk.
    /// </summary>
    public class LDInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.LD, originalOp, parameter1, parameter2)
    {
        private readonly byte _upperNib = (byte)((originalOp >> 12) & 0x000F);
        private readonly byte _lowerNib = (byte)(originalOp & 0x00FF);

        public LDInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            switch (_upperNib)
            {
                case 0x6:
                    GetParameter(0)!.RegisterType = RegisterTypes.V;
                    break;
                case 0x8:
                    GetParameter(0)!.RegisterType = RegisterTypes.V;
                    GetParameter(1)!.RegisterType = RegisterTypes.V;
                    break;
                case 0xA:
                    GetParameter(0)!.RegisterType = RegisterTypes.I;
                    break;
                case 0xF:
                    switch (_lowerNib)
                    {
                        case 0x07:
                            GetParameter(0)!.RegisterType = RegisterTypes.V;
                            GetParameter(1)!.RegisterType = RegisterTypes.DT;
                            break;
                        case 0x0A:
                            GetParameter(0)!.RegisterType = RegisterTypes.V;
                            break;
                        case 0x15:
                            GetParameter(0)!.RegisterType = RegisterTypes.DT;
                            GetParameter(1)!.RegisterType = RegisterTypes.V;
                            break;
                        case 0x18:
                            GetParameter(1)!.RegisterType = RegisterTypes.V;
                            break;
                        case 0x29:
                            GetParameter(1)!.RegisterType = RegisterTypes.V;
                            break;
                        case 0x33:
                            GetParameter(1)!.RegisterType = RegisterTypes.V;
                            break;
                        case 0x55:
                            GetParameter(0)!.RegisterType = RegisterTypes.I;
                            GetParameter(1)!.RegisterType = RegisterTypes.V;
                            break;
                        case 0x65:
                            GetParameter(0)!.RegisterType = RegisterTypes.V;
                            GetParameter(1)!.RegisterType = RegisterTypes.I;
                            break;
                    }
                    break;
            }
        }

        public override string ToString() => _upperNib switch
        {
            0x6 => $"LD V{GetParameter(0)!.Value:x1}, {GetParameter(1)!.Value:x2}",
            0x8 => $"LD V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}",
            0xA => $"LD I, {GetParameter(0)!.Value:x3}",
            0xF => _lowerNib switch
            {
                0x07 => $"LD V{GetParameter(0)!.Value:x1},DT",
                0x0A => $"LD V{GetParameter(0)!.Value:x1},K",
                0x15 => $"LD DT,V{GetParameter(0)!.Value:x1}",
                0x18 => $"LD ST,V{GetParameter(0)!.Value:x1}",
                0x29 => $"LD F,V{GetParameter(0)!.Value:x1}",
                0x33 => $"LD B,V{GetParameter(0)!.Value:x1}",
                0x55 => $"LD [I],V{GetParameter(0)!.Value:x1}",
                0x65 => $"LD V{GetParameter(0)!.Value:x1}, [I]",
                _ => string.Empty
            },
            _ => string.Empty
        };

        public override void Execute(CPU cpu)
        {
            switch (_upperNib)
            {
                case 0x6:
                    cpu.VRegisters[GetParameter(0)!.Value] = (byte)GetParameter(1)!.Value;
                    break;
                case 0x8:
                    cpu.VRegisters[GetParameter(0)!.Value] = cpu.VRegisters[GetParameter(1)!.Value];
                    break;
                case 0xA:
                    cpu.I = GetParameter(1)!.Value;
                    break;
                case 0xF:
                    switch (_lowerNib)
                    {
                        case 0x07:
                            cpu.VRegisters[GetParameter(0)!.Value] = cpu.DT;
                            break;
                        case 0x0A:
                            // Wait for key press
                            cpu.WaitingForKey = true;
                            cpu.WaitingKeyRegister = (byte)GetParameter(0)!.Value;
                            break;
                        case 0x15:
                            cpu.DT = cpu.VRegisters[GetParameter(0)!.Value];
                            break;
                        case 0x18:
                            cpu.ST = cpu.VRegisters[GetParameter(0)!.Value];
                            break;
                        case 0x29:
                            // Set I to location of font sprite for digit VX
                            var digit = cpu.VRegisters[GetParameter(0)!.Value];
                            cpu.I = (ushort)FontData.GetFontAddress(digit);
                            break;
                        case 0x33:
                            var vxValue = cpu.VRegisters[GetParameter(0)!.Value];
                            var hundredDigit = vxValue / 100;
                            var tensDigit = (vxValue / 10) % 10;
                            var onesDigit = vxValue % 10;
                            cpu.SystemMemory.SetByteAtAddress(cpu.I, (byte)hundredDigit);
                            cpu.SystemMemory.SetByteAtAddress(cpu.I + 1, (byte)tensDigit);
                            cpu.SystemMemory.SetByteAtAddress(cpu.I + 2, (byte)onesDigit);
                            break;
                        case 0x55:
                            var address = cpu.I;
                            for (var i = 0; i <= GetParameter(0)!.Value; i++)
                            {
                                cpu.SystemMemory.SetByteAtAddress(address,cpu.VRegisters[i]);
                                address++;
                            }
                            break;
                        case 0x65:
                            var addressToRead = cpu.I;
                            for (var i = 0; i <= GetParameter(0)!.Value; i++)
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
    public class ADDInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.ADD, originalOp, parameter1, parameter2)
    {
        private readonly byte _upperNib = (byte)((originalOp >> 12) & 0x000F);

        public ADDInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            switch (_upperNib)
            {
                case 0x7:
                    GetParameter(0)!.RegisterType = RegisterTypes.V;
                    break;
                case 0x8:
                    GetParameter(1)!.RegisterType = RegisterTypes.V;
                    break;
                case 0xF:
                    GetParameter(0)!.RegisterType = RegisterTypes.I;
                    break;
            }
        }

        public override string ToString() => _upperNib switch
        {
            0x7 => $"ADD V{GetParameter(0)!.Value:x1}, {GetParameter(1)!.Value:x2}",
            0x8 => $"ADD V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}",
            0xF => $"ADD I,V{GetParameter(0)!.Value:x1}",
            _ => string.Empty
        };

        public override void Execute(CPU cpu)
        {
            switch (_upperNib)
            {
                case 0x7:
                    cpu.VRegisters[GetParameter(0)!.Value] += (byte) GetParameter(1)!.Value;
                    break;
                case 0x8:
                    ushort value = (ushort)(cpu.VRegisters[GetParameter(0)!.Value] + cpu.VRegisters[GetParameter(1)!.Value]);
                    if (value > 0x255)
                        cpu.VRegisters[0xF] = 1;
                    cpu.VRegisters[GetParameter(0)!.Value] = (byte) value;
                    break;
                case 0xF:
                    cpu.I = (byte)(cpu.I + cpu.VRegisters[GetParameter(0)!.Value]);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Set Vx = Vx OR Vy.
    /// </summary>
    public class ORInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.OR, originalOp, parameter1, parameter2)
    {
        public ORInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"OR V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            cpu.VRegisters[GetParameter(0)!.Value] = (byte)(cpu.VRegisters[GetParameter(0)!.Value] | cpu.VRegisters[GetParameter(1)!.Value]);
        }
    }

    public class ANDInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.AND, originalOp, parameter1, parameter2)
    {
        public ANDInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"AND V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            cpu.VRegisters[GetParameter(0)!.Value] = (byte)(cpu.VRegisters[GetParameter(0)!.Value] & cpu.VRegisters[GetParameter(1)!.Value]);
        }
    }

    /// <summary>
    /// Set Vx = Vx XOR Vy.
    /// </summary>
    public class XORInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.XOR, originalOp, parameter1, parameter2)
    {
        public XORInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"XOR V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            cpu.VRegisters[GetParameter(0)!.Value] = (byte)(cpu.VRegisters[GetParameter(0)!.Value] ^ cpu.VRegisters[GetParameter(1)!.Value]);
        }
    }

    /// <summary>
    /// Set Vx = Vx - Vy, set VF = NOT borrow.
    /// </summary>
    public class SUBInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SUB, originalOp, parameter1, parameter2)
    {
        public SUBInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"SUB V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            if (cpu.VRegisters[GetParameter(0)!.Value] > cpu.VRegisters[GetParameter(1)!.Value])
                cpu.VRegisters[0xF] = 1;
            cpu.VRegisters[GetParameter(0)!.Value] = (byte)(cpu.VRegisters[GetParameter(0)!.Value] - cpu.VRegisters[GetParameter(1)!.Value]);
        }
    }

    /// <summary>
    /// Set Vx = Vx SHR 1.
    /// </summary>
    public class SHRInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SHR, originalOp, parameter1, parameter2)
    {
        public SHRInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"SHR V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            if ((cpu.VRegisters[GetParameter(0)!.Value] & 1) == 1)
            {
                cpu.VRegisters[0xF] = 1;
            }
            else
            {
                cpu.VRegisters[0xF] = 0;
            }
            cpu.VRegisters[GetParameter(0)!.Value] = (byte)(cpu.VRegisters[GetParameter(0)!.Value] >> 1);
        }
    }

    /// <summary>
    /// Set Vx = Vy - Vx, set VF = NOT borrow.
    /// </summary>
    public class SUBNInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SUBN, originalOp, parameter1, parameter2)
    {
        public SUBNInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"SUBN V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            if (cpu.VRegisters[GetParameter(1)!.Value] > cpu.VRegisters[GetParameter(0)!.Value])
            {
                cpu.VRegisters[0xF] = 1;
            }
            else
            {
                cpu.VRegisters[0xF] = 0;
            }
            cpu.VRegisters[GetParameter(0)!.Value] = (byte)(cpu.VRegisters[GetParameter(1)!.Value] - cpu.VRegisters[GetParameter(0)!.Value]);
        }
    }

    /// <summary>
    /// Set Vx = Vx SHL 1.
    /// </summary>
    public class SHLInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SHL, originalOp, parameter1, parameter2)
    {
        public SHLInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"SHL V{GetParameter(0)!.Value:x1},V{GetParameter(1)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            // Check MSB (bit 7) before shift
            if ((cpu.VRegisters[GetParameter(0)!.Value] & 0x80) != 0)
            {
                cpu.VRegisters[0xF] = 1;
            }
            else
            {
                cpu.VRegisters[0xF] = 0;
            }
            cpu.VRegisters[GetParameter(0)!.Value] = (byte)(cpu.VRegisters[GetParameter(0)!.Value] << 1);
        }
    }

    public class RNDInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.RND, originalOp, parameter1, parameter2)
    {
        public RNDInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"RND V{GetParameter(0)!.Value:x1}, {GetParameter(1)!.Value:x2}";

        public override void Execute(CPU cpu)
        {
            var random = (byte)Random.Shared.Next(0, 255);
            cpu.VRegisters[GetParameter(0)!.Value] = (byte) (random & GetParameter(1)!.Value);
        }
    }

    public class DRWInstruction(ushort originalOp, ushort parameter1, ushort parameter2, byte nibble)
        : Instruction<ushort>(OpNames.DRW, originalOp, parameter1, parameter2)
    {
        private readonly byte _nibble = nibble;

        public DRWInstruction(ushort originalOp, ushort parameter1, ushort parameter2, byte nibble, bool init)
            : this(originalOp, parameter1, parameter2, nibble)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
            GetParameter(1)!.RegisterType = RegisterTypes.V;
            Parameters.Add(new Parameter<ushort>() {Value = nibble});
        }

        public override string ToString() => $"DRW V{GetParameter(0)!.Value:x1}, V{GetParameter(1)!.Value:x1}, {_nibble:x1}";

        public override void Execute(CPU cpu)
        {
            // Get coordinates from registers
            int x = cpu.VRegisters[GetParameter(0)!.Value];
            int y = cpu.VRegisters[GetParameter(1)!.Value];

            // Read sprite data from memory starting at address I
            byte[] spriteData = new byte[_nibble];
            for (int i = 0; i < _nibble; i++)
            {
                spriteData[i] = cpu.SystemMemory.ReadByteAtAddress(cpu.I + i);
            }

            // Draw sprite and set VF to collision flag
            bool collision = cpu.Display.DrawSprite(x, y, spriteData);
            cpu.VRegisters[0xF] = collision ? (byte)1 : (byte)0;
        }
    }

    public class SKPInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SKP, originalOp, parameter1, parameter2)
    {
        public SKPInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"SKP V{GetParameter(0)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            // Skip next instruction if key stored in VX is pressed
            byte key = cpu.VRegisters[GetParameter(0)!.Value];
            if (cpu.Input.IsKeyPressed(key))
            {
                cpu.PC += 0x2;
            }
        }
    }

    public class SKNPInstruction(ushort originalOp, ushort parameter1, ushort parameter2)
        : Instruction<ushort>(OpNames.SKNP, originalOp, parameter1, parameter2)
    {
        public SKNPInstruction(ushort originalOp, ushort parameter1, ushort parameter2, bool init)
            : this(originalOp, parameter1, parameter2)
        {
            GetParameter(0)!.RegisterType = RegisterTypes.V;
        }

        public override string ToString() => $"SKNP V{GetParameter(0)!.Value:x1}";

        public override void Execute(CPU cpu)
        {
            // Skip next instruction if key stored in VX is NOT pressed
            byte key = cpu.VRegisters[GetParameter(0)!.Value];
            if (!cpu.Input.IsKeyPressed(key))
            {
                cpu.PC += 0x2;
            }
        }
    }
}
