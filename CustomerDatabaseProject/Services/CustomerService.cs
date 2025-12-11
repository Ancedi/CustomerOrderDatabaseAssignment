using CustomerDatabaseProject.Data;
using CustomerDatabaseProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDatabaseProject.Services
{
    public class CustomerService
    {
        
        //SHOW CUSTOMERS IN DATABASE WITH LIST
        public static async Task ListCustomersAsync()
        {
            using var db = new ApplicationContext();

            var customers = await db.Customers.AsNoTracking()
                .OrderBy(x => x.CustomerId)
                .ToListAsync();
            Console.WriteLine("Customer List");
            if (!customers.Any())
            {
                Console.WriteLine("Empty");
                return;
            }
            Console.WriteLine("ID | Name | City | Email");
            foreach (var customer in customers)
            {
                var decryptedEmail = EncryptionAssistance.Decrypt(customer.Email);
                var decryptedPassword = EncryptionAssistance.Decrypt(customer.Password);
                Console.WriteLine($"{customer.CustomerId} | {customer.Name} | {customer.City} | {decryptedEmail} | {decryptedPassword}");
            }
        }

        /*
         * addOrderTransactionAsync
         * using var db = new ApplicationContext();
         * await using var transaction = await db.Database.BeginTransaction();
         * try
         * {
         *  var customers = await db.AsNoTracking().OrderBy().ToListAsync()
         *  if (!customers.Any()
         *  {
         *  foreach
         *  {
         *  console.writeline customer id
         *  if try parse consolereadline customerId
         *  {
         *  var product = await db.product.asnotracking...
         *  if (!product.any
         *  {
         *  var productlook = products.ToDictionary(p => p.ProductId, p => p);
         *  var order = new order
         *  {
         *  customerid,orderdate,status,totalamount
         *  
         *  while true
         *  {
         *  console write add order
         *  var answer = console wrteline string empty
         *  if answer tolower != y break;
         *  foreach var product in products
         *  {
         *  console wrltline product.name product.productid
         *  
         *  console wrtline product id
         *  if !int try parse console.realdine out  war productid
         *  console wrtline quantity
         *  if !int try parse console readline out var quantity
         *  {
         *  
         *  var row = new orderrorw();
         *  {
         *  productid,quantity,unitprice
         *  
         *  order orderrow add row
         *  order.orderrows = orderrows;
         *  order.totalamount = orderrows.sum();
         *  db.orders.add order
         *  await db savechangesasync
         *  throw new exception simulated error
         *  await transaction.commitasync
         * }
         * 
         * catch
         * {
         * await transaction.rolback async
         * 
         */

        //ADD CUSTOMER TO DATABASE
        public static async Task AddCustomerAsync()
        {
            using var db = new ApplicationContext();

            await using var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                //Enter NAME
                Console.Write("Name: ");
                string name = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Name is required.");
                    return;
                }

                //Enter EMAIL
                Console.Write("Email: ");
                string email = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("Email is required.");
                    return;
                }
                

                //Enter CITY
                Console.Write("City: ");
                string city = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(city))
                {
                    Console.WriteLine("City is required.");
                    return;
                }

                //Enter PASSWORD
                Console.Write("Password: ");
                string password = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Password Is Required.");
                    return;
                }

                //Secure EMAIL & PASSWORD through Encryption.
                var encryptedEmail = EncryptionAssistance.Encrypt(email);
                var encryptedPassword = EncryptionAssistance.Encrypt(password);

                await db.Customers.AddAsync(new Customer { Name = name, Email = encryptedEmail, City = city, Password = encryptedPassword });

                try
                {
                    await db.SaveChangesAsync();
                    Console.WriteLine("Customer Added.");
                }
                catch (DbUpdateException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                await transaction.CommitAsync();

            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
                await transaction.RollbackAsync();
            }
        }

        //EDIT CUSTOMER DATA SAVED IN DATABASE
        public static async Task EditCustomerAsync(int id)
        {
            using var db = new ApplicationContext();

            await using var transaction = db.Database.BeginTransaction();
            try
            {
                await ListCustomersAsync();

                Console.Write("Enter Customer ID: ");
                var customer = await db.Customers.FirstOrDefaultAsync(x => x.CustomerId == id);
                if (customer == null)
                {
                    Console.WriteLine("Customer not found.");
                    return;
                }

                //Overwrite saved name with another name.
                Console.WriteLine($"Name: {customer.Name}");
                Console.Write("New Name: ");
                var name = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrEmpty(name))
                {
                    customer.Name = name;
                }

                //Overwrite saved email with another email.
                Console.WriteLine($"Email: {customer.Email}");
                Console.Write("New Email: ");
                var email = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrEmpty(email))
                {
                    customer.Email = email;
                }

                //Overwrite saved city with another city.
                Console.WriteLine($"City: {customer.City}");
                Console.Write("New City: ");
                var city = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrEmpty(city))
                {
                    customer.City = city;
                }

                //Overwrite saved password with another password.
                Console.WriteLine($"Password: {customer.Password}");
                Console.Write("New Password: ");
                var password = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrEmpty(city))
                {
                    customer.Password = password;
                }

                //Encrypted data
                var encryptedEmail = EncryptionAssistance.Encrypt(email);
                var encryptedPassword = EncryptionAssistance.Encrypt(password);

                try
                {
                    await db.SaveChangesAsync();
                    Console.WriteLine("Customer Edited.");
                }
                catch (DbUpdateException exception)
                {
                    Console.WriteLine(exception.Message);
                }
                await transaction.CommitAsync();
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
                await transaction.RollbackAsync();
            }

            
        }

        //DELETE CUSTOMER DATA SAVED IN DATABASE
        public static async Task DeleteCustomerAsync(int customerId)
        {
            using var db = new ApplicationContext();
            await ListCustomersAsync();

            Console.Write("Enter Customer ID: ");
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            db.Customers.Remove(customer);

            try
            {
                await db.SaveChangesAsync();
                Console.WriteLine("Customer Deleted.");
            }
            catch (DbUpdateException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
