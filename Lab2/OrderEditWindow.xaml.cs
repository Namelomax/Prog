using System.Windows;
using DBManager;

namespace CafeOrderApp
{
    public partial class OrderEditWindow : Window
    {
        // Используем публичное свойство Order для редактируемого заказа
        public Order Order { get; set; }
        private DbManager dbManager;

        public OrderEditWindow(Order order)
        {
            InitializeComponent();
            Order = order;
            DataContext = this; // Для двусторонней привязки
        }

        private void RemoveDish_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItemsListBox.SelectedItem is OrderItem selectedItem)
            {
                Order.Items.Remove(selectedItem);
                OrderItemsListBox.Items.Refresh();
            }
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            dbManager = new DbManager("Server=Jacob-book\\SQLEXPRESS;Database=CafeOrderDB;User Id=MyUser4;Password=123;TrustServerCertificate=True;");
            dbManager.UpdateOrder(Order);
            DialogResult = true;
            Close();
        }
    }
}
