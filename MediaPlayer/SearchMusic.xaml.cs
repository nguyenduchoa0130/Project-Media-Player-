using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using Path = System.IO.Path;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for SearchMusic.xaml
    /// </summary>
    public partial class SearchMusic
    {
        public delegate void MusicChangedHandler(ObservableCollection<Object> newObjects);
        public delegate void indexChangedHandler(int oldIndex);

        public event MusicChangedHandler MusicsChanged;
        public event indexChangedHandler IndexChanged;
        public ObservableCollection<Object> oldObjects { get; set; }
        public ObservableCollection<Object> Objects { get; set; }
        public ObservableCollection<Object> playList = new ObservableCollection<Object>();
        public int oldIndex { get; set; }

        public string oldKeywork { get; set; }

        public SearchMusic(ObservableCollection<Object> newObjects, string newKeywork)
        {
            InitializeComponent();
            oldObjects = newObjects;
            oldKeywork = newKeywork;
            Objects = newObjects;
            musicListView.ItemsSource = oldObjects;
            this.DataContext = oldObjects;
        }

        void UpdateObjects()
        {
            var _subItems = new ObservableCollection<Object>(Objects.Where(music => music.Name.Contains(oldKeywork)).ToList());

            oldObjects = _subItems;

            if (oldKeywork == "")
            {
                oldObjects = Objects;
            }

            MusicsChanged.Invoke(oldObjects);
            musicListView.ItemsSource = oldObjects;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.IndexChanged += (newIndex) =>
            {
                oldIndex = newIndex;
                musicListView.SelectedIndex = newIndex;
            };

            MainWindow.KeyworkChanged += (newKeywork) =>
            {
                oldKeywork = newKeywork;
                //Debug.WriteLine(oldKeywork);
                UpdateObjects();
            };
        }

        private void musicListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            oldIndex =  index;
            if (IndexChanged != null)
            {
                IndexChanged(oldIndex);
            }
        }

        private void NewPlayListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            var select = oldObjects[index];
            var screen = new NewPlayListWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            var result = screen.ShowDialog();

            if (result == true)
            {
                var personPath = Path.GetFullPath("PlayList");
                string file = $"{personPath}\\{screen.playList}.txt";
                string music = $"{select.Dir}|{select.Name}|{select.Extension}";
                //File.Create(file);
                File.WriteAllText(file, music);
            }
            else
            {

            }
        }

        private void CurrentPlayListMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            var select = oldObjects[index];
            var screen = new PlayListSelect { WindowStartupLocation = WindowStartupLocation.CenterScreen };
            var result = screen.ShowDialog();

            if (result == true)
            {
                var playList = screen.playListSelect;
                string music = $"{select.Dir}|{select.Name}|{select.Extension}";
                File.WriteAllTextAsync(playList, music);
            }
            else
            {

            }
        }
    }
}
