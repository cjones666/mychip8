using System;
using MyChip8.Interpreter;

namespace DisassemblerUI.Models
{
    public class InstructionModel
    {
        // Display properties
        public string AddressDisplay { get; set; }
        public string Operation { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }

        // Internal storage for non-hex display
        public int Address { get; set; }
        private IParameter[] _parameters;

        public static InstructionModel GetModel(int address, IInstruction<ushort> instruction)
        {
            var model = new InstructionModel
            {
                Address = address,
                AddressDisplay = $"0x{address:X3}",
                Operation = instruction.Name,
                _parameters = new IParameter[3]
            };

            // Store parameters for later toggling
            for (int i = 0; i < 3; i++)
            {
                model._parameters[i] = instruction.GetParameter(i);
            }

            // Initial display with decimal format
            model.UpdateParameterDisplay(false);

            return model;
        }

        /// <summary>
        /// Updates parameter display between hex and decimal formats.
        /// </summary>
        public void UpdateParameterDisplay(bool useHex)
        {
            Parameter1 = _parameters[0] != null ? _parameters[0].GetString(useHex) : "";
            Parameter2 = _parameters[1] != null ? _parameters[1].GetString(useHex) : "";
            Parameter3 = _parameters[2] != null ? _parameters[2].GetString(useHex) : "";
        }
    }
}
