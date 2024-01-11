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
using MaterialDesignThemes.Wpf;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for PlaylistsUserControl.xaml
    /// </summary>
    public partial class PlaylistsUserControl
    {
        public delegate void MusicChangedHandler(ObservableCollection<Object> newObjects);
        public delegate void indexChangedHandler(int oldIndex);

        public event MusicChangedHandler MusicsChanged;
        public event indexChangedHandler IndexChanged;

        string filename = @"PlayList";
        string personPath;
        List<string> listFileMusic;

        public ObservableCollection<Object> oldObjects { get; set; }  

        public PlaylistsUserControl(ObservableCollection<Object> newObjects)
        {
            InitializeComponent();
            oldObjects = newObjects;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            personPath = Path.GetFullPath(filename);
            //Debug.WriteLine(personPath);
            DirectoryInfo di = new DirectoryInfo(filename);
            
            FileInfo[] files = di.GetFiles("*.txt"); ;

            foreach (var file in files)
            {
                //Debug.WriteLine($"{file}");
                if (!File.Exists(file.FullName)) continue;
                oldObjects.Add(new Object
                {
                    Name = Path.GetFileNameWithoutExtension(file.FullName),
                    Dir = Path.GetDirectoryName(file.FullName) + "\\",
                    Extension = Path.GetExtension(file.FullName)
                });

                for (int i = 0; i < oldObjects.Count; i++)
                {
                    for (int j = i + 1; j < oldObjects.Count; j++)
                    {
                        if (oldObjects[i].Name == oldObjects[j].Name)
                        {
                            oldObjects.Remove(oldObjects[j]);
                        }
                    }
                }
            }

            playListListView.Items.Clear();
            foreach (Object obj in oldObjects)
            {
                playListListView.Items.Add(obj);
            }

            if (MusicsChanged != null)
            {
                MusicsChanged.Invoke(oldObjects);
            }
        }

        private void newPlayList_Click(object sender, RoutedEventArgs e)
        {

        }

        private void playListListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = playListListView.SelectedIndex;
            if (IndexChanged != null)
            {
                IndexChanged.Invoke(index);
            }
        }

        private void MenuRemoveItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
