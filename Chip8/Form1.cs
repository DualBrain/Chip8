using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8
{
    public partial class Form1 : Form
    {
        CPU cpu = new CPU();
        Bitmap bmp;
        Graphics g;

        public Form1()
        {
            InitializeComponent();
            cpu = new CPU();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bmp);

            cpu.InitMemory();
            cpu.LoadProgram(File.ReadAllBytes("minimal.ch8"));

            Thread t = new Thread(() => { while (true) { cpu.RunCycle(); Thread.Sleep(new TimeSpan(1000)); } });
            //t.Start();
        }

        private void UpdateScreen()
        {
            int multX = pictureBox1.Width / cpu.Display.Width;
            int multY = pictureBox1.Height / cpu.Display.Height;


            for (int y = 0; y < cpu.Display.Height; y++)
                for (int x = 0; x < cpu.Display.Width; x++)
                {
                    g.FillRectangle(cpu.Display[x, y] == 1 ? Brushes.White : Brushes.Black, x * multX, y * multY, multX, multY);
                }

            pictureBox1.Image = bmp;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int key = 0;
            switch (e.KeyCode)
            {
                case Keys.D0:
                    key = 0;
                    break;
                case Keys.D1:
                    key = 1;
                    break;
                case Keys.D2:
                    key = 2;
                    break;
                case Keys.D3:
                    key = 3;
                    break;
                case Keys.D4:
                    key = 4;
                    break;
                case Keys.D5:
                    key = 5;
                    break;
                case Keys.D6:
                    key = 6;
                    break;
                case Keys.D7:
                    key = 7;
                    break;
                case Keys.D8:
                    key = 8;
                    break;
                case Keys.D9:
                    key = 9;
                    break;
                case Keys.A:
                    key = 10;
                    break;
                case Keys.B:
                    key = 11;
                    break;
                case Keys.C:
                    key = 12;
                    break;
                case Keys.D:
                    key = 13;
                    break;
                case Keys.E:
                    key = 14;
                    break;
                case Keys.F:
                    key = 15;
                    break;
                default:
                    return;
            }
            cpu.KeyPad.SetKeyDown((byte)key);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            int key = 0;
            switch (e.KeyCode)
            {
                case Keys.D0:
                    key = 0;
                    break;
                case Keys.D1:
                    key = 1;
                    break;
                case Keys.D2:
                    key = 2;
                    break;
                case Keys.D3:
                    key = 3;
                    break;
                case Keys.D4:
                    key = 4;
                    break;
                case Keys.D5:
                    key = 5;
                    break;
                case Keys.D6:
                    key = 6;
                    break;
                case Keys.D7:
                    key = 7;
                    break;
                case Keys.D8:
                    key = 8;
                    break;
                case Keys.D9:
                    key = 9;
                    break;
                case Keys.A:
                    key = 10;
                    break;
                case Keys.B:
                    key = 11;
                    break;
                case Keys.C:
                    key = 12;
                    break;
                case Keys.D:
                    key = 13;
                    break;
                case Keys.E:
                    key = 14;
                    break;
                case Keys.F:
                    key = 15;
                    break;
                default:
                    return;
            }
            cpu.KeyPad.SetKeyUp((byte)key);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 50; i++) cpu.RunCycle();
            cpu.UpdateTimers();
            UpdateScreen();
        }
    }
}
