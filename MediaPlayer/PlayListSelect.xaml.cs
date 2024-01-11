using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for PlayListSelect.xaml
    /// </summary>
    public partial class PlayListSelect : Window
    {

        public List<Object> playList = new List<Object>();
        
        public PlayListSelect()
        {
            InitializeComponent();
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string playListSelect { get; set; }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int index = categoriesComboBox.SelectedIndex;

            if (index >= 0 && index < playList.Count)
            {
                playListSelect = $"{playList[index].Dir}{playList[index].Name}{playList[index].Extension}";
                DialogResult = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var personPath = Path.GetFullPath("PlayList");
            DirectoryInfo di = new DirectoryInfo(personPath);
            FileInfo[] files = di.GetFiles("*.txt");

            foreach (var file in files)
            {
                if (!File.Exists(file.FullName)) continue;
                playList.Add(new Object
                {
                    Name = Path.GetFileNameWithoutExtension(file.FullName),
                    Dir = Path.GetDirectoryName(file.FullName) + "\\",
                    Extension = Path.GetExtension(file.FullName)
                });
            }

            for (int i = 0; i < playList.Count; i++)
            {
                for (int j = i + 1; j < playList.Count; j++)
                {
                    if (playList[i].Name == playList[j].Name)
                    {
                        playList.Remove(playList[j]);
                    }
                }
            }
            categoriesComboBox.ItemsSource = playList;
        }
    }
}
