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
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for PlaylistsUserControl.xaml
    /// </summary>
    public partial class PlaylistsUserControl : UserControl
    {
        public PlaylistsUserControl()
        {
            InitializeComponent();
        }

        private void newPlayList_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void musicListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = musicListView.SelectedIndex;
           
        }

        string filename = @"PlayList";
        string personPath;
        List<string> playList = new List<string>();

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            personPath = Path.GetFullPath(filename);
            Debug.WriteLine(personPath);
            DirectoryInfo di = new DirectoryInfo(filename);
            
            //if (di.GetFiles("*.txt", SearchOption.AllDirectories) == null)
            //{
            //    Debug.WriteLine("null");
            //}
            FileInfo[] files = di.GetFiles("*.txt"); ;

            foreach (var file in files)
            {
                Debug.WriteLine($"{file}");
                playList.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
            //this.DataContext = playList;
            musicListView.ItemsSource = playList;
        }
    }
}
