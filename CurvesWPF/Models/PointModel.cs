using System.Windows;
using System.Windows.Media.Media3D;

namespace CurvesWPF.Models
{
    public class PointModel : PropertyChangedBase
    {
        public double X;
        public double Y;

        public double DX;
        public double DY;

        public double ScaledX;
        public double ScaledY;

        public double Weight;

        private double _topLeftX;
        public double TopLeftX
        {
            get => _topLeftX;
            set
            {
                if (_topLeftX != value)
                {
                    _topLeftX = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _topLeftY;
        public double TopLeftY
        {
            get => _topLeftY;
            set
            {
                if (_topLeftY != value)
                {
                    _topLeftY = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _diameter;

        public double Diameter
        {
            get => _diameter;
            set
            {
                if (_diameter != value)
                {
                    _diameter = value;
                    OnPropertyChanged();
                }
            }
        }

        public PointModel(double x, double y, double scaledX, double scaledY)
        {
            X = x;
            Y = y;
            ScaledX = scaledX;
            ScaledY = scaledY;

            Diameter = 8;

            Weight = 1.0;

            TopLeftX = ScaledX - Diameter / 2;
            TopLeftY = ScaledY - Diameter / 2;
        }

        public override string ToString()
        {
            return $"X:{X} Y:{Y}";
        }

        public Point ToPoint() => new(ScaledX, ScaledY);

        public void ResizePoint()
        {

            if (Diameter == 8)
            {
                Diameter = 4;
                TopLeftX += 2;
                TopLeftY += 2;
            }
            else
            {
                Diameter = 8;
                TopLeftX -= 2;
                TopLeftY -= 2;
            }
        }

        public void DisplacePoint(Point displacement, double scale)
        {
            X += displacement.X;
            Y += displacement.Y;

            ScaledX += scale * displacement.X;
            ScaledY += scale * displacement.Y;

            TopLeftX = ScaledX - Diameter / 2; 
            TopLeftY = ScaledY - Diameter / 2;
        }
    }
}
