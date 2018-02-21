using System;
using System.Globalization;
using System.Windows.Data;
// ReSharper disable PossibleNullReferenceException

namespace Countdown
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var totalProgress = (double)value;
            var factor = double.Parse(parameter.ToString());
            var step = 1 / factor;

            var current = 0.0;

            while (current + step < totalProgress)
            {
                current += step;
            }

            var to = current + step;
            var from = current;

            return (totalProgress - from) / (to - from);
            
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}