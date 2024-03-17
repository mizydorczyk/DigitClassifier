using DigitClassifier.Models;
using Microsoft.UI.Xaml.Data;

namespace DigitClassifier.Converters
{
    public class CalculatedResultsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Dictionary<int, string> label = new Dictionary<int, string>()
            {
                { 0, "Zero" },
                { 1, "One" },
                { 2, "Two" },
                { 3, "Three" },
                { 4, "Four" },
                { 5, "Five" },
                { 6, "Six" },
                { 7, "Seven" },
                { 8, "Eight" },
                { 9, "Nine" }
            };

            var calculatedResult = (CalculatedResult)value;
            return $"{label[calculatedResult.Label]}\t{calculatedResult.Result:F}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}