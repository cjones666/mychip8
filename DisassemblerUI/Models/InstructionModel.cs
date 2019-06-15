using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChip8.Interpreter;

namespace DisassemblerUI.Models
{
    public class InstructionModel
    {
        public int Address;
        public string Instruction;
        public int FirstParameter;
        public int SecondParameter;

        public static InstructionModel GetModel(IInstruction instruction)
        {
            var model = new InstructionModel();
            model.Instruction = instruction.ToString();
        }
    }
}
