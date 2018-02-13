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
using Button = System.Windows.Controls.Button;
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
        private byte _currentState = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        public List<List<HeartCell>> ImagesArray { get; set; } = new List<List<HeartCell>>();

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
                    var transform= new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
                    var vp2d3d = CreateViewPort(transform);
                    vp3d.Children.Add(vp2d3d);

                    vp3d.SetValue(Grid.ColumnProperty, i);
                    vp3d.SetValue(Grid.RowProperty, j);

                    TheGrid.Children.Add(vp3d);

                    var cell = new HeartCell() {ViewPort = vp2d3d, Transform= transform };
                    ImagesArray[i].Add(cell);
                }
            }

            _animationGlobalTimer = new Timer();

            _animationGlobalTimer.Interval = 5000;
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

        private Viewport2DVisual3D CreateViewPort(Transform3D transform)
        {
            var vp = new Viewport2DVisual3D();
            var geometry = new MeshGeometry3D();
            geometry.Positions = new Point3DCollection()
            {
                new Point3D(-1, 1, 0),
                new Point3D(-1, -1, 0),
                new Point3D(1, -1, 0),
                new Point3D(1, 1, 0),
            };
            geometry.TextureCoordinates = new PointCollection()
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(1, 1),
                new Point(1, 0),
            };
            geometry.TriangleIndices = new Int32Collection() {0, 1, 2, 0, 2, 3};
            vp.Geometry = geometry;

            var material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff)));
            Viewport2DVisual3D.SetIsVisualHostMaterial(material, true);
            vp.Material = material;

            vp.Transform = transform;

            var image = new Image {Source = OpacityMaskImage};
            vp.Visual = image;//new Button { Content = "Testing", Background = Brushes.Aqua };

            return vp;
        }

        private static PerspectiveCamera CreateCamera()
        {
            var camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 3);
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
            var pictureName= string.Empty;
            switch (_currentState)
            {
                case 0:
                    pictureName = "rose_PNG644";
                    break;
                case 1:
                    pictureName = "rose_PNG638";
                    break;
                case 2:
                    pictureName = "rose_PNG650";
                    break;
                case 3:
                    pictureName = "rose_PNG641";
                    break;
                case 4:
                    pictureName = "couple";
                    break;
                case 5:
                    pictureName = "rose_PNG642";
                    _currentState = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _currentState++;
            var pic = new BitmapImage(new Uri($"pack://application:,,,/SntValentineScreensaver;component/Images/{pictureName}.png", UriKind.Absolute));

            ChangePicture(pic);
        }

        private void ChangePicture(BitmapImage pic)
        {
            var flipOutAnimation = FlipOutAnimation();
            var flipInAnimation = FlipInAnimation();

            int i = 0;
            float totalDuration = 3000;
            var baseDelay = totalDuration / 2;
            foreach (var row in ImagesArray)
            {
                int j = 0;
                var rowDelay = baseDelay - (baseDelay * ((float) i / ImagesArray.Count));
                foreach (var heartCell in row)
                {
                    var cellDelay = (baseDelay * ((float) j / row.Count)) + rowDelay;
                    var beginTime = TimeSpan.FromMilliseconds(cellDelay);
                    var img = (Image)heartCell.ViewPort.Visual;

                    var flipOutStoryboard = new Storyboard();
                    flipOutStoryboard.BeginTime = beginTime;
                    Storyboard.SetTarget(flipOutAnimation, heartCell.ViewPort);
                    Storyboard.SetTargetProperty(flipOutAnimation, new PropertyPath("(Viewport2DVisual3D.Transform).(RotateTransform3D.Rotation)"));

                    flipOutStoryboard.Children.Add(flipOutAnimation);
                    flipOutStoryboard.Completed += (s, e) =>
                    {
                        img.Source = pic;
                        Storyboard.SetTarget(flipInAnimation, heartCell.ViewPort);
                        Storyboard.SetTargetProperty(flipInAnimation, new PropertyPath("(Viewport2DVisual3D.Transform).(RotateTransform3D.Rotation)"));
                        var flipInStoryboard = new Storyboard();
                        flipInStoryboard.Children.Add(flipInAnimation);
                        img.BeginStoryboard(flipInStoryboard);
                    };
                    img.BeginStoryboard(flipOutStoryboard);
                    j++;
                }
                i++;
            }

        }

        private Rotation3DAnimation FlipInAnimation()
        {
            Rotation3D fromValue = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90);
            Rotation3D toValue = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            var t = new Rotation3DAnimation(null, toValue, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.Stop);
            t.BeginTime = TimeSpan.FromSeconds(1);
            return t;
        }

        private Rotation3DAnimation FlipOutAnimation()
        {
            Rotation3D fromValue = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            Rotation3D toValue = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 100);
            var t = new Rotation3DAnimation(null, toValue, new Duration(TimeSpan.FromSeconds(1)), FillBehavior.HoldEnd);
            return t;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Application.Current.Shutdown();
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
        public Viewport2DVisual3D ViewPort { get; set; }
        public RotateTransform3D Transform { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}