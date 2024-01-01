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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Path = System.IO.Path;
using System.Reflection;
using System.Media;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for RecentPlaysUserControl.xaml
    /// </summary>
    public partial class RecentPlaysUserControl : UserControl
    {
        public RecentPlaysUserControl()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string line in File.ReadAllLines(@"D:\Nam4\Window\Project2\MediaPlayer\RecentPlays\recentPlaysList.txt"))
            {
                recentPlaysListView.Items.Add(line);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            recentPlaysListView.Items.RemoveAt(recentPlaysListView.Items.IndexOf(recentPlaysListView.SelectedItem));
        }

        private void skipPreviousButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void skipNextButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void musicListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
