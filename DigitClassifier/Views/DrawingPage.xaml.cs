using DigitClassifier.ViewModels;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using System.Numerics;
using Microsoft.UI.Xaml.Input;

namespace DigitClassifier.Views
{
    public sealed partial class DrawingPage : Page
    {
        private const int _targetImageSize = 28;
        private const int _baseDrawingControlSize = 560;
        private const int _brushSize = 40;

        private bool _isDrawing = false;
        private float _scaleFactor = 1;
        private Vector2 _previousPoint;
        private CanvasRenderTarget _renderTarget;
        public DrawingViewModel ViewModel { get; private set; }

        public DrawingPage()
        {
            ViewModel = App.GetService<DrawingViewModel>();
            InitializeComponent();
            RegisterKeyboardAccelerators();
        }

        private void RegisterKeyboardAccelerators()
        {
            var clearDrawingAccelerator = new KeyboardAccelerator { Key = Windows.System.VirtualKey.C };
            clearDrawingAccelerator.Invoked += OnClearDrawingAcceleratorInvoked;
            KeyboardAccelerators.Add(clearDrawingAccelerator);
        }

        private void OnClearDrawingAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            using (var ds = _renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Black);
            }

            DrawingControl.Invalidate();
        }

        private double[] ResizeImage(byte[] input)
        {
            var pixels = new double[_targetImageSize * _targetImageSize];
            var scale = (float)_baseDrawingControlSize / _targetImageSize;

            for (int y = 0; y < _targetImageSize; y++)
            {
                // calculate y range for sampling in the original image
                var startY = (int)(y * scale);
                var endY = (int)((y + 1) * scale);

                for (var x = 0; x < _targetImageSize; x++)
                {
                    // calculate x range for sampling in the original image
                    var startX = (int)(x * scale);
                    var endX = (int)((x + 1) * scale);

                    var sum = 0;

                    // accumulate sum over the sampled region
                    for (int j = startY; j < endY; j++)
                    {
                        for (int i = startX; i < endX; i++)
                        {
                            int index = (i + j * _baseDrawingControlSize) * 4;
                            sum += input[index]; // considering the R channel since the image is B&W only
                        }
                    }

                    // calculate the average intensity for the region
                    var average = (byte)(sum / ((endX - startX) * (endY - startY)));

                    pixels[x + y * _targetImageSize] = average;
                }
            }

            return pixels;
        }


        private void DrawingControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            args.DrawingSession.Transform = Matrix3x2.CreateScale(_scaleFactor);
            args.DrawingSession.DrawImage(_renderTarget);

            var drawingControlPixels = _renderTarget.GetPixelBytes();
            var pixels = ResizeImage(drawingControlPixels);

            ViewModel.DrawingChangedCommand.Execute(pixels);
        }

        private void DrawingControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            if (e.Pointer.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Mouse || !properties.IsLeftButtonPressed)
                return;

            _isDrawing = true;
            _previousPoint = e.GetCurrentPoint(DrawingControl).Position.ToVector2() / (_scaleFactor * _scaleFactor);
            DrawPoint(_previousPoint.X, _previousPoint.Y);
            DrawingControl.Invalidate();
        }

        private void DrawingControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            if (e.Pointer.PointerDeviceType != Microsoft.UI.Input.PointerDeviceType.Mouse || !properties.IsLeftButtonPressed)
                return;

            if (!_isDrawing) return;

            var currentPoint = e.GetCurrentPoint(DrawingControl).Position.ToVector2() / (_scaleFactor * _scaleFactor);

            DrawPoint(currentPoint.X, currentPoint.Y);
            DrawLine(_previousPoint, currentPoint);

            _previousPoint = currentPoint;
            DrawingControl.Invalidate();
        }

        private void DrawingControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isDrawing = false;
        }

        private void DrawingControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isDrawing = false;
        }

        private void DrawingControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            _renderTarget = new CanvasRenderTarget(DrawingControl, _baseDrawingControlSize, _baseDrawingControlSize, 96);
            using (var ds = _renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.Black);
            }
        }

        private void DrawingControl_Grid_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            var grid = (Grid)sender;
            DrawingControl.Width = grid.ActualHeight;
            DrawingControl.Height = grid.ActualHeight;
        }

        private void DrawingControl_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            _scaleFactor = (float)(Math.Min(DrawingControl.ActualWidth, DrawingControl.ActualHeight) / _baseDrawingControlSize);
            DrawingControl.Invalidate();
        }

        private void DrawPoint(float x, float y)
        {
            using (var ds = _renderTarget.CreateDrawingSession())
            {
                ds.Transform = Matrix3x2.CreateScale(_scaleFactor);
                ds.Antialiasing = CanvasAntialiasing.Antialiased;
                ds.FillCircle(x, y, _brushSize / 2 / _scaleFactor, Colors.White);
            }
        }

        private void DrawLine(Vector2 a, Vector2 b)
        {
            using (var ds = _renderTarget.CreateDrawingSession())
            {
                ds.Transform = Matrix3x2.CreateScale(_scaleFactor);
                ds.Antialiasing = CanvasAntialiasing.Antialiased;
                ds.DrawLine(a, b, Colors.White, _brushSize / _scaleFactor);
            }
        }
    }
}