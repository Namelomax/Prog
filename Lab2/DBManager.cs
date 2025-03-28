using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.ObjectModel;
using CafeOrderApp;
namespace DBManager{
public class DbManager
{
    private readonly string connectionString;

    public DbManager(string connectionString)
    {
        this.connectionString = connectionString;
    }

        // Метод для добавления блюда в базу данных
    public void AddOrder(string name, string orderType, ObservableCollection<OrderItem> orderItems)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Вставляем заказ в таблицу Orders
            var insertOrderCommand = new SqlCommand("INSERT INTO Orders (ClientName, OrderType) VALUES (@ClientName, @OrderType); SELECT SCOPE_IDENTITY()", connection);
            insertOrderCommand.Parameters.AddWithValue("@ClientName", name);
            insertOrderCommand.Parameters.AddWithValue("@OrderType", orderType);
            int orderId = Convert.ToInt32(insertOrderCommand.ExecuteScalar());

            // Вставляем блюда и их опции
            foreach (var item in orderItems)
            {
                var insertOrderDishCommand = new SqlCommand("INSERT INTO OrderDishes (OrderId, Dish, Quantity) VALUES (@OrderId, @Dish, @Quantity); SELECT SCOPE_IDENTITY()", connection);
                insertOrderDishCommand.Parameters.AddWithValue("@OrderId", orderId);
                insertOrderDishCommand.Parameters.AddWithValue("@Dish", item.Dish.Name);
                insertOrderDishCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                int orderDishId = Convert.ToInt32(insertOrderDishCommand.ExecuteScalar());

                // Вставляем опции для каждого блюда
                foreach (var option in item.Options)
                {
                    var insertOptionCommand = new SqlCommand("INSERT INTO OrderOptions (OrderDishId, OptionName) VALUES (@OrderDishId, @OptionName)", connection);
                    insertOptionCommand.Parameters.AddWithValue("@OrderDishId", orderDishId);
                    insertOptionCommand.Parameters.AddWithValue("@OptionName", option);
                    insertOptionCommand.ExecuteNonQuery();
                }
            }
        }
    }
        public void ClearAllOrders()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            // Удаляем все записи из OrderOptions и OrderDishes сначала, чтобы избежать ошибок внешнего ключа
            var deleteOptionsCommand = new SqlCommand("DELETE FROM OrderOptions", connection);
            deleteOptionsCommand.ExecuteNonQuery();

            var deleteDishesCommand = new SqlCommand("DELETE FROM OrderDishes", connection);
            deleteDishesCommand.ExecuteNonQuery();

            // Теперь можно безопасно удалить записи из Orders
            var deleteOrdersCommand = new SqlCommand("DELETE FROM Orders", connection);
            deleteOrdersCommand.ExecuteNonQuery();
        }
    }
       public List<Order> GetOrders()
{
    var orders = new List<Order>();

    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();

        // Получаем все заказы
        var getOrdersCommand = new SqlCommand("SELECT OrderId, ClientName, OrderType FROM Orders", connection);
        var orderDict = new Dictionary<int, Order>();

        using (var orderReader = getOrdersCommand.ExecuteReader())
        {
            while (orderReader.Read())
            {
                var order = new Order
                {
                    OrderId = orderReader.GetInt32(orderReader.GetOrdinal("OrderId")),
                    ClientName = orderReader.GetString(orderReader.GetOrdinal("ClientName")),
                    OrderType = orderReader.GetString(orderReader.GetOrdinal("OrderType")),
                    Items = new ObservableCollection<OrderItem>()
                };

                orders.Add(order);
                orderDict[order.OrderId] = order;
            }
        }

        // Получаем все блюда для всех заказов за один запрос
        var getDishesCommand = new SqlCommand("SELECT OrderDishId, OrderId, Dish, Quantity FROM OrderDishes", connection);
        var dishDict = new Dictionary<int, OrderItem>();

        using (var dishReader = getDishesCommand.ExecuteReader())
        {
            while (dishReader.Read())
            {
                int orderId = dishReader.GetInt32(dishReader.GetOrdinal("OrderId"));

                var orderItem = new OrderItem
                {
                    Dish = new Dish { Name = dishReader.GetString(dishReader.GetOrdinal("Dish")) },
                    Quantity = dishReader.GetInt32(dishReader.GetOrdinal("Quantity")),
                    Options = new List<string>()
                };

                int orderDishId = dishReader.GetInt32(dishReader.GetOrdinal("OrderDishId"));

                // Добавляем блюдо к заказу
                if (orderDict.ContainsKey(orderId))
                {
                    orderDict[orderId].Items.Add(orderItem);
                    dishDict[orderDishId] = orderItem;
                }
            }
        }

        // Получаем все опции для всех блюд за один запрос
        var getOptionsCommand = new SqlCommand("SELECT OrderDishId, OptionName FROM OrderOptions", connection);

        using (var optionReader = getOptionsCommand.ExecuteReader())
        {
            while (optionReader.Read())
            {
                int orderDishId = optionReader.GetInt32(optionReader.GetOrdinal("OrderDishId"));
                string option = optionReader.GetString(optionReader.GetOrdinal("OptionName"));

                if (dishDict.ContainsKey(orderDishId))
                {
                    dishDict[orderDishId].Options.Add(option);
                }
            }
        }
    }

    return orders;
}
public void UpdateOrder(Order order)
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();

        // Обновляем заказ в таблице Orders
        var updateOrderCommand = new SqlCommand("UPDATE Orders SET ClientName = @ClientName, OrderType = @OrderType WHERE OrderId = @OrderId", connection);
        updateOrderCommand.Parameters.AddWithValue("@OrderId", order.OrderId);
        updateOrderCommand.Parameters.AddWithValue("@ClientName", order.ClientName);
        updateOrderCommand.Parameters.AddWithValue("@OrderType", order.OrderType);
        updateOrderCommand.ExecuteNonQuery();

        // Сначала удалим старые блюда и опции, связанные с этим заказом
        var deleteOrderDishesCommand = new SqlCommand("DELETE FROM OrderDishes WHERE OrderId = @OrderId", connection);
        deleteOrderDishesCommand.Parameters.AddWithValue("@OrderId", order.OrderId);
        deleteOrderDishesCommand.ExecuteNonQuery();

        var deleteOrderOptionsCommand = new SqlCommand("DELETE FROM OrderOptions WHERE OrderDishId IN (SELECT OrderDishId FROM OrderDishes WHERE OrderId = @OrderId)", connection);
        deleteOrderOptionsCommand.Parameters.AddWithValue("@OrderId", order.OrderId);
        deleteOrderOptionsCommand.ExecuteNonQuery();

        // Вставляем обновленные блюда и их опции
        foreach (var item in order.Items)
        {
            // Вставляем или обновляем блюда
            var insertOrderDishCommand = new SqlCommand("INSERT INTO OrderDishes (OrderId, Dish, Quantity) VALUES (@OrderId, @Dish, @Quantity); SELECT SCOPE_IDENTITY()", connection);
            insertOrderDishCommand.Parameters.AddWithValue("@OrderId", order.OrderId);
            insertOrderDishCommand.Parameters.AddWithValue("@Dish", item.Dish.Name);
            insertOrderDishCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
            int orderDishId = Convert.ToInt32(insertOrderDishCommand.ExecuteScalar());

            // Вставляем опции для каждого блюда
            foreach (var option in item.Options)
            {
                var insertOptionCommand = new SqlCommand("INSERT INTO OrderOptions (OrderDishId, OptionName) VALUES (@OrderDishId, @OptionName)", connection);
                insertOptionCommand.Parameters.AddWithValue("@OrderDishId", orderDishId);
                insertOptionCommand.Parameters.AddWithValue("@OptionName", option);
                insertOptionCommand.ExecuteNonQuery();
            }
        }
    }
}


}
}