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

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for RecentPlaysUserControl.xaml
    /// </summary>
    public partial class RecentPlaysUserControl 
    {
        public delegate void MusicChangedHandler(ObservableCollection<Object> newObjects);
        public delegate void indexChangedHandler(int oldIndex);

        public event MusicChangedHandler MusicsChanged;
        public event indexChangedHandler IndexChanged;

        string filename = @"RecentPlays//recentPlaysList.txt";
        string personPath;
        List<string> listFileMusic;
        ObservableCollection<Object> oldObjects { get; set; }

        public RecentPlaysUserControl(ObservableCollection<Object> newObjects)
        {
            InitializeComponent();
            oldObjects = newObjects;
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Object> newObjects = new ObservableCollection<Object>();

            personPath = Path.GetFullPath(filename);
            listFileMusic = new List<string>(File.ReadAllLines(personPath));

            foreach (string line in listFileMusic)
            {
                string[] temp = line.Split('|');
                if (File.Exists(@$"{temp[0]}{temp[1]}{temp[2]}"))
                {
                    newObjects.Add(new Object
                    {
                        Dir = temp[0],
                        Name = temp[1],
                        Extension = temp[2]
                    });
                    for (int i = 0; i < newObjects.Count; i++)
                    {
                        for (int j = i + 1; j < newObjects.Count; j++)
                        {
                            if (newObjects[i].Name == newObjects[j].Name)
                                newObjects.Remove(newObjects[j]);
                        }
                    }
                }
            }

            musicListView.Items.Clear();
            foreach (Object obj in newObjects)
            {
                musicListView.Items.Add(obj);
            }

            oldObjects = newObjects;

            if (MusicsChanged != null)
            {   
                MusicsChanged.Invoke(newObjects);
            }

            MainWindow.IndexChanged += (newIndex) =>
            {
                musicListView.SelectedIndex = newIndex;
            };
        }

        private void musicListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            if (IndexChanged != null)
            {
                IndexChanged.Invoke(index);
            }
        }

        private void MenuRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            musicListView.Items.RemoveAt(index);
            oldObjects.RemoveAt(index);
            listFileMusic.RemoveAt(index);
            if (MusicsChanged != null)
            {
                MusicsChanged.Invoke(oldObjects);
            }

            File.WriteAllLines(personPath, listFileMusic);
        }
    }
}
