using System.Windows;
using System.Windows.Media;
using CurvesWPF.Models;

namespace CurvesWPF.Shared.Splines
{
    public static class BezierSpline
    {
        public static PointCollection Interpolate(int method, double step, IList<PointModel> controlPoints)
        {
            var pointCollection = new PointCollection();

            double t = 0;
            for (; t <= 1; t += step)
            {
                var point = CalculatePoint(method, t, controlPoints);
                pointCollection.Add(point);
            }

            var lastPoint = CalculatePoint(method, 1, controlPoints);
            pointCollection.Add(lastPoint);

            return pointCollection;
        }

        private static Point CalculatePoint(int method, double t, IList<PointModel> controlPoints)
        {
            switch (method)
            {
                case 1:
                    return CalculatePoint(t, controlPoints);
                case 2:
                    return CalculatePointUsingPrecomputedBinomial(t, controlPoints);
                case 3:
                    return CalculatePointUsingModifiedBinomial(t, controlPoints);
                default:
                    throw new ArgumentException("Invalid method specified.");
            }
        }

        private static double BernsteinCoefficient(int n, int i, double t)
        {
            double coefficient = BinomialCoefficient(n, i);
            double power1 = Math.Pow(1 - t, n - i);
            double power2 = Math.Pow(t, i);

            return coefficient * power1 * power2;
        }

        private static double BernsteinCoefficientCached(int n, int i, double t)
        {
            int[] binomialCoefficients = GetBinomialCoefficients(n);
            double coefficient = binomialCoefficients[i];
            double power1 = Math.Pow(1 - t, n - i);
            double power2 = Math.Pow(t, i);

            return coefficient * power1 * power2;
        }

        private static double BernsteinCoefficientWithModifiedBinomial(int n, int i, double t)
        {
            double coefficient = BinomialCoefficientModified(n, i);
            double power1 = Math.Pow(1 - t, n - i);
            double power2 = Math.Pow(t, i);

            return coefficient * power1 * power2;
        }

        private static Point CalculatePoint(double t, IList<PointModel> controlPoints)
        {
            int n = controlPoints.Count - 1;
            Point point = new Point();

            for (int i = 0; i <= n; i++)
            {
                double bernsteinValue = BernsteinCoefficient(n, i, t);
                point.X += controlPoints[i].ScaledX * bernsteinValue;
                point.Y += controlPoints[i].ScaledY * bernsteinValue;
            }

            return point;
        }

        private static Point CalculatePointUsingPrecomputedBinomial(double t, IList<PointModel> controlPoints)
        {
            int n = controlPoints.Count - 1;
            Point point = new Point();

            for (int i = 0; i <= n; i++)
            {
                double bernsteinValue = BernsteinCoefficientCached(n, i, t);
                point.X += controlPoints[i].ScaledX * bernsteinValue;
                point.Y += controlPoints[i].ScaledY * bernsteinValue;
            }

            return point;
        }

        private static Point CalculatePointUsingModifiedBinomial(double t, IList<PointModel> controlPoints)
        {
            int n = controlPoints.Count - 1;

            Point point = new Point();

            for (int i = 0; i <= n; i++)
            {
                double bernsteinValue = BernsteinCoefficientWithModifiedBinomial(n, i, t);
                point.X += controlPoints[i].ScaledX * bernsteinValue;
                point.Y += controlPoints[i].ScaledY * bernsteinValue;
            }

            return point;
        }

        private static int BinomialCoefficient(int n, int k)
        {
            if (k == 0 || k == n)
                return 1;

            int numerator = 1;
            int denominator = 1;

            for (int i = 1; i <= k; i++)
            {
                numerator *= n - i + 1;
                denominator *= i;
            }

            return numerator / denominator;
        }

        private static double BinomialCoefficientModified(int n, int k)
        {
            if (k < 0 || k > n)
                return 0;

            if (k == 0 || k == n)
                return 1;

            k = Math.Min(k, n - k);
            int result = 1;

            for (int i = 0; i < k; i++)
            {
                result *= n - i;
                result /= i + 1;
            }

            return result;
        }

        private static readonly Dictionary<int, int[]> BinomialCoefficientsCache = new();

        private static int[] GetBinomialCoefficients(int n)
        {
            if (BinomialCoefficientsCache.ContainsKey(n))
                return BinomialCoefficientsCache[n];

            int[] coefficients = new int[n + 1];
            coefficients[0] = 1;

            for (int i = 1; i <= n; i++)
            {
                coefficients[i] = coefficients[i - 1] * (n - i + 1) / i;
            }

            BinomialCoefficientsCache[n] = coefficients;
            return coefficients;
        }
    }
}
