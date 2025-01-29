using System.Windows;
using System.Windows.Media.Animation;

namespace CafeOrderApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на ввод имени
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                StatusLabel.Content = "Пожалуйста, введите имя!";
                StatusLabel.Foreground = System.Windows.Media.Brushes.Red;
                StatusLabel.Visibility = Visibility.Visible;
                StartFadeOutAnimation();
                return;
            }

            // Проверка выбора блюда
            if (DishComboBox.SelectedItem == null)
            {
                StatusLabel.Content = "Пожалуйста, выберите основное блюдо!";
                StatusLabel.Foreground = System.Windows.Media.Brushes.Red;
                StatusLabel.Visibility = Visibility.Visible;
                StartFadeOutAnimation();
                return;
            }

            string dish = DishComboBox.SelectedItem.ToString();

            string extras = "";
            if (CheeseCheckBox.IsChecked == true) extras += "Сыр, ";
            if (SauceCheckBox.IsChecked == true) extras += "Соус, ";
            if (PepperCheckBox.IsChecked == true) extras += "Перец, ";
            extras = extras.TrimEnd(',', ' ');
            string orderExtras = string.IsNullOrEmpty(extras) ? "" : $"({extras})";
            string method = TakeawayRadioButton.IsChecked == true ? "Взять с собой" : "Доставка";
            string order = $"{name} заказал {dish} {orderExtras} [{method}]";

            // Добавляем заказ в список
            OrdersListBox.Items.Add(order);
             if (OrdersListBox.Visibility == Visibility.Collapsed){
            OrdersListBox.Visibility = Visibility.Visible;}

            // Сбрасываем статус
            StatusLabel.Content = "Заказ оформлен!";
            StatusLabel.Foreground = System.Windows.Media.Brushes.Green;
            StatusLabel.Visibility = Visibility.Visible;

            // Запуск анимации для плавного исчезновения
            StartFadeOutAnimation();
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
    }
}