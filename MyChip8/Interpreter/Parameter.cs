using MyChip8.SystemComponents;

namespace MyChip8.Interpreter;

public class Parameter<T>
{
    public T? Value;
    public RegisterTypes RegisterType;

    public string GetString(bool useHex) =>
        RegisterType == RegisterTypes.None
            ? Value?.ToString() ?? string.Empty
            : RegisterType is RegisterTypes.I or RegisterTypes.DT
                ? RegisterType.ToString()
                : $"{RegisterType}{Value:X0}";
}
