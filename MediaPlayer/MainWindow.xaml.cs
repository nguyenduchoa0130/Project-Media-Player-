using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Microsoft.Win32;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Threading;
using Path = System.IO.Path;


namespace MediaPlayerNameSpace
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public delegate void IndexChangedHandler(int newIndex);
        public static event IndexChangedHandler IndexChanged;

        public ObservableCollection<Object> Objects  = new ObservableCollection<Object>();
        public ObservableCollection<Object> PlayLists = new ObservableCollection<Object>();
        public static int _index { get; set; } = -1;
        private bool _playing = false;
		private bool _shuffle = false;
        DispatcherTimer? _timer;
		private List<string> listFileMusic;
		string personPath;
        string filename = @"RecentPlays//recentPlaysList.txt";
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

        public void listViewMusic(object sender, RoutedEventArgs e, Object file)
        {
            Objects.Clear();
            listFileMusic = new List<string>(File.ReadAllLines($"{file.Dir}{file.Name}{file.Extension}"));

            foreach (string line in listFileMusic)
            {
                string[] temp = line.Split('|');
                if (File.Exists(@$"{temp[0]}{temp[1]}{temp[2]}"))
                {
                    Objects.Add(new Object
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

        private void listViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GridMain.Children.Clear();
            Objects.Clear();
            _index = -1;
            myMediaElement.Source = null;
            updateButton();
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "MyMusicItem":
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
            personPath = Path.GetFullPath(filename);
            listFileMusic = new List<string>(File.ReadAllLines(personPath));

            // Khởi tạo các button
            //playButton.IsEnabled = false;
            skipNextButton.IsEnabled = false;
            skipPreviousButton.IsEnabled = false;
            shuffleButton.IsEnabled = false;
            repeatButton.IsEnabled = false;
            _shuffle = false;


            GridMain.Children.Clear();
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
        }

        private void updateButton()
        {
            skipNextButton.IsEnabled = true;
            skipPreviousButton.IsEnabled = true;
            shuffleButton.IsEnabled = true;
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
            int minutes = myMediaElement.Position.Minutes;
            int seconds = myMediaElement.Position.Seconds;

            currentPosition.Text = $"{hours}:{minutes}:{seconds}";

            if (myMediaElement.NaturalDuration.HasTimeSpan)
            {
                progressSlider.Value = myMediaElement.Position.TotalSeconds;
            }
        }

        private void playMusic(object sender, RoutedEventArgs e, int index)
		{
            if (index >= 0 && index < Objects.Count())
            {
                Object play = Objects[index];
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

                totalPosition.Text = $"{hours}:{minutes}:{seconds}";

                progressSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }
    }
}
