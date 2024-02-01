using MNISTReader.Extensions;
using Shared.Models;

namespace MNISTReader
{
    public static class ImagesReader
    {
        public static List<Image> Read(string labelsPath, string imagesPath)
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

            using var labelsFileStream = new FileStream(labelsPath, FileMode.Open);
            using var labelsBinaryReader = new BinaryReader(labelsFileStream);

            int magicNumberLabels = labelsBinaryReader.ReadBigEndianInt32();
            int numberOfLabels = labelsBinaryReader.ReadBigEndianInt32();

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

            using var imagesFileStream = new FileStream(imagesPath, FileMode.Open);
            using var imagesBinaryReader = new BinaryReader(imagesFileStream);

            int magicNumberImages = imagesBinaryReader.ReadBigEndianInt32();
            int numberOfImages = imagesBinaryReader.ReadBigEndianInt32();
            int imageWidth = imagesBinaryReader.ReadBigEndianInt32();
            int imageHeight = imagesBinaryReader.ReadBigEndianInt32();

            if (numberOfImages != numberOfLabels)
                throw new ArgumentException("The number of images is different from labels.");

            var images = new List<Image>();

            // parse actuall images and labels

            for (int i = 0; i < numberOfImages; i++)
            {
                var label = Convert.ToInt32(labelsBinaryReader.ReadByte());
                var bytes = imagesBinaryReader.ReadBytes(imageWidth * imageHeight);

                images.Add(new Image(label, bytes, imageWidth, imageHeight));
            }

            return images;
        }
    }
}
