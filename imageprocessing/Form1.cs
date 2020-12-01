using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace imageprocessing
{
    public partial class Form1 : Form
    {
        private List<Bitmap> _bitmaps = new List<Bitmap>();
        private Random _random = new Random();
        public Form1()
        {
            InitializeComponent();
        }

              
        
        private async void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                var sw = Stopwatch.StartNew();
                menuStrip2.Enabled = trackBar2.Enabled = false;
                pictureBox2.Image = null;
                _bitmaps.Clear();
                var bitmap = new Bitmap(openFileDialog2.FileName);
                await Task.Run(() => { RunProcessing(bitmap); });
                menuStrip2.Enabled = trackBar2.Enabled = true;
                sw.Stop();
                Text = sw.Elapsed.ToString();
            }
        }

        private void RunProcessing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;
            var currentPixelsSet = new List<Pixel>(pixels.Count - pixelsInStep);
            for (int i = 1; i < trackBar2.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    var index = _random.Next(pixels.Count);
                    currentPixelsSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }
                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);
                foreach (var pixel in currentPixelsSet)
                                currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                _bitmaps.Add(currentBitmap);
                this.Invoke(new Action(()=> 
                {
                    Text = $"{i} %";
                }));
                
            }
            _bitmaps.Add(bitmap);
        }

        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel() 
                    {
                        Color = bitmap.GetPixel(x,y),
                        Point = new Point() {X = x, Y = y}
                    });
                }
            }
            return pixels;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (_bitmaps == null || _bitmaps.Count == 0)
                return;
            pictureBox2.Image = _bitmaps[trackBar2.Value - 1];
        }
    }
}
