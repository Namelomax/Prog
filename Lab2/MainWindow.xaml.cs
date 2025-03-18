using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using DBManager;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Lab1;
namespace CafeOrderApp
{
    public partial class MainWindow : Window
    {
        private DbManager dbManager;
        private ObservableCollection<OrderItem> selectedDishes = new ObservableCollection<OrderItem>();
        
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
        this.Close();
    }
        }
private void LoadOrders(){
    var orders = dbManager.GetOrders();
    OrdersListBox.ItemsSource = orders;
    OrdersListBox.Items.Refresh();
    OrdersListBox.Visibility = Visibility.Visible;
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

public List<OrderItem> GetSelectedDishes()
{
    return selectedDishes.Select(selectedDish => new OrderItem
    {
        Dish = selectedDish.Dish,  // Теперь передаем сам объект Dish, а не только его название
        Options = GetSelectedOptionsForDish(selectedDish.Dish), // Опции
        Quantity = selectedDish.Quantity
    }).ToList();
}



public List<string> GetSelectedOptionsForDish(Dish dish)
{
    // Здесь должен быть реальный выбор опций, пока заглушка
    return new List<string> { "Option 1", "Option 2" };
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
            string orderType = GetSelectedOrderType(); // Комбо-бокс для типа заказа

            // Отправляем заказ в базу данных


        dbManager.AddOrder(name, orderType, selectedDishes);
            MessageBox.Show("Заказ успешно оформлен!");

        // Обновляем список заказов на форме
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
    try
    {
        string themeFile = isDarkTheme ? "Styles.xaml" : "DarkTheme.xaml";
        Uri themeUri = new Uri(themeFile, UriKind.Relative);
        ResourceDictionary themeDictionary = new ResourceDictionary { Source = themeUri };

        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(themeDictionary);

        isDarkTheme = !isDarkTheme;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Произошла ошибка при переключении темы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
private void SelectDishesButton_Click(object sender, RoutedEventArgs e)
{
    OrderSelect orderSelect = new OrderSelect(this);
     
    orderSelect.Owner = this;
     
    orderSelect.ShowDialog();
}

public void GetDishes(ObservableCollection<OrderItem> dishes)
{
    selectedDishes = dishes;
    CartItemsControl.ItemsSource = null;
    CartItemsControl.ItemsSource = dishes;
    CartPanel.Visibility = Visibility.Visible;
}

}
    }
