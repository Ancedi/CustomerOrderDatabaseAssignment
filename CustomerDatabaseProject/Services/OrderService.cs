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
    public class OrderService
    {
        public static async Task ViewOrderAsync()
        {
            using var db = new ApplicationContext();
            var orders = await db.OrderViews.OrderByDescending(x => x.OrderId).ToListAsync();
            Console.WriteLine("Order Summary View");
            if (!orders.Any())
            {
                Console.WriteLine("Empty");
            }
            Console.WriteLine("Order ID | Order Date | Status | Total Amount | Customer Name | Customer Email");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderId} - {order.OrderDate} - {order.TotalAmount} - {order.CustomerName} - {order.CustomerEmail}");
            }
        }
        public static async Task ListSimpleOrdersAsync()
        {
            using var db = new ApplicationContext();

            var orders = await db.Orders.AsNoTracking().Include(x => x.Customer).OrderBy(x => x.OrderId).ToListAsync();
            Console.WriteLine("Order List");
            if (!orders.Any())
            {
                Console.WriteLine("Empty");
                return;
            }
            Console.WriteLine("ID | Status | Order Date | Customer (ID)");
            foreach (var order in orders)
            {
                Console.WriteLine($"{order.OrderId} | {order.Status} | {order.OrderDate} | {order.Customer?.Name} ({order.CustomerId})");
            }
        }
        public static async Task ListDetailedOrderAsync()
        {
            using var db = new ApplicationContext();

            var orderDetails = await db.Orders
                .AsNoTracking()
                .Include(x => x.OrderRows)
                    .ThenInclude(x => x.Product)
                .Include(x => x.Customer)
                .OrderBy(x => x.OrderId)
                .ToListAsync();
            Console.WriteLine("Order List");
            if (!orderDetails.Any())
            {
                Console.WriteLine("Empty");
                return;
            }
            foreach (var orderDetail in orderDetails)
            {
                Console.WriteLine($"Order ID: {orderDetail.OrderId}\nStatus: {orderDetail.Status}");
                Console.WriteLine($"Product Name (Product ID) | Quantity | Unit Price");
                foreach(var orderRow in orderDetail.OrderRows)
                {
                    Console.WriteLine($"{orderRow.Product?.ProductName} ({orderRow.ProductId}) | {orderRow.Quantity} | {orderRow.UnitPrice}");
                }
            }
        }

        public static async Task ListOrderByCustomerAsync(int customerId)
        {
            using var db = new ApplicationContext();


            var customerOrders = await db.Orders.AsNoTracking()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
            Console.WriteLine("Order List");
            if (!customerOrders.Any())
            {
                Console.WriteLine("Empty");
                return;
            }
            Console.WriteLine("Order ID | Order Date | Total Amount | Status");
            foreach (var customerOrder in customerOrders)
            {
                Console.WriteLine($"{customerOrder.OrderId}" +
                    $" | {customerOrder.OrderDate}" +
                    $" | {customerOrder.TotalAmount}" +
                    $" | {customerOrder.Status}");
            }
        }

        public static async Task ListOrderPagesAsync(int page, int pageSize)
        {
            using var db = new ApplicationContext();

            var sw = System.Diagnostics.Stopwatch.StartNew();

            var query = db.Orders.Include(c => c.Customer).AsNoTracking().OrderByDescending(o => o.OrderDate);
            sw.Stop();
            Console.WriteLine($"Total Time Elapsed (Query): {sw.ElapsedMilliseconds}");
            var totalOrderCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrderCount / (double)pageSize);

            var orders = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            Console.WriteLine("Order ID | Order Date | Total Amount (SEK) | Email");
            foreach(var order in orders)
            {
                Console.WriteLine($"{order.OrderId} | {order.OrderDate} | {order.TotalAmount} kr | {order.Customer?.Email}");
            }
            Console.WriteLine($"Page: {page}/{totalPages}, Page Size {pageSize}");
        }

        public static async Task AddOrderAsync()
        {
            using var db = new ApplicationContext();

            await using var transaction = db.Database.BeginTransaction();
            try
            {
                Console.WriteLine("Select Customer:");
                var customers = await db.Customers.AsNoTracking()
                    .OrderBy(x => x.CustomerId)
                    .ToListAsync();
                Console.WriteLine("Customer List");
                if (!customers.Any())
                {
                    Console.WriteLine("Empty");
                    return;
                }
                Console.Write("Customer ID: ");
                if (!int.TryParse(Console.ReadLine(), out var customerId))
                {
                    Console.WriteLine("Invalid ID. Try Again!");
                    return;
                }
                var customer = await db.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    Console.WriteLine("Customer Not Found.");
                    return;
                }

                var newOrder = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerId = customerId,
                    Status = "Pending",
                    TotalAmount = 0
                };

                var orderRows = new List<OrderRow>();

                while (true)
                {
                    Console.WriteLine("Available Products:");
                    await ProductService.ListProductsAsync();

                    Console.Write("Product ID: ");
                    if (!int.TryParse(Console.ReadLine(), out var productId))
                    {
                        Console.WriteLine("Invalid ID. Try Again!");
                        return;
                    }

                    var product = await db.Products.FindAsync(productId);
                    if (product == null)
                    {
                        Console.WriteLine("Product Not Found.");
                        return;
                    }

                    Console.Write("Quantity: ");
                    if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Invalid Input. Try Again With Numbers Only Or Higher Quantity.");
                        return;
                    }
                    var row = new OrderRow
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.UnitPrice
                    };
                    orderRows.Add(row);
                    Console.WriteLine($"Product {product.ProductName} Added");

                    Console.WriteLine("Add Another Product? Y/N");
                    var answer = Console.ReadLine()?.ToUpper();
                    if (answer == "Y" || answer == "YES" || string.IsNullOrEmpty(answer))
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

                newOrder.OrderRows = orderRows;
                newOrder.TotalAmount = orderRows.Sum(o => o.UnitPrice * o.Quantity);

                Console.Write("Confirm Purchase: ");
                var password = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Please Confirm Purchase With Password.");
                    return;
                }
                if (password == customer.Password)
                {
                    db.Orders.Add(newOrder);

                    try
                    {
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        Console.WriteLine("Order Added.");
                    }
                    catch (DbUpdateException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
            catch (DbUpdateException exception)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(exception.Message);
            }
        }
        public static async Task EditOrderAsync(int orderId)
        {
            using var db = new ApplicationContext();

            await ListSimpleOrdersAsync();

            Console.Write("Enter Order ID: ");
            var order = await db.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            Console.WriteLine($"Status: {order.Status}");
            Console.Write("New Status: ");
            var status = Console.ReadLine();
            if (!string.IsNullOrEmpty(status))
            {
                order.Status = status;
                order.OrderDate = DateTime.Today;
            }

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Order Edited.");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public static async Task DeleteOrderAsync(int orderId)
        {
            using var db = new ApplicationContext();

            await ListSimpleOrdersAsync();

            Console.Write("Enter Order ID: ");
            var order = await db.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            db.Orders.Remove(order);

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Order Deleted");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

    }
}
