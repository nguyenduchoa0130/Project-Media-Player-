using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MediaPlayerNameSpace
{
    public  class HandleConverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = (string)value;
            string[] temp = time.Split(':');
            time="";
            for(int i=0;i<temp.Count();i++)
            {
                if (temp[i].Count()==1)
                {
                    temp[i] =  '0'+ temp[i];
                }
                time = time + temp[i]+':';
                
            }
            time = time.Remove(time.Length - 1);
            return time;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
