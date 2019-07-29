
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace _02_LeonceEcrit
{

    public class ImageFileInfo : INotifyPropertyChanged
    {
        public ImageFileInfo(ImageProperties properties, StorageFile imageFile,
            BitmapImage src, string imageName,string name, string type)
        {
            ImageProperties = properties;
            ImageSource = src;
            ImageName = name;
            ImageFileType = type;
            ImageFile = imageFile;
            string imageText = imageName.Split(new char[] { '_' }, 2)[1];
            imageText = imageText.Replace('_', ' ');
            imageText = imageText.Split(new char[] { '.' }, 2)[0];

            ImageText = imageText.ToUpper();
            
            var rating = (int)properties.Rating;
            var random = new Random();
            ImageRating = rating == 0 ? random.Next(1, 5) : rating;
        }


       

        public StorageFile ImageFile { get; }

        public string ImageText { get; set; }


        public ImageProperties ImageProperties { get; }

        private BitmapImage _imageSource = null;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public string ImageName { get; }

        public string ImageFileType { get; }

        public string ImageDimensions => $"{ImageSource.PixelWidth} x {ImageSource.PixelHeight}";

        public string ImageTitle
        {
            get => String.IsNullOrEmpty(ImageProperties.Title) ? ImageName : ImageProperties.Title;
            set
            {
                if (ImageProperties.Title != value)
                {
                    ImageProperties.Title = value;
                    var ignoreResult = ImageProperties.SavePropertiesAsync();
                    OnPropertyChanged();
                }
            }
        }

        public int ImageRating
        {
            get => (int)ImageProperties.Rating;
            set
            {
                if (ImageProperties.Rating != value)
                {
                    ImageProperties.Rating = (uint)value;
                    var ignoreResult = ImageProperties.SavePropertiesAsync();
                    OnPropertyChanged();
                }
            }
        }

        private bool _needsSaved = false;
        private ImageProperties properties;
        private StorageFile file;
        private BitmapImage bitmapImage;
        private string displayName;
        private string displayType;
        private string displayType1;

        public bool NeedsSaved
        {
            get => _needsSaved;
            set => SetProperty(ref _needsSaved, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetEditingProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
               
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }
    }
}