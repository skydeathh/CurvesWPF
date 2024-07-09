using System.Collections.ObjectModel;

namespace CurvesWPF.Models
{
    public class ControlPointsModel : PropertyChangedBase
    {
        private bool _visibility;

        public bool Visibility
        {
            get => _visibility;
            private set { _visibility = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PointModel> ControlPointsNotScaled { get; set; }

        public ControlPointsModel()
        {
            ControlPointsNotScaled = new ObservableCollection<PointModel>();
        }

        public void ChangeVisibility()
        {
            Visibility = !Visibility;
        }
    }
}
