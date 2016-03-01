using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chip8
{
    class KeyPad
    {
        bool[] keysDown = new bool[16];
        internal bool waiting = false;
        bool doneWaiting = false;
        byte newKey = 0;

        public bool IsDown(byte key)
        {
            return keysDown[key];
        }

        public byte WaitForKey()
        {
            if (!waiting)
            {
                doneWaiting = false;
                waiting = true;
            }

            if (doneWaiting)
            {
                waiting = false;
            }
            return newKey;
        }

        public void SetKeyDown(byte key)
        {
            keysDown[key] = true;

            if (waiting)
            {
                newKey = key;
                doneWaiting = true;
            }
        }

        public void SetKeyUp(byte key)
        {
            keysDown[key] = false;
        }
    }
}
