using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemComponents;

namespace MyChip8.Interpreter
{
    public interface IInstruction<T>
    {
        string Name { get; set; }
        List<Parameter<T>> Parameters { get; set; }

        void Execute(CPU cpu);
        void Finalize(CPU cpu);
        Parameter<T> GetParameter(int index);
    }
}
