using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace SntValentineScreensaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HwndSource winWPFContent;
        private static Stopwatch _currentStopwatch;

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s"))
            {
                foreach (var s in Screen.AllScreens)
                {
                    Window window = null;
                    if (Equals(s, Screen.PrimaryScreen))
                    {
                        window = new MainWindow();
                    }
                    else
                    {
                        window = new EmptyWindow();
                    }
                    window.Left = s.WorkingArea.Left;
                    window.Top = s.WorkingArea.Top;
                    window.Width = s.WorkingArea.Width;
                    window.Height = s.WorkingArea.Height;
                    window.Show();

                    _currentStopwatch = new Stopwatch();
                    _currentStopwatch.Start();
                }
            }
            else if (e.Args[0].ToLower().StartsWith("/p"))
            {
                MainWindow window = new MainWindow();
                Int32 previewHandle = Convert.ToInt32(e.Args[1]);
                IntPtr pPreviewHnd = new IntPtr(previewHandle);
                RECT lpRect = new RECT();
                bool bGetRect = Win32API.GetClientRect(pPreviewHnd, ref lpRect);

                HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams");

                sourceParams.PositionX = 0;
                sourceParams.PositionY = 0;
                sourceParams.Height = lpRect.Bottom - lpRect.Top;
                sourceParams.Width = lpRect.Right - lpRect.Left;
                sourceParams.ParentWindow = pPreviewHnd;
                sourceParams.WindowStyle = (int)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN);

                winWPFContent = new HwndSource(sourceParams);
                winWPFContent.Disposed += (o, args) => window.Close();
                winWPFContent.RootVisual = window.MainGrid;

            }
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
            }
        }

        public static void ShutdownIfTimeout()
        {
            if (_currentStopwatch.Elapsed > TimeSpan.FromSeconds(10))
            {
                Application.Current.Shutdown();
            }
        }
    }
}
