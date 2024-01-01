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

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for MyMusicUserControl.xaml
    /// </summary>
    public partial class MyMusicUserControl : UserControl
    {
        private bool _playing = false;
        private bool _shuffle = false;
        enum repeatMode
        {
            unrepeat,
            repeatone,
            repeatall
        }
        private int repeat = (int)repeatMode.unrepeat;

        MediaPlayer _mediaPlayer = new MediaPlayer();

        ObservableCollection<Object> Objects = new ObservableCollection<Object>();

        public string pathFile;
        List<string> listMusic = new List<string>();
        string filemusic = "";
        public MyMusicUserControl()
        {
            InitializeComponent();
            playButton.IsEnabled = false;
            skipNextButton.IsEnabled = false;
            skipPreviousButton.IsEnabled = false;
            shuffleButton.IsEnabled = false;
            repeatButton.IsEnabled = false;
        }
        public MyMusicUserControl()
        {
            InitializeComponent();
            init();
        }

        public class Object : INotifyPropertyChanged
        {
            private string name = "";
            private string dir = "";
            private string extension = "";
            public string Name
            {
                get => name; set
                {
                    name = value;
                    RaiseEvent();
                }
            }
            public string Extension
            {
                get => extension; set
                {
                    extension = value;
                    RaiseEvent();
                }
            }
            public string Dir
            {
                get => dir; set
                {
                    dir = value;
                    RaiseEvent();
                }
            }
            public event PropertyChangedEventHandler? PropertyChanged;

            void RaiseEvent([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        DispatcherTimer? _timer;
        private void addMusicFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;

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
                    if (listMusic != null)
                        filemusic += Path.GetFullPath(sFileName) + "\r";
                }
                System.IO.File.WriteAllText(@"D:\Nam4\Window\Project2\MediaPlayer\RecentPlays\recentPlaysList.txt", filemusic);

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

                if (musicListView.SelectedItem != null && mediaElement.Source == null)
                {
                    Object play = (Object)musicListView.SelectedItem;
                    mediaElement.Source = new Uri($"{play.Dir}{play.Name}");
                }

                updateSkipButton();

                if (mediaElement.Source != null)
                {
                    //while (!mediaElement.NaturalDuration.HasTimeSpan) { } // Đợi có timespan rồi chạy tiếp

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


            //progressSlider.Value = _mediaPlayer.Position.TotalSeconds;

            if (mediaElement.NaturalDuration.HasTimeSpan)
            {
                progressSlider.Value = mediaElement.Position.TotalSeconds;
            }

        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
			int hours = mediaElement.Position.Hours;
			int minutes = mediaElement.Position.Minutes;
			int seconds = mediaElement.Position.Seconds;

			currentPosition.Text = $"{hours}:{minutes}:{seconds}";


			//progressSlider.Value = _mediaPlayer.Position.TotalSeconds;

			if (mediaElement.NaturalDuration.HasTimeSpan)
			{
				progressSlider.Value = mediaElement.Position.TotalSeconds;
			}

		}

        private void skipNextButton_Click(object sender, RoutedEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            if (index < Objects.Count - 1)
            {
                musicListView.SelectedIndex += 1;
                index += 1;
                Object play = Objects[index];
                mediaElement.Source = new Uri($"{play.Dir}{play.Name}");
                updateSkipButton();

                _playing = false;

                playButton_Click(sender, e);


            }
            if (shuffleButton.IsEnabled == true)
                shuffleMode(sender, e);
        }

        private void skipPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            int index = musicListView.SelectedIndex;
            if (index > 0)
            {
                musicListView.SelectedIndex -= 1;
                index -= 1;
                Object play = Objects[index];
                mediaElement.Source = new Uri($"{play.Dir}{play.Name}");
                updateSkipButton();

                _playing = false;

                playButton_Click(sender, e);

            }
        }

        private void updateSkipButton()
        {
            int index = musicListView.SelectedIndex;
            skipNextButton.IsEnabled = true;
            skipPreviousButton.IsEnabled = true;

            if (index == 0)
            {
                skipPreviousButton.IsEnabled = false;
            }

            if (index == Objects.Count - 1 && shuffleButton.IsEnabled == false)
            {
                skipNextButton.IsEnabled = false;
            }
            if (shuffleButton.IsEnabled != false)
            {
                skipNextButton.IsEnabled = true;

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
                _playing = false;
                Object play = Objects[index];
                mediaElement.Source = new Uri($"{play.Dir}{play.Name}");

                playButton_Click(sender, e);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            musicListView.Items.RemoveAt(musicListView.Items.IndexOf(musicListView.SelectedItem));
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

            Object play = Objects[init];
            musicListView.SelectedIndex = init;
            _mediaPlayer.Open(new Uri($"{play.Dir}{play.Name}"));
            updateSkipButton();
            mediaElement.Source = new Uri($"{play.Dir}{play.Name}");
            mediaElement_MediaOpened(sender, e);
            mediaElement.Play();
            _playing = true;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Start();
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
        }

        private void playNextMusic(object sender, RoutedEventArgs e, int index)
        {
            Object play = Objects[index];
            _mediaPlayer.Open(new Uri($"{play.Dir}{play.Name}"));
            updateSkipButton();
            mediaElement.Source = new Uri($"{play.Dir}{play.Name}");
            mediaElement_MediaOpened(sender, e);
            mediaElement.Play();
            _playing = true;
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_shuffle != true)
            {
                if (repeat == (int)repeatMode.repeatone)
                {
                    int index = musicListView.SelectedIndex;
                    playNextMusic(sender, e, index);
                }

                if (repeat == (int)repeatMode.unrepeat)
                {
                    int index = musicListView.SelectedIndex;
                    if (index < Objects.Count - 1)
                    {
                        musicListView.SelectedIndex += 1;
                        index += 1;
                        playNextMusic(sender, e, index);
                    }
                }

                if (repeat == (int)repeatMode.repeatall)
                {
                    int index = musicListView.SelectedIndex;
                    if (index == Objects.Count - 1)
                    {
                        musicListView.SelectedIndex = 0;
                        index = 0;
                        playNextMusic(sender, e, index);
                    }
                    else if (index < Objects.Count - 1)
                    {
                        musicListView.SelectedIndex += 1;
                        index += 1;
                        playNextMusic(sender, e, index);
                    }
                }
            }
            else
            {
                if (repeat == (int)repeatMode.unrepeat || repeat == (int)repeatMode.repeatall)
                    shuffleMode(sender, e);
                if (repeat == (int)repeatMode.repeatone)
                {
                    int index = musicListView.SelectedIndex;
                    playNextMusic(sender, e, index);
                }
            }
        }
    }
}
