using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing.Visualizer
{
    class VForm : Form
    {
        internal VForm()
        {
            
        }
    }

    internal class Visualizer
    {
        private Size Resolution;
        internal PictureBox VisualizerPictureBox;
        internal Bitmap BitmapBuffer;
        
        public VForm VisualizerForm;
        internal Visualizer(Size Resolution)
        {
            this.Resolution = Resolution;

            VisualizerForm = new VForm();
            
            VisualizerPictureBox = new PictureBox();
            VisualizerForm.Size = Resolution;
            VisualizerForm.Text = "Realtime viewport";

            VisualizerPictureBox = new PictureBox();
            VisualizerPictureBox.Location = new Point(0, 0);
            VisualizerPictureBox.Size = Resolution;
            VisualizerPictureBox.Image = BitmapBuffer;


            VisualizerForm.Controls.Add(VisualizerPictureBox);
            
        }

        internal void ShowVisualizerWindow()
        {
            
            VisualizerForm.ShowDialog();
        }

        internal void UpdatePicture()
        {
            lock (VisualizerPictureBox)
            {
                VisualizerForm.Invoke(() =>
                {
                        VisualizerPictureBox.Image = BitmapBuffer;
                        VisualizerPictureBox.Refresh();
                });
            }
            
        }

        internal void TransmitData(Bitmap bitmap)
        {
            BitmapBuffer = bitmap;
        }
    }
}
