using DigitClassifier.Extensions;
using DigitClassifier.Models;

namespace DigitClassifier.Helpers
{
    public class ImagesReader : IDisposable
    {
        private readonly FileStream _labelsFileStream;
        private readonly FileStream _imagesFileStream;

        public ImagesReader(string labelsPath, string imagesPath)
        {
            // check if path exists
            if (!Path.Exists(labelsPath))
                throw new ArgumentException("Labels path is not valid");

            if (!Path.Exists(imagesPath))
                throw new ArgumentException("Images path is not valid");

            _labelsFileStream = new FileStream(labelsPath, FileMode.Open);
            _imagesFileStream = new FileStream(imagesPath, FileMode.Open);
        }
        public List<Image> Read(ImageCategory category = default)
        {
            // open FileStream, BinaryReader for labels and get the header

            // [offset] [type]          [value]          [description]
            // 0000     32 bit integer  0x00000801(2049) magic number(MSB first)
            // 0004     32 bit integer  60000            number of items
            // 0008     unsigned byte   ?? label
            // 0009     unsigned byte   ?? label
            // ........
            // xxxx     unsigned byte   ?? label
            // The labels values are 0 to 9.

            int magicNumberLabels = _labelsFileStream.ReadBigEndianInt32();
            int numberOfLabels = _labelsFileStream.ReadBigEndianInt32();

            // open FileStream, BinaryReader for images and get the header

            // [offset] [type]          [value]          [description]
            // 0000     32 bit integer  0x00000803(2051) magic number
            // 0004     32 bit integer  60000            number of images
            // 0008     32 bit integer  28               number of rows
            // 0012     32 bit integer  28               number of columns
            // 0016     unsigned byte   ?? pixel
            // 0017     unsigned byte   ?? pixel
            // ........
            // xxxx     unsigned byte   ?? pixel
            // Pixels are organized row-wise. Pixel values are 0 to 255. 0 means background(white), 255 means foreground(black). 

            int magicNumberImages = _imagesFileStream.ReadBigEndianInt32();
            int numberOfImages = _imagesFileStream.ReadBigEndianInt32();
            int imageWidth = _imagesFileStream.ReadBigEndianInt32();
            int imageHeight = _imagesFileStream.ReadBigEndianInt32();

            if (numberOfImages != numberOfLabels)
                throw new ArgumentException("The number of images is different from labels.");

            var images = new List<Image>();

            // parse actuall images and labels
            for (int i = 0; i < numberOfImages; i++)
            {
                var label = Convert.ToInt32(_labelsFileStream.ReadByte());
                var bytes = new byte[imageWidth * imageHeight];
                _imagesFileStream.Read(bytes, 0, imageWidth * imageHeight);

                var image = new Image(label, category, bytes, imageWidth, imageHeight);
                images.Add(image);
            }

            return images;
        }

        public void Dispose()
        {
            _labelsFileStream.Dispose();
            _imagesFileStream.Dispose();
        }
    }
}