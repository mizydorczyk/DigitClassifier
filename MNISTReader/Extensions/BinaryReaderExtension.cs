namespace MNISTReader.Extensions
{
    public static class BinaryReaderExtension
    {
        // Int32 (ie. header/metadata of dataset) parsed from MNIST datasets are in high endian format, ie. you need to reverse them if you’re on little endian processor PC
        public static int ReadBigEndianInt32(this BinaryReader binaryReader)
        {
            var bytes = binaryReader.ReadBytes(sizeof(Int32));

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
                
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
