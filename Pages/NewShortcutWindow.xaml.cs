using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shapes;
using TileIconifier.Core.Custom;
using TileIconifier.Core.Custom.Builder;
using TileIconifier.Core.Shortcut;
using TileIconifier.Core.TileIconify;

namespace TileIconifier2._0.Pages
{
    /// <summary>
    /// Interaction logic for NewShortcutWindow.xaml
    /// </summary>
    public partial class NewShortcutWindow : Window
    {
        public Storyboard EgsShow;
        public Storyboard CustomShow;
        public Storyboard SteamShow;

        public NewShortcutWindow()
        {
            InitializeComponent();
            EgsShow = (Storyboard)FindResource("EgsShow");
            CustomShow = (Storyboard)FindResource("CustomShow");
            SteamShow = (Storyboard)FindResource("SteamShow");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var uri = url.Text;
            var parameters = GenerateParams(uri, "", true);
            try
            {
                GenerateShortcut(new UriCustomShortcutBuilder(parameters), name.Text);
            } catch (Exception)
            {
                MessageBox.Show("Error");
            }
            this.Close();
        }

        private GenerateCustomShortcutParams GenerateParams(string shortcutTarget, string shortcutArguments,
            bool useImage = false)
        {
            return new GenerateCustomShortcutParams(shortcutTarget, shortcutArguments, CustomShortcutGetters.CustomShortcutCurrentUserPath)
            {
                Image = null
            };
        }

        private void GenerateShortcut(BaseCustomShortcutBuilder baseCustomShortcutBuilder, string shname)
        {
            var shortcutName = GetSafeFilename(shname);
            try
            {
                var customShortcut = baseCustomShortcutBuilder.GenerateCustomShortcut(shortcutName);
                BuildIconifiedTile(ImageToByteArray(Properties.Resources.defa), customShortcut);
            }
            catch (Exception)
            {

            }
        }

        private static void BuildIconifiedTile(byte[] imageToUse, CustomShortcut customShortcut)
        {
            //Iconify a TileIconifier shortcut for this with default settings
            var newShortcutItem = customShortcut.ShortcutItem;
            newShortcutItem.Properties.CurrentState.MediumImage.SetImage(imageToUse,
                ShortcutConstantsAndEnums.MediumShortcutDisplaySize);
            newShortcutItem.Properties.CurrentState.SmallImage.SetImage(imageToUse,
                ShortcutConstantsAndEnums.SmallShortcutDisplaySize);
            var iconify = new TileIcon(newShortcutItem);
            iconify.RunIconify();
        }

        public static byte[] ImageToByteArray(System.Drawing.Bitmap imageIn)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(imageIn.Clone(), typeof(byte[]));
        }

        private void customlogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CustomShow.Begin();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (egscombobox.SelectedItem == null) return;
            EpicGamesLauncherVideoGame ev = (EpicGamesLauncherVideoGame)egscombobox.SelectedItem;
            var parameters = GenerateParams(ev.GetLaunchUri(), "", true);
            try
            {
                GenerateShortcut(new UriCustomShortcutBuilder(parameters), ev.DisplayName);
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
            this.Close();
        }

        private void egslogo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var games = EpicGamesService.GetInstalledVideoGames();
            egscombobox.ItemsSource = games;
            EgsShow.Begin();
        }

        public string GetSafeFilename(string filename)
        {

            return string.Join("", filename.Split(System.IO.Path.GetInvalidFileNameChars()));

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (steamcombobox.SelectedItem == null) return;
            SteamAppState ev = (SteamAppState)steamcombobox.SelectedItem;
            var parameters = GenerateParams(ev.GetLaunchUri(), "", true);
            try
            {
                GenerateShortcut(new UriCustomShortcutBuilder(parameters), ev.name);
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
            this.Close();
        }

        private void steamlogo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var games = SteamService.GetSteamVideoGames();
            steamcombobox.ItemsSource = games;
            SteamShow.Begin();
        }
    }
}
