using System;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Threading;
using MyChip8.Interpreter;

namespace MyChip8.SystemComponents
{
    public class CPU
    {
        private Timer _cpuThread;
        private Memory _systemMemory;

        public Memory SystemMemory
        {
            get
            {
                return _systemMemory;
               
            }
            private set { _systemMemory = value; }
        }

        private List<byte> _vRegisters;

        public List<byte> VRegisters
        {
            get
            {
                return _vRegisters;                
            }
            private set { _vRegisters = value; }
        }

        public ushort I { get; set; }

        private List<ushort> _stack;

        public List<ushort> Stack
        {
            get { return _stack; }
            private set { _stack = value; }
        }
        private byte _stackPointer;
        public byte SP
        {
            get
            {
                return _stackPointer;    
            }
            set
            {
                _stackPointer = value;
                _stackPointer = (byte) (_stackPointer < 0 ? 0 : (_stackPointer >= 16 ? 15 : _stackPointer));
            }
        }
        private ushort _programCounter;
        public ushort PC
        {
            get { return _programCounter; }
            set { _programCounter = value; }
        }

        public byte DT { get; set; }
        public byte ST { get; set; }

        private InstructionHandler _instructionHandler;

        public CPU(Memory systemMemory)
        {
            _instructionHandler = new InstructionHandler();
            _systemMemory = systemMemory;
            // Initialize Registers
            _vRegisters = new List<byte>(0xF);
            for (var i = 0; i < 0xF; i++)
            {
                _vRegisters.Add(0x0);
            }

            DT = 0;
            ST = 0;
            I = 0x0000;
            _stack = new List<ushort>(16);
            _stackPointer = 0;
            _programCounter = 0;
        }

        public void Start()
        {
            _cpuThread = new Timer(Update,this,0,1000);
            //_cpuThread = new Thread(Update);
            //_cpuThread.Start();
        }

        public void Update(object sender)
        {
            if (_programCounter == 0)
                return;

            // Fetch instruction
            var instructionBytes = _systemMemory.ReadByteAtAddress(_programCounter);
            var instruction = InstructionHandler.GetInstruction(instructionBytes);
         
            // Do instruction
            instruction.Execute(this);
            instruction.Finalize(this);
            // Update timers / registers / etc.
        }
    }
}
