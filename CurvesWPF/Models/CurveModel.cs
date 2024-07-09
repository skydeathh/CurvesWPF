using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CurvesWPF.Enums;
using CurvesWPF.Shared.Splines;

namespace CurvesWPF.Models
{
    public class CurveModel : PropertyChangedBase
    {
        public ControlPointsModel ControlPointsModel { get; set; }
        public PolylineModel PolylineModel { get; set; }

        public double Step { get; set; }

        public int Degree { get; set; }

        private Point TopLeftPoint => CalculateTopLeftPoint();
        private Point BottomRightPoint => CalculateBottomRightPoint();
        private Point CenterPoint => CalculateCenterPoint();
        public CurveTypeEnum CurveType { get; private set; }

        public ObservableCollection<PointModel> ControlPointsNotScaled => ControlPointsModel.ControlPointsNotScaled;
        public PointCollection Polyline => PolylineModel.Polyline;

        private PointCollection _points;
        public PointCollection Points
        {
            get => _points;
            private set
            {
                _points = value;
                OnPropertyChanged();
            }
        }

        private SolidColorBrush _color;
        public SolidColorBrush Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _thickness;
        public int Thickness
        {
            get => _thickness;
            set
            {
                if (_thickness != value)
                {
                    _thickness = value;
                    OnPropertyChanged(nameof(Thickness));
                }
            }
        }

        public CurveModel()
        {
            Points = new PointCollection();
            PolylineModel = new PolylineModel();
            ControlPointsModel = new ControlPointsModel();

            Thickness = 1;
            Degree = 1;
            Color = new SolidColorBrush(new Color() { A = 50, B = 255, G = 0, R = 0});
        }

        public async Task DrawCurveAsync() => Points = await ChoseDrawCurveMethodAsync(Step);

