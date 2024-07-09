using System.Windows.Media;
using System.Windows;
using CurvesWPF.Models;

namespace CurvesWPF.Shared.Splines
{
    public static class CubicHermiteSpline
    {
        public static PointCollection Interpolate(IList<PointModel> controlPoints, double step)
        {
            var sortedControlPoints = controlPoints.OrderBy(p => p.ScaledX).ToList();

            var result = new PointCollection();
            if (sortedControlPoints == null || sortedControlPoints.Count < 4)
                return result;

            var newstep = CalculateStep(step, sortedControlPoints);

            var first = sortedControlPoints.First().X;
            var last = sortedControlPoints.Last().X;

            var firstPoint = sortedControlPoints[0];
            var secondPoint = sortedControlPoints[1];

            var firstSectionDerivative = fd(sortedControlPoints[0].ScaledX, sortedControlPoints[1].ScaledX, sortedControlPoints[2].ScaledX, sortedControlPoints[0].ScaledY, sortedControlPoints[1].ScaledY, sortedControlPoints[2].ScaledY);

            for (double t = firstPoint.ScaledX; t < secondPoint.ScaledX; t += newstep)
            {
                double interpolatedValue = InterpolateSegment(firstPoint, secondPoint, firstSectionDerivative, firstSectionDerivative, t);
                result.Add(new Point(t, interpolatedValue));
            }

            for (int i = 1; i < sortedControlPoints.Count - 2; i++)
            {
                var p0 = sortedControlPoints[i];
                var p1 = sortedControlPoints[i + 1];

                var p0Derivative = fd(sortedControlPoints[i - 1].ScaledX, p0.ScaledX, p1.ScaledX, sortedControlPoints[i - 1].ScaledY, p0.ScaledY, p1.ScaledY);
                var p1Derivative = fd(p0.ScaledX, p1.ScaledX, sortedControlPoints[i + 2].ScaledX, p0.ScaledY, p1.ScaledY, sortedControlPoints[i + 2].ScaledY);

                for (double t = p0.ScaledX; t < p1.ScaledX; t += newstep)
                {
                    double interpolatedValue = InterpolateSegment(p0, p1, p0Derivative, p1Derivative, t);
                    result.Add(new Point(t, interpolatedValue));
                }
            }

            var preLastPoint = sortedControlPoints[sortedControlPoints.Count - 2];
            var lastPoint = sortedControlPoints[sortedControlPoints.Count - 1];

            var lastSectionDerivative = fd(sortedControlPoints[sortedControlPoints.Count - 3].ScaledX, preLastPoint.ScaledX, lastPoint.ScaledX, sortedControlPoints[sortedControlPoints.Count - 3].ScaledY, preLastPoint.ScaledY, lastPoint.ScaledY);

            for (double t = preLastPoint.ScaledX; t < lastPoint.ScaledX; t += newstep)
            {
                double interpolatedValue = InterpolateSegment(preLastPoint, lastPoint, lastSectionDerivative, lastSectionDerivative, t);
                result.Add(new Point(t, interpolatedValue));
            }

            double lastValue = InterpolateSegment(preLastPoint, lastPoint, lastSectionDerivative, lastSectionDerivative, lastPoint.ScaledX);
            result.Add(new Point(lastPoint.ScaledX, lastValue));

            return result;
        }

        private static double InterpolateSegment(PointModel p0, PointModel p1, double p0Derivative, double p1Derivative, double t)
        {
            double h = p1.ScaledX - p0.ScaledX;
            double tNorm = (t - p0.ScaledX) / h;

            double interpolatedValue = h00(tNorm) * p0.ScaledY + h10(tNorm) * h * p0Derivative + h01(tNorm) * p1.ScaledY + h11(tNorm) * h * p1Derivative;

            return interpolatedValue;
        }

        private static double CalculateStep(double step, IList<PointModel> controlPoints) => (controlPoints.Last().X - controlPoints.First().X) * step;

        private static double hi(double x1, double x2) => x2 - x1;

        private static double h00(double x) => (1 + 2 * x) * (1 - x) * (1 - x);
        private static double h10(double x) => x * (1 - x) * (1 - x);
        private static double h01(double x) => x * x * (3 - 2 * x);
        private static double h11(double x) => x * x * (x - 1);

        private static double ld(double x0, double x1, double x2) => -hi(x1, x2) / (hi(x0, x1) * (hi(x0, x1) + hi(x1, x2)));
        private static double cd(double x0, double x1, double x2) => -(hi(x0, x1) - hi(x1, x2)) / (hi(x0, x1) * hi(x1, x2));
        private static double rd(double x0, double x1, double x2) => hi(x0, x1) / (hi(x1, x2) * (hi(x0, x1) + hi(x1, x2)));

        private static double fd(double x0, double x1, double x2, double y0, double y1, double y2) => y0 * ld(x0, x1, x2) + y1 * cd(x0, x1, x2) + y2 * rd(x0, x1, x2);
    }
}
