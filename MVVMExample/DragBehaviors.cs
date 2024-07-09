using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows;

namespace MVVMExample
{
    public class DragBehaviors : Behavior<UIElement>
    {
        private bool isDragging = false;
        private Point mouseOffset;
        private Canvas canvas;

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
            isDragging = true;

            mouseOffset = e.GetPosition(AssociatedObject);

            AssociatedObject.CaptureMouse();
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                if (canvas == null)
                {
                    canvas = VisualTreeHelper.GetParent(this.AssociatedObject) as Canvas;
                    if (canvas == null)
                        return; // Return if canvas is still null
                }

                Point point = e.GetPosition(canvas);
                AssociatedObject.SetValue(Canvas.TopProperty, point.Y - mouseOffset.Y);
                AssociatedObject.SetValue(Canvas.LeftProperty, point.X - mouseOffset.X);
            }
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                AssociatedObject.ReleaseMouseCapture();
                isDragging = false;
            }
        }
    }
}