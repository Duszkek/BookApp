using System.Globalization;

namespace BookApp.Converters
{
    public class BoolToSaveStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSaved)
            {
                return isSaved ? "Saved" : "Save";
            }

            return "Save";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return String.Compare(s, "Saved", StringComparison.OrdinalIgnoreCase) == 0;
            }

            return false;        
        }
    }
}