﻿using DigitClassifier.Helpers;
using DigitClassifier.Interfaces;
using DigitClassifier.Models;

namespace DigitClassifier.Services
{
    public class ImagesService : IImagesService
    {
        private readonly ILocalSettingsService _localSettingsService;
        private List<Image>? _images;

        public ImagesService(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
        }

        public async Task<List<Image>> GetImagesAsync(bool refresh = false)
        {
            if (_images != null && !refresh)
                return _images;
            var images = new List<Image>();

            var options = new string[]
            {
                "TrainingImagesFile",
                "TrainingLabelsFile",
                "TestImagesFile",
                "TestLabelsFile"
            };

            var paths = new Dictionary<string, string>();
            foreach (var option in options)
            {
                var path = await _localSettingsService.ReadSettingAsync<string>(option);

                if (path != null)
                    paths.Add(option, path);
                else
                    throw new Exception($"The setting for the key: {option} was not found");
            }

            using (var reader = new ImagesReader(paths["TrainingLabelsFile"], paths["TrainingImagesFile"]))
            {
                var trainingSet = reader.Read(ImageCategory.Training);
                images.AddRange(trainingSet);
            }

            using (var reader = new ImagesReader(paths["TestLabelsFile"], paths["TestImagesFile"]))
            {
                var testSet = reader.Read(ImageCategory.Test);
                images.AddRange(testSet);
            }

            _images = images;

            return images;
        }
    }
}