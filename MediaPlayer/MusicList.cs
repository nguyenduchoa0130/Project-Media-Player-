using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerNameSpace
{
    public class MusicList : INotifyPropertyChanged, ICloneable
    {
        public ObservableCollection<Object> Objects { get; set; } = new ObservableCollection<Object>();
        public int index { get; set; } = -1;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
