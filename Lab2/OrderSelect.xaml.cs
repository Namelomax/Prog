using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CafeOrderApp{
public partial class OrderSelect : Window
{
    private ObservableCollection<OrderItem> cartItems = new ObservableCollection<OrderItem>();
    Style style = (Style)Application.Current.FindResource("TextBlockInListBoxItemsControl");
    private MainWindow mainWindow;
    

    public OrderSelect(MainWindow mainWindow)
    {
                   try
    {
        InitializeComponent();
        this.mainWindow= mainWindow;
        CartItemsControl.ItemsSource = cartItems;
        LoadDishes();
            }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown();
    }
    }

 private void LoadDishes()
        {
            var dishes = new List<Dish>
            {
                new Dish { Name = "Пицца", ImagePath = "Images/pizza.jpg", Description = "Вкусная пицца с томатным соусом и сыром.", Options = new[] { "Сыр", "Соус", "Перец" } },
                new Dish { Name = "Бургер", ImagePath = "Images/burger.jpg", Description = "Сочный бургер с котлетой и свежими овощами.", Options = new[] { "Сыр", "Салат", "Бекон" } },
                new Dish { Name = "Салат", ImagePath = "Images/salad.jpg", Description = "Полезный овощной салат с оливковым маслом.", Options = new[] { "Сыр", "Курица", "Гренки" } },
                new Dish { Name = "Паста", ImagePath = "Images/pasta.jpg", Description = "Итальянская паста с томатным соусом и базиликом.", Options = new[] { "Сыр", "Соус", "Грибы" } }
            };

            foreach (var dish in dishes)
            {
                var dishBorder = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10),
                    Margin = new Thickness(5)
                };

                var grid = new Grid { Margin = new Thickness(5) };
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Добавки
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Картинка + описание
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Кнопки

                // Панель с CheckBox для добавок
                var optionsPanel = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
                foreach (var option in dish.Options)
                {
                    optionsPanel.Children.Add(new CheckBox { Content = option, Margin = new Thickness(5, 0, 5, 0) });
                }
                Grid.SetRow(optionsPanel, 0);
                grid.Children.Add(optionsPanel);

                // Панель с картинкой и описанием
                var contentPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(dish.ImagePath, UriKind.Relative)),
                    Width = 140,
                    Height = 140,
                    Margin = new Thickness(10)
                };
                contentPanel.Children.Add(image);

                var textPanel = new StackPanel { Margin = new Thickness(10, 0, 0, 0) };
                textPanel.Children.Add(new TextBlock { Text = dish.Name, FontSize = 16, FontWeight = FontWeights.Bold, Style=style });
                textPanel.Children.Add(new TextBlock { Text = dish.Description, Style=style, FontSize = 12, TextWrapping = TextWrapping.Wrap, MaxWidth = 200 });
                contentPanel.Children.Add(textPanel);

                Grid.SetRow(contentPanel, 1);
                grid.Children.Add(contentPanel);

                // Панель для кнопок +/-
                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(5) };

                var decreaseButton = new Button { Content = "-", Width = 30, Height = 30};
                var quantityText = new TextBlock { Text = "1", FontSize = 16, Width = 30, Style=style, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                var increaseButton = new Button { Content = "+", Width = 30, Height = 30};

                int quantity = 1;
                decreaseButton.Click += (s, e) => 
                {
                    if (quantity > 1) 
                    {
                        quantity--;
                        quantityText.Text = quantity.ToString();
                    }
                };
                increaseButton.Click += (s, e) =>
                {
                    quantity++;
                    quantityText.Text = quantity.ToString();
                };

                buttonPanel.Children.Add(decreaseButton);
                buttonPanel.Children.Add(quantityText);
                buttonPanel.Children.Add(increaseButton);

                var addButton = new Button
                {
                    Content = "Добавить в корзину",
                    Width = 150,
                    Height = 40,
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                addButton.Click += (s, e) => AddToCart(dish, optionsPanel, quantity);

                var buttonStack = new StackPanel { Orientation = Orientation.Vertical };
                buttonStack.Children.Add(buttonPanel);
                buttonStack.Children.Add(addButton);

                Grid.SetRow(buttonStack, 2);
                grid.Children.Add(buttonStack);

                dishBorder.Child = grid;
                DishesGrid.Children.Add(dishBorder);
            }
        }

       private void AddToCart(Dish dish, WrapPanel optionsPanel, int quantity)
{
    // Получаем выбранные опции
    var selectedOptions = optionsPanel.Children.OfType<CheckBox>()
        .Where(cb => cb.IsChecked == true)
        .Select(cb => cb.Content.ToString())
        .ToList();
    // Ищем, есть ли уже это блюдо в корзине
    var existingItem = cartItems.FirstOrDefault(item => item.Dish.Name == dish.Name && 
                                                    item.Options.SequenceEqual(selectedOptions));
    if (existingItem != null)
    {
        // Если блюдо уже есть в корзине, просто увеличиваем количество
        existingItem.Quantity += quantity;
        existingItem.Options = selectedOptions; // Обновляем опции, если они изменились
    }
    else
    {
        // Добавляем новое блюдо в корзину
        var orderItem = new OrderItem
        {
            Dish = dish,
            Quantity = quantity,
            Options = selectedOptions
        };
        cartItems.Add(orderItem);
    }
}


    private void ConfirmSelection_Click(object sender, RoutedEventArgs e)
    {
        mainWindow.GetDishes(cartItems);
        Close();
    }
}

public class Dish
{
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public string Description { get; set; }
    public string[] Options { get; set; }
}
public class OrderItem : INotifyPropertyChanged
{
    public Dish Dish { get; set; }

    private int quantity;
    public int Quantity
    {
        get => quantity;
        set
        {
            quantity = value;
            OnPropertyChanged();
        }
    }

    private List<string> options;
    public List<string> Options
    {
        get => options;
        set
        {
            options = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(OptionsString)); // Уведомляем и о строке
        }
    }

    public string OptionsString => Options != null && Options.Any() ? string.Join(", ", Options) : "Нет опций";

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
}