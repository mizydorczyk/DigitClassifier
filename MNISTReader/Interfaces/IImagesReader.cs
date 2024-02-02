using Shared.Models;

namespace MNISTReader.Interfaces
{
    public interface IImagesReader
    {
        public List<Image> Read(string labelsPath, string imagesPath);
    }
}
