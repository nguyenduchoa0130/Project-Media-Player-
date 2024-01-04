using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerNameSpace
{
    public class Object : INotifyPropertyChanged, ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }

        void RaiseEvent([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
