using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

//Szablon elementu Okno dialogowe zawartości jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace RocketChatUWP.Views
{
    public sealed partial class ImagePreviewDialog : ContentDialog
    {
        private BitmapImage image;
        public ImagePreviewDialog(BitmapImage image)
        {
            InitializeComponent();
            this.image = image;
            imagePreview.Source = this.image;
            imagePreview.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
            imagePreview.Loaded += ImagePreview_Loaded;
        }

        private void ImagePreview_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            scrollViewer.ChangeView(null, null, 0.4f);
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var zoom = float.Parse(Math.Round(scrollViewer.ZoomFactor, 1).ToString());
            if (zoom == 0.4f)
                scrollViewer.ChangeView(null, null, 1.0f);
            else
                scrollViewer.ChangeView(null, null, 0.4f);
        }
    }
}
