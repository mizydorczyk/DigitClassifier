using NeuralNetwork.Interfaces;

namespace NeuralNetwork
{
    public class Network
    {
        private readonly Layer[] _layers;
        public Network(int[] layerSizes, IActivationFunction activationFunction)
        {
            _layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], activationFunction);
            }
        }

        public double[] Calculate(double[] inputs)
        {
            // calculate each consecutively
            for (int i = 0; i < _layers.Length; i++)
            {
                inputs = _layers[i].Calculate(inputs);
            }

            return inputs;
        }
    }
}
