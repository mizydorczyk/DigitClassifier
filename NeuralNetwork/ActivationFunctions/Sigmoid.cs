using NeuralNetwork.Interfaces;

namespace NeuralNetwork.ActivationFunctions
{
    public class Sigmoid : IActivationFunction
    {
        public double Activate(double[] weightedInputs, int nodeIndex)
        {
            return 1.0 / (1 + Math.Exp(-weightedInputs[nodeIndex]));
        }

        public double Derivative(double[] weightedInputs, int nodeIndex)
        {
            var activated = Activate(weightedInputs, nodeIndex);
            return activated * (1 - activated);
        }
    }
}