namespace NeuralNetwork.Interfaces
{
    public interface IActivationFunction
    {
        public double Activate(double[] weightedInputs, int nodeIndex);
    }
}