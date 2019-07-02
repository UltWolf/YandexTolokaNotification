using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace YandexTolokaNotification.Services.Converters
{
   public  class BoolVisConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { 


            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        { 

            if ((Visibility)value==Visibility.Visible){
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
