using CurvesWPF.Enums;
using CurvesWPF.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace CurvesWPF.Test
{
    public class CurveModelTest
    {
        private static readonly Random random = new Random();
        private const string OutputFilePath = "CurveModelTestResults.txt";

        public static void RunTests()
        {
            var controlPoints4 = GenerateRandomPoints(4);
            var controlPoints5 = GenerateRandomPoints(5);
            var controlPoints10 = GenerateRandomPoints(10);
            var controlPoints20 = GenerateRandomPoints(20);

            var steps = new[] { 0.001 }; // 0.01, 0.001
            var controlPointsSets = new[] { controlPoints4, controlPoints5, controlPoints10, controlPoints20 };

            using (var writer = new StreamWriter(OutputFilePath))
            {
                foreach (CurveTypeEnum curveType in Enum.GetValues(typeof(CurveTypeEnum)))
                {
                    if (!IsValidCurveType(curveType))
                        continue;

                    writer.WriteLine($"Testing {curveType}");

                    foreach (var step in steps)
                    {
                        foreach (var controlPoints in controlPointsSets)
                        {
                            int degree = controlPoints.Count - 1;
                            double averageTime = MeasureAverageTime(curveType, step, controlPoints, degree);
                            writer.WriteLine($"Step: {step}, Control Points: {controlPoints.Count}, Degree: {degree}, Average Time: {averageTime} ms");
                        }
                    }

                    writer.WriteLine();
                }
            }

            Console.WriteLine($"Test results written to {OutputFilePath}");
        }

        private static List<PointModel> GenerateRandomPoints(int count)
        {
            var points = new List<PointModel>();
            for (int i = 0; i < count; i++)
            {
                var x = random.Next(0, 501);
                var y = random.Next(0, 501);

                points.Add(new PointModel(x, y, x, y));
            }
            return points;
        }

        private static double MeasureAverageTime(CurveTypeEnum curveType, double step, List<PointModel> controlPoints, int degree)
        {
            const int iterations = 5;
            double totalTime = 0;

            var curveModel = new CurveModel { Step = step, Degree = degree };
            curveModel.ControlPointsModel.ControlPointsNotScaled = new ObservableCollection<PointModel>(controlPoints);
            curveModel.SetCurveType(curveType);

            for (int i = 0; i < iterations; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                curveModel.DrawCurve();
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalMilliseconds;
            }

            return totalTime / iterations;
        }

        private static bool IsValidCurveType(CurveTypeEnum curveType)
        {
            return curveType == CurveTypeEnum.BezierSpline ||
                   curveType == CurveTypeEnum.BezierSplineModified ||
                   curveType == CurveTypeEnum.BezierSplineNumerical ||
                   curveType == CurveTypeEnum.CubicHermiteSpline ||
                   curveType == CurveTypeEnum.NURBSpline;
        }
    }
}
