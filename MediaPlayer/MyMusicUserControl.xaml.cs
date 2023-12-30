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

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MyMusicUserControl.xaml
    /// </summary>
    public partial class MyMusicUserControl : UserControl
    {
        public MyMusicUserControl()
        {
            InitializeComponent();
        }

		public class Object : INotifyPropertyChanged
		{
			public string name;
			public string dir;
			public string extension;
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
			public event PropertyChangedEventHandler PropertyChanged;

			void RaiseEvent([CallerMemberName] string propertyName = "")
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}

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

	}
}
