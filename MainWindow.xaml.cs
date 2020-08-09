using System;
using System.Collections.Generic;
using System.Linq;
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
using TileIconifier.Core.Shortcut;
using TileIconifier2._0.Pages;

namespace TileIconifier2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public static Frame AppFrame;
        public static List<ShortcutItem> ShortcutItems;
        public static List<ViewableShortcutItem> ViewableShortcutItems;
        public static bool IsPinningDataAvailable;
        public static TextBox ErrorTextBox;
        public static Storyboard ThrowExIn;
        public static Storyboard ThrowExOut;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            AppFrame = Frame;
            ErrorTextBox = ErrorMsg;
            ThrowExIn = (Storyboard)FindResource("ThrowExIn");
            ThrowExOut = (Storyboard)FindResource("ThrowExOut");
        }
    }
}
