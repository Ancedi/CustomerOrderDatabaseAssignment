using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDatabaseProject.Data
{
    public class DbSeeder
    {
        public static async Task DataBaseAsync()
        {
            Console.WriteLine("DB: " + Path.Combine(AppContext.BaseDirectory), "shop.db");
            using var db = new ApplicationContext();

            await db.Database.MigrateAsync();
        }
    }
}
