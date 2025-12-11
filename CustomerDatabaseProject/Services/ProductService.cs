using CustomerDatabaseProject.Data;
using CustomerDatabaseProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDatabaseProject.Services
{
    public class ProductService
    {
        public static async Task ListProductsAsync()
        {
            using var db = new ApplicationContext();

            var products = await db.Products.AsNoTracking().OrderBy(x => x.ProductId).ToListAsync();
            Console.WriteLine("Product List");
            if (!products.Any())
            {
                Console.WriteLine("Empty.");
                return;
            }
            Console.WriteLine("ID | Product | Price");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductId} | {product.ProductName} | {product.UnitPrice}");
            }
        }
        public static async Task AddProductAsync()
        {
            using var db = new ApplicationContext();

            Console.Write("Product Name: ");
            string productName = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(productName))
            {
                Console.WriteLine("Product name is required.");
                return;
            }

            Console.Write("Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out var unitPrice))
            {
                Console.WriteLine("Invalid Input. Please try again, with numbers.");
                return;
            }

            await db.Products.AddAsync(new Product { ProductName = productName, UnitPrice = unitPrice });

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Product Added.");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        public static async Task EditProductAsync(int productId)
        {
            using var db = new ApplicationContext();

            Console.WriteLine("Product List");
            await ListProductsAsync();

            Console.Write($"Product ID: ");
            var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Product Not Found.");
                return;
            }

            Console.WriteLine($"Product Name: {product.ProductName}");
            Console.Write($"New Product Name: ");
            string name = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                product.ProductName = name;
            }

            Console.WriteLine($"Product Price: {product.UnitPrice}");
            Console.Write($"New Product Price: ");
            if (decimal.TryParse(Console.ReadLine(), out var unitPrice))
            {
                product.UnitPrice = unitPrice;
            }

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Product Edited.");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        public static async Task DeleteProductAsync(int productId)
        {
            using var db = new ApplicationContext();

            await ListProductsAsync();

            Console.Write("Enter Product ID: ");
            var product = await db.Products.FirstOrDefaultAsync(x => x.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            db.Products.Remove(product);

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Product Deleted.");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
