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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using ImageBrush = System.Windows.Media.ImageBrush;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace SntValentineScreensaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _animationGlobalTimer;
        private ImageName _currentState = ImageName.Start;

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

            //InitImageArray();
            //LoadImages(ImageName.Start);

            var columnCount = TheGrid.ColumnDefinitions.Count;
            var rowCount = TheGrid.RowDefinitions.Count;
            for (var i = 0; i < columnCount; i++)
            {
                ImagesArray.Add(new List<HeartCell>(rowCount));
                for (var j = 0; j < rowCount; j++)
                {
                    var vp3d = new Viewport3D();
                    var camera = CreateCamera();
                    vp3d.Camera = camera;

                    var light = CreateLight();
                    vp3d.Children.Add(light);
                    var vp2d3d = CreateViewPort();
                    vp3d.Children.Add(vp2d3d);

                    var image = new Image();
                    //animationTrigger.EnterActions.Add();
                    //image.Triggers.Add();
                    image.Source = OpacityMaskImage;
                    image.SetValue(Grid.ColumnProperty, i);
                    image.SetValue(Grid.RowProperty, j);
                    TheGrid.Children.Add(image);

                    var cell = new HeartCell() { Image = image };
                    ImagesArray[i].Add(cell);
                }
            }

            _animationGlobalTimer = new Timer();

            _animationGlobalTimer.Interval = 2000;
            _animationGlobalTimer.Enabled = true;
            _animationGlobalTimer.Start();
            _animationGlobalTimer.Tick += OnGlobalTimerTick;
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

        private static Viewport2DVisual3D CreateViewPort()
        {
            var vp= new Viewport2DVisual3D();
            var geometry = new MeshGeometry3D();
            return vp;
        }

        private static PerspectiveCamera CreateCamera()
        {
            var camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 4);
            return camera;
        }

        private static ModelVisual3D CreateLight()
        {
            var light = new ModelVisual3D();
            var lightColor = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            light.Content = new DirectionalLight(lightColor, new Vector3D(0, 0, -1));
            return light;
        }

        private void OnGlobalTimerTick(object sender, EventArgs e)
        {
            string pictureName=String.Empty;
            switch (_currentState)
            {
                case ImageName.Start:
                    pictureName = "HeartOpacityMask";
                    _currentState = ImageName.First;
                    break;
                case ImageName.First:
                    pictureName = "HeartOpacityMask";
                    _currentState = ImageName.Second;
                    break;
                case ImageName.Second:
                    pictureName = "HeartOpacityMask";
                    _currentState = ImageName.Third;
                    break;
                case ImageName.Third:
                    pictureName = "HeartOpacityMask";
                    _currentState = ImageName.Start;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var pic = new BitmapImage(new Uri($"pack://application:,,,/SntValentineScreensaver;component/Images/{pictureName}.png", UriKind.Absolute));

            ChangePicture(pic);
        }

        private void ChangePicture(BitmapImage pic)
        {
            var flipOutAnimation = FlipOutAnimation();
            var flipInAnimation = FlipInAnimation();
            flipOutAnimation.Completed += (s, e) =>
            {
            };

            foreach (var row in ImagesArray)
            {
                foreach (var heartCell in row)
                {
                    //heartCell.Image.BeginAnimation(flipInAnimation);
                }
            }

        }

        private Rotation3DAnimation FlipInAnimation()
        {
            Rotation3D fromValue = new AxisAngleRotation3D(new Vector3D(1, 1, 0), 90);
            Rotation3D toValue = new AxisAngleRotation3D(new Vector3D(1, 1, 0), 0);
            var t = new Rotation3DAnimation(fromValue, toValue, new Duration(TimeSpan.FromSeconds(1)));
            return t;
        }

        private Rotation3DAnimation FlipOutAnimation()
        {
            Rotation3D fromValue = new AxisAngleRotation3D(new Vector3D(1, 1, 0), 0);
            Rotation3D toValue = new AxisAngleRotation3D(new Vector3D(1, 1, 0), 90);
            var t = new Rotation3DAnimation(fromValue, toValue, new Duration(TimeSpan.FromSeconds(1)));
            return t;
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