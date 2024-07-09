using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace CurvesWPF
{
    public class ResizeBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty CanvasWidthProperty = DependencyProperty.Register(
            "CanvasWidth", typeof(double), typeof(ResizeBehavior), new PropertyMetadata(default(double)));

        public double CanvasWidth
        {
            get { return (double)GetValue(CanvasWidthProperty); }
            set { SetValue(CanvasWidthProperty, value); }
        }

        public static readonly DependencyProperty CanvasHeightProperty = DependencyProperty.Register(
            "CanvasHeight", typeof(double), typeof(ResizeBehavior), new PropertyMetadata(default(double)));

        public double CanvasHeight
        {
            get { return (double)GetValue(CanvasHeightProperty); }
            set { SetValue(CanvasHeightProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;
        }

        private void AssociatedObjectOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CanvasWidth = e.NewSize.Width;
            CanvasHeight = e.NewSize.Height;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SizeChanged -= AssociatedObjectOnSizeChanged;
        }
    }
}