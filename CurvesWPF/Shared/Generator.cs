using CurvesWPF.Enums;
using CurvesWPF.Models;
using CurvesWPF.Shared.Splines;

namespace CurvesWPF.Shared
{
    public static class Generator
    {
        public static List<double> GenerateKnots(int controlPointsCount, int degree,KnotsTypeEnum knotType = KnotsTypeEnum.Open)
        {
            switch (knotType)
            {
                case KnotsTypeEnum.Bezier:
                    return GenerateBezierKnots(controlPointsCount, degree);
                    break;
                default:
                    return GenerateKnots(controlPointsCount, degree);
            }
        }

        private static List<double> GenerateBezierKnots(int controlPointsCount, int degree)
        {
            int n = controlPointsCount - 1;
            int knotsCount = n + degree + 2;
            var knots = new List<double>(new double[knotsCount]);

            for (int i = degree + 1; i < knotsCount - degree - 1; i++)
            {
                knots[i] = (double)(i - degree) / (n - degree + 1);
            }

            for (int i = knotsCount - degree - 1; i < knotsCount; i++)
            {
                knots[i] = 1.0;
            }

            return knots;
        }

        private static List<double> GenerateKnots(int controlPointsCount, int degree)
        {
           var knotsCount = controlPointsCount + degree + 1;

            var knots = new List<double>();

            for (int i = 0; i < knotsCount; i++)
            {
                knots.Add(i);
            }

            return knots;
        }

        public static List<PointModel> GenerateClosedControlPoints(int degree, IList<PointModel> controlPoints)
        {
            var closedControlPoints = new List<PointModel>(controlPoints);

            for (int i = 0; i < degree; i++)
            {
                closedControlPoints.Add(controlPoints[i]);
            }

            return closedControlPoints;
        }
    }
}
