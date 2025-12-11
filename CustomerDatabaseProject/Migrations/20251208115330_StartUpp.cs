using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyModel.Resolution;

#nullable disable

namespace CustomerDatabaseProject.Migrations
{
    /// <inheritdoc />
    public partial class StartUpp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS OrdersMadeView AS
            SELECT
                c.CustomerId,
                c.Name AS CustomerName,
                c.Email AS CustomerEmail,
                COUNT(o.OrderId) AS OrdersMade
            FROM Customers c
            LEFT JOIN Orders o ON o.CustomerId = c.CustomerId
            GROUP BY c.CustomerId, c.Name, c.Email;
");
            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS ProductsSoldView AS
            SELECT
                p.ProductId,
                p.ProductName,
                IFNULL(SUM(orw.Quantity), 0) AS ProductsSold
            FROM Products p
            LEFT JOIN OrderRows orw ON orw.ProductId = p.ProductId
            GROUP BY p.ProductId, p.ProductName;
");

            migrationBuilder.Sql(@"
            CREATE VIEW IF NOT EXISTS OrderSummaryView AS
            SELECT
                o.OrderId,
                o.OrderDate,
                c.Name AS CustomerName,
                c.Email AS CustomerEmail,
                IFNULL(SUM(orw.Quatity * orw.UnitPrice), 0) AS TotalAmount
            FROM Orders o
            JOIN Customers c ON c.CustomerId = o.CustomerId
            LEFT JOIN OrderRows orw ON orw.OrderId = o.OrderId
            GROUP BY o.OrderId, o.OrderDate, c.Name, c.Email;
");
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS TriggeredOrderRow_INSERT
            AFTER INSERT ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = (
                                    SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                                    FROM OrderRows
                                    WHERE OrderId = NEW.OrderId
                                )
                                WHERE OrderId = NEW.OrderId;
            END;
");
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS TriggerOrderRow_Update
            AFTER UPDATE ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = ( SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                FROM OrderRows WHERE OrderId = NEW.OrderId)
            WHERE OrderId = NEW.OrderId;
            END;
");
            migrationBuilder.Sql(@"
            CREATE TRIGGER IF NOT EXISTS TriggerOrderRow_Delete
            AFTER DELETE ON OrderRows
            BEGIN
                UPDATE Orders
                SET TotalAmount = (
                    SELECT IFNULL(SUM(Quantity * UnitPrice), 0)
                    FROM OrderRows
                    WHERE OrderId = NEW.OrderId
                    )
                WHERE OrderId = NEW.OrderId;
            END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP VIEW IF EXISTS OrdersMadeView
");
            migrationBuilder.Sql(@"
            DROP VIEW IF EXISTS ProductsSoldView
");
            migrationBuilder.Sql(@"
            DROP VIEW IF EXISTS OrderSummaryView
");
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS TriggerOrderRow_INSERT
");
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS TriggerOrderRow_Update
");
            migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS TriggerOrderRow_Delete
");
        }
    }
}
