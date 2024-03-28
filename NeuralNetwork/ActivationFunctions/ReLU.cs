using NeuralNetwork.Interfaces;

namespace NeuralNetwork.ActivationFunctions
{
    public class ReLU : IActivationFunction
    {
        public double Activate(double x)
        {
            return Math.Max(0, x);
        }

        public double Derivative(double x)
        {
            return (x > 0) ? 1 : 0;
        }
    }
}