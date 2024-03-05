using Microsoft.UI.Xaml.Data;

namespace DigitClassifier.Converters
{
    public class NullToAllConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "All";
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
