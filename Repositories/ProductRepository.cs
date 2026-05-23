using System.Collections.Generic;
using mvvm_edp.Models;
using MySqlConnector;
using System.Threading.Tasks;
using System.Linq;


namespace mvvm_edp.Repositories;

public class ProductRepository : IRepository<Product>
{
    public async Task<List<Product>> GetAllItemsAsync()
    {

        var products = new List<Product>();
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "SELECT * FROM products;",
            conn
            );

        await using var reader = await payload.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product(
                reader.GetInt32("product_id"),
                reader.GetString("name"),
                reader.GetString("description"),
                reader.GetDecimal("price")
                ));
        }
        return products;
    }

    public async Task<int> AddItemAsync(Product item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "INSERT INTO products(name, description, price)" +
            "VALUES(@name, @description, @price);",
            conn
            );

        payload.Parameters.AddWithValue("@name", item.Name);
        payload.Parameters.AddWithValue("@description", item.Description);
        payload.Parameters.AddWithValue("@price", item.Price);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task<int> RemoveItemAsync(Product item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);
        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "DELETE FROM products WHERE product_id = @product_id;",
            conn
            );

        payload.Parameters.AddWithValue("@product_id", item.ProductId);
        return await payload.ExecuteNonQueryAsync();
    }

    public async Task RemoveAllItemsAsync(List<Product> items)
    {
        foreach (var p in items.Where(p => p.IsSelected))
        {
            await RemoveItemAsync(p);
        }
    }
    public async Task<int> UpdateItemAsync(Product item)
    {
        await using var conn = new MySqlConnection(DatabaseConnection.ConnectionString);

        await conn.OpenAsync();

        await using var payload = new MySqlCommand(
            "UPDATE products SET name=@name, description=@description, price=@price WHERE product_id = @product_id;",
            conn
            );

        payload.Parameters.AddWithValue("@name", item.Name);
        payload.Parameters.AddWithValue("@description", item.Description);
        payload.Parameters.AddWithValue("@price", item.Price);
        payload.Parameters.AddWithValue("@product_id", item.ProductId);

        return await payload.ExecuteNonQueryAsync();
    }

    public async Task UpdateAllItemsAsync(List<Product> items)
    {
        foreach (var p in items.Where(p => !p.IsSelected))
        {
            await UpdateItemAsync(p);

        }
    }

    public ProductRepository()
    {

    }

}

