using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using TileIconifier.Core.Shortcut;
using TileIconifier.Core.TileIconify;
using System.Windows.Documents;
using System.Threading.Tasks;

namespace TileIconifier2._0.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public bool IsInit = false;
        public Storyboard NoResultsIn;
        public Storyboard NoResultsOut;
        public Storyboard ShowPSH;
        public Storyboard HidePSH;
        public Storyboard HideContent;
        public Storyboard ShowContent;
        public Paragraph PowershellConsole;
        public SolidColorBrush PSHConsoleForeground;
        public SolidColorBrush PSHConsoleBackground;

        public static Rectangle ShortcutImage;

        public MainPage()
        {
            InitializeComponent();
            NoResultsIn = (Storyboard)FindResource("NoResultsIn");
            NoResultsOut = (Storyboard)FindResource("NoResultsOut");
            ShowPSH = (Storyboard)FindResource("ShowPSH");
            HidePSH = (Storyboard)FindResource("HidePSH");
            HideContent = (Storyboard)FindResource("HideContent");
            ShowContent = (Storyboard)FindResource("ShowContent");
            PowershellConsole = new Paragraph();
            PSHConsole.Document = new FlowDocument(PowershellConsole);
            PSHConsoleForeground = (SolidColorBrush)PSHConsole.Foreground;
            PSHConsoleBackground = (SolidColorBrush)PSHConsole.Background;
            ShortcutImage = LargeThumb;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public void WriteLine(string text, SolidColorBrush foreground, SolidColorBrush background)
        {
            PowershellConsole.Inlines.Add(new Run(text)
            {
                Foreground = foreground,
                Background = background
            });
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShortcutListViewer.Items.Clear();
            playstationResultsView.Items.Clear();
            ShortcutListViewer.ItemsSource = MainWindow.ViewableShortcutItems;
            IsInit = true;
        }

        private void ShortcutListViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ViewableShortcutItem v = (ViewableShortcutItem)e.AddedItems[0];
                ImageSource s = v.Path;
                if (v.ShortcutItem.IsIconified)
                {
                    s = new BitmapImage(new Uri(v.ShortcutItem.FullMediumIconPath));
                }
                BrushAnimation b = new BrushAnimation()
                {
                    To = new ImageBrush(s),
                    Duration = new Duration(TimeSpan.FromMilliseconds(350))
                };
                LargeThumb.BeginAnimation(Rectangle.FillProperty, b);
                IsIconified.Text = v.ShortcutItem.IsIconified ? "IsIconified: Yes" : "IsIconified: No";
                if (v.ShortcutItem.IsPinned != null)
                    IsPinned.Text = (bool)v.ShortcutItem.IsPinned ? "IsPinned: Yes" : "IsPinnned: No";
                else
                    IsPinned.Text = "IsPinned: No";
                ShortcutName.Text = v.Name;
                ShortcutName.IsEnabled = true;
            } catch (Exception) { }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsInit) return;
            ShortcutListViewer.ItemsSource = from Shortcuts in MainWindow.ViewableShortcutItems where Shortcuts.Name.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0 select Shortcuts;
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchPS(PSSearch.Text);
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            SearchPS(ShortcutName.Text);
        }
        public void SearchPS(string query)
        {
            NoResultsOut.Begin();
            playstationResultsView.ItemsSource = null;
            query = WebUtility.UrlEncode(query);
            string url = $"https://store.playstation.com/store/api/chihiro/00_09_000/tumbler/SA/en/999/{query}?suggested_size=128&mode=game";
            string body = Get(url);
            dynamic g = JsonConvert.DeserializeObject(body);
            List<SearchResultItem> s = new List<SearchResultItem>();
            bool gotResult = false;
            foreach (dynamic o in g.links)
            {
                gotResult = true;
                try
                {
                    s.Add(new SearchResultItem()
                    {
                        Title = o.name,
                        ImageUrl = o.images[0].url
                    });
                }
                catch (Exception) { }
            }
            if (gotResult)
                playstationResultsView.ItemsSource = s;
            else
            {
                NoResults.Text = $"No Results For: \"{WebUtility.UrlDecode(query)}\"";
                NoResultsIn.Begin();
            }
        }
        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public string ReadyFileName(string name)
        {
            return Regex.Replace(name, "[^a-zA-Z0-9`~!@#$%^&()\\-_=+\\[\\]{};',. ]+", "", RegexOptions.Compiled);
        }
        private void playstationResultsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var d = (SearchResultItem)playstationResultsView.SelectedItem;
                if (d == null) return;
                BrushAnimation b = new BrushAnimation()
                {
                    To = new ImageBrush(d.ImageUrl),
                    Duration = new Duration(TimeSpan.FromMilliseconds(350))
                };
                LargeThumb.BeginAnimation(Rectangle.FillProperty, b);
            } catch (Exception) { }
        }

        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Clipboard.ContainsImage())
            {
                IDataObject clipboardData = Clipboard.GetDataObject();
                if (clipboardData != null)
                {
                    if (clipboardData.GetDataPresent(DataFormats.Bitmap))
                    {
                        try
                        {
                            BrushAnimation b = new BrushAnimation()
                            {
                                To = new ImageBrush(Clipboard.GetImage()),
                                Duration = new Duration(TimeSpan.FromMilliseconds(350))
                            };
                            LargeThumb.BeginAnimation(Rectangle.FillProperty, b);
                        } catch (Exception) { }
                    }
                }
            }

            try
            {
                var d = (SearchResultItem)playstationResultsView.SelectedItem;
                if (d == null) return;
            }
            catch (Exception) { }
        }

        private async void TextBlock_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            ShowPSH.Begin();
            PowershellConsole.Inlines.Clear();
            WriteLine("Readying Tile Iconifier... ", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("3. ", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(1000);
            WriteLine("2. ", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(1000);
            WriteLine("1. ", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(1000);
            WriteLine("\n", PSHConsoleForeground, PSHConsoleBackground);
            WriteLine("\nCore build started...", new SolidColorBrush(Color.FromRgb(255,255,0)), PSHConsoleBackground);
            WriteLine("\nPacking LNK nodes...", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(100);
            WriteLine("\nLoaded Windows.FileSystem", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(50);
            WriteLine("\nLoaded Windows.Media.Imaging", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(50);
            WriteLine("\nLoaded Windows.Start.Tiles", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(250);
            WriteLine("\nLoaded Windows.Shell.Api", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(60);
            WriteLine("\nLoaded Windows.Shell.Users", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(10);
            WriteLine("\nLoaded Windows.Permissions.Networking", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(150);
            WriteLine("\nDownloading full resoloution image from the playstation network...", PSHConsoleForeground, PSHConsoleBackground);
            WriteLine("\n2000x2000 - [https://telno.apollo.playstation.com/api/shegu/store/titles/090cv996769878cv70b?mdr=1", new SolidColorBrush(Color.FromRgb(60,60,60)), PSHConsoleBackground);
            WriteLine("\n#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(100);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(100);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(100);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(300);
            WriteLine("#", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(150);
            WriteLine("\nEncoding png to bmp...", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(500);
            WriteLine("\nComplete", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(100);
            WriteLine("\nRegistering Changes With Windows...", new SolidColorBrush(Color.FromRgb(0,0,0)), new SolidColorBrush(Color.FromRgb(255,255,255)));
            try
            {
                byte[] imageToUse = ImageSourceToBytes(new JpegBitmapEncoder(), ((ImageBrush)LargeThumb.Fill).ImageSource);
                ViewableShortcutItem v = (ViewableShortcutItem)ShortcutListViewer.SelectedItem;
                var newShortcutItem = v.ShortcutItem;
                newShortcutItem.Properties.CurrentState.MediumImage.SetImage(imageToUse,
                    ShortcutConstantsAndEnums.MediumShortcutDisplaySize);
                newShortcutItem.Properties.CurrentState.SmallImage.SetImage(imageToUse,
                    ShortcutConstantsAndEnums.SmallShortcutDisplaySize);
                var iconify = new TileIcon(newShortcutItem);
                iconify.RunIconify();
                var dir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(newShortcutItem.ShortcutFileInfo.DirectoryName);
                File.Move(newShortcutItem.ShortcutFileInfo.Name, ReadyFileName(ShortcutName.Text) + ".lnk");
                Directory.SetCurrentDirectory(dir);
            }
            catch (Exception ex)
            {
                WriteLine("\nFATAL ERROR", new SolidColorBrush(Color.FromRgb(255, 255, 255)), new SolidColorBrush(Color.FromRgb(255, 0, 0)));
                WriteLine(ex.Message, new SolidColorBrush(Color.FromRgb(255, 0, 0)), PSHConsoleBackground);
            }
            await Task.Delay(600);
            WriteLine("\nRegistered", PSHConsoleForeground, PSHConsoleBackground);
            await Task.Delay(50);
            WriteLine("\nWaiting for next system thumbnail refresh...", new SolidColorBrush(Color.FromRgb(255,255,0)), PSHConsoleBackground);
            Pages.Splash.instance.GetStuff();
            Pages.Splash.instance.ParseStuff();
            ShortcutListViewer.ItemsSource = from Shortcuts in MainWindow.ViewableShortcutItems where Shortcuts.Name.IndexOf(Search.Text, StringComparison.OrdinalIgnoreCase) >= 0 select Shortcuts;
            await Task.Delay(500);
            WriteLine("\nRefresh Complete, Tile Iconified Successfully", new SolidColorBrush(Color.FromRgb(0,255,0)), new SolidColorBrush(Color.FromRgb(0,0,0)));
            await Task.Delay(300);
            HidePSH.Begin();
        }
        public byte[] ImageSourceToBytes(BitmapEncoder encoder, ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        private void TextBlock_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            NewShortcutWindow n = new NewShortcutWindow();
            n.ShowDialog();
            EventHandler eh = null;
            eh = (x, y) =>
            {
                HideContent.Completed -= eh;
                Pages.Splash.instance.GetStuff();
                Pages.Splash.instance.ParseStuff();
                ShowContent.Begin();
            };
            HideContent.Completed += eh;
            HideContent.Begin();
        }

        private void LargeThumb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
    public class SearchResultItem
    {
        public string Title { get; set; }
        public ImageSource ImageUrl { get; set; }
    }
    public class BrushAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType
        {
            get
            {
                return typeof(Brush);
            }
        }

        public override object GetCurrentValue(object defaultOriginValue,
                                               object defaultDestinationValue,
                                               AnimationClock animationClock)
        {
            return GetCurrentValue(defaultOriginValue as Brush,
                                   defaultDestinationValue as Brush,
                                   animationClock);
        }
        public object GetCurrentValue(Brush defaultOriginValue,
                                      Brush defaultDestinationValue,
                                      AnimationClock animationClock)
        {
            if (!animationClock.CurrentProgress.HasValue)
                return Brushes.Transparent;

            //use the standard values if From and To are not set 
            //(it is the value of the given property)
            defaultOriginValue = this.From ?? defaultOriginValue;
            defaultDestinationValue = this.To ?? defaultDestinationValue;

            if (animationClock.CurrentProgress.Value == 0)
                return defaultOriginValue;
            if (animationClock.CurrentProgress.Value == 1)
                return defaultDestinationValue;

            return new VisualBrush(new Border()
            {
                Width = 1,
                Height = 1,
                Background = defaultOriginValue,
                Child = new Border()
                {
                    Background = defaultDestinationValue,
                    Opacity = animationClock.CurrentProgress.Value,
                }
            });
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }

        //we must define From and To, AnimationTimeline does not have this properties
        public Brush From
        {
            get { return (Brush)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        public Brush To
        {
            get { return (Brush)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(Brush), typeof(BrushAnimation));
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(Brush), typeof(BrushAnimation));
    }
}
