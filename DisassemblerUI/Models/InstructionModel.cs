using System.ComponentModel;
using System.Runtime.CompilerServices;
using MyChip8.Interpreter;

namespace DisassemblerUI.Models;

public class InstructionModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Display properties
    public string AddressDisplay { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;

    private string _parameter1 = string.Empty;
    public string Parameter1
    {
        get => _parameter1;
        set
        {
            if (_parameter1 != value)
            {
                _parameter1 = value;
                OnPropertyChanged();
            }
        }
    }

    private string _parameter2 = string.Empty;
    public string Parameter2
    {
        get => _parameter2;
        set
        {
            if (_parameter2 != value)
            {
                _parameter2 = value;
                OnPropertyChanged();
            }
        }
    }

    private string _parameter3 = string.Empty;
    public string Parameter3
    {
        get => _parameter3;
        set
        {
            if (_parameter3 != value)
            {
                _parameter3 = value;
                OnPropertyChanged();
            }
        }
    }

    // Internal storage for non-hex display
    public int Address { get; set; }
    private Parameter<ushort>?[] _parameters = new Parameter<ushort>?[3];

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

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
