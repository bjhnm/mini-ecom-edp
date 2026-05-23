using mvvm_edp.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvvm_edp.Repositories;

public class OrderRepository : IRepository<Order>
{
    public async Task<List<Order>> GetAllItemsAsync()
    {

        var orders = new List<Order>();
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "SELECT * FROM orders;",
            conn
            );

        await using var reader = await payload.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            orders.Add(new Order(
                reader.GetInt32("order_id"),
                reader.GetInt32("customer_id"),
                reader.GetDateTime("order_date")
                ));
        }
        return orders;
    }

    public async Task<int> AddItemAsync(Order order)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "INSERT INTO orders(customer_id) VALUES(@customer_id);",
            conn
        );

        payload.Parameters.AddWithValue("@customer_id", order.CustomerId);

        await payload.ExecuteNonQueryAsync();

        await using var cmd = new MySqlCommand("SELECT LAST_INSERT_ID();", conn);
        var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

        return id;
    }

    public async Task<int> RemoveItemAsync(Order item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "DELETE FROM orders WHERE order_id = @order_id;",
            conn
            );

        payload.Parameters.AddWithValue("@order_id", item.OrderId);
        return await payload.ExecuteNonQueryAsync();
    }

    /**
    public async Task RemoveAllItemsAsync(List<Order> items)
    {
        foreach (var p in items.Where(p => p.IsSelected))
        {
            await RemoveItemAsync(p);
        }
    }
    */

    public async Task RemoveAllItemsAsync(List<Order> items) { }
    public async Task<int> UpdateItemAsync(Order item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);

        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "UPDATE orders SET customer_ide=@customer_id, order_date=@order_date, price=@price WHERE order_id = @order_id;",
            conn
            );

        payload.Parameters.AddWithValue("@order_id", item.OrderId);
        payload.Parameters.AddWithValue("@customer_id", item.CustomerId);
        payload.Parameters.AddWithValue("@order_date", item.OrderDate);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task UpdateAllItemsAsync(List<Order> items)
    {
        foreach (var p in items)
        {
            await UpdateItemAsync(p);

        }
    }

    public OrderRepository()
    {

    }

}

