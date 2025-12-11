using CustomerDatabaseProject.Data;
using CustomerDatabaseProject.Services;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

await DbSeeder.DataBaseAsync();
bool Ongoing = true;
while (Ongoing)
{
    Console.WriteLine("Select Category:\n1. Customer | 2. Order | 3. Product | 4. Exit");
    if (!int.TryParse(Console.ReadLine(), out var category))
    {
        Console.WriteLine("Invalid Input. Try Again With Numbers Only!");
        continue;
    }
    if (category == 4)
    {
        Ongoing = false;
    }
    switch (category)
    {
        case 1:
            Console.WriteLine("Customer Commands:\n 1. List | 2. Add | 3. Edit | 4. Delete");
            var commandCustomer = Console.ReadLine() ?? string.Empty;

            switch (commandCustomer)
            {
                case "1":
                    await CustomerService.ListCustomersAsync();
                    break;
                case "2":
                    await CustomerService.AddCustomerAsync();
                    break;
                case "3":
                    if (int.TryParse(Console.ReadLine(), out var IdE))
                    {
                        await CustomerService.EditCustomerAsync(IdE);
                    }
                    break;
                case "4":
                    if (int.TryParse(Console.ReadLine(), out var IdD))
                    {
                        await CustomerService.DeleteCustomerAsync(IdD);
                    }
                    break;
                default:
                    Console.WriteLine("Unknown Command. Try Again!");
                    break;
            }
            break;
        case 2:
            Console.WriteLine("Order Commands:\n 1. List | 2. Add | 3. Edit | 4. Delete");
            var commandOrder = Console.ReadLine();
            switch (commandOrder)
            {
                case "1":
                    Console.WriteLine("Select Format:\n1. Simple | 2. Detailed | 3. Search By Page | 4. Search By Customer");
                    var format = Console.ReadLine();
                    switch (format)
                    {
                        case "1":
                            await OrderService.ListSimpleOrdersAsync();
                            break;
                        case "2":
                            await OrderService.ListDetailedOrderAsync();
                            break;
                        case "3":
                            Console.Write("Select Size: ");
                            if (!int.TryParse(Console.ReadLine(), out var pageSize))
                            {
                                return;
                            }
                            Console.Write("Select Page: ");
                            if (int.TryParse(Console.ReadLine(), out var page))
                            {
                                await OrderService.ListOrderPagesAsync(page, pageSize);
                            }
                            break;
                        case "4":
                            if (int.TryParse(Console.ReadLine(), out var IdC))
                            {
                                await OrderService.ListOrderByCustomerAsync(IdC);
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown Command. Try Again.");
                            break;
                    }
                    break;
                case "2":
                    await OrderService.AddOrderAsync();
                    break;
                case "3":
                    if (int.TryParse(Console.ReadLine(), out var IdE))
                    {
                        await OrderService.EditOrderAsync(IdE);
                    }
                    break;
                case "4":
                    if (int.TryParse(Console.ReadLine(), out var IdD))
                    {
                        await OrderService.DeleteOrderAsync(IdD);
                    }
                    break;
                default:
                    Console.WriteLine("Unknown Command. Try Again!");
                    break;
            }
            break;
        case 3:
            Console.WriteLine("Product Commands:\n 1. List | 2. Add | 3. Edit | 4. Delete");
            var commandProduct = Console.ReadLine();
            switch (commandProduct)
            {
                case "1":
                    await ProductService.ListProductsAsync();
                    break;
                case "2":
                    await ProductService.AddProductAsync();
                    break;
                case "3":
                    if (int.TryParse(Console.ReadLine(), out var IdE))
                    {
                        await ProductService.EditProductAsync(IdE);
                    }
                    break;
                case "4":
                    if (int.TryParse(Console.ReadLine(), out var IdD))
                    {
                        await ProductService.DeleteProductAsync(IdD);
                    }
                    break;
                default:
                    Console.WriteLine("Unknown Command. Try Again!");
                    break;
            }
            break;
        default:
            Console.WriteLine("Unknown Command. Try Again!");
            return;
    }
}