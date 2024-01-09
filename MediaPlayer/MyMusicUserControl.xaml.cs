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

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for MyMusicUserControl.xaml
    /// </summary>
    public partial class MyMusicUserControl : UserControl
    {
        public delegate void MusicChangedHandler(MusicList musics);
        public event MusicChangedHandler MusicsChanged;
        private MusicList _musics;

        public MyMusicUserControl(MusicList newMusics)
        {
            InitializeComponent();
            _musics = newMusics;
            //_musics.index = 1;
            this.DataContext = _musics;
        }

        private void addMusicFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            MusicList newMusics = new MusicList();

            if (_musics != null)
            {
                newMusics = (MusicList)_musics.Clone();
            }

            if (dialog.ShowDialog() == true)
            {
                foreach (string sFileName in dialog.FileNames)
                {
                    newMusics.Objects.Add(new Object { Name = Path.GetFileNameWithoutExtension(sFileName), Dir = Path.GetDirectoryName(sFileName) + "\\", Extension = Path.GetExtension(sFileName) });
                    for (int i = 0; i < newMusics.Objects.Count; i++)
                    {
                        for (int j = i + 1; j < newMusics.Objects.Count; j++)
                        {
                            if (newMusics.Objects[i].Name == newMusics.Objects[j].Name)
                                newMusics.Objects.Remove(newMusics.Objects[j]);
                        }
                    }
                }

                musicListView.Items.Clear();
                foreach (Object obj in newMusics.Objects)
                {
                    musicListView.Items.Add(obj);
                }
                //musicListView.SelectedIndex = newMusics.index;
            }

            if (MusicsChanged != null)
            {
                MusicsChanged.Invoke(newMusics);
                _musics = newMusics;
            }
        }
    }
}
