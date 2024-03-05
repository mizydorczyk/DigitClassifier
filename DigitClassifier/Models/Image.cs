using System.Diagnostics;

namespace DigitClassifier.Models
{
    public class Image
    {
        public byte[] Pixels { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Label { get; private set; }
        public ImageCategory Category { get; private set; }

        public Image(int label, ImageCategory category, byte[] pixels, int width, int height)
        {
            Label = label;
            Category = category;
            Width = width;
            Height = height;
            Pixels = pixels;
        }
    }
}
