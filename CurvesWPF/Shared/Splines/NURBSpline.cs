using System.Windows;
using System.Windows.Media;
using CurvesWPF.Models;
using System.Collections.Generic;
using CurvesWPF.Enums;

namespace CurvesWPF.Shared.Splines
{
    public static class NURBSpline
    {
        private static int _degree;
        private static IList<double> _knots;
        private static IList<PointModel> _controlPoints;
        private static IList<double> _weight;

        public static PointCollection Interpolate(double tStep, int degree, IList<PointModel> controlPoints, KnotsTypeEnum knotType)
        {
            _degree = degree;
            _controlPoints = new List<PointModel>(controlPoints);

            var polyline = new PointCollection();

            if (degree < 1)
            {
                return polyline;
            }

            if (controlPoints.Count <= degree) return polyline;

            ChoseNURBSType(knotType);

            double startT = _knots[_degree];
            double endT = _knots[_knots.Count - _degree - 1];

            for (double t = startT; t <= endT; t += tStep)
            {
                var point = Evaluate(t);
                polyline.Add(point);
            }

            var lastPoint = Evaluate(endT);
            polyline.Add(lastPoint);

            return polyline;
        }

        private static Point Evaluate(double t)
        {
            int n = _controlPoints.Count - 1;
            for (int i = 0; i <= n; i++)
            {
                if (t >= _knots[i] && t < _knots[i + 1])
                {
                    return DeBoor(i, _degree, t);
                }
            }
            return DeBoor(n, _degree, t);
        }

        private static Point DeBoor(int i, int k, double t)
        {
            if (k == 0)
            {
                return _controlPoints[i].ToPoint();
            }

            double alpha = (t - _knots[i]) / (_knots[i + _degree + 1 - k] - _knots[i]);

            Point p1 = DeBoor(i - 1, k - 1, t);
            Point p2 = DeBoor(i, k - 1, t);

            double w1 = _controlPoints[i - 1].Weight;
            double w2 = _controlPoints[i].Weight;

            double x = ((1 - alpha) * p1.X * w1 + alpha * p2.X * w2) / ((1 - alpha) * w1 + alpha * w2);
            double y = ((1 - alpha) * p1.Y * w1 + alpha * p2.Y * w2) / ((1 - alpha) * w1 + alpha * w2);

            return new Point(x, y);
        }

        private static void ChoseNURBSType(KnotsTypeEnum knotType)
        {
            if (knotType == KnotsTypeEnum.Closed) _controlPoints = Generator.GenerateClosedControlPoints(_degree, _controlPoints);

            _knots = Generator.GenerateKnots(_controlPoints.Count, _degree,knotType);
        }
    }
}
