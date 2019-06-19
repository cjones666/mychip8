using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemComponents;

namespace MyChip8.Interpreter
{
    public class Parameter<T>
    {
        public T Value;
        public RegisterTypes RegisterType;

        public string GetString(bool useHex)
        {
            if (RegisterType == RegisterTypes.None)
            {
                return Value.ToString();
            }
            else
            {
                if (RegisterType == RegisterTypes.I || 
                    RegisterType == RegisterTypes.DT)
                    return RegisterType.ToString();
                return RegisterType + $"{Value:X0}";
            }
        }
    }
}
