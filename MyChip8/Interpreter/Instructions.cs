using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemComponents;

namespace MyChip8.Interpreter
{
    public class Instructions
    {
        public abstract class Instruction : IInstruction
        {
            public abstract void Execute(CPU cpu);
        }

        public class SYSInstruction : Instruction
        {
            private ushort _address;

            public SYSInstruction(ushort address)
            {
                _address = address;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class CLSInstruction : Instruction
        {
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class RETInstruction : Instruction
        {
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// Jump to location nnn.
        /// </summary>
        public class JPInstruction : Instruction
        {
            private ushort _address;

            public JPInstruction(ushort address)
            {
                _address = address;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Call subroutine at nnn.
        /// </summary>
        public class CALLInstruction : Instruction
        {
            private ushort _address;

            public CALLInstruction(ushort address)
            {
                _address = address;
            }
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Skip next instruction if Vx = kk.
        /// </summary>
        public class SEInstruction : Instruction
        {

            private byte _VRegister;
            private byte _value;

            public SEInstruction(byte vRegister, byte value)
            {
                _VRegister = vRegister;
                _value = value;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Skip next instruction if Vx != kk.
        /// </summary>
        public class SNEInstruction : Instruction
        {
            private byte _VRegister;
            private byte _value;

            public SNEInstruction(byte vRegister, byte value)
            {
                _VRegister = vRegister;
                _value = value;
            }
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Set Vx = kk.
        /// </summary>
        public class LDInstruction : Instruction
        {
            protected byte _VRegister;
            private byte _value;

            public LDInstruction(byte vRegister, byte value)
            {
                _VRegister = vRegister;
                _value = value;
            }

            protected LDInstruction()
            {
                throw new NotImplementedException();
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Set Vx = Vx + kk.
        /// </summary>
        public class ADDInstruction : Instruction
        {
            protected byte _VRegister;
            private byte _value;

            public ADDInstruction(byte vRegister, byte value)
            {
                _VRegister = vRegister;
                _value = value;
            }

            protected ADDInstruction()
            {
                throw new NotImplementedException();
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Set Vx = Vy.
        /// </summary>
        public class LDRegisterInstruction : LDInstruction
        {
            private byte _VRegister2;

            public LDRegisterInstruction(byte vRegister, byte vRegister2) : base()
            {
                _VRegister = vRegister;
                _VRegister2 = vRegister2;
            }
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Set Vx = Vx OR Vy.
        /// </summary>
        public class ORInstruction : Instruction
        {
            public byte _vRegister;
            public byte _vRegister2;

            public ORInstruction(byte vRegister, byte vRegister2)
            {
                _vRegister2 = vRegister2;
                _vRegister = vRegister;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class ANDInstruction : Instruction
        {
            public byte _vRegister;
            public byte _vRegister2;

            public ANDInstruction(byte vRegister, byte vRegister2)
            {
                _vRegister = vRegister;
                _vRegister2 = vRegister2;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Set Vx = Vx XOR Vy.
        /// </summary>
        public class XORInstruction : Instruction
        {
            public byte _vRegister;
            public byte _vRegister2;

            public XORInstruction(byte vRegister, byte vRegister2)
            {
                _vRegister = vRegister;
                _vRegister2 = vRegister2;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Set Vx = Vx + Vy, set VF = carry.
        /// </summary>
        public class ADDRegisterInstruction : ADDInstruction
        {
            private byte _vRegister2;

            public ADDRegisterInstruction(byte vRegister,byte register2) : base()
            {
                _VRegister = vRegister;
                _vRegister2 = register2;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Set Vx = Vx - Vy, set VF = NOT borrow.
        /// </summary>
        public class SUBInstruction : Instruction
        {
            public byte _vRegister;
            public byte _vRegister2;

            public SUBInstruction(byte vRegister, byte vRegister2)
            {
                _vRegister = vRegister;
                _vRegister2 = vRegister2;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Set Vx = Vx SHR 1.
        /// </summary>
        public class SHRInstruction : Instruction
        {
            public byte _vRegister;

            public SHRInstruction(byte vRegister)
            {
                _vRegister = vRegister;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Set Vx = Vy - Vx, set VF = NOT borrow.
        /// </summary>
        public class SUBNInstruction : Instruction
        {
            public byte _vRegister;
            public byte _vRegister2;

            public SUBNInstruction(byte vRegister, byte vRegister2)
            {
                _vRegister = vRegister;
                _vRegister2 = vRegister2;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Set Vx = Vx SHL 1.
        /// </summary>
        public class SHLInstruction : Instruction
        {
            public byte _vRegister;

            public SHLInstruction(byte vRegister)
            {
                _vRegister = vRegister;
            }

            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class RNDInstruction : Instruction
        {
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class DRWInstruction : Instruction
        {
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class SKPInstruction : Instruction
        {
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }

        public class SKNPInstruction : Instruction
        {
            public override void Execute(CPU cpu)
            {
                throw new NotImplementedException();
            }
        }
    }
}
