using NeuralNetwork.Interfaces;

namespace NeuralNetwork
{
    public class Layer
    {
        public int NumberOfNodesIn { get; private set; }
        public int NumberOfNodesOut { get; private set; }
        public double[] Weights { get; private set; }
        public double[] Biases { get; private set; }
        public ActivationFunctionType ActivationFunctionType { get; private set; }

        public Layer(int numberOfNodesIn, int numberOfNodesOut, ActivationFunctionType activationFunctionType)
        {
            NumberOfNodesIn = numberOfNodesIn;
            NumberOfNodesOut = numberOfNodesOut;

            // each neuron has number of nodes in weights and only one bias
            Weights = new double[numberOfNodesIn * numberOfNodesOut];
            Biases = new double[numberOfNodesOut];

            ActivationFunctionType = activationFunctionType;
        }

        public Layer(int numberOfNodesIn, int numberOfNodesOut, double[] weights, double[] biases, ActivationFunctionType activationFunctionType)
        {
            NumberOfNodesIn = numberOfNodesIn;
            NumberOfNodesOut = numberOfNodesOut;

            Weights = weights;
            Biases = biases;

            ActivationFunctionType = activationFunctionType;
        }

        public double[] Calculate(double[] inputs)
        {
            // calculate weighted inputs
            double[] weightedInputs = new double[NumberOfNodesOut];

            for (int nodeOut = 0; nodeOut < NumberOfNodesOut; nodeOut++)
            {
                // assign bias to weightedInput as starting point
                double weightedInput = Biases[nodeOut];

                for (int nodeIn = 0; nodeIn < NumberOfNodesIn; nodeIn++)
                {
                    // modify weightedInput by the weight of the incoming neuron multiplied by the input from the previous layer
                    weightedInput += inputs[nodeIn] * Weights[nodeOut * NumberOfNodesIn + nodeIn];
                }

                weightedInputs[nodeOut] = weightedInput;
            }

            // create activation function
            var activationFunction = ActivationFunctionFactory.Create(ActivationFunctionType);

            // apply activation function
            double[] activations = new double[NumberOfNodesOut];
            for (int nodeOut = 0; nodeOut < activations.Length; nodeOut++)
            {
                activations[nodeOut] = activationFunction.Activate(weightedInputs, nodeOut);
            }

            return activations;
        }
    }
}