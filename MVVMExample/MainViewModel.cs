using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace MVVMExample
{
    public class MouseMoveBehavior : Behavior<Canvas>
    {
        public double MouseTop
        {
            get { return (double)GetValue(MouseTopProperty); }
            set { SetValue(MouseTopProperty, value); }
        }

        public static readonly DependencyProperty MouseTopProperty =
            DependencyProperty.Register("MouseTop", typeof(double), typeof(MouseMoveBehavior), new PropertyMetadata(0d));

        public double MouseLeft
        {
            get { return (double)GetValue(MouseLeftProperty); }
            set { SetValue(MouseLeftProperty, value); }
        }

        public static readonly DependencyProperty MouseLeftProperty =
            DependencyProperty.Register("MouseLeft", typeof(double), typeof(MouseMoveBehavior), new PropertyMetadata(0d));

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var currentPosition = e.GetPosition(AssociatedObject);
            MouseLeft = currentPosition.X;
            MouseTop = currentPosition.Y;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

   
}

