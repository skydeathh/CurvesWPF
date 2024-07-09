using System.Windows.Media;

namespace CurvesWPF.Models
{
    public class PolylineModel : PropertyChangedBase
    {
        private bool _visibility;

        public bool Visibility
        {
            get => _visibility;
            set  { _visibility = value; OnPropertyChanged(); }
        }

        private PointCollection _polyline;
        public PointCollection Polyline
        {
            get => _polyline;
            set
            {
                _polyline = value;
                OnPropertyChanged();
            }
        }

        public PolylineModel()
        {
            Polyline = new PointCollection();
        }

        public void ChangeVisibility()
        {
            Visibility = !Visibility;
        }
    }
}
