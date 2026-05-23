using System.Collections.Generic;
using mvvm_edp.Models;
using MySqlConnector;
using System.Threading.Tasks;
using System.Linq;


namespace mvvm_edp.Repositories;

public class OrderItemRepository : IRepository<OrderItem>
{
    public async Task<List<OrderItem>> GetAllItemsAsync()
    {

        var orderItems = new List<OrderItem>();
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "SELECT * FROM order_items;",
            conn
            );

        await using var reader = await payload.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            orderItems.Add(new OrderItem(
                reader.GetInt32("order_id"),
                reader.GetInt32("product_id"),
                reader.GetInt32("quantity")
                ));
        }
        return orderItems;
    }

    public async Task<int> AddItemAsync(OrderItem item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "INSERT INTO order_items(order_id, product_id, quantity)" +
            "VALUES(@order_id, @product_id, @quantity);",
            conn
            );

        payload.Parameters.AddWithValue("@order_id", item.OrderId);
        payload.Parameters.AddWithValue("@product_id", item.ProductId);
        payload.Parameters.AddWithValue("@quantity", item.Quantity);

        return await payload.ExecuteNonQueryAsync();
    }

    // No deletion. unused
    public async Task<int> RemoveItemAsync(OrderItem item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "DELETE FROM order_items WHERE orderItem_id = @orderItem_id;",
            conn
            );

        payload.Parameters.AddWithValue("@orderItem_id", item.OrderId);
        return await payload.ExecuteNonQueryAsync();
    }

    public async Task RemoveAllItemsAsync(List<OrderItem> items)
    {
        foreach (var p in items)
        {
            await RemoveItemAsync(p);
        }
    }
    public async Task<int> UpdateItemAsync(OrderItem item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);

        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "UPDATE order_items SET quantity=@quantity WHERE order_id = @order_id AND product_id = @product_id;",
            conn
            );

        payload.Parameters.AddWithValue("@order_id", item.OrderId);
        payload.Parameters.AddWithValue("@product_id", item.ProductId);
        payload.Parameters.AddWithValue("@quantity", item.Quantity);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task UpdateAllItemsAsync(List<OrderItem> items)
    {
        foreach (var p in items)
        {
            await UpdateItemAsync(p);

        }
    }

    public OrderItemRepository()
    {

    }

}

