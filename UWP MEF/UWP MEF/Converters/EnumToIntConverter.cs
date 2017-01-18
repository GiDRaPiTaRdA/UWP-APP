using System;
using Windows.UI.Xaml.Data;

namespace UWP_MEF.Converters
{
    public class EnumToIntConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = System.Convert.ToInt32((Enum)value);
            return a;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}