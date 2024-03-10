using NeuralNetwork.ActivationFunctions;
using NeuralNetwork.Interfaces;

namespace NeuralNetwork
{
    public static class ActivationFunctionFactory
    {
        public static IActivationFunction Create(ActivationFunctionType activationFunctionType)
        {
            return (activationFunctionType) switch
            {
                ActivationFunctionType.Sigmoid => new Sigmoid(),
                _ => throw new Exception($"The activation function for the key: {activationFunctionType} was not found")
            };
        }
    }
}