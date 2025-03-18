using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using CafeOrderApp;
namespace DBManager{
public class DbManager
{
    private readonly string connectionString;

    public DbManager(string connectionString)
    {
        this.connectionString = connectionString;
    }

    // Метод для добавления нового заказа
public int AddOrder(string clientName, string dish, string orderType, List<string> options)
{
    int orderId = 0;

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        SqlCommand command = new SqlCommand("AddOrder", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@ClientName", clientName);
        command.Parameters.AddWithValue("@Dish", dish);
        command.Parameters.AddWithValue("@OrderType", orderType);
        SqlParameter outputId = new SqlParameter("@OrderId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(outputId);

        connection.Open();
        command.ExecuteNonQuery();
        orderId = (int)outputId.Value;
    }

    // Добавляем опции в базу
    foreach (string option in options)
    {
        AddOrderOption(orderId, option);
    }

    return orderId;
}

private void AddOrderOption(int orderId, string optionName)
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        SqlCommand command = new SqlCommand("AddOrderOption", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("@OrderId", orderId);
        command.Parameters.AddWithValue("@OptionName", optionName);

        connection.Open();
        command.ExecuteNonQuery();
    }
}


    // Метод для получения всех заказов
public List<Order> GetOrders()
{
    List<Order> orders = new List<Order>();

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        SqlCommand command = new SqlCommand("GetOrders", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            orders.Add(new Order
            {
                OrderId = (int)reader["OrderId"],
                ClientName = reader["ClientName"].ToString(),
                Dish = reader["Dish"].ToString(),
                OrderType = reader["OrderType"].ToString(),
                Options = (reader["Options"]?.ToString() ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList(),
            });
        }
    }

    return orders;
}
    // Метод для удаления заказа
    public void ClearAllOrders()
    {
            using (SqlConnection connection = new SqlConnection(connectionString))
    {
        SqlCommand command = new SqlCommand("ClearOrders", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        command.ExecuteNonQuery();
    }
    }
}
}