using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace VkTunes.AudioList
{
    public class BytesToSizeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is long?))
                return value;

            var size = (long?) value;
            if (size == null)
                return size;

            var mbsize = size/(1024M*1024M);

            return $"{mbsize:##.##}MB";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}