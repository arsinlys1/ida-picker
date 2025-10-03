using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ida_picker
{
    public static class IconExtractor
    {
        public static ImageSource ExtractIcon(string installPath)
        {
            try
            {
                string icoPath = Path.Combine(installPath, "ida.ico");
                if (File.Exists(icoPath))
                {
                    return new BitmapImage(new Uri(icoPath, UriKind.Absolute));
                }

                string ida64Path = Path.Combine(installPath, "ida64.exe");
                if (File.Exists(ida64Path))
                {
                    return ExtractIconFromExe(ida64Path);
                }

                string idaPath = Path.Combine(installPath, "ida.exe");
                if (File.Exists(idaPath))
                {
                    return ExtractIconFromExe(idaPath);
                }
            }
            catch (Exception)
            {
                // Fall through to default icon
            }

            // Return a default icon if extraction fails
            return CreateDefaultIcon();
        }

        private static ImageSource ExtractIconFromExe(string exePath)
        {
            try
            {
                using (Icon? icon = Icon.ExtractAssociatedIcon(exePath))
                {
                    if (icon != null)
                    {
                        return Imaging.CreateBitmapSourceFromHIcon(
                            icon.Handle,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                    }
                }
            }
            catch (Exception)
            {
                // Fall through to default
            }

            return CreateDefaultIcon();
        }

        private static ImageSource CreateDefaultIcon()
        {
            // Create a simple default icon
            var drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    System.Windows.Media.Brushes.LightGray,
                    new System.Windows.Media.Pen(System.Windows.Media.Brushes.Gray, 1),
                    new Rect(0, 0, 64, 64));
                
                drawingContext.DrawText(
                    new FormattedText("IDA",
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        20,
                        System.Windows.Media.Brushes.Black,
                        VisualTreeHelper.GetDpi(drawingVisual).PixelsPerDip),
                    new System.Windows.Point(12, 22));
            }

            var renderTargetBitmap = new RenderTargetBitmap(64, 64, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
    }
}
