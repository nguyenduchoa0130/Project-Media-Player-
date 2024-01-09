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
    public partial class RecentPlaysUserControl : UserControl
    {

        public delegate void MusicChangedHandler(MusicList musics);
        public event MusicChangedHandler MusicsChanged;

        string filename = @"RecentPlays//recentPlaysList.txt";
        string personPath;
        List<string> listFileMusic;

        MusicList oldMusic;
        public RecentPlaysUserControl(MusicList newMusics)
        {
            InitializeComponent();
            oldMusic = newMusics;
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MusicList newMusics = new MusicList();
            personPath = Path.GetFullPath(filename);
            listFileMusic = new List<string>(File.ReadAllLines(personPath));

            personPath = Path.GetFullPath(filename);
            foreach (string line in listFileMusic)
            {
                string[] temp = line.Split('|');
                if (File.Exists(@$"{temp[0]}{temp[1]}{temp[2]}"))
                {
                    newMusics.Objects.Add(new Object
                    {
                        Dir = temp[0],
                        Name = temp[1],
                        Extension = temp[2]
                    });
                    for (int i = 0; i < newMusics.Objects.Count; i++)
                    {
                        for (int j = i + 1; j < newMusics.Objects.Count; j++)
                        {
                            if (newMusics.Objects[i].Name == newMusics.Objects[j].Name)
                                newMusics.Objects.Remove(newMusics.Objects[j]);
                        }
                    }
                }
            }

            musicListView.Items.Clear();
            foreach (Object obj in newMusics.Objects)
            {
                musicListView.Items.Add(obj);
            }

            if (MusicsChanged != null)
            {
                MusicsChanged.Invoke(newMusics);
                oldMusic = newMusics;
            }
            
        }
    }
}
