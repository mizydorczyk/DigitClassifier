using NeuralNetwork.Interfaces;
using System.Text.Json.Serialization;

namespace NeuralNetwork
{
    public class NetworkLearnData
    {
        public LayerLearnData[] layerData;

        public NetworkLearnData(Layer[] layers)
        {
            layerData = new LayerLearnData[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                layerData[i] = new LayerLearnData(layers[i]);
            }
        }
    }

    public class LayerLearnData
    {
        public double[] Inputs;
        public double[] WeightedInputs;
        public double[] Activations;
        public double[] NodeValues;

        public LayerLearnData(Layer layer)
        {
            WeightedInputs = new double[layer.NumberOfNodesOut];
            Activations = new double[layer.NumberOfNodesOut];
            NodeValues = new double[layer.NumberOfNodesOut];
        }
    }
    public class Network
    {
        [JsonIgnore] public string Name { get; set; }
        public Layer[] Layers { get; private set; }
        [JsonIgnore] public NetworkLearnData[] batchLearnData { get; private set; }

        public Network(Layer[] layers)
        {
            Layers = layers;
        }

        public Network(int[] layerSizes, ActivationFunctionType activationFunctionType)
        {
            Layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new Layer(layerSizes[i], layerSizes[i + 1], activationFunctionType);
            }

            InitializeRandomValues(new Random());
        }

        public void Learn(double[][] inputs, double[][] targets, double learnRate, double regularization = 0, double momentum = 0)
        {

            if (batchLearnData == null || batchLearnData.Length != inputs.Length)
            {
                batchLearnData = new NetworkLearnData[inputs.Length];
                for (int i = 0; i < batchLearnData.Length; i++)
                {
                    batchLearnData[i] = new NetworkLearnData(Layers);
                }
            }

            Parallel.For(0, inputs.Length, (i) =>
            {
                UpdateGradients(inputs[i], targets[i], batchLearnData[i]);
            });


            // update weights and biases based on the calculated gradients
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i].ApplyGradients(learnRate / inputs.Length, regularization, momentum);
            }
        }


        void UpdateGradients(double[] input, double[] target, NetworkLearnData learnData)
        {
            // feed forward
            double[] inputsToNextLayer = input;

            for (int i = 0; i < Layers.Length; i++)
            {
                inputsToNextLayer = Layers[i].FeedForward(inputsToNextLayer, learnData.layerData[i]);
            }

            // back propagate
            int outputLayerIndex = Layers.Length - 1;
            Layer outputLayer = Layers[outputLayerIndex];
            LayerLearnData outputLearnData = learnData.layerData[outputLayerIndex];

            // update output layer gradients
            outputLayer.CalculateOutputLayerNodeValues(outputLearnData, target);
            outputLayer.UpdateGradients(outputLearnData);

            // update hidden layer gradients
            for (int i = outputLayerIndex - 1; i >= 0; i--)
            {
                var layerLearnData = learnData.layerData[i];
                var hiddenLayer = Layers[i];

                hiddenLayer.CalculateHiddenLayerNodeValues(layerLearnData, Layers[i + 1], learnData.layerData[i + 1].NodeValues);
                hiddenLayer.UpdateGradients(layerLearnData);
            }

        }

        public double[] FeedForward(double[] inputs)
        {
            // calculate each consecutively
            for (int i = 0; i < Layers.Length; i++)
            {
                inputs = Layers[i].FeedForward(inputs);
            }

            return inputs;
        }

        private void InitializeRandomValues(Random random)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i].InitializeRandomValues(random);
            }
        }
    }
}