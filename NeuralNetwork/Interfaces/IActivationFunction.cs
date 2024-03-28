namespace NeuralNetwork.Interfaces
{
    public interface IActivationFunction
    {
        public double Activate(double x);
        public double Derivative(double x);
    }
}