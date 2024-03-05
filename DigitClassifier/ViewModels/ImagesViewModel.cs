using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitClassifier.Interfaces;
using DigitClassifier.Models;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace DigitClassifier.ViewModels
{
    public partial class ImagesViewModel : ObservableRecipient, INavigationAware
    {
        private readonly IImagesService _imagesService;

        public ObservableCollection<BitmapImage> Images { get; private set; } = new ObservableCollection<BitmapImage>();
        public ICommand CategorySelectionChangedCommand { get; }
        public ICommand LabelsSelectionChangedCommand { get; }

        [ObservableProperty]
        private ImageCategory? _selectedCategory = null;
        [ObservableProperty]
        private int? _selectedLabel = null;

        private Func<Models.Image, bool>? _categoryFilter;
        private Func<Models.Image, bool>? _labelsFilter;
        private CancellationTokenSource _loadingCancellationToken;

        public ImagesViewModel(IImagesService imagesService)
        {
            _imagesService = imagesService;

            CategorySelectionChangedCommand = new RelayCommand<object>(OnCategorySelectionChanged);
            LabelsSelectionChangedCommand = new RelayCommand<object>(OnLabelsSelectionChanged);
        }

        public void OnNavigatedFrom()
        {
            _loadingCancellationToken.Cancel();
        }

        public async void OnNavigatedTo(object parameter)
        {
            await LoadImages();
        }

        private async Task LoadImages()
        {
            if (_loadingCancellationToken != null)
            {
                _loadingCancellationToken.Cancel();
            }

            _loadingCancellationToken = new CancellationTokenSource();
            var loadingCancellationToken = _loadingCancellationToken.Token;

            var images = await _imagesService.GetImagesAsync();

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

        private async void OnCategorySelectionChanged(object args)
        {
            if (args is SelectionChangedEventArgs selectionChangedEventArgs)
            {
                if (!string.IsNullOrEmpty((string)selectionChangedEventArgs.AddedItems[0]))
                {
                    if ((string)selectionChangedEventArgs.AddedItems[0] == "All")
                        _categoryFilter = null;
                    else
                        _categoryFilter = new Func<Models.Image, bool>(x => x.Category.ToString() == (string)selectionChangedEventArgs.AddedItems[0]);

                    Images.Clear();
                    await LoadImages();
                }
            }
        }

        private async void OnLabelsSelectionChanged(object args)
        {
            if (args is SelectionChangedEventArgs selectionChangedEventArgs)
            {
                if (!string.IsNullOrEmpty((string)selectionChangedEventArgs.AddedItems[0]))
                {
                    if ((string)selectionChangedEventArgs.AddedItems[0] == "All")
                        _labelsFilter = null;
                    else
                        _labelsFilter = new Func<Models.Image, bool>(x => x.Label.ToString() == (string)selectionChangedEventArgs.AddedItems[0]);

                    Images.Clear();
                    await LoadImages();
                }
            }
        }
    }
}
