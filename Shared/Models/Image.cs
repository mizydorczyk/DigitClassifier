namespace Shared.Models
{
    public class Image
    {
        private readonly byte[] _pixels;
        private readonly int _width;
        private readonly int _height;

        public int Label { get; private set; }
        public Image(int label, byte[] pixels, int width, int height)
        {
            Label = label;

            _pixels = pixels;
            _width = width;
            _height = height;
        }

        public byte this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0)
                    throw new ArgumentOutOfRangeException("X and Y must be greater than 0.");

                if (x >= _width)
                    throw new ArgumentOutOfRangeException($"X must be lower than {_width}.");

                if (y >= _height)
                    throw new ArgumentOutOfRangeException($"Y must be lower than {_height}.");

                return _pixels[x + _width * y]; 
            }
        }

        public byte[] Pixels
        {
            get
            { 
                return _pixels; 
            } 
        }
    }
}
