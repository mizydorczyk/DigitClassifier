using Microsoft.UI.Xaml.Controls;
using NeuralNetwork.Interfaces;

namespace DigitClassifier.Views
{
    public sealed partial class CreateNetworkDialogPage : Page
    {
        public event EventHandler<string> LayersTextChanged;

        public CreateNetworkDialogPage()
        {
            InitializeComponent();

            var activationFunctions = (ActivationFunctionType[])Enum.GetValues(typeof(ActivationFunctionType));
            ActivationFunction.ItemsSource = activationFunctions;

            Layers.TextChanged += Layers_TextChanged;
        }

        private void Layers_TextChanged(object sender, TextChangedEventArgs e)
        {
            LayersTextChanged?.Invoke(this, Layers.Text);
        }

        public bool IsValid()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Layers.Text) || Layers.Text.Trim().Split(',').Select(int.Parse).Count() <= 1)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public string[] GetLayersInput() => Layers.Text.Trim().Split(',');
        public ActivationFunctionType GetActivationFunctionInput() => (ActivationFunctionType)ActivationFunction.SelectedItem;
    }
}