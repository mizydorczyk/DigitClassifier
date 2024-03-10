using Microsoft.UI.Xaml.Data;
using NeuralNetwork;
using System.Text;

namespace DigitClassifier.Converters
{
    public class LayerToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var layers = (Layer[])value;

            var result = new StringBuilder();
            result.Append("Layers: ");
            result.Append($"{layers.First().NumberOfNodesIn}, ");
            result.Append(string.Join(", ", layers.Select(x => x.NumberOfNodesOut).ToList()));
            result.AppendLine();
            result.Append($"Activation function: {layers.First().ActivationFunctionType}");

            return result.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}