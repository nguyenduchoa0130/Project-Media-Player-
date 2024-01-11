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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for MusicScreen.xaml
    /// </summary>
    public partial class MusicScreen : UserControl
    {
        public MusicScreen(Uri source)
        {
            InitializeComponent();
            myMediaElement.Source = source;
            myMediaElement.Play();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void myMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {

        }
    }
}
