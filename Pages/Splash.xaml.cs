using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TileIconifier.Core.Shortcut;

namespace TileIconifier2._0.Pages
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Page
    {
        public static Splash instance;
        public Splash()
        {
            InitializeComponent();
            instance = this;
        }

        private void page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    GetStuff();
                    Dispatcher.Invoke(() =>
                    {
                        ParseStuff();
                    });
                    Task.Delay(100).ContinueWith(x =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Storyboard s = (Storyboard)FindResource("FinishAnimation");
                            EventHandler h = null;
                            h = (g, y) =>
                            {
                                MainWindow.AppFrame.Navigate(new Pages.MainPage());
                                s.Completed -= h;
                            };
                            s.Completed += h;
                            s.Begin();
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                MainWindow.ErrorTextBox.Text = $"--- ERROR ---\n{ex.Message}";
                MainWindow.ThrowExIn.Begin();
                Task.Delay(4000).ContinueWith(x =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainWindow.ThrowExOut.Begin();
                    });
                });
            }
        }
        public void GetStuff()
        {
            try
            {
                Exception pinningException;
                MainWindow.ShortcutItems = ShortcutItemEnumeration.TryGetShortcutsWithPinning(out pinningException, true);
                if (pinningException != null)
                {
                    throw pinningException;
                }
                MainWindow.IsPinningDataAvailable = true;
            }
            catch (Exception)
            {
                MainWindow.ShortcutItems = ShortcutItemEnumeration.GetShortcuts();
                MainWindow.IsPinningDataAvailable = false;
            }
        }
        public void ParseStuff()
        {
            try
            {
                MainWindow.ViewableShortcutItems = new List<ViewableShortcutItem>();
                foreach (ShortcutItem s in MainWindow.ShortcutItems)
                {
                    ViewableShortcutItem si = new ViewableShortcutItem()
                    {
                        ShortcutItem = s,
                        Name = s.ShortcutFileInfo.Name.Substring(0, s.ShortcutFileInfo.Name.Length - s.ShortcutFileInfo.Extension.Length)
                    };
                    si.Path = ImageSourceFromBitmap(s.StandardIcon);
                    MainWindow.ViewableShortcutItems.Add(si);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MainWindow.ErrorTextBox.Text = $"--- ERROR ---\n{ex.Message}";
                    MainWindow.ThrowExIn.Begin();
                    Task.Delay(4000).ContinueWith(x =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MainWindow.ThrowExOut.Begin();
                        });
                    });
                });
            }
        }
        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
    public class ViewableShortcutItem
    {
        public ShortcutItem ShortcutItem;
        public string Name { get; set; }
        public ImageSource Path { get; set; }
        public override string ToString()
        {
            return ShortcutItem.ShortcutFileInfo.Name.Substring(0, ShortcutItem.ShortcutFileInfo.Name.Length - ShortcutItem.ShortcutFileInfo.Extension.Length);
        }
    }
}
