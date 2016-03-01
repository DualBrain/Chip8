using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chip8
{
    class Display
    {
        byte[] DisplayMem;
        int width, height;
        internal bool dirty = false;

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public Display()
        {
            SetResolution(64, 32);
        }

        public void SetResolution(int x, int y)
        {
            width = x;
            height = y;
            DisplayMem = new byte[x * y];
        }

        public byte this[int x, int y]
        {
            set
            {
                int x0 = x % width, y0 = y % height;
                DisplayMem[y0 * width + x0] = (byte)(value & 1);

                dirty = true;
            }
            get
            {
                int x0 = x % width, y0 = y % height;
                return DisplayMem[y0 * width + x0];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < DisplayMem.Length; i++)
            {
                DisplayMem[i] = 0;
            }
            dirty = true;
        }
    }
}
