using MyChip8.Interpreter;

namespace DisassemblerUI.Models;

public class InstructionModel
{
    // Display properties
    public string AddressDisplay { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Parameter1 { get; set; } = string.Empty;
    public string Parameter2 { get; set; } = string.Empty;
    public string Parameter3 { get; set; } = string.Empty;

    // Internal storage for non-hex display
    public int Address { get; set; }
    private Parameter<ushort>?[] _parameters = new Parameter<ushort>?[3];

    public static InstructionModel GetModel(int address, IInstruction<ushort> instruction)
    {
        var model = new InstructionModel
        {
            Address = address,
            AddressDisplay = $"0x{address:X3}",
            Operation = instruction.Name
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
        Parameter1 = _parameters[0]?.GetString(useHex) ?? string.Empty;
        Parameter2 = _parameters[1]?.GetString(useHex) ?? string.Empty;
        Parameter3 = _parameters[2]?.GetString(useHex) ?? string.Empty;
    }
}