        private async Task<PointCollection> ChoseDrawCurveMethodAsync(double step)
        {
            return await Task.Run(() =>
            {
                switch (CurveType)
                {
                    case CurveTypeEnum.BezierSpline:
                        return BezierSpline.Interpolate(1, step, ControlPointsNotScaled);
                    case CurveTypeEnum.BezierSplinePrecomputed:
                        return BezierSpline.Interpolate(2, step, ControlPointsNotScaled);
                    case CurveTypeEnum.BezierSplineModified:
                        return BezierSpline.Interpolate(3, step, ControlPointsNotScaled);
                    case CurveTypeEnum.BezierSplineNumerical:
                        return BSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Bezier);
                    case CurveTypeEnum.BSpline:
                        return BSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Open);
                    case CurveTypeEnum.BSplineClosed:
                        return BSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Closed);
                    case CurveTypeEnum.CubicHermiteSpline:
                        return CubicHermiteSpline.Interpolate(ControlPointsNotScaled, step);
                    case CurveTypeEnum.NURBSpline:
                        return NURBSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Bezier);
                    case CurveTypeEnum.NURBSplineOpened:
                        return NURBSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Open);
                    case CurveTypeEnum.NURBSplineClosed:
                        return NURBSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Closed);
                    default:
                        return new PointCollection();
                }
            });
        }

        public void DrawCurve() => Points = ChoseDrawCurveMethod(Step);

        private PointCollection ChoseDrawCurveMethod(double step)
        {
            switch (CurveType)
            {
                case CurveTypeEnum.BezierSpline:
                    return BezierSpline.Interpolate(1, step, ControlPointsNotScaled);
                case CurveTypeEnum.BezierSplinePrecomputed:
                    return BezierSpline.Interpolate(2, step, ControlPointsNotScaled);
                case CurveTypeEnum.BezierSplineModified:
                    return BezierSpline.Interpolate(3, step, ControlPointsNotScaled);
                case CurveTypeEnum.BezierSplineNumerical:
                    return BSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Bezier);
                case CurveTypeEnum.BSpline:
                    return BSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Open);
                case CurveTypeEnum.BSplineClosed:
                    return BSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Closed);
                case CurveTypeEnum.CubicHermiteSpline:
                    return CubicHermiteSpline.Interpolate(ControlPointsNotScaled, step);
                case CurveTypeEnum.NURBSpline:
                    return NURBSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Bezier);
                case CurveTypeEnum.NURBSplineOpened:
                    return NURBSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Open);
                case CurveTypeEnum.NURBSplineClosed:
                    return NURBSpline.Interpolate(step, Degree, ControlPointsNotScaled, KnotsTypeEnum.Closed);
                default:
                    return new();
            }
        }

        public void SetCurveType(CurveTypeEnum curveType)
        {
            CurveType = curveType;
            DrawCurve();
        }

        private Point CalculateTopLeftPoint()
        {
            if (ControlPointsNotScaled.Count == 0 || ControlPointsNotScaled.Count == 0)
                return new Point();

            double minX = ControlPointsNotScaled.Min(p => p.X);
            double minY = ControlPointsNotScaled.Min(p => p.Y);

            return new Point(minX, minY);
        }

        private Point CalculateBottomRightPoint()
        {
            if (ControlPointsNotScaled.Count == 0 || ControlPointsNotScaled.Count == 0)
                return new Point();

            double maxX = ControlPointsNotScaled.Max(p => p.X);
            double maxY = ControlPointsNotScaled.Max(p => p.Y);

            return new Point(maxX, maxY);
        }

        private Point CalculateCenterPoint()
        {
            double centerX = (TopLeftPoint.X + BottomRightPoint.X) / 2;
            double centerY = (TopLeftPoint.Y + BottomRightPoint.Y) / 2;

            return new Point(centerX, centerY);
        }

        public Point CalculateCenterDisplacement(double canvasCenterX, double canvasCenterY) => new(canvasCenterX - CenterPoint.X, canvasCenterY - CenterPoint.Y);

        public void DisplaceCurve(Point displacement, double scale)
        {
            var polyline = new PointCollection();

            foreach (var point in ControlPointsNotScaled)
            {
                point.DisplacePoint(displacement, scale);

                polyline.Add(point.ToPoint());
            }

            PolylineModel.Polyline = polyline;
            DrawCurve();
        }

        public void AddPolylinePoint(Point point)
        {
            var polyline = new PointCollection();
            foreach (var polylinePoint in Polyline)
            {
                polyline.Add(polylinePoint);
            }
            polyline.Add(point);
            PolylineModel.Polyline = polyline;
        }

        public void UpdatePolylinePoints()
        {
            var polyline = new PointCollection();

            for (int i = 0; i < ControlPointsNotScaled.Count; i++)
            {
                polyline.Add(new Point(ControlPointsNotScaled[i].ScaledX, ControlPointsNotScaled[i].ScaledY));
            }

            PolylineModel.Polyline = polyline;
        }

        public void UpdateControlPoints(double zoom, double canvasCenterX, double canvasCenterY)
        {
            for (int i = 0; i < ControlPointsNotScaled.Count; i++)
            {
                double newX = (ControlPointsNotScaled[i].ScaledX - canvasCenterX) / zoom + canvasCenterX;
                double newY = (ControlPointsNotScaled[i].ScaledY - canvasCenterY) / zoom + canvasCenterY;

                ControlPointsNotScaled[i].X = newX;
                ControlPointsNotScaled[i].Y = newY;
            }
        }

        public void ScaleCurve(double zoom, double canvasCenterX, double canvasCenterY)
        {
            var polyline = new PointCollection();

            for (int i = 0; i < ControlPointsNotScaled.Count; i++)
            {
                double newX = (ControlPointsNotScaled[i].X - canvasCenterX) * zoom + canvasCenterX;
                double newY = (ControlPointsNotScaled[i].Y - canvasCenterY) * zoom + canvasCenterY;

                var displacedPoint = new Point(newX, newY);

                ControlPointsNotScaled[i].ScaledX = newX;
                ControlPointsNotScaled[i].ScaledY = newY;

                ControlPointsNotScaled[i].TopLeftX = newX - ControlPointsNotScaled[i].Diameter / 2;
                ControlPointsNotScaled[i].TopLeftY = newY - ControlPointsNotScaled[i].Diameter / 2;

                polyline.Add(displacedPoint);
            }

            PolylineModel.Polyline = polyline;
            DrawCurve();
        }

        public double InscribeCurve(double canvasWidth, double canvasHeigth, double zoom)
        {
            double canvasLeft = 0;
            double canvasTop = 0;

            double curveLeft = double.MaxValue;
            double curveTop = double.MaxValue;
            double curveRight = double.MinValue;
            double curveBottom = double.MinValue;

            double minX = ControlPointsModel.ControlPointsNotScaled.Min(p => p.ScaledX);
            double minY = ControlPointsModel.ControlPointsNotScaled.Min(p => p.ScaledY);
            double maxX = ControlPointsModel.ControlPointsNotScaled.Max(p => p.ScaledX);
            double maxY = ControlPointsModel.ControlPointsNotScaled.Max(p => p.ScaledY);

            curveLeft = Math.Min(curveLeft, minX);
            curveTop = Math.Min(curveTop, minY);
            curveRight = Math.Max(curveRight, maxX);
            curveBottom = Math.Max(curveBottom, maxY);

            double scaleX = (canvasWidth - canvasLeft) / (curveRight - curveLeft);
            double scaleY = (canvasHeigth - canvasTop) / (curveBottom - curveTop);
            double scale = Math.Min(scaleX, scaleY);

            return scale * zoom;
        }

        public void DeleteLastPoint()
        {
            if (ControlPointsModel.ControlPointsNotScaled.Count > 0)
            {
                ControlPointsModel.ControlPointsNotScaled.Remove(ControlPointsModel.ControlPointsNotScaled.Last());

                var polyline = new PointCollection();

                for (int i = 0; i < PolylineModel.Polyline.Count - 1; i++)
                {
                    polyline.Add(new Point(PolylineModel.Polyline[i].X, PolylineModel.Polyline[i].Y));
                }

                PolylineModel.Polyline = polyline;
            }
        }

        public void ChangeCurveSelectivity()
        {
            foreach (var point in ControlPointsModel.ControlPointsNotScaled)
            {
                point.ResizePoint();
            }

            PolylineModel.Visibility = false;
        }
    }
}
