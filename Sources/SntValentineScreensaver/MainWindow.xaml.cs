using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageBrush = System.Windows.Media.ImageBrush;

namespace SntValentineScreensaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        public List<List<HeartCell>> ImagesArray { get; set; } = new List<List<HeartCell>>();

        public string BGColor { get; set; } = "#a23a61";
        public BitmapImage OpacityMaskImage { get; set; }
        public ImageBrush OpacityBrush { get; set; }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            Mouse.OverrideCursor = Cursors.None;
            this.DataContext = this;
            OpacityMaskImage = new BitmapImage(new Uri(@"pack://application:,,,/SntValentineScreensaver;component/Images/HeartOpacityMask.png", UriKind.Absolute));
            OpacityBrush = new ImageBrush(OpacityMaskImage);

            InitImageArray();
            LoadImages(ImageName.Start);

            var columnCount = TheGrid.ColumnDefinitions.Count;
            var rowCount = TheGrid.RowDefinitions.Count;
            for (var i = 0; i < columnCount; i++)
            {
                ImagesArray.Add(new List<HeartCell>(rowCount));
                for (var j = 0; j < rowCount; j++)
                {
                    var image = new Image();
                    image.Source = OpacityMaskImage;
                    image.SetValue(Grid.ColumnProperty, i);
                    image.SetValue(Grid.RowProperty, j);
                    TheGrid.Children.Add(image);

                    var cell = new HeartCell(){Image = image};
                    ImagesArray[i].Add(cell);
                }
            }

            //TransformGroup group = new TransformGroup();
            //double width = this.MainGrid.RenderSize.Width;
            //DoubleAnimation animation = new DoubleAnimation((width / 2) * -1, width / 2 + logo.ActualWidth, new Duration(new TimeSpan(0, 0, 0, 10)));
            //animation.RepeatBehavior = RepeatBehavior.Forever;
            //TranslateTransform tt = new TranslateTransform(-logo.ActualWidth * 2, 0);
            //logo.RenderTransform = group;
            //logo.Width = 200;
            //logo.Height = 200;
            //group.Children.Add(tt);
            //tt.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        private void InitImageArray()
        {
            for (var i = 0; i < 10; i++)
            {
                var imageRow = new List<HeartCell>();
                for (var j = 0; j < 10; j++)
                {
                    imageRow.Add(new HeartCell());
                }

                ImagesArray.Add(imageRow);
            }
        }

        private void LoadImages(ImageName imageName)
        {
            foreach (var imageRow in ImagesArray)
            {
                foreach (var cell in imageRow)
                {
                    var imageUrl = GetImageUrl(imageName);
                    cell.Image.BeginInit();
                    //cell.Image.UriSource = imageUrl;
                    cell.Image.EndInit();
                }
            }
        }

        private Uri GetImageUrl(ImageName imageName)
        {
            switch (imageName)
            {
                case ImageName.Start:
                    return null;
            }

            return null;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class HeartCell : INotifyPropertyChanged
    {
        private Image _image;
        private string _backgroundColor;

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public int AnimationDelay { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal enum ImageName
    {
        Start,
        First,
        Second,
        Third
    }
}