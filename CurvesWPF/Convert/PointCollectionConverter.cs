using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace CurvesWPF.Convert
{
    public class PointCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Point> points)
            {
                PointCollection pointCollection = new PointCollection();
                foreach (var point in points)
                {
                    pointCollection.Add(point);
                }
                return pointCollection;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
