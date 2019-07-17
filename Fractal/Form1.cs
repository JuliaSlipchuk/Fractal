using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            trackBar1.Minimum = 1; trackBar1.Maximum = 15;
            trackBar2.Minimum = 10; trackBar2.Maximum = 255;
        }

        public static Random rand = new Random();
        public static Object obj = new object();

        public static double dynCx;
        public static bool boolCx = false;
        public static double dynCy;
        public static bool boolCy;
        public static int dynIteration;
        public static bool boolIter;

        Fractal fractCx; Fractal fractCy; Fractal fractIter; Fractal fractCxCyIter;
        private void button1_Click(object sender, EventArgs e)
        {
            fractCx = new Fractal(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), trackBar1.Value, trackBar2.Value);
            fractCy = new Fractal(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), trackBar1.Value, trackBar2.Value);
            fractIter = new Fractal(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), trackBar1.Value, trackBar2.Value);
            fractCxCyIter = new Fractal(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox2.Text), trackBar1.Value, trackBar2.Value);

            int interval = Convert.ToInt32(textBox7.Text);
            timer1.Interval = interval + rand.Next(0, 9);
            timer1.Tick += TimerCx;
            timer2.Interval = interval + rand.Next(0, 9);
            timer2.Tick += TimerCy;
            timer3.Interval = interval + rand.Next(0, 9);
            timer3.Tick += TimerTickIteration;
            timer4.Interval = interval + rand.Next(0, 9);
            timer4.Tick += TimerCxCyItert;
            timer1.Start(); timer2.Start(); timer3.Start(); timer4.Start();
        }
        
        public void TimerCx(object obj, EventArgs e)
        {
            pictureBox1.Image = fractCx.GetBitmap(fractCx.iter, fractCx.zoom, dynCx, fractCx.cy, pictureBox1.Width, pictureBox1.Height);
            if (boolCx)
            {
                dynCx += rand.Next(0, 1);
                boolCx = false;
            }
            else
            {
                dynCx -= rand.Next(0, 1);
                boolCx = true;
            }
        }

        
        public void TimerCy(object obj, EventArgs e)
        {
            pictureBox2.Image = fractCy.GetBitmap(fractCy.iter, fractCy.zoom, fractCy.cx, dynCy, pictureBox2.Width, pictureBox2.Height);
            if (boolIter)
            {
                dynCy += rand.Next(0, 1);
                boolCy = false;
            }
            else
            {
                dynCy -= rand.Next(0, 1);
                boolCy = true;
            }
        }
        
        private void TimerTickIteration(object sender, EventArgs e)
        {
            pictureBox3.Image = fractIter.GetBitmap(dynIteration, fractIter.zoom, fractIter.cx, fractIter.cy, pictureBox3.Width, pictureBox3.Height);
            if (boolIter)
            {
                dynIteration += rand.Next(10, 255);
                if (dynIteration > 255)
                {
                    dynIteration = 200;
                }
                boolIter = false;
            }
            else
            {
                dynIteration -= rand.Next(10, 255);
                if (dynIteration <= 1)
                {
                    dynIteration = 5;
                }
                boolIter = true;
            }
        }

        private void TimerCxCyItert(object sender, EventArgs e)
        {
            pictureBox4.Image = fractCxCyIter.GetBitmap(dynIteration, fractCxCyIter.zoom, dynCx, dynCy, pictureBox4.Width, pictureBox4.Height);
            lock(obj)
            {
                System.Threading.Thread.Sleep(10);
                dynCx = rand.Next(-1, 1);
                System.Threading.Thread.Sleep(10);
                dynCy = rand.Next(-1, 1);
                System.Threading.Thread.Sleep(10);
                dynIteration = rand.Next(10, 255);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bitmapToFile = new Bitmap(3000, 3000);
                bitmapToFile = fractCxCyIter.GetBitmap(fractCxCyIter.iter, fractCxCyIter.zoom, fractCxCyIter.cx, fractCxCyIter.cy, 3000, 3000);
                bitmapToFile.Save(dialog.FileName, ImageFormat.Png);
            }
        }
    }

    public class Fractal
    {
        public double cx;
        public double cy;
        public int zoom;
        public int iter;
        public Fractal(double cx, double cy, int zoom, int iter)
        {
            this.cx = cx;
            this.cy = cy;
            this.zoom = zoom;
            this.iter = iter;
        }

        public Bitmap GetBitmap(int maxIteration, int zoom, double cx, double cy, int widht, int height)
        {
            double real;
            double image;
            int iteration;
            double xTemp;
            Bitmap bitmap = null;
            Color[] colors = (from c in Enumerable.Range(0, 256) select Color.FromArgb((c >> 5) * 36, (c >> 3 & 7) * 36, (c & 3) * 85)).ToArray();
            bitmap = new Bitmap(widht, height);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    real = 1.5 * (x - bitmap.Width / 2) / (0.5 * zoom * bitmap.Width);
                    image = 1.0 * (y - bitmap.Height / 2) / (0.5 * zoom * bitmap.Height);
                    iteration = 0;
                    while (Math.Pow(real, 2) + Math.Pow(image, 2) < 4 && iteration < maxIteration)
                    {
                        xTemp = Math.Pow(real, 2) - Math.Pow(image, 2);
                        image = 2 * real * image + cy;
                        real = xTemp + cx;
                        iteration = iteration + 1;
                    }
                    bitmap.SetPixel(x, y, colors[maxIteration - iteration]);
                }
            }
            return bitmap;
        }
    }
}
