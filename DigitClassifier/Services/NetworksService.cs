using DigitClassifier.Interfaces;
using NeuralNetwork;
using NeuralNetwork.Interfaces;
using System.Text.Json;

namespace DigitClassifier.Services
{
    public class NetworksService : INetworksService
    {
        private readonly ILocalSettingsService _localSettingsService;
        private List<Network>? _networks;
        private string? _activeNetworkName;

        public NetworksService(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
        }

        public Network ActiveNetwork
        {
            get
            {
                if (_networks == null || _networks.Count == 0)
                    return null;

                var network = _networks.FirstOrDefault(x => x.Name == _activeNetworkName);

                return network ?? _networks.First();
            }
            set { _activeNetworkName = value.Name; }
        }

        public async Task DeleteNetworkAsync(Network network)
        {
            var option = "NetworksFolder";
            var folder = await _localSettingsService.ReadSettingAsync<string>(option) ??
                         throw new Exception($"The setting for the key: {option} was not found");

            if (!Path.Exists(folder) || File.GetAttributes(folder) != FileAttributes.Directory)
                throw new ArgumentException("Networks folder path is not valid");

            var path = Directory.GetFiles(folder).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == network.Name)
                       ?? throw new ArgumentException("Could not find given network file");

            File.Delete(path);

            _networks?.Remove(network);
        }

        public async Task<List<Network>> GetNetworksAsync(bool refresh = false)
        {
            if (_networks != null && !refresh)
                return _networks;

            var networks = new List<Network>();

            var option = "NetworksFolder";
            var folder = await _localSettingsService.ReadSettingAsync<string>(option) ??
                         throw new Exception($"The setting for the key: {option} was not found");

            if (!Path.Exists(folder) || File.GetAttributes(folder) != FileAttributes.Directory)
                throw new ArgumentException("Networks folder path is not valid");

            var paths = Directory.GetFiles(folder);
            foreach (var path in paths)
            {
                using var stream = new FileStream(path, FileMode.Open);
                using var jsonDocument = JsonDocument.Parse(stream);

                var root = jsonDocument.RootElement;
                var layers = new List<Layer>();

                var layersElement = root.GetProperty("Layers");
                foreach (var layerElement in layersElement.EnumerateArray())
                {
                    int numberOfNodesIn = layerElement.GetProperty("NumberOfNodesIn").GetInt32();
                    int numberOfNodesOut = layerElement.GetProperty("NumberOfNodesOut").GetInt32();
                    int activationFunctionType = layerElement.GetProperty("ActivationFunctionType").GetInt32();

                    var weights = JsonSerializer.Deserialize<double[]>(layerElement.GetProperty("Weights").GetRawText());
                    var biases = JsonSerializer.Deserialize<double[]>(layerElement.GetProperty("Biases").GetRawText());

                    var layer = new Layer(numberOfNodesIn, numberOfNodesOut, weights, biases, (ActivationFunctionType)activationFunctionType);
                    layers.Add(layer);
                }

                var network = new Network([.. layers]);
                network.Name = Path.GetFileNameWithoutExtension(path);
                networks.Add(network);
            }

            _networks = networks;

            if (_networks.FirstOrDefault(x => x.Name == _activeNetworkName) == null)
                _activeNetworkName = null;

            return networks;
        }

        public async Task SaveNetworkAsync(Network network)
        {
            var option = "NetworksFolder";
            var folder = await _localSettingsService.ReadSettingAsync<string>(option) ??
                         throw new Exception($"The setting for the key: {option} was not found");

            if (!Path.Exists(folder))
                Directory.CreateDirectory(folder);

            if (Path.Exists(folder) && File.GetAttributes(folder) != FileAttributes.Directory)
                throw new ArgumentException("Networks folder path is not valid");

            var fileName = (File.Exists(Path.Combine(folder, $"{network.Name}.json"))) switch
            {
                true => $"{network.Name}.json",
                false => $"DigitClassifier_Network_{DateTime.UtcNow.ToString("ddMMyyyy_HHmmss")}.json"
            };

            using var stream = File.Open(Path.Combine(folder, fileName), FileMode.OpenOrCreate);
            await JsonSerializer.SerializeAsync(stream, network);
        }
    }
}