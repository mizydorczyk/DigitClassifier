using Shared.Interfaces;

namespace Shared.Models
{
    public class Layer
    {
        private readonly int _numberOfNodesIn;
        private readonly int _numberOfNodesOut;
        private readonly IActivationFunction _activationFunction;
        private readonly double[] _weights;
        private readonly double[] _biases;

        public Layer(int numberOfNodesIn, int numberOfNodesOut, IActivationFunction activationFunction)
        {
            _numberOfNodesIn = numberOfNodesIn;
            _numberOfNodesOut = numberOfNodesOut;

            // each neuron has number of nodes in weights and only one bias
            _weights = new double[numberOfNodesIn * numberOfNodesOut];
            _biases = new double[numberOfNodesOut];
            
            _activationFunction = activationFunction;
        }

        public double[] Calculate(double[] inputs)
        {
            // calculate weighted inputs
            double[] weightedInputs = new double[_numberOfNodesOut];

            for (int nodeOut = 0; nodeOut < _numberOfNodesOut; nodeOut++)
            {
                // assign bias to weightedInput as starting point
                double weightedInput = _biases[nodeOut];

                for (int nodeIn = 0; nodeIn < _numberOfNodesIn; nodeIn++)
                {
                    // modify weightedInput by the weight of the incoming neuron multiplied by the input from the previous layer
                    weightedInput += inputs[nodeIn] * _weights[nodeOut * _numberOfNodesIn + nodeIn];
                }
                weightedInputs[nodeOut] = weightedInput;
            }

            // apply activation function
            double[] activations = new double[_numberOfNodesOut];
            for (int nodeOut = 0; nodeOut < activations.Length; nodeOut++)
            {
                activations[nodeOut] = _activationFunction.Activate(weightedInputs, nodeOut);
            }

            return activations;
        }
    }
}
