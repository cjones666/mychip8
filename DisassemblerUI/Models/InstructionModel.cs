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
        public int Address { get; set; }
        public string Operation { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }


        public static InstructionModel GetModel(int address, IInstruction<ushort> instruction)
        {
            var model = new InstructionModel
            {
                Address = address,
                Operation = instruction.Name
            };

            var parameter1 = instruction.GetParameter(0);
            if (parameter1 != null)
                model.Parameter1 = parameter1.GetString(false); 
            var parameter2 = instruction.GetParameter(1);
            if (parameter2 != null)
                model.Parameter2 = parameter2.GetString(false);
            var parameter3 = instruction.GetParameter(2);
            if (parameter3 != null)
                model.Parameter3 = parameter3.GetString(false);

            return model;
        }
    }
}
