namespace DigitClassifier.Extensions
{
    public static class FileStreamExtension
    {
        // Int32 (ie. header/metadata of dataset) parsed from MNIST datasets are in high endian format, ie. you need to reverse them if you’re on little endian processor PC
        public static int ReadBigEndianInt32(this FileStream fileStream)
        {
            byte[] bytes = new byte[sizeof(Int32)];
            fileStream.Read(bytes, 0, sizeof(int));

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}