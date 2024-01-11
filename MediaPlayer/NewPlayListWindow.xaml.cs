using System;
using System.Collections.Generic;
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

namespace MediaPlayerNameSpace
{
    /// <summary>
    /// Interaction logic for NewPlayListWindow.xaml
    /// </summary>
    public partial class NewPlayListWindow : Window
    {
        public NewPlayListWindow()
        {
            InitializeComponent();
        }

        public string playList { get; set; }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            playList = dataTextBox.Text;
            DialogResult = true;
        }
    }
}
