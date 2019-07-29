using F23.StringSimilarity;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _02_LeonceEcrit
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public ObservableCollection<ImageFileInfo> Images { get; } = new ObservableCollection<ImageFileInfo>();
        private ILogger log;

        public MainPage()
        {
            this.InitializeComponent();

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new MetroLog.Targets.FileStreamingTarget());

            log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();

            log.Trace("This is a trace message.");
            // Remove this when replaced with XAML bindings

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;

            // Remove this when replaced with XAML bindings
            flipView.ItemsSource = Images;

            if (Images.Count == 0)
            {
                await GetItemsAsync();
            }

            base.OnNavigatedTo(e);
        }

        private async Task GetItemsAsync()
        {
            // https://docs.microsoft.com/uwp/api/windows.ui.xaml.controls.image#Windows_UI_Xaml_Controls_Image_Source
            // See "Using a stream source to show images from the Pictures library".
            // This code is modified to get images from the app folder.

            // Get the app folder where the images are stored.
            StorageFolder appInstalledFolder = Package.Current.InstalledLocation; 
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets\\img");

            // Get and process files in folder
            IReadOnlyList<StorageFile> fileList = await assets.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                // Limit to only png or jpg files.
                if (file.ContentType == "image/png" || file.ContentType == "image/jpeg")
                {
                    Images.Add(await LoadImageInfo(file));
                }
            }
        }

        public async static Task<ImageFileInfo> LoadImageInfo(StorageFile file)
        {
            // Open a stream for the selected file.
            // The 'using' block ensures the stream is disposed
            // after the image is loaded.
            using (IRandomAccessStream fileStream = await file.OpenReadAsync())
            {
                // Create a bitmap to be the image source.
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(fileStream);

                var properties = await file.Properties.GetImagePropertiesAsync();
                ImageFileInfo info = new ImageFileInfo(
                    properties, file, bitmapImage,  file.Name,
                    file.DisplayName, file.DisplayType) ;

                return info;
            }
        }

        private async Task speachAsync(string  texte)
        {
            MediaElement mediaElement = new MediaElement();
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(texte);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();



        }

        private async void ButtonValid_Click(object sender, RoutedEventArgs e)
        {
            string inputString = myInputTextBox.Text;
            ImageFileInfo selectedImage = Images[flipView.SelectedIndex];

            string[] lines = inputString.Split('\r') ;
            string voiceString = inputString;
            double ratingResult = 0.0;
            foreach (var L in lines)
            {
                var l = new NormalizedLevenshtein();
                double distance = l.Similarity(L, selectedImage.ImageText);
                // distance est entre 0 [PERFECT] et 1 
                if (distance > ratingResult)
                    ratingResult = distance;

                log.Trace("Compare " + L + ", " + selectedImage.ImageText);
                log.Trace("results =" + l.Similarity(L, selectedImage.ImageText));
                if (L == selectedImage.ImageText)
                {
                    voiceString = L;
                }
            }


            // transformation du result 0..1 en 0..5 (5 est 5 etoile)
            log.Trace("ratingResults =" + ratingResult +"  ---");
            log.Trace("5*ratingResult =" + 5 * (ratingResult) + "  ---");

            ratingResult = 5 * ratingResult;
            int ratingResultInt = (int)ratingResult;
            log.Trace(" (int)ratingResult =" + ratingResult + "  ---");

            string bravoText = "";
            switch (ratingResultInt)
            {
                case 5:
                    bravoText = " BRAVO !!, ";
                    break;
                case 4:
                    bravoText = " PRESQUE !!";
                    break;
                case 2:
                case 3:
                    bravoText = " ESSAYE ENCORE !!";
                    break;
                case 0:
                case 1:
                    bravoText = " RECOMENCE !!";
                    break;
                default:
                    bravoText = " DEFAULT !!";
                    break;

            }
            Flyout tmpFlyout = Resources["MyFlyout"] as Flyout;
            MyRating.Value = ratingResultInt;
            MyResultText.Text = bravoText;
            voiceString = string.Concat(bravoText+",", voiceString);
            myInputTextBox.Focus(FocusState.Programmatic);

            await this.speachAsync( voiceString);
             tmpFlyout.ShowAt(myInputTextBox);
            // rend le focus a la text box
           
            
        }





        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myInputTextBox.Text = "";
            myInputTextBox.Focus(FocusState.Programmatic);
        }
    }
}
