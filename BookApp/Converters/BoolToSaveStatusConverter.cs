using System.Globalization;

namespace BookApp.Converters
{
    public class BoolToSaveStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSaved)
            {
                return isSaved ? Strings.Read : Strings.Unread;
            }

            return Strings.Read;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return String.Compare(s, Strings.Read, StringComparison.OrdinalIgnoreCase) == 0;
            }

            return false;        
        }
    }
}