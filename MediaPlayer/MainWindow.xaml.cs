﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace MediaPlayerNameSpace
{
	public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string Keywork { get; set; } = "";

        public event PropertyChangedEventHandler? PropertyChanged;


        public delegate void IndexChangedHandler(int newIndex);
        public static event IndexChangedHandler IndexChanged;

        public ObservableCollection<MediaPlayerName> Objects  = new ObservableCollection<MediaPlayerName>();
        public ObservableCollection<MediaPlayerName> PlayLists = new ObservableCollection<MediaPlayerName>();
        public static int _index { get; set; } = -1;
        private bool _playing = false;
		private bool _shuffle = false;
        DispatcherTimer? _timer;
		private List<string> listFileMusic;
		string personPath;
        string filename = @"RecentsList//recentPlaysList.txt";
        public delegate void KeyworkChangedHandler(string newKeywork);
        public static event KeyworkChangedHandler KeyworkChanged;
        public class Wrapper:INotifyPropertyChanged
        {
            public string CurrentTime { get; set; }
            public string TotalTime { get; set; } 

            public event PropertyChangedEventHandler? PropertyChanged;
        }
        Wrapper wrapper;

        enum repeatMode
        {
            unrepeat,
            repeatone,
            repeatall
        }
        private int repeat = (int)repeatMode.unrepeat;


        public MainWindow()
		{
			InitializeComponent();
           
        }

		private void buttonOpenMenu_Click(object sender, RoutedEventArgs e)
		{
			buttonCloseMenu.Visibility = Visibility.Visible;
			buttonOpenMenu.Visibility = Visibility.Collapsed;
			menuWidth!.Width = new GridLength(200);
		}

		private void buttonCloseMenu_Click(object sender, RoutedEventArgs e)
		{
			buttonCloseMenu.Visibility = Visibility.Collapsed;
			buttonOpenMenu.Visibility = Visibility.Visible;
			menuWidth.Width = new GridLength(60);
		}

        //public event PropertyChangedEventHandler? PropertyChanged;

        public void listViewMusic(object sender, RoutedEventArgs e, MediaPlayerName file)
        {
            Objects.Clear();
            listFileMusic = new List<string>(File.ReadAllLines($"{file.Dir}{file.Name}{file.Extension}"));

            foreach (string line in listFileMusic)
            {
                string[] temp = line.Split('|');
                if (File.Exists(@$"{temp[0]}{temp[1]}{temp[2]}"))
                {
                    Objects.Add(new MediaPlayerName
                    {
                        Dir = temp[0],
                        Name = temp[1],
                        Extension = temp[2]
                    });
                    for (int i = 0; i < Objects.Count; i++)
                    {
                        for (int j = i + 1; j < Objects.Count; j++)
                        {
                            if (Objects[i].Name == Objects[j].Name)
                                Objects.Remove(Objects[j]);
                        }
                    }
                }
            }

            var screen = new MyMusicUserControl(Objects);

            screen.IndexChanged += (newIndex) =>
            {
                _index = newIndex;
                playMusic(sender, e, _index);
            };

            screen.MusicsChanged += (newObjects) =>
            {
                Objects = newObjects;
            };

            GridMain.Children.Clear();
            GridMain.Children.Add(screen);
        }

        private void loadMusic()
        {
            string curPath = Path.GetFullPath(@"RecentsList//musics.txt");
            List<string> temp = new List<string>(File.ReadAllLines(curPath));

            foreach (string line in temp)
            {
                string[] cur = line.Split('|');
                if (File.Exists($@"{cur[0]}{cur[1]}{cur[2]}"))
                {
                    Objects.Add(new MediaPlayerName
                    {
                        Dir = cur[0],
                        Name = cur[1],
                        Extension = cur[2]
                    });

                    for (int i = 0; i < Objects.Count; i++)
                    {
                        for (int j = i + 1; j < Objects.Count; j++)
                        {
                            if (Objects[i].Name == Objects[j].Name)
                            {
                                Objects.Remove(Objects[j]);
                            }
                        }
                    }
                }
            }

            //File.WriteAllText(curPath, "");
            List<string> strs = new List<string>();
            foreach (var obj in Objects)
            {
                strs.Add($"{obj.Dir}|{obj.Name}|{obj.Extension}");
            }
            File.WriteAllLines(curPath, strs);
        }

        private void listViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GridMain.Children.Clear();
            Objects.Clear();
            _index = -1;
            myMediaElement.Source = null;
            updateButton();
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "SearchMusicItem":

                    loadMusic();

                    var screen4 = new SearchMusic(Objects, Keywork);
                    screen4.MusicsChanged += (newObjects) =>
                    {
                        Objects = newObjects;
                    };

                    screen4.IndexChanged += (newIndex) =>
                    {
                        _index = newIndex;
                        playMusic(sender, e, _index);
                    };

                    GridMain.Children.Add(screen4);


                    break;
                case "MyMusicItem":
                    loadMusic();
                    var screen1 = new MyMusicUserControl(Objects);
                    screen1.IndexChanged += (newIndex) =>
                    {
                        _index = newIndex;
                        playMusic(sender, e, _index);
                    };

                    screen1.MusicsChanged += (newObjects) =>
                    {
                        Objects = newObjects;
                    };


                    GridMain.Children.Add(screen1);

                    break;
                case "RecentPlaysItem":
                    var screen2 = new RecentPlaysUserControl(Objects);
                    screen2.MusicsChanged += (newObjects) =>
                    {
                        Objects = newObjects;
                    };

                    screen2.IndexChanged += (newIndex) =>
                    {
                        _index = newIndex;
                        playMusic(sender, e, _index);
                    };

                    GridMain.Children.Add(screen2);


                    break;
                case "PlaylistsItem":
                    var screen3 = new PlaylistsUserControl(PlayLists);
                    screen3.MusicsChanged += (newPlayLists) =>
                    {
                        PlayLists = newPlayLists;
                    };
                    screen3.IndexChanged += (newIndex) =>
                    {
                        _index = newIndex;
                        var file = PlayLists[_index];
                        listViewMusic(sender, e, file);
                    };

                    GridMain.Children.Add(screen3);
					break;
				default:
                    loadMusic();
                    var screen = new MyMusicUserControl(Objects);
                    screen.MusicsChanged += (newObjects) =>
                    {
                        Objects = newObjects;
                    };

                    screen.IndexChanged += (newIndex) =>
                    {
                        _index = newIndex;
                        playMusic(sender, e, _index);
                    };
                    GridMain.Children.Add(screen);
                    break;
            }
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
            wrapper = new Wrapper
            {
                CurrentTime = "00:00:00",
                TotalTime = "00:00:00"
            };
            DataContext = wrapper;
            personPath = Path.GetFullPath(filename);
            listFileMusic = new List<string>(File.ReadAllLines(personPath));
            repeatIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.RepeatOff;
            shuffleIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.ShuffleDisabled;

            //playButton.IsEnabled = false;
            skipPreviousButton.IsEnabled = false;
            skipNextButton.IsEnabled = false;
            repeatButton.IsEnabled = false;
            shuffleButton.IsEnabled = false;
            _shuffle = false;


            GridMain.Children.Clear();
            loadMusic();
            var screen = new MyMusicUserControl(Objects);
            GridMain.Children.Add(screen);
            screen.MusicsChanged += (NewObjects) =>
            {
                Objects = NewObjects;
            };
            screen.IndexChanged += (newIndex) =>
            {
                _index = newIndex;
                playMusic(sender, e, _index);
            };
            //this.DataContext = this;
        }

        private void updateButton()
        {
            skipPreviousButton.IsEnabled = true;
            shuffleButton.IsEnabled = true;
            skipNextButton.IsEnabled = true;
            repeatButton.IsEnabled = true;

            if (_index == -1)
            {
                _playing = false;
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                skipPreviousButton.IsEnabled = false;
                skipNextButton.IsEnabled = false;
                return;
            }

            if (_index == 0)
            {
                skipPreviousButton.IsEnabled = false;
            }

            if (_index == Objects.Count - 1 && _shuffle == false)
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

        private void _timer_Tick(object? sender, EventArgs e)
        {
            int hours = myMediaElement.Position.Hours;
            int seconds = myMediaElement.Position.Seconds;
            int minutes = myMediaElement.Position.Minutes;

            wrapper.CurrentTime = $"{hours}:{minutes}:{seconds}";

            if (myMediaElement.NaturalDuration.HasTimeSpan)
            {
                progressSlider.Value = myMediaElement.Position.TotalSeconds;
            }
        }

        private void playMusic(object sender, RoutedEventArgs e, int index)
		{
            if (index >= 0 && index < Objects.Count())
            {
                MediaPlayerName play = Objects[index];
                myMediaElement.Source = new Uri($"{play.Dir}{play.Name}{play.Extension}");
                musicName.Text = play.Name;

                if (!listFileMusic.Contains($"{play.Dir}|{play.Name}|{play.Extension}"))
                {
                    listFileMusic.Add($"{play.Dir}|{play.Name}|{play.Extension}");
                    File.AppendAllText(personPath, $"{play.Dir}|{play.Name}|{play.Extension}\n");
                }

                myMediaElement_MediaOpened(sender, e);
                myMediaElement.Play();
                _playing = true;
                _timer = new DispatcherTimer();
                _timer.Tick += _timer_Tick;
                _timer.Start();
                _index = index;

                if (IndexChanged != null)
                {
                    IndexChanged.Invoke(_index);
                }

                updateButton();
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (Objects.Count > 0)
            {
                if (_playing)
                {
                    myMediaElement.Pause();
                    _playing = false;
                    playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                    _timer?.Stop();
                }
                else
                {
                    if (myMediaElement.Source == null)
                    {
                        _index = 0;
                        playMusic(sender, e, _index);
                    }

                    if (myMediaElement.Source != null)
                    {
                        //mediaElement_MediaOpened(sender, e);
                        myMediaElement.Play();
                        _playing = true;
                        playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;

                        _timer = new DispatcherTimer();
                        _timer.Tick += _timer_Tick;
                        _timer.Start();
                    }
                }
            }
        }

        private void shuffleMode(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int number = Objects.Count;
            int init;
            do
            {
                init = random.Next(number);
            } while (init == _index);

            playMusic(sender, e, init);
        }

        private void skipNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_index >= 0 && _index < Objects.Count)
            {
                if (_shuffle == true)
                {
                    shuffleMode(sender, e);
                }
                else
                {
                    int index = _index;
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
            else
            {
                _index = Objects.Count - 1;
            }
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

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = progressSlider.Value;
            TimeSpan newPostition = TimeSpan.FromSeconds(value);

            myMediaElement.Position = newPostition;
        }

        private void skipPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_shuffle == true)
            {
                shuffleMode(sender, e);
            }
            else
            {
                int index = _index;
                index -= 1;
                if (repeat == (int)repeatMode.repeatall)
                {
                    index = (index + Objects.Count) % Objects.Count;
                }
                else if (repeat == (int)repeatMode.repeatone)
                {
                    index += 1;
                }
                playMusic(sender, e, index);
            }
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


        private void myMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (myMediaElement.NaturalDuration.HasTimeSpan)
            {
                int hours = myMediaElement.NaturalDuration.TimeSpan.Hours;
                int minutes = myMediaElement.NaturalDuration.TimeSpan.Minutes;
                int seconds = myMediaElement.NaturalDuration.TimeSpan.Seconds;

                //totalPosition.Text = $"{hours}:{minutes}:{seconds}";
                wrapper.TotalTime = $"{hours}:{minutes}:{seconds}";

                progressSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Keywork = searchMusic.Text;
            if (KeyworkChanged != null)
            {
                KeyworkChanged.Invoke(Keywork);
            }
        }

        private void myMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_shuffle != true)
            {
                if (repeat == (int)repeatMode.repeatone)
                {
                    int index = _index;
                    playMusic(sender, e, index);
                }

                if (repeat == (int)repeatMode.unrepeat)
                {
                    int index = _index;
                    if (index < Objects.Count - 1)
                    {
                        _index += 1;
                        index += 1;
                        playMusic(sender, e, index);
                    }
                }

                if (repeat == (int)repeatMode.repeatall)
                {
                    int index = _index + 1;
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
                    int index = _index;
                    playMusic(sender, e, index);
                }
            }
        }
    }
}
