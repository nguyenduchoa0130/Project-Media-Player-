using Microsoft.Win32;
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
using System.IO;
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

		private void init()
		{
            skipNextButton.IsEnabled = false;
            skipPreviousButton.IsEnabled = false;
            playButton.IsEnabled = false;
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

		//public MediaPlayer mediaElement = new MediaPlayer();
		private bool _playing = false;
		DispatcherTimer? _timer;

		ObservableCollection<Object> Objects = new ObservableCollection<Object>();

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

				}

				musicListView.Items.Clear();
				foreach (Object obj in Objects)
				{
					musicListView.Items.Add(obj);
				}
			}

			playButton.IsEnabled = true;
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

            if (index == Objects.Count - 1)
            {
                skipNextButton.IsEnabled = false;
            }
        }

        private void musicListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
    }
}
