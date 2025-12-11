using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDatabaseProject.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required, MaxLength(100)]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public decimal UnitPrice { get; set; }
        public List<OrderRow> OrderRows { get; set; } = new();
    }
}
