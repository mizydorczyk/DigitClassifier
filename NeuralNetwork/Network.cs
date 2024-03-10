using NeuralNetwork.Interfaces;

namespace NeuralNetwork
{
    public class Network
    {
        public string Name { get; set; }
        public Layer[] Layers { get; private set; }

        public Network(Layer[] layers)
        {
            Layers = layers;
        }

        public Network(int[] layerSizes, ActivationFunctionType activationFunctionType)
        {
            Layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], activationFunctionType);
            }
        }

        public double[] Calculate(double[] inputs)
        {
            // calculate each consecutively
            for (int i = 0; i < Layers.Length; i++)
            {
                inputs = Layers[i].Calculate(inputs);
            }

            return inputs;
        }
    }
}