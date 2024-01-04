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
using System.Reflection;
using System.Media;
using System.Windows.Threading;
using System.Security;
using System.IO;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for MyMusicUserControl.xaml
    /// </summary>
    public partial class MyMusicUserControl : UserControl
    {
        string filename = @"RecentPlays//recentPlaysList.txt";
        string personPath;

        List<string> listFileMusic; 
        private bool _playing = false;
        private bool _shuffle = false;
        enum repeatMode
        {
            unrepeat,
            repeatone,
            repeatall
        }
        private int repeat = (int)repeatMode.unrepeat;

        public ObservableCollection<Object> Objects = new ObservableCollection<Object>();

        DispatcherTimer? _timer;

        public MyMusicUserControl()
        {
            InitializeComponent();
            personPath = Path.GetFullPath(filename);
            listFileMusic = new List<string>(File.ReadAllLines(personPath));
            playButton.IsEnabled = false;
            skipNextButton.IsEnabled = false;
            skipPreviousButton.IsEnabled = false;
            shuffleButton.IsEnabled = false;
            repeatButton.IsEnabled = false;
        }

        private void addMusicFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            int index = musicListView.SelectedIndex;
            if (dialog.ShowDialog() == true)
            {
                foreach (string sFileName in dialog.FileNames)
                {
                    Objects.Add(new Object { Name = Path.GetFileName(sFileName), Dir = Path.GetDirectoryName(sFileName) + "\\", Extension = Path.GetExtension(sFileName) });
                    for (int i = 0; i < Objects.Count; i++)
                    {
                        for (int j = i + 1; j < Objects.Count; j++)
                        {
                            if (Objects[i].Name == Objects[j].Name)
                                Objects.Remove(Objects[j]);
                        }
                    }
                    //if (listMusic != null)
                    //    filemusic += Path.GetFullPath(sFileName) + "\r";
                }
                //System.IO.File.WriteAllText(@"D:\Nam4\Window\Project2\MediaPlayer\RecentPlays\recentPlaysList.txt", filemusic);

                musicListView.Items.Clear();
                foreach (Object obj in Objects)
                {
                    musicListView.Items.Add(obj);
                }
            }
            playButton.IsEnabled = true;
            shuffleButton.IsEnabled = true;
            repeatButton.IsEnabled = true;
            repeatIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.RepeatOff;
            shuffleIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ShuffleDisabled;
            musicListView.SelectedIndex = index;
            updateButton();
        }

        private void updateButton()
        {
            int index = musicListView.SelectedIndex;
            skipNextButton.IsEnabled = true;
            skipPreviousButton.IsEnabled = true;

            if (index == -1)
            {
                _playing = false;
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                skipPreviousButton.IsEnabled = false;
                skipNextButton.IsEnabled = false;
                return;
            }

            if (index == 0)
            {
                skipPreviousButton.IsEnabled = false;
            }

            if (index == Objects.Count - 1 && _shuffle == false)
            {
                skipNextButton.IsEnabled = false;
            }

            if (repeat == (int)repeatMode.repeatall || _shuffle == true || repeat == (int)repeatMode.repeatone)
            {
                skipNextButton.IsEnabled = true;
                skipPreviousButton.IsEnabled = true;
            }

            if (_playing)
            {
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
            }
            else
            {
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            }
        }

        private void playMusic(object sender, RoutedEventArgs e, int index)
        {
            Object play = Objects[index];
            mediaElement.Source = new Uri($"{play.Dir}{play.Name}");

            if (!listFileMusic.Contains($"{play.Dir}|{play.Name}|{play.Extension}"))
            {
                listFileMusic.Add($"{play.Dir}|{play.Name}|{play.Extension}");
                File.AppendAllText(personPath, $"{play.Dir}|{play.Name}|{play.Extension}\n");
            }

            mediaElement_MediaOpened(sender, e);
            mediaElement.Play();
            _playing = true;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Start();
            musicListView.SelectedIndex = index;
            updateButton();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playing)
            {
                mediaElement.Pause();
                _playing = false;
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                _timer?.Stop();
            }
            else
            {
                if (mediaElement.Source == null)
                {
                    int index = 0;
                    playMusic(sender, e, index);
                }

                if (mediaElement.Source != null)
                {
                    mediaElement_MediaOpened(sender, e);
                    mediaElement.Play();
                    _playing = true;
                    playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;

                    _timer = new DispatcherTimer();
                    _timer.Tick += _timer_Tick;
                    _timer.Start();
                }
            }
        }
       
        private void _timer_Tick(object? sender, EventArgs e)
        {
			int hours = mediaElement.Position.Hours;
			int minutes = mediaElement.Position.Minutes;
			int seconds = mediaElement.Position.Seconds;

			currentPosition.Text = $"{hours}:{minutes}:{seconds}";

			if (mediaElement.NaturalDuration.HasTimeSpan)
			{
				progressSlider.Value = mediaElement.Position.TotalSeconds;
			}
		}

        private void skipNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shuffle == true)
            {
                shuffleMode(sender, e);
            }
            else
            {
                int index = musicListView.SelectedIndex;
                index += 1;
                if (repeat == (int)repeatMode.repeatall)
                {
                    index %= Objects.Count;
                }
                else if (repeat == (int)repeatMode.repeatone)
                {
                    index -= 1;
                }
                playMusic(sender, e, index);
            }
        }

        private void skipPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            if (index > 0)
            {
                index -= 1;
                playMusic(sender, e, index);
            }
        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = progressSlider.Value;
            TimeSpan newPostition = TimeSpan.FromSeconds(value);

            mediaElement.Position = newPostition;
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                int hours = mediaElement.NaturalDuration.TimeSpan.Hours;
                int minutes = mediaElement.NaturalDuration.TimeSpan.Minutes;
                int seconds = mediaElement.NaturalDuration.TimeSpan.Seconds;

                totalPosition.Text = $"{hours}:{minutes}:{seconds}";

                progressSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void musicListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            if (index >= 0 && index < Objects.Count)
            {
                playMusic(sender, e, index);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int index = musicListView.Items.IndexOf(musicListView.SelectedItem);
            int currIndex = musicListView.SelectedIndex;
            musicListView.Items.RemoveAt(musicListView.Items.IndexOf(musicListView.SelectedItem));
            Object temp = Objects[index];
            listFileMusic.Remove($"{temp.Dir}|{temp.Name}|{temp.Extension}");
            File.WriteAllLines(personPath, listFileMusic);
            if (index == currIndex)
            {
                mediaElement.Source = null;
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                updateButton();
                //_playing = false;
                //playButton_Click(sender, e);
            }
            Objects.RemoveAt(index);
        }

        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (repeat == (int)repeatMode.repeatone)
            {
                repeat = (int)repeatMode.unrepeat;
                repeatIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.RepeatOff;
            }
            else if (repeat == (int)repeatMode.repeatall)
            {
                repeat = (int)repeatMode.repeatone;
                repeatIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.RepeatOne;
            }
            else
            {
                repeat = (int)repeatMode.repeatall;
                repeatIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Repeat;
            }
            updateButton();
        }

        private void shuffleMode(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int index = musicListView.SelectedIndex;
            int number = musicListView.Items.Count;

            int init;
            do
            {
                init = random.Next(number);
            } while (init == index);

            playMusic(sender, e, init);
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shuffle)
            {
                _shuffle = false;
                shuffleIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ShuffleDisabled;
            }
            else
            {
                _shuffle = true;
                shuffleIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Shuffle;
            }
            updateButton();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_shuffle != true)
            {
                if (repeat == (int)repeatMode.repeatone)
                {
                    int index = musicListView.SelectedIndex;
                    playMusic(sender, e, index);
                }

                if (repeat == (int)repeatMode.unrepeat)
                {
                    int index = musicListView.SelectedIndex;
                    if (index < Objects.Count - 1)
                    {
                        musicListView.SelectedIndex += 1;
                        index += 1;
                        playMusic(sender, e, index);
                    }
                }

                if (repeat == (int)repeatMode.repeatall)
                {
                    int index = musicListView.SelectedIndex + 1;
                    index %= Objects.Count;
                    playMusic(sender, e, index);
                }
            }
            else
            {
                if (repeat == (int)repeatMode.unrepeat || repeat == (int)repeatMode.repeatall)
                {
                    shuffleMode(sender, e);
                }

                if (repeat == (int)repeatMode.repeatone)
                {
                    int index = musicListView.SelectedIndex;
                    playMusic(sender, e, index);
                }
            }
        }
    }
}
