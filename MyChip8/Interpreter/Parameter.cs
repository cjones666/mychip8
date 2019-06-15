using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemComponents;

namespace MyChip8.Interpreter
{
    public class Parameter<T>
    {
        public T Value;
        public bool IsRegister;
        public RegisterTypes RegisterType;
    }
}
