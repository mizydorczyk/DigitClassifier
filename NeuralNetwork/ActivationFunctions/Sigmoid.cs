using NeuralNetwork.Interfaces;

namespace NeuralNetwork.ActivationFunctions
{
    public class Sigmoid : IActivationFunction
    {
        public double Activate(double x)
        {
            return 1.0 / (1 + Math.Exp(-x));
        }

        public double Derivative(double x)
        {
            var activated = Activate(x);
            return activated * (1 - activated);
        }
    }
}