using System.Collections.Generic;
using mvvm_edp.Models;
using MySqlConnector;
using System.Threading.Tasks;
using System.Linq;


namespace mvvm_edp.Repositories;

public class PaymentRepository : IRepository<Payment>
{
    public async Task<List<Payment>> GetAllItemsAsync()
    {

        var payments = new List<Payment>();
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "SELECT * FROM payments;",
            conn
            );

        await using var reader = await payload.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            payments.Add(new Payment(
                reader.GetInt32("payment_id"),
                reader.GetInt32("order_id"),
                reader.GetDecimal("amount"),
                reader.GetDateTime("payment_date"),
                reader.GetString("payment_status"),
                reader.GetString("payment_method")
                ));
        }
        return payments;
    }

    public async Task<int> AddItemAsync(Payment item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "INSERT INTO payments(order_id, amount, payment_method, payment_status)" +
            "VALUES(@order_id, @amount, @payment_method, @payment_status);",
            conn
            );

        payload.Parameters.AddWithValue("@order_id", item.OrderId);
        //payload.Parameters.AddWithValue("@payment_date", item.PaymentDate);
        payload.Parameters.AddWithValue("@amount", item.Amount);
        payload.Parameters.AddWithValue("@payment_method", item.PaymentMethod);
        payload.Parameters.AddWithValue("@payment_status", item.PaymentStatus);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task<int> RemoveItemAsync(Payment item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "DELETE FROM payments WHERE payment_id = @payment_id;",
            conn
            );

        payload.Parameters.AddWithValue("@payment_id", item.PaymentId);
        return await payload.ExecuteNonQueryAsync();
    }

    /**
    public async Task RemoveAllItemsAsync(List<Payment> items)
    {
        foreach (var p in items.Where(p => p.IsSelected))
        {
            await RemoveItemAsync(p);
        }
    }
    */
    public async Task RemoveAllItemsAsync(List<Payment> items)
    { }

    public async Task<int> UpdateItemAsync(Payment item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);

        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "UPDATE payments SET order_id=@order_id, payment_date=@payment_date, amount=@amount, payment_method=@payment_method, payment_status=@payment_status WHERE payment_id = @payment_id;",
            conn
            );

        payload.Parameters.AddWithValue("@order_id", item.OrderId);
        payload.Parameters.AddWithValue("@payment_date", item.PaymentDate);
        payload.Parameters.AddWithValue("@amount", item.Amount);
        payload.Parameters.AddWithValue("@payment_method", item.PaymentMethod);
        payload.Parameters.AddWithValue("@payment_status", item.PaymentStatus);
        payload.Parameters.AddWithValue("@payment_id", item.PaymentId);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task UpdateAllItemsAsync(List<Payment> items)
    {
        foreach (var i in items)
        {
            await UpdateItemAsync(i);

        }
    }

    public PaymentRepository()
    {

    }

}

