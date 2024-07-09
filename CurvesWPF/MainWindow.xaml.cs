using System.Windows;
using CurvesWPF.Test;
using CurvesWPF.ViewModels;

namespace CurvesWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            //var CurveModelTest = new CurveModelTest();
            //CurveModelTest.RunTests();
        }
    }
}
