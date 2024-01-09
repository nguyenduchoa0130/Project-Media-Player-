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
        private MusicList musics;
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
        //public int _index { get; set; } = -1;


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

		private void listViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GridMain.Children.Clear();
			switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
			{
				case "MyMusicItem":
                    var screen1 = new MyMusicUserControl(musics);
                    GridMain.Children.Add(screen1);
                    screen1.MusicsChanged += (newMusics) =>
                    {
                        musics = newMusics;
                    };

					break;
				case "RecentPlaysItem":
                    var screen2 = new RecentPlaysUserControl(musics);
                    GridMain.Children.Add(screen2);
                    screen2.MusicsChanged += (newMusics) =>
                    {
                        musics = newMusics;
                    };

					break;
				case "PlaylistsItem":
					GridMain.Children.Add(new PlaylistsUserControl());
					break;
				default:
                    var screen = new MyMusicUserControl(musics);
                    GridMain.Children.Add(screen);
                    screen.MusicsChanged += (newMusics) =>
                    {
                        musics = newMusics;
                    };
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


            GridMain.Children.Clear();
            var screen = new MyMusicUserControl(musics);
            GridMain.Children.Add(screen);
            screen.MusicsChanged += (newMusics) =>
            {
                musics = newMusics;
            };
        }

        private void updateButton()
        {
            skipNextButton.IsEnabled = true;
            skipPreviousButton.IsEnabled = true;
            shuffleButton.IsEnabled = true;
            repeatButton.IsEnabled = true;

            if (musics.index == -1)
            {
                _playing = false;
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                skipPreviousButton.IsEnabled = false;
                skipNextButton.IsEnabled = false;
                return;
            }

            if (musics.index == 0)
            {
                skipPreviousButton.IsEnabled = false;
            }

            if (musics.index == musics.Objects.Count - 1 && _shuffle == false)
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
            Object play = musics.Objects[index];
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
            musics.index = index;
            updateButton();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (musics.Objects.Count > 0)
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
                        musics.index = 0;
                        playMusic(sender, e, musics.index);
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
            int number = musics.Objects.Count;
            int init;
            do
            {
                init = random.Next(number);
            } while (init == musics.index);

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
                int index = musics.index;
                index += 1;
                if (repeat == (int)repeatMode.repeatall)
                {
                    index %= musics.Objects.Count;
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
                int index = musics.index;
                index -= 1;
                if (repeat == (int)repeatMode.repeatall)
                {
                    index = (index + musics.Objects.Count) % musics.Objects.Count;
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
