namespace RayTracing.Visualizer;

internal class VForm : Form
{
}

internal class Visualizer
{
    internal Bitmap BitmapBuffer;
    private Size Resolution;

    public VForm VisualizerForm;
    internal PictureBox VisualizerPictureBox;

    internal Visualizer(Size Resolution)
    {
        this.Resolution = Resolution;

        VisualizerForm = new VForm();

        VisualizerPictureBox = new PictureBox();
        VisualizerForm.Size = Resolution;
        VisualizerForm.Text = "Realtime viewport";

        VisualizerPictureBox = new PictureBox
        {
            Location = new Point(0, 0),
            Size = Resolution,
            Image = BitmapBuffer
        };


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