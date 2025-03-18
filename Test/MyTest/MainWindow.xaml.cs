using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace MyTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
        private void Button_Click(object sender, RoutedEventArgs e)
    {
        ColorAnimation colorAnimation = new ColorAnimation();
        colorAnimation.From = Colors.White;
        colorAnimation.To = Colors.LightBlue;
        colorAnimation.Duration = TimeSpan.FromSeconds(1);
        myGrid.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
    }
}