using System.Globalization;

namespace SmartSearch.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isTrue && parameter is string colors)
        {
            var colorParts = colors.Split(',');
            if (colorParts.Length == 2)
            {
                return isTrue ? colorParts[0] : colorParts[1];
            }
        }
        return "#CCCCCC";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 