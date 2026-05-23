using System.Collections.Generic;
using mvvm_edp.Models;
using MySqlConnector;
using System.Threading.Tasks;
using System.Linq;


namespace mvvm_edp.Repositories;

public class CustomerRepository : IRepository<Customer>
{
    public async Task<List<Customer>> GetAllItemsAsync()
    {

        var products = new List<Customer>();
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "SELECT * FROM customers;",
            conn
            );

        await using var reader = await payload.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Customer(
                reader.GetInt32("customer_id"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.GetString("phone"),
                reader.GetString("address")
                ));
        }
        return products;
    }

    public async Task<int> AddItemAsync(Customer customer)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "INSERT INTO customers(first_name, last_name, phone, address)" +
            "VALUES(@first_name, @last_name, @phone, @address);",
            conn
            );

        payload.Parameters.AddWithValue("@first_name", customer.FirstName);
        payload.Parameters.AddWithValue("@last_name", customer.LastName);
        payload.Parameters.AddWithValue("@phone", customer.Phone);
        payload.Parameters.AddWithValue("@address", customer.Address);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task<int> RemoveItemAsync(Customer customer)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "DELETE FROM customers WHERE customer_id = @customer_id;",
            conn
            );

        payload.Parameters.AddWithValue("@customer_id", customer.CustomerId);
        return await payload.ExecuteNonQueryAsync();
    }

    public async Task RemoveAllItemsAsync(List<Customer> customer)
    {
        foreach (var c in customer.Where(c => c.IsSelected))
        {
            await RemoveItemAsync(c);
        }
    }
    public async Task<int> UpdateItemAsync(Customer customer)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);

        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "UPDATE customers SET first_name=@first_name, last_name=@last_name, phone=@phone, address=@address WHERE customer_id = @customer_id;",
            conn
            );

        payload.Parameters.AddWithValue("@first_name", customer.FirstName);
        payload.Parameters.AddWithValue("@last_name", customer.LastName);
        payload.Parameters.AddWithValue("@phone", customer.Phone);
        payload.Parameters.AddWithValue("@address", customer.Address);
        payload.Parameters.AddWithValue("@customer_id", customer.CustomerId);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task UpdateAllItemsAsync(List<Customer> customer)
    {
        foreach (var c in customer.Where(c => !c.IsSelected))
        {
            await UpdateItemAsync(c);
        }
    }

    public CustomerRepository()
    {

    }

}
