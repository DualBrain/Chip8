using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8
{
    class CPU
    {
        private byte[] Memory = new byte[4 * 1024];
        private byte[] Registers = new byte[16];
        private byte[] Timers = new byte[2];    //[0] = Delay Timer, [1] = Sound Timer
        private ushort PC = 0;
        private ushort I = 0;
        private Stack<ushort> Stack = new Stack<ushort>();
        internal Display Display;
        internal KeyPad KeyPad;

        public CPU()
        {
            Display = new Display();
            KeyPad = new KeyPad();
        }

        public void InitMemory()
        {
            //Load in the font
            int i = 0;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x90;
            Memory[i++] = 0x90;
            Memory[i++] = 0x90;
            Memory[i++] = 0xF0;

            Memory[i++] = 0x20;
            Memory[i++] = 0x60;
            Memory[i++] = 0x20;
            Memory[i++] = 0x20;
            Memory[i++] = 0x70;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x10;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0xF0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x10;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x10;
            Memory[i++] = 0xF0;

            Memory[i++] = 0x90;
            Memory[i++] = 0x90;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x10;
            Memory[i++] = 0x10;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x10;
            Memory[i++] = 0xF0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x90;
            Memory[i++] = 0xF0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x10;
            Memory[i++] = 0x20;
            Memory[i++] = 0x40;
            Memory[i++] = 0x40;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x90;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x90;
            Memory[i++] = 0xF0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x90;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x10;
            Memory[i++] = 0xF0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x90;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x90;
            Memory[i++] = 0x90;

            Memory[i++] = 0xE0;
            Memory[i++] = 0x90;
            Memory[i++] = 0xE0;
            Memory[i++] = 0x90;
            Memory[i++] = 0xE0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0x80;
            Memory[i++] = 0x80;
            Memory[i++] = 0xF0;

            Memory[i++] = 0xE0;
            Memory[i++] = 0x90;
            Memory[i++] = 0x90;
            Memory[i++] = 0x90;
            Memory[i++] = 0xE0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0xF0;

            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0xF0;
            Memory[i++] = 0x80;
            Memory[i++] = 0x80;
        }

        public void LoadProgram(byte[] data)
        {
            Array.Copy(data, 0, Memory, 0x200, data.Length);
            PC = 0x200;
        }

        public void Start(ushort pc)
        {
            PC = pc;
            while (PC != 0)
            {
                RunCycle();
            }
        }

        public void RunCycle()
        {
            if (KeyPad.waiting) PC -= 2;
            ushort instruction = (ushort)(Memory[PC++] << 8);
            instruction |= Memory[PC++];

            InterpretOpCode(instruction);
        }

        public void UpdateTimers()
        {
            if (Timers[0] != 0) Timers[0]--;
            if (Timers[1] != 0) Timers[1]--;
        }

        public void InterpretOpCode(ushort opcode)
        {
            switch (opcode >> 12)
            {
                case 0:
                    switch ((opcode >> 8) & 0xF)
                    {
                        case 0:
                            switch (opcode & 0xFF)
                            {
                                case 0xE0:
                                    Display.Clear();
                                    break;
                                case 0xEE:
                                    PC = Stack.Pop();
                                    break;
                            }
                            break;
                    }
                    break;
                case 1:
                    PC = (ushort)(opcode & 0xFFF);
                    break;
                case 2:
                    Stack.Push(PC);
                    PC = (ushort)(opcode & 0xFFF);
                    break;
                case 3:
                    if (Registers[(opcode >> 8) & 0xF] == (opcode & 0xFF))
                        PC += 2;
                    break;
                case 4:
                    if (Registers[(opcode >> 8) & 0xF] != (opcode & 0xFF))
                        PC += 2;
                    break;
                case 5:
                    switch (opcode & 0xF)
                    {
                        case 0:
                            if (Registers[(opcode >> 8) & 0xF] == Registers[((opcode >> 4) & 0xF)])
                                PC += 2;
                            break;
                    }
                    break;
                case 6:
                    Registers[(opcode >> 8) & 0xF] = (byte)(opcode & 0xFF);
                    break;
                case 7:
                    Registers[(opcode >> 8) & 0xF] += (byte)(opcode & 0xFF);
                    break;
                case 8:
                    switch (opcode & 0xF)
                    {
                        case 0:
                            Registers[(opcode >> 8) & 0xF] = Registers[((opcode >> 4) & 0xF)];
                            break;
                        case 1:
                            Registers[(opcode >> 8) & 0xF] |= Registers[((opcode >> 4) & 0xF)];
                            break;
                        case 2:
                            Registers[(opcode >> 8) & 0xF] &= Registers[((opcode >> 4) & 0xF)];
                            break;
                        case 3:
                            Registers[(opcode >> 8) & 0xF] ^= Registers[((opcode >> 4) & 0xF)];
                            break;
                        case 4:
                            {
                                var sum = Registers[(opcode >> 8) & 0xF] + Registers[((opcode >> 4) & 0xF)];
                                if (sum > 255) Registers[0xF] = 1;
                                Registers[(opcode >> 8) & 0xF] = (byte)sum;
                            }
                            break;
                        case 5:
                            {
                                var diff = Registers[(opcode >> 8) & 0xF] - Registers[((opcode >> 4) & 0xF)];
                                Registers[0xF] = (byte)((Registers[(opcode >> 8) & 0xF] > Registers[((opcode >> 4) & 0xF)]) ? 1 : 0);
                                Registers[(opcode >> 8) & 0xF] = (byte)diff;
                            }
                            break;
                        case 6:
                            Registers[0xF] = (byte)(Registers[(opcode >> 8) & 0xF] & 1);
                            Registers[(opcode >> 8) & 0xF] >>= 1;
                            break;
                        case 7:
                            {
                                var diff = Registers[(opcode >> 4) & 0xF] - Registers[((opcode >> 8) & 0xF)];
                                Registers[0xF] = (byte)((Registers[(opcode >> 4) & 0xF] > Registers[((opcode >> 8) & 0xF)]) ? 1 : 0);
                                Registers[(opcode >> 8) & 0xF] = (byte)diff;
                            }
                            break;
                        case 0xE:
                            Registers[0xF] = (byte)(Registers[(opcode >> 8) & 0xF] & (1 << 7));
                            Registers[(opcode >> 8) & 0xF] <<= 1;
                            break;
                    }
                    break;
                case 9:
                    switch (opcode & 0xF)
                    {
                        case 0:
                            if (Registers[(opcode >> 8) & 0xF] != Registers[((opcode >> 4) & 0xF)])
                                PC += 2;
                            break;
                    }
                    break;
                case 0xA:
                    I = (ushort)(opcode & 0xFFF);
                    break;
                case 0xB:
                    PC = (ushort)(Registers[0] + (opcode & 12));
                    break;
                case 0xC:
                    Registers[(opcode >> 8) & 0xF] = (byte)(new Random().Next(256) & (opcode & 0xFF));
                    break;
                case 0xD:
                    {
                        bool col = false;
                        int y = Registers[(opcode >> 4) & 0xF];
                        for (int i = 0; i < (opcode & 0xF); i++)
                        {
                            int x = Registers[(opcode >> 8) & 0xF];
                            for (int i0 = 7; i0 >= 0; i0--)
                            {
                                bool v = Display[x, y] > 0;

                                if ((Memory[I + i] & (1 << i0)) != 0) Display[x, y] ^= 1;

                                if (v && Display[x, y] == 0) col = true;
                                x++;
                            }
                            y++;
                        }
                        Registers[0xF] = (byte)(col ? 1 : 0);
                    }
                    //Draw (opcode & 0xF)-byte sprite starting at location I at (opcode >> 8 & 0xF),(opcode >> 4 & 0xF)
                    //XOR pixels onto screen, if pixels are erased, set VF = 1 else set VF = 0
                    //Wraparound if out of bounds
                    break;
                case 0xE:
                    switch (opcode & 0xFF)
                    {
                        case 0x9E:
                            if (KeyPad.IsDown(Registers[(opcode >> 8) & 0xF])) PC += 2;
                            break;
                        case 0xA1:
                            if (!KeyPad.IsDown(Registers[(opcode >> 8) & 0xF])) PC += 2;
                            break;
                    }
                    break;
                case 0xF:
                    switch (opcode & 0xFF)
                    {
                        case 0x07:
                            Registers[(opcode >> 8) & 0xF] = Timers[0];
                            break;
                        case 0x0A:
                            Registers[(opcode >> 8) & 0xF] = KeyPad.WaitForKey();
                            break;
                        case 0x15:
                            Timers[0] = Registers[(opcode >> 8) & 0xF];
                            break;
                        case 0x18:
                            Timers[1] = Registers[(opcode >> 8) & 0xF];
                            break;
                        case 0x1E:
                            I += Registers[(opcode >> 8) & 0xF];
                            break;
                        case 0x29:
                            I = (ushort)(Registers[(opcode >> 8) & 0xF] * 5);
                            break;
                        case 0x33:
                            Memory[I] = (byte)(Registers[(opcode >> 8) & 0xF] / 100);
                            Memory[I + 1] = (byte)((Registers[(opcode >> 8) & 0xF] % 100) / 10);
                            Memory[I + 2] = (byte)(Registers[(opcode >> 8) & 0xF] % 10);
                            break;
                        case 0x55:
                            for (int i = 0; i <= ((opcode >> 8) & 0xF); i++)
                            {
                                Memory[I + i] = Registers[i];
                            }
                            break;
                        case 0x65:
                            for (int i = 0; i <= ((opcode >> 8) & 0xF); i++)
                            {
                                Registers[i] = Memory[I + i];
                            }
                            break;
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
