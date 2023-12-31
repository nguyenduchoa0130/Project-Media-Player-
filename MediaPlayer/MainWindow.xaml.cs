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

namespace MediaPlayerNameSpace
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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
					GridMain.Children.Add(new MyMusicUserControl());
					break;
				case "RecentPlaysItem":
					GridMain.Children.Add(new RecentPlaysUserControl());
					break;
				case "PlaylistsItem":
					GridMain.Children.Add(new PlaylistsUserControl());
					break;
				default:
					GridMain.Children.Add(new MyMusicUserControl());
					break;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			GridMain.Children.Clear();
			GridMain.Children.Add(new MyMusicUserControl());
		}
	}
}
