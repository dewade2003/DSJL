using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace DSJL.Export
{
    class SaveUIElementToImage
    {
        public static void SaveToImage(FrameworkElement ui, string fileName)
        {
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            int imgWidth = int.Parse(ui.Width.ToString()) * 4;
            int imgHeight = int.Parse(ui.Height.ToString()) * 4;
            RenderTargetBitmap bmp = new RenderTargetBitmap(imgWidth, imgHeight, 384, 384, PixelFormats.Pbgra32);
            bmp.Render(ui);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(fs);
            fs.Close();
        }
    }
}
