using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace DirectoryColorizer.Wpf;

static class BitmapExtension
{
    public static BitmapImage ToBitmapImage(this Bitmap value)
    {
        MemoryStream ms = new MemoryStream();
        value.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        BitmapImage image = new BitmapImage();
        image.BeginInit();
        ms.Seek(0, SeekOrigin.Begin);
        image.StreamSource = ms;
        image.EndInit();
        return image;
    }
}