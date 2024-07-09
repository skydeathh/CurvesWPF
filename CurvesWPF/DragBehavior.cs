using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using CurvesWPF.Models;
using CurvesWPF.ViewModels;

namespace CurvesWPF
{
    public class DragBehavior : Behavior<UIElement>
    {
        private Point mouseOffset;
        private Canvas canvas;
        private Ellipse ellipse;
        public PointModel AssociatedPoint { get; set; }
        private bool isDreagging;

        public static readonly DependencyProperty IsDraggingProperty =
        DependencyProperty.Register("IsDragging", typeof(bool), typeof(DragBehavior));

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            this.AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            this.AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            this.AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ellipse = sender as Ellipse;

            if (ellipse != null)
            {
                isDreagging = true;
                mouseOffset = e.GetPosition(ellipse);

                var itemControl = VisualTreeHelper.GetParent(ellipse);
                canvas = VisualTreeHelper.GetParent(itemControl) as Canvas;

                if (canvas == null)
                    return;
            }
            AssociatedPoint = (sender as FrameworkElement).DataContext as PointModel;
            ellipse.CaptureMouse();
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDreagging && ellipse != null)
            {
                IsDragging = true;
                Point canvasMousePosition = e.GetPosition(canvas);
                double newX = canvasMousePosition.X - mouseOffset.X;
                double newY = canvasMousePosition.Y - mouseOffset.Y;

                if (ellipse.RenderTransform is TranslateTransform translateTransform)
                {
                    translateTransform.X = newX;
                    translateTransform.Y = newY;
                }

                AssociatedPoint.ScaledX = canvasMousePosition.X;
                AssociatedPoint.ScaledY = canvasMousePosition.Y;
            }
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDreagging && ellipse != null)
            {
                ellipse.ReleaseMouseCapture();
                isDreagging = false;
                ellipse = null;
            }
        }
    }
}