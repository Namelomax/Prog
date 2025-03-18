using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using DBManager;
using Lab1;
namespace CafeOrderApp
{
    public partial class MainWindow : Window
    {
        private DbManager dbManager;
        public MainWindow()
        {
            try
    {
        InitializeComponent();
        dbManager = new DbManager("Server=Jacob-book\\SQLEXPRESS;Database=CafeOrderDB;User Id=MyUser4;Password=123;TrustServerCertificate=True;");
        LoadOrders();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown();
    }
        }
private void LoadOrders(){
    var orders = dbManager.GetOrders();
    OrdersListBox.ItemsSource = orders;
    OrdersListBox.Items.Refresh();
    OrdersListBox.Visibility = Visibility.Visible;
}
private List<string> GetSelectedOptions()
{
    List<string> options = new List<string>();;
    if (CheeseCheckBox.IsChecked == true) options.Add("Сыр");
    if (SauceCheckBox.IsChecked == true) options.Add("Соус");
    if (PepperCheckBox.IsChecked == true) options.Add("Перец");
    return options;
}

        private string GetSelectedOrderType()
        {
            return TakeawayRadioButton.IsChecked == true ? "Взять с собой" : "Доставка";
        }
private void ClearOrdersButton_Click(object sender, RoutedEventArgs e)
        {
 try
    {
        dbManager.ClearAllOrders();
        LoadOrders();
        OrdersListBox.Visibility = Visibility.Collapsed;
        StatusLabel.Foreground = Brushes.Green;
        StatusLabel.Content = "Все заказы удалены из базы данных!";
        StatusLabel.Visibility = Visibility.Visible;
        StartFadeOutAnimation();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при очистке базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
        }
private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
{
    try
    {

        string name = NameTextBox.Text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            StatusLabel.Content = "Пожалуйста, введите имя!";
            StatusLabel.Foreground = Brushes.Red;
            StatusLabel.Visibility = Visibility.Visible;
            StartFadeOutAnimation();
            return;
        }

        if (DishComboBox.SelectedItem == null)
        {
            StatusLabel.Content = "Пожалуйста, выберите основное блюдо!";
            StatusLabel.Foreground = Brushes.Red;
            StatusLabel.Visibility = Visibility.Visible;
            StartFadeOutAnimation();
            return;
                    }
            var options = GetSelectedOptions();
            var orderType = GetSelectedOrderType();
            var selectedDish = DishComboBox.SelectedItem?.ToString() ?? ""; // Проверка на null

            Order newOrder = new Order
            {
                ClientName = name,
                Dish = selectedDish,
                OrderType = orderType,
                Options = options,
            };

            dbManager.AddOrder(newOrder.ClientName, newOrder.Dish, newOrder.OrderType, newOrder.Options);
            LoadOrders();

        StatusLabel.Content = "Заказ оформлен!";
        StatusLabel.Foreground = Brushes.Green;
        StatusLabel.Visibility = Visibility.Visible;    
        StartFadeOutAnimation();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
        private void StartFadeOutAnimation()
        {
            // Создаем анимацию для исчезновения
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(2))
            };

            // Применяем анимацию к свойству Opacity
            StatusLabel.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }
        private bool isDarkTheme = false;

private void ToggleTheme_Click(object sender, RoutedEventArgs e)
{
    var theme = isDarkTheme ? "Styles.xaml" : "DarkTheme.xaml";
    var dict = new ResourceDictionary { Source = new Uri(theme, UriKind.Relative) };

    Application.Current.Resources.MergedDictionaries.Clear();
    Application.Current.Resources.MergedDictionaries.Add(dict);

    isDarkTheme = !isDarkTheme;
}
}
    }
