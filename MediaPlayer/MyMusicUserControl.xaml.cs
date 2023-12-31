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

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for MyMusicUserControl.xaml
    /// </summary>
    public partial class MyMusicUserControl : UserControl
    {
        public MyMusicUserControl()
        {
            InitializeComponent();
            skipNextButton.IsEnabled = false;
			skipPreviousButton.IsEnabled = false;
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

		MediaPlayer _mediaPlayer = new MediaPlayer();
		private bool _playing = false;

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

		}


        private void playButton_Click(object sender, RoutedEventArgs e)
        {
			if (_playing)
			{
				_mediaPlayer.Pause();
				_playing = false;
                playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            }
			else
			{
				
				if (musicListView.SelectedItem != null && _mediaPlayer.Source == null)
				{
					Object play = (Object)musicListView.SelectedItem;
					_mediaPlayer.Open(new Uri($"{play.Dir}{play.Name}"));
					
					if (musicListView.SelectedIndex < Objects.Count - 1)
					{
                        skipNextButton.IsEnabled = true;
					}

					if (musicListView.SelectedIndex > 0)
					{
						skipPreviousButton.IsEnabled = true;
					}
                }

				if (_mediaPlayer.Source != null)
				{
                    _mediaPlayer.Play();
                    _playing = true;
                    playIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;

                }
            }
        }

        private void skipNextButton_Click(object sender, RoutedEventArgs e)
        {
			int index = musicListView.SelectedIndex;
			if (index < Objects.Count - 1)
			{
				musicListView.SelectedIndex += 1;
				index += 1;

				if (index == Objects.Count - 1) 
				{
					skipNextButton.IsEnabled = false;
				}

                Object play = Objects[index];
                _mediaPlayer.Open(new Uri($"{play.Dir}{play.Name}"));
				skipPreviousButton.IsEnabled |= true;

                if (_playing)
                {
                    _mediaPlayer.Play();
                }
            }
        }

        private void skipPreviousButton_Click(object sender, RoutedEventArgs e)
        {
			int index = musicListView.SelectedIndex;
			if (index > 0)
			{
                musicListView.SelectedIndex -= 1;
				index -= 1;

                if (index == 0)
                {
                    skipPreviousButton.IsEnabled = false;
                }

                Object play = Objects[index];
                _mediaPlayer.Open(new Uri($"{play.Dir}{play.Name}"));
				skipNextButton.IsEnabled |= true;

				if (_playing)
				{
                    _mediaPlayer.Play();
                }
            }
        }
    }
}
