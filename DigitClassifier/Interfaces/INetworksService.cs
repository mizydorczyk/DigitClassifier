using NeuralNetwork;

namespace DigitClassifier.Interfaces
{
    public interface INetworksService
    {
        Task DeleteNetworkAsync(Network network);
        Task<List<Network>> GetNetworksAsync(bool refresh = false);
        Task SaveNetworkAsync(Network network);
        Network ActiveNetwork { get; set; }
    }
}