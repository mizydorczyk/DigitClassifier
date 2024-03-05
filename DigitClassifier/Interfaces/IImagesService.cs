using DigitClassifier.Models;

namespace DigitClassifier.Interfaces
{
    public interface IImagesService
    {
        Task<List<Image>> GetImagesAsync(bool refresh = false);
    }
}
