using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDatabaseProject.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int OrderRowId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required, MaxLength(100)]
        public string Status { get; set; } = string.Empty;
        [Required]
        public decimal TotalAmount { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderRow> OrderRows { get; set; } = new();
    }
}
