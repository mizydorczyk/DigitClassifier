using NeuralNetwork.Interfaces;

namespace NeuralNetwork.ActivationFunctions
{
    public class Sigmoid : IActivationFunction
    {
        public double Activate(double[] weightedInputs, int nodeIndex)
        {
            return 1.0 / (1 + Math.Exp(-weightedInputs[nodeIndex]));
        }
    }
}