using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemMemory;

namespace MyChip8.Interpreter
{
    public class Instructions
    {
        public abstract class Instruction : IInstruction
        {
            public abstract void Execute(CPU cpu, Memory mem);
        }

        public class SYSInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class CLSInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class RETInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class JPInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class CALLInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SEInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SNEInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class LDInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class ADDInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }
        
        public class ORInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class XORInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SUBInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SHRInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SUBNInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SHLInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class RNDInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class DRWInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SKPInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }

        public class SKNPInstruction : Instruction
        {
            public override void Execute(CPU cpu, Memory mem)
            {
                throw new NotImplementedException();
            }
        }
    }
}
