using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Path = System.IO.Path;
using System.Media;
using System.Security;
using System.IO;
using System.Diagnostics;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for MyMusicUserControl.xaml
    /// </summary>
    public partial class MyMusicUserControl
    {

        public delegate void MusicChangedHandler(ObservableCollection<Object> newObjects);
        public delegate void indexChangedHandler(int oldIndex);

        public event MusicChangedHandler MusicsChanged;
        public event indexChangedHandler IndexChanged;
        public ObservableCollection<Object> oldObjects { get; set; }

        public ObservableCollection<Object> playList = new ObservableCollection<Object>();
        public int oldIndex { get; set; }

        public MyMusicUserControl(ObservableCollection<Object> newObjects)
        {
            InitializeComponent();
            oldObjects = newObjects;
        }

        private void addMusicFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            if (oldObjects != null)
            {
                oldObjects = (ObservableCollection<Object>)oldObjects;
            }

            if (dialog.ShowDialog() == true)
            {
                foreach (string sFileName in dialog.FileNames)
                {
                   oldObjects.Add(new Object { Name = Path.GetFileNameWithoutExtension(sFileName), Dir = Path.GetDirectoryName(sFileName) + "\\", Extension = Path.GetExtension(sFileName) });
                    for (int i = 0; i < oldObjects.Count; i++)
                    {
                        for (int j = i + 1; j < oldObjects.Count; j++)
                        {
                            if (oldObjects[i].Name == oldObjects[j].Name)
                                oldObjects.Remove(oldObjects[j]);
                        }
                    }
                }

                musicListView.Items.Clear();
                foreach (Object obj in oldObjects)
                {
                    musicListView.Items.Add(obj);
                }
            }

            if (MusicsChanged != null)
            {
                MusicsChanged.Invoke(oldObjects);
                oldObjects = oldObjects;
                //oldIndex = newIndex;
            }
            

            string path = Path.GetFullPath(@"RecentPlays\\musics.txt");
            foreach(var obj in oldObjects)
            {
                File.AppendAllText(path, $"{obj.Dir}|{obj.Name}|{obj.Extension}\n");
            }
        }

        private void MenuRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            musicListView.Items.RemoveAt(index);    
            oldObjects.RemoveAt(index);
            if (MusicsChanged != null)
            {
                MusicsChanged.Invoke(oldObjects);
            }
        }

        private void musicListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            oldIndex = index;
            if (IndexChanged != null)
            {
                IndexChanged.Invoke(oldIndex);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //musicListView.SelectedIndex = MainWindow._index;
            MainWindow.IndexChanged += (newIndex) =>
            {
                oldIndex = newIndex;
                musicListView.SelectedIndex = newIndex;
            };

            musicListView.Items.Clear();
            foreach (Object obj in oldObjects)
            {
                musicListView.Items.Add(obj);
            }

            

            this.DataContext = new
            {
                ListMusic = oldObjects
            };
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
            var screen = new PlayListSelect { WindowStartupLocation= WindowStartupLocation.CenterScreen };
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
