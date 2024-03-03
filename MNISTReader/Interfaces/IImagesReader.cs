namespace MNISTReader.Interfaces
{
    public interface IImagesReader
    {
        public IList<Image> Read(Category category, string labelsPath, string imagesPath);
    }
}
