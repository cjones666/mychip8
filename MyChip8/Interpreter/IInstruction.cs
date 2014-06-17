using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MyChip8.SystemMemory;

namespace MyChip8.Interpreter
{
    public interface IInstruction
    {
        void Execute(CPU cpu, Memory mem);
    }
}
