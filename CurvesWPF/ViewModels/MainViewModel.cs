using CurvesWPF.Commands;
using CurvesWPF.Models;
using CurvesWPF.Enums;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;
using System.ComponentModel;

namespace CurvesWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private double _canvasWidthInitialize => 1011;
        private double _canvasHeightInitialize => 663;

        private int CurrentCurve = 0;

        private int _selectedSlider;

        public int SelectedSlider
        {
            get => _selectedSlider;
            set
            {
                SetProperty(ref _selectedSlider, value);
                OnPropertyChanged(nameof(SelectedSlider));
            }
        }

        private int _canvasWidth;
        public int CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                var displacement = (_canvasWidth - value) / 2;

                DisplaceCurves(new Point(-displacement, 0));

                SetProperty(ref _canvasWidth, value);
                OnPropertyChanged(nameof(CanvasCenterX));
            }
        }

        private int _canvasHeight;
        public int CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                SetProperty(ref _canvasHeight, value);
                OnPropertyChanged(nameof(CanvasCenterY));
            }
        }

        public double CanvasCenterX => CanvasWidth / 2;
        public double CanvasCenterY => CanvasHeight / 2;

        private double _canvasPositionX;
        public double CanvasPositionX
        {
            get => _canvasPositionX;
            set
            {
                SetProperty(ref _canvasPositionX, value);
                OnPropertyChanged(nameof(LabelX));
            }
        }

        private double _canvasPositionY;
        public double CanvasPositionY
        {
            get => _canvasPositionY;
            set
            {
                SetProperty(ref _canvasPositionY, value);
                OnPropertyChanged(nameof(LabelY));
            }
        }

        private double _realCanvasPositionX;
        public double RealCanvasPositionX
        {
            get => _realCanvasPositionX;
            set
            {
                SetProperty(ref _realCanvasPositionX, value);
                OnPropertyChanged(nameof(LabelX));
            }
        }

        private double _realCanvasPositionY;
        public double RealCanvasPositionY
        {
            get => _realCanvasPositionY;
            set
            {
                SetProperty(ref _realCanvasPositionY, value);
                OnPropertyChanged(nameof(LabelY));
            }
        }

        private Point _realCenter;

        public int LabelX => (int)CanvasPositionX;
        public int LabelY => (int)CanvasPositionY;

        //private Point CalculateRealPoint()
        //{
        //    var realX = CanvasPositionX - CanvasCenterX;
        //    var realY = CanvasCenterY - CanvasPositionY;

        //    var newX = (double)(realX) / Zoom;
        //    var newY = (double)(realY) / Zoom;


        //    return new Point(newX, newY);
        //}

        private double _zoom = 1.0;
        public double Zoom
        {
            get => _zoom;
            set
            {
                SetProperty(ref _zoom, value);
                OnPropertyChanged();
            }
        }

        public void ZoomIn()
        {
            Zoom = Math.Truncate(Zoom * 10 + 0.0001) / 10;
            Zoom += 0.1;
            ScaleCurves();
        }

        public void ZoomOut()
        {
            Zoom = Math.Truncate(Zoom * 10 + 0.0001) / 10;
            Zoom -= 0.1;
            ScaleCurves();
        }

        private bool _isControlPointsVisible;

        public bool IsControlPointsVisible
        {
            get => _isControlPointsVisible;
            set => SetProperty(ref _isControlPointsVisible, value);
        }

        private bool _isBezierSegmentVisible;

        public bool IsBezierSegmentVisible
        {
            get => _isBezierSegmentVisible;
            set => SetProperty(ref _isBezierSegmentVisible, value);
        }

        private bool _isBezierCurveVisible;

        public bool IsBezierCurveVisible
        {
            get => _isBezierCurveVisible;
            set => SetProperty(ref _isBezierCurveVisible, value);
        }

        private double _step;
        public double Step
        {
            get => _step;
            set
            {
                SetProperty(ref _step, value);
                Curves[CurrentCurve].Step = value;
                DrawCurves();
            }
        }

        private BezierCurveComboboxModel _selectedCurve;
        public BezierCurveComboboxModel SelectedCurve
        {
            get => _selectedCurve;
            set
            {
                SelectedCurveString = value.Name;

                SetProperty(ref _selectedCurve, value);
                Curves[CurrentCurve].SetCurveType(SelectedCurve.CurveType);

                Sliders.Clear();
                if (IsSplineNURB())
                {
                    for (int i = 0; i < Curves[CurrentCurve].ControlPointsNotScaled.Count; i++)
                    {
                        AddSlider(i);
                    }
                }
                OnPropertyChanged();
                DrawCurves();
            }
        }

        private string _selectedCurveString;
        public string SelectedCurveString
        {
            get => _selectedCurveString;
            set
            {
                SetProperty(ref _selectedCurveString, value);
            }
        }

        private int _curveDegree;
        public int CurveDegree
        {
            get => _curveDegree;
            set
            {
                Curves[CurrentCurve].Degree = value;
                SetProperty(ref _curveDegree, value);
                DrawCurves();
            }
        }

        private string _time;
        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        private int _thickness;
        public int CurveThickness
        {
            get => _thickness;
            set
            {
                if (Curves[CurrentCurve] != null)
                {

                    Curves[CurrentCurve].Thickness = value;
                    _thickness = value;
                    OnPropertyChanged();
                }
            }
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                if (Curves[CurrentCurve] != null)
                {
                    Curves[CurrentCurve].Color = new SolidColorBrush(value);
                }

                OnPropertyChanged();
            }
        }

        private bool _isDragging;
        public bool IsDragging
        {
            get { return _isDragging; }
            set
            {
                _isDragging = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BezierCurveComboboxModel> BezierCurveComboboxSections { get; }
        public ObservableCollection<string> ListBoxPoints { get; }
        public ObservableCollection<SliderModel> Sliders { get; set; }
        public ObservableCollection<CurveModel> Curves { get; }

        public ICommand MouseRightClickCommand { get; }
        public ICommand MouseLeftClickCommand { get; }
        public ICommand DeleteLastPointCommand { get; }
        public ICommand ClearCanvasCommand { get; }
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand CenterPolylineCommand { get; }
        public ICommand AddNewCurveCommand { get; }
        public ICommand SwitchCurveCommand { get; }
        public ICommand InscribeCurveCommand { get; }
        public ICommand ChangePolylineVisibilityCommand { get; }
        public ICommand UpCommand { get; }
        public ICommand DownCommand { get; }
        public ICommand LeftCommand { get; }
        public ICommand RightCommand { get; }
        public ICommand AddSliderCommand { get; }
        public ICommand RemoveSliderCommand { get; }

        public MainViewModel()
        {
            _isControlPointsVisible = true;
            _isBezierCurveVisible = true;
            _thickness = 1;
            _curveDegree = 1;
            _step = 0.01;

            MouseRightClickCommand = new RelayCommand(MouseRightClick);
            MouseLeftClickCommand = new RelayCommand(MouseLeftClick);
            DeleteLastPointCommand = new RelayCommand(DeleteLastPoint);
            ZoomInCommand = new RelayCommand(ZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut);
            CenterPolylineCommand = new RelayCommand(CenterPolyline);
            SwitchCurveCommand = new RelayCommand(SwitchCurve);
            AddNewCurveCommand = new RelayCommand(AddNewCurve);
            ClearCanvasCommand = new RelayCommand(ClearCanvas);
            InscribeCurveCommand = new RelayCommand(InscribeCurve);
            ChangePolylineVisibilityCommand = new RelayCommand(ChangePolylineVisibility);
            UpCommand = new RelayCommand(DisplaceCurveUp);
            DownCommand = new RelayCommand(DisplaceCurveDown);
            LeftCommand = new RelayCommand(DisplaceCurveLeft);
            RightCommand = new RelayCommand(DisplaceCurveRight);
            AddSliderCommand = new RelayCommand(AddSlider);
            RemoveSliderCommand = new RelayCommand(RemoveSlider);

            ListBoxPoints = new();
            Sliders = new ObservableCollection<SliderModel>();

            BezierCurveComboboxSections = new ObservableCollection<BezierCurveComboboxModel>
            {
                new() { CurveType = CurveTypeEnum.BezierSpline, Name = "Bezier Standard Binomial" },
                new() { CurveType = CurveTypeEnum.BezierSplineModified, Name = "Bezier Modified Binomial" },
                new() { CurveType = CurveTypeEnum.BezierSplineNumerical, Name = "Bezier Numerical (BSpline)" },
                new() { CurveType = CurveTypeEnum.BSpline, Name = "BSpline Opened" },
                new() { CurveType = CurveTypeEnum.BSplineClosed, Name = "BSpline Closed" },
                new() { CurveType = CurveTypeEnum.CubicHermiteSpline, Name = "Cubic Hermite Spline" },
                new() { CurveType = CurveTypeEnum.NURBSpline, Name = "Bezier Numerical (NURBS)" },
                new() { CurveType = CurveTypeEnum.NURBSplineOpened, Name = "NURBS Opened" },
                new() { CurveType = CurveTypeEnum.NURBSplineClosed, Name = "NURBS Closed" }
            };

            Curves = new();
            Curves.Add(new() { Step = _step });
            SelectedCurve = BezierCurveComboboxSections.FirstOrDefault(c => c.CurveType == CurveTypeEnum.BezierSpline);
            _realCenter = new Point(_canvasWidthInitialize / 2, _canvasHeightInitialize / 2);
        }

        public bool IsSplineNURB()
        {
            return SelectedCurve.CurveType == CurveTypeEnum.NURBSpline ||
                   SelectedCurve.CurveType == CurveTypeEnum.NURBSplineOpened ||
                   SelectedCurve.CurveType == CurveTypeEnum.NURBSplineClosed;
        }

        private void AddSlider()
        {
            AddSlider(Sliders.Count);
        }

        private void AddSlider(int index)
        {
            if (IsSplineNURB())
            {
                var slider = new SliderModel { Value = 1, Index = index };
                slider.PropertyChanged += Slider_PropertyChanged;
                Sliders.Add(slider);
                OnPropertyChanged(nameof(Sliders));
            }
        }

        private void RemoveSlider()
        {
            if (Sliders.Count > 0 && IsSplineNURB())
            {
                var slider = Sliders[Sliders.Count - 1];
                slider.PropertyChanged -= Slider_PropertyChanged;
                Sliders.RemoveAt(Sliders.Count - 1);
                OnPropertyChanged(nameof(Sliders));
            }
        }

        private void Slider_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SliderModel.Value))
            {
                var slider = (SliderModel)sender;
                SelectedSlider = slider.Index;
                if (Curves[CurrentCurve].ControlPointsModel.ControlPointsNotScaled.Count != 0)
                {
                    Curves[CurrentCurve].ControlPointsModel.ControlPointsNotScaled[SelectedSlider].Weight = slider.Value;
                    DrawCurves();
                }
            }
        }

        public void MouseLeftClick()
        {
            Curves[CurrentCurve].DrawCurve();
            Curves[CurrentCurve].UpdatePolylinePoints();
            Curves[CurrentCurve].UpdateControlPoints(_zoom, CanvasCenterX, CanvasCenterY);
        }

        private void MouseRightClick()
        {
            var point = new PointModel((CanvasPositionX - CanvasCenterX) / _zoom + CanvasCenterX,
                                        (CanvasPositionY - CanvasCenterY) / _zoom + CanvasCenterY,
                                         CanvasPositionX, CanvasPositionY);

            Curves[CurrentCurve].ControlPointsModel.ControlPointsNotScaled.Add(point);
            Curves[CurrentCurve].AddPolylinePoint(new Point(CanvasPositionX, CanvasPositionY));
            var stopwatch = new Stopwatch();

            Curves[CurrentCurve].DrawCurve(); //????
            AddSlider();
            ListBoxPoints.Add(point.ToString());
        }

        private void DeleteLastPoint()
        {
            if (Curves[CurrentCurve].ControlPointsNotScaled.Count != 0)
            {
                Curves[CurrentCurve].DeleteLastPoint();
                Curves[CurrentCurve].DrawCurve();
                ListBoxPoints.Remove(ListBoxPoints.Last());

                RemoveSlider();
            }
        }

        private void DrawCurves()
        {
            foreach (var curve in Curves)
            {
                curve.DrawCurve();
            }

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            Curves[CurrentCurve].DrawCurve();
            stopwatch.Stop();

            var elapsedTime = stopwatch.Elapsed;
            double seconds = elapsedTime.TotalSeconds;

            Time = string.Format("{0:F6}", seconds);
        }

        private void ClearCanvas()
        {
            Curves.Clear();
            Curves.Add(new() { Step = Step });
            CurrentCurve = 0;
            Curves[CurrentCurve].SetCurveType(SelectedCurve.CurveType);
        }

        private void CenterPolyline()
        {
            var displacement = Curves[CurrentCurve].CalculateCenterDisplacement(CanvasCenterX, CanvasCenterY);

            DisplaceCurves(displacement);
        }

        private void DisplaceCurves(Point displacement)
        {
            for (int i = 0; i < Curves.Count; i++)
            {
                Curves[i].DisplaceCurve(displacement, Zoom);
            }

            _realCenter = new Point(_realCenter.X + displacement.X, _realCenter.Y + displacement.Y);
        }

        private void AddNewCurve()
        {
            if (Curves.Count != 0)
            {
                Curves[CurrentCurve].ChangeCurveSelectivity();
            }

            var newCurve = new CurveModel() { Step = _step };
            newCurve.SetCurveType(SelectedCurve.CurveType);

            Curves.Add(newCurve);
            CurrentCurve++;

            ListBoxPoints.Clear();
            Sliders.Clear();
        }

        private void SwitchCurve()
        {
            if (Curves.Count == 1) return;

            Curves[CurrentCurve].ChangeCurveSelectivity();
            CurrentCurve++;
            if (Curves.Count == CurrentCurve)
            {
                CurrentCurve = 0;
            }

            Sliders.Clear();
            if (IsSplineNURB())
            {
                for (int i = 0; i < Curves[CurrentCurve].ControlPointsNotScaled.Count; i++)
                {
                    AddSlider(i);
                }
            }

            Curves[CurrentCurve].ChangeCurveSelectivity();
            AddCurrentCurvePointsToListBox();
        }

        private void AddCurrentCurvePointsToListBox()
        {
            ListBoxPoints.Clear();

            foreach (var point in Curves[CurrentCurve].ControlPointsNotScaled)
            {
                ListBoxPoints.Add(point.ToString());
            }
        }

        private void ScaleCurves()
        {
            foreach (var curve in Curves)
            {
                curve.ScaleCurve(_zoom, CanvasCenterX, CanvasCenterY);
            }

            DrawCurves();
        }

        private void InscribeCurve()
        {
            if (Curves.Count != 0)
            {
                Zoom = Curves[CurrentCurve].InscribeCurve(CanvasWidth, CanvasHeight, _zoom);

                CenterPolyline();
                ScaleCurves();
            }
        }

        private void ChangePolylineVisibility()
        {
            Curves[CurrentCurve].PolylineModel.ChangeVisibility();
        }

        private void DisplaceCurveUp()
        {
            DisplaceCurves(new Point(0, -10));
        }

        private void DisplaceCurveDown()
        {
            DisplaceCurves(new Point(0, 10));
        }

        private void DisplaceCurveLeft()
        {
            DisplaceCurves(new Point(-10, 0));
        }

        private void DisplaceCurveRight()
        {
            DisplaceCurves(new Point(10, 0));
        }
    }
}