using NeuralNetwork.Interfaces;
using System.Text.Json.Serialization;

namespace NeuralNetwork
{
    public class Layer
    {
        public int NumberOfNodesIn { get; private set; }
        public int NumberOfNodesOut { get; private set; }
        public double[] Weights { get; private set; }
        public double[] Biases { get; private set; }
        [JsonIgnore] public double[] CostGradientW { get; private set; }
        [JsonIgnore] public double[] CostGradientB { get; private set; }
        [JsonIgnore] public  double[] WeightVelocities { get; private set; }
        [JsonIgnore] public double[] BiasVelocities { get; private set; }
        public ActivationFunctionType ActivationFunctionType { get; private set; }

        public Layer(int numberOfNodesIn, int numberOfNodesOut, ActivationFunctionType activationFunctionType)
        {
            NumberOfNodesIn = numberOfNodesIn;
            NumberOfNodesOut = numberOfNodesOut;

            // each neuron has number of nodes in weights and only one bias
            Weights = new double[numberOfNodesIn * numberOfNodesOut];
            Biases = new double[numberOfNodesOut];

            CostGradientW = new double[Weights.Length];
            CostGradientB = new double[Biases.Length];

            WeightVelocities = new double[Weights.Length];
            BiasVelocities = new double[Biases.Length];

            ActivationFunctionType = activationFunctionType;
        }

        public Layer(int numberOfNodesIn, int numberOfNodesOut, double[] weights, double[] biases, ActivationFunctionType activationFunctionType)
        {
            NumberOfNodesIn = numberOfNodesIn;
            NumberOfNodesOut = numberOfNodesOut;

            Weights = weights;
            Biases = biases;

            CostGradientW = new double[Weights.Length];
            CostGradientB = new double[Biases.Length];

            WeightVelocities = new double[Weights.Length];
            BiasVelocities = new double[Biases.Length];

            ActivationFunctionType = activationFunctionType;
        }

        public double[] FeedForward(double[] inputs)
        {
            // calculate weighted inputs
            var weightedInputs = new double[NumberOfNodesOut];

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
                activations[nodeOut] = activationFunction.Activate(weightedInputs[nodeOut]);
            }

            return activations;
        }

        public double[] FeedForward(double[] inputs, LayerLearnData learnData)
        {
            // calculate weighted inputs
            learnData.Inputs = inputs;

            for (int nodeOut = 0; nodeOut < NumberOfNodesOut; nodeOut++)
            {
                // assign bias to weightedInput as starting point
                double weightedInput = Biases[nodeOut];

                for (int nodeIn = 0; nodeIn < NumberOfNodesIn; nodeIn++)
                {
                    // modify weightedInput by the weight of the incoming neuron multiplied by the input from the previous layer
                    weightedInput += inputs[nodeIn] * Weights[nodeOut * NumberOfNodesIn + nodeIn];
                }

                learnData.WeightedInputs[nodeOut] = weightedInput;
            }

            // create activation function
            var activationFunction = ActivationFunctionFactory.Create(ActivationFunctionType);

            // apply activation function
            for (int nodeOut = 0; nodeOut < learnData.Activations.Length; nodeOut++)
            {
                learnData.Activations[nodeOut] = activationFunction.Activate(learnData.WeightedInputs[nodeOut]);
            }

            return learnData.Activations;
        }

        public void CalculateOutputLayerNodeValues(LayerLearnData layerLearnData, double[] expectedOutputs)
        {
            // create activation function
            var activationFunction = ActivationFunctionFactory.Create(ActivationFunctionType);

            for (int i = 0; i < layerLearnData.NodeValues.Length; i++)
            {
                // evaluate partial derivatives for mean squared error function
                double errorDerivative = layerLearnData.Activations[i] - expectedOutputs[i];
                double activationDerivative = activationFunction.Derivative(layerLearnData.WeightedInputs[i]);
                layerLearnData.NodeValues[i] = errorDerivative * activationDerivative;
            }
        }

        public void CalculateHiddenLayerNodeValues(LayerLearnData layerLearnData, Layer oldLayer, double[] oldNodeValues)
        {
            // create activation function
            var activationFunction = ActivationFunctionFactory.Create(ActivationFunctionType);

            for (int newNodeIndex = 0; newNodeIndex < NumberOfNodesOut; newNodeIndex++)
            {
                double newNodeValue = 0;
                for (int oldNodeIndex = 0; oldNodeIndex < oldNodeValues.Length; oldNodeIndex++)
                {
                    // partial derivative of the weighted input with respect to the input
                    double weightedInputDerivative = oldLayer.Weights[oldNodeIndex * NumberOfNodesOut + newNodeIndex];
                    newNodeValue += weightedInputDerivative * oldNodeValues[oldNodeIndex];
                }
                newNodeValue *= activationFunction.Derivative(layerLearnData.WeightedInputs[newNodeIndex]);
                layerLearnData.NodeValues[newNodeIndex] = newNodeValue;
            }

        }

        // update weights and biases based on previously calculated gradients
        public void ApplyGradients(double learnRate, double regularization, double momentum)
        {
            double weightDecay = (1 - regularization * learnRate);

            for (int i = 0; i < Weights.Length; i++)
            {
                double weight = Weights[i];
                double velocity = WeightVelocities[i] * momentum - CostGradientW[i] * learnRate;
                WeightVelocities[i] = velocity;
                Weights[i] = weight * weightDecay + velocity;
                CostGradientW[i] = 0;
            }


            for (int i = 0; i < Biases.Length; i++)
            {
                double velocity = BiasVelocities[i] * momentum - CostGradientB[i] * learnRate;
                BiasVelocities[i] = velocity;
                Biases[i] += velocity;
                CostGradientB[i] = 0;
            }
        }

        public void UpdateGradients(LayerLearnData layerLearnData)
        {
            // update cost gradient with respect to weights
            lock (CostGradientW)
            {
                for (int nodeOut = 0; nodeOut < NumberOfNodesOut; nodeOut++)
                {
                    double nodeValue = layerLearnData.NodeValues[nodeOut];
                    for (int nodeIn = 0; nodeIn < NumberOfNodesIn; nodeIn++)
                    {
                        // evaluate the partial derivative: cost / weight
                        CostGradientW[nodeOut * NumberOfNodesIn + nodeIn] += layerLearnData.Inputs[nodeIn] * nodeValue;
                    }
                }
            }

            // update cost gradient with respect to biases
            lock (CostGradientB)
            {
                for (int nodeOut = 0; nodeOut < NumberOfNodesOut; nodeOut++)
                {
                    // evaluate partial derivative: cost / bias
                    CostGradientB[nodeOut] += layerLearnData.NodeValues[nodeOut];
                }
            }
        }

        public void InitializeRandomValues(Random random)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                var a = random.NextDouble();
                var b = random.NextDouble();
                Weights[i] = Math.Sqrt(-2.0 * Math.Log(a)) * Math.Sin(2.0 * Math.PI * b);
            }

            for (int i = 0; i < Biases.Length; i++)
            {
                var a = random.NextDouble();
                var b = random.NextDouble();
                Biases[i] = Math.Sqrt(-2.0 * Math.Log(a)) * Math.Sin(2.0 * Math.PI * b);
            }
        }
    }
}