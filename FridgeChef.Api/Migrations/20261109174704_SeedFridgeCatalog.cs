using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FridgeChef.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedFridgeCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 2, "Молочні продукти" },
                    { 3, "М'ясо та ковбаси" },
                    { 4, "Овочі" },
                    { 5, "Фрукти" },
                    { 6, "Яйця" },
                    { 7, "Бакалія" },
                    { 8, "Напої" }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Name", "ShortName" },
                values: new object[,]
                {
                    { 1, "Штуки", "шт" },
                    { 2, "Мілілітри", "мл" },
                    { 3, "Літри", "л" },
                    { 4, "Грами", "г" },
                    { 5, "Кілограми", "кг" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Name" },
                values: new object[,]
                {
                    { 1, 2, "Молоко" },
                    { 2, 2, "Сир" },
                    { 3, 2, "Масло" },
                    { 4, 2, "Сметана" },
                    { 5, 2, "Йогурт" },
                    { 6, 2, "Кефір" },
                    { 7, 3, "Ковбаса" },
                    { 8, 3, "Куряче філе" },
                    { 9, 3, "Яловичина" },
                    { 10, 3, "Фарш" },
                    { 11, 4, "Помідори" },
                    { 12, 4, "Огірки" },
                    { 13, 4, "Картопля" },
                    { 14, 4, "Морква" },
                    { 15, 4, "Цибуля" },
                    { 16, 4, "Перець" },
                    { 17, 5, "Яблука" },
                    { 18, 5, "Банани" },
                    { 19, 5, "Апельсини" },
                    { 20, 5, "Лимони" },
                    { 21, 6, "Яйця" },
                    { 22, 7, "Рис" },
                    { 23, 7, "Гречка" },
                    { 24, 7, "Макарони" },
                    { 25, 7, "Борошно" },
                    { 26, 7, "Цукор" },
                    { 27, 8, "Вода" },
                    { 28, 8, "Сік" }
                });
            migrationBuilder.Sql("""
    SELECT setval(
        pg_get_serial_sequence('"Categories"', 'Id'),
        COALESCE(MAX("Id"), 1),
        MAX("Id") IS NOT NULL
    ) FROM "Categories";

    SELECT setval(
        pg_get_serial_sequence('"Products"', 'Id'),
        COALESCE(MAX("Id"), 1),
        MAX("Id") IS NOT NULL
    ) FROM "Products";

    SELECT setval(
        pg_get_serial_sequence('"Units"', 'Id'),
        COALESCE(MAX("Id"), 1),
        MAX("Id") IS NOT NULL
    ) FROM "Units";
    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
