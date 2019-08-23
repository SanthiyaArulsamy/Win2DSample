using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FilterUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool isBlurred;
        GaussianBlurEffect blur = new GaussianBlurEffect();
        CanvasControl canv = new CanvasControl();
        CanvasBitmap cbi;
        double imageHeight, imageWidth;
        public MainPage()
        {
            this.InitializeComponent();            
        }
        private void AddCanvas_Click(object sender, RoutedEventArgs e)
        {
            canv.HorizontalAlignment = HorizontalAlignment.Left;
            canv.VerticalAlignment = VerticalAlignment.Top;
            canv.Draw += Canv_Draw;
            canv.CreateResources += Canv_CreateResources;
            canv.Height = 640;
            canv.Width = 960;
            imageGrid.Children.Add(canv);
        }

        private void Canv_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            args.DrawingSession.Units = CanvasUnits.Pixels;
            var size = cbi.Size;
            
            Rect rect = new Rect(0, 0, size.Width * 1.5, size.Height*1.5);
            if (isBlurred)
            {
                args.DrawingSession.DrawImage(blur);
            }
        }

        private void Canv_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
          
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }
        async Task CreateResourcesAsync(CanvasControl sender)
        {
            var display = DisplayInformation.GetForCurrentView();
            var actualWidth = imageWidth * 1.5; // 1344
            var actualheight = imageHeight * 1.5; //895.666
            var width = Math.Pow(actualWidth, 2); 
            var height = Math.Pow(actualheight, 2);
            var total = Math.Sqrt(width + height);
            float dpis = (float)(Math.Round(total) / display.DiagonalSizeInInches);  // display.DiagonalSizeInInches = 14.031256127922504
            cbi = await CanvasBitmap.LoadAsync(sender, "Flower1.jpg", (float)dpis);// dpi = 115.1001 - It is not working
            // If i set the dpi = 68.5 means it working. Is need to calculate the seprate DPI value for the image.
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            isBlurred = true;
            blur.Source = cbi;
            blur.BlurAmount = (float)e.NewValue;
            canvas.Invalidate();
        }

        private void ImageGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var DPISacle = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            imageHeight = e.NewSize.Height ;
            imageWidth = e.NewSize.Width;
            canvas.Height = imageHeight ;
            canvas.Width = imageWidth ;
        }
    }
}
