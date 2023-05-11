using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Resources;


namespace DesktopApp.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object condition, Type tartgetType, object parameter, CultureInfo culture)
        {
            if (condition is bool isCancelled && parameter is string target)
            {
                return (isCancelled ? target == "activate" : target == "storno") ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object condition, Type targetType, object parameter, CultureInfo culture)
        {
            if (condition is bool isVisible)
            {
                return !isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }
    }

    public class LocalizationConverter : IValueConverter
    {
        public string ResourceKey { get; set; }

        public Dictionary<string, string?> Translations { get; set; }

        public LocalizationConverter() { }

        public LocalizationConverter(Dictionary<string, string?> d) { Translations = d; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value is Enum en)
            //{
            //    Dictionary<string, string?> translations = (Dictionary<string, string?>)parameter;
            //    string name = en.ToString();
            //    return translations[name];
            //}

            if (value is string str) { return Translations[str]; }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                foreach ((var key, var val) in Translations)
                {
                    if (val == str) { return key; }
                }
            }

            return value;
        }
    }
}
