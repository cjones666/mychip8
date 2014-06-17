using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyChip8.Interpreter
{
    public class InstructionType
    {
        public enum Op
        {
            SYS,
            CLS,
            RET,
            JP,
            CALL,
            SE,
            SNE,
            LD,
            ADD,
            OR,
            XOR,
            SUB,
            SHR,
            SUBN,
            SHL,
            RND,
            DRW,
            SKP,
            SKNP
        }
    }
}
