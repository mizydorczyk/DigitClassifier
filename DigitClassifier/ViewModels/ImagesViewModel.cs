﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitClassifier.Interfaces;
using DigitClassifier.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Serilog;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace DigitClassifier.ViewModels
{
    public partial class ImagesViewModel : ObservableRecipient, INavigationAware
    {
        private readonly IImagesService _imagesService;
        private readonly INotificationService _notificationService;

        public ObservableCollection<BitmapImage> Images { get; private set; } = new();
        public ICommand CategorySelectionChangedCommand { get; }
        public ICommand LabelsSelectionChangedCommand { get; }

        private Func<Models.Image, bool>? _categoryFilter;
        private Func<Models.Image, bool>? _labelsFilter;
        private CancellationTokenSource? _loadingCancellationToken;

        public ImagesViewModel(IImagesService imagesService, INotificationService notificationService)
        {
            _imagesService = imagesService;
            _notificationService = notificationService;

            CategorySelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(OnCategorySelectionChanged);
            LabelsSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(OnLabelsSelectionChanged);
        }

        public void OnNavigatedFrom()
        {
            _loadingCancellationToken?.Cancel();
        }

        public async void OnNavigatedTo(object parameter)
        {
            await LoadImages(true);
        }

        private async Task LoadImages(bool refresh = false)
        {
            Images.Clear();

            _loadingCancellationToken?.Cancel();
            _loadingCancellationToken = new CancellationTokenSource();
            var loadingCancellationToken = _loadingCancellationToken.Token;

            try
            {
                var images = await _imagesService.GetImagesAsync(refresh);

                if (_categoryFilter != null)
                    images = images.Where(_categoryFilter).ToList();

                if (_labelsFilter != null)
                    images = images.Where(_labelsFilter).ToList();

                var width = images[0].Width;
                var height = images[0].Height;

                foreach (var image in images)
                {
                    if (loadingCancellationToken.IsCancellationRequested)
                    {
                        Images.Clear();
                        break;
                    }

                    using (var softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Gray8, width, height))
                    {
                        softwareBitmap.CopyFromBuffer(image.Pixels.AsBuffer());

                        using (var stream = new InMemoryRandomAccessStream())
                        {
                            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                            encoder.SetSoftwareBitmap(SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Rgba8));
                            await encoder.FlushAsync();

                            var bitmapImage = new BitmapImage();
                            bitmapImage.DecodePixelWidth = width;
                            bitmapImage.DecodePixelHeight = height;

                            bitmapImage.SetSource(stream);

                            Images.Add(bitmapImage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());
                await _notificationService.ShowAsync(ex.Message, InfoBarSeverity.Error);
            }
        }

        private async void OnCategorySelectionChanged(SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!string.IsNullOrEmpty((string)selectionChangedEventArgs.AddedItems[0]))
            {
                if ((string)selectionChangedEventArgs.AddedItems[0] == "All")
                    _categoryFilter = null;
                else
                    _categoryFilter = new Func<Models.Image, bool>(x => x.Category.ToString() == (string)selectionChangedEventArgs.AddedItems[0]);

                await LoadImages();
            }
        }

        private async void OnLabelsSelectionChanged(SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (!string.IsNullOrEmpty((string)selectionChangedEventArgs.AddedItems[0]))
            {
                if ((string)selectionChangedEventArgs.AddedItems[0] == "All")
                    _labelsFilter = null;
                else
                    _labelsFilter = new Func<Models.Image, bool>(x => x.Label.ToString() == (string)selectionChangedEventArgs.AddedItems[0]);

                await LoadImages();
            }
        }
    }
}