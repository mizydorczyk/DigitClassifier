using NeuralNetwork.Interfaces;

namespace NeuralNetwork.ActivationFunctions
{
    public class ReLU : IActivationFunction
    {
        public double Activate(double[] weightedInputs, int nodeIndex)
        {
            return Math.Max(0, weightedInputs[nodeIndex]);
        }

        public double Derivative(double[] weightedInputs, int nodeIndex)
        {
            return (weightedInputs[nodeIndex] > 0) ? 1 : 0;
        }
    }
}
