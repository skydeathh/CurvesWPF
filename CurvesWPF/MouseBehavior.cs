using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CurvesWPF
{
    public class MouseBehaviour : Behavior<Canvas>
    {
        public static readonly DependencyProperty MouseYProperty = DependencyProperty.Register(
            "MouseY", typeof(int), typeof(MouseBehaviour), new PropertyMetadata(default(int)));

        public int MouseY
        {
            get { return (int)GetValue(MouseYProperty); }
            set { SetValue(MouseYProperty, value); }
        }

        public static readonly DependencyProperty MouseXProperty = DependencyProperty.Register(
            "MouseX", typeof(int), typeof(MouseBehaviour), new PropertyMetadata(default(int)));

        public int MouseX
        {
            get { return (int)GetValue(MouseXProperty); }
            set { SetValue(MouseXProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var pos = mouseEventArgs.GetPosition(AssociatedObject);
            MouseX = (int)pos.X;
            MouseY = (int)pos.Y;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
        }
    }
}
