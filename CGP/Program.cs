using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using Console = Colorful.Console;

namespace CGP
{
    class Program
    {
        // Settings
        static string IMAGE_PATH = "donut.gif";
        static Size IMAGE_SIZE = new Size(32, 32);

        static void Main(string[] args)
        {
            // Init
            Console.SetWindowSize(IMAGE_SIZE.Width, IMAGE_SIZE.Height);
            Console.SetBufferSize(IMAGE_SIZE.Width, IMAGE_SIZE.Height);
            Console.CursorVisible = false;
            Console.BackgroundColor = Color.Black;

            // Loading/resize/convert gif
            Bitmap[] frames = ConvertTo8bpp(Resize(GetFrames(Image.FromFile(IMAGE_PATH)),IMAGE_SIZE));

            while (true)
            {
                foreach (Bitmap frame in frames)
                {
                    Console.Clear();
                    DrawImage(frame);
                    Thread.Sleep(1000);
                } 
            }
        }

        // Convert image to 8bit - 16 colors
        private static Bitmap[] ConvertTo8bpp(Bitmap[] frames)
        {
            Bitmap[] bitmaps = new Bitmap[frames.Length];
            var imageCodecInfo = GetEncoderInfo("image/tiff");
            var encoder = Encoder.ColorDepth;
            var encoderParameters = new EncoderParameters(1);
            var encoderParameter = new EncoderParameter(encoder, 8L);
            encoderParameters.Param[0] = encoderParameter;
            var memoryStream = new MemoryStream();

            for(int i =0;i< frames.Length;i++)
            {
                frames[i].Save(memoryStream, imageCodecInfo, encoderParameters);
                bitmaps[i] = (Bitmap)Image.FromStream(memoryStream);
            }
            return frames;
        }

        // Get encoder info
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            return imageEncoders.FirstOrDefault(t => t.MimeType == mimeType);
        }

        // Resize 
        static Bitmap[] Resize(Bitmap[] frames,Size size)
        {
            Bitmap[] resized = new Bitmap[frames.Length];
            for(int i =0;i<frames.Length;i++)
            {
                resized[i] = new Bitmap(frames[i], size);
            }
            return resized;
        }

        // Draw Frame
        static void DrawImage(Bitmap bmp)
        {
            for(int y = 0; y< bmp.Height;y++)
            {
                for(int x = 0; x < bmp.Width;x++)
                {
                    Color color = bmp.GetPixel(x, y);
                   
                    Console.Write('█',color);
                }
                Console.Write("\n");
            }
        }

        // Get frames for gif
        static Bitmap[] GetFrames(Image gif)
        {
            int numberOfFrames = gif.GetFrameCount(FrameDimension.Time);
            Bitmap[] frames = new Bitmap[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                gif.SelectActiveFrame(FrameDimension.Time, i);
                frames[i] = ((Bitmap)gif.Clone());
            }
            return frames;
        }
    }
}
