
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeChef.Api.Migrations
{
    public partial class SeedFridgeCatalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                -- 1. Додаємо категорії, яких ще немає
                INSERT INTO "Categories" ("Name")
                SELECT v."Name"
                FROM (VALUES
                    ('Молочні продукти'),
                    ('М''ясо та ковбаси'),
                    ('Овочі'),
                    ('Фрукти'),
                    ('Яйця'),
                    ('Бакалія'),
                    ('Напої')
                ) AS v("Name")
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "Categories" c
                    WHERE LOWER(TRIM(c."Name")) = LOWER(TRIM(v."Name"))
                );

                -- 2. Додаємо одиниці вимірювання, яких ще немає
                INSERT INTO "Units" ("Name", "ShortName")
                SELECT v."Name", v."ShortName"
                FROM (VALUES
                    ('Штуки', 'шт'),
                    ('Мілілітри', 'мл'),
                    ('Літри', 'л'),
                    ('Грами', 'г'),
                    ('Кілограми', 'кг')
                ) AS v("Name", "ShortName")
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "Units" u
                    WHERE LOWER(TRIM(u."Name")) = LOWER(TRIM(v."Name"))
                       OR LOWER(TRIM(u."ShortName")) = LOWER(TRIM(v."ShortName"))
                );

                -- 3. Додаємо продукти, яких ще немає
                INSERT INTO "Products" ("Name", "CategoryId")
                SELECT v."Name", c."Id"
                FROM (VALUES
                    ('Молоко', 'Молочні продукти'),
                    ('Сир', 'Молочні продукти'),
                    ('Масло', 'Молочні продукти'),
                    ('Сметана', 'Молочні продукти'),
                    ('Йогурт', 'Молочні продукти'),
                    ('Кефір', 'Молочні продукти'),
                    ('Ковбаса', 'М''ясо та ковбаси'),
                    ('Куряче філе', 'М''ясо та ковбаси'),
                    ('Яловичина', 'М''ясо та ковбаси'),
                    ('Фарш', 'М''ясо та ковбаси'),
                    ('Помідори', 'Овочі'),
                    ('Огірки', 'Овочі'),
                    ('Картопля', 'Овочі'),
                    ('Морква', 'Овочі'),
                    ('Цибуля', 'Овочі'),
                    ('Перець', 'Овочі'),
                    ('Яблука', 'Фрукти'),
                    ('Банани', 'Фрукти'),
                    ('Апельсини', 'Фрукти'),
                    ('Лимони', 'Фрукти'),
                    ('Яйця', 'Яйця'),
                    ('Рис', 'Бакалія'),
                    ('Гречка', 'Бакалія'),
                    ('Макарони', 'Бакалія'),
                    ('Борошно', 'Бакалія'),
                    ('Цукор', 'Бакалія'),
                    ('Вода', 'Напої'),
                    ('Сік', 'Напої')
                ) AS v("Name", "CategoryName")
                INNER JOIN "Categories" c
                    ON LOWER(TRIM(c."Name")) = LOWER(TRIM(v."CategoryName"))
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "Products" p
                    WHERE LOWER(TRIM(p."Name")) = LOWER(TRIM(v."Name"))
                );

                -- 4. Синхронізуємо лічильники ID
                SELECT setval(
                    pg_get_serial_sequence('"Categories"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM "Categories"), 1),
                    EXISTS (SELECT 1 FROM "Categories")
                );

                SELECT setval(
                    pg_get_serial_sequence('"Products"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM "Products"), 1),
                    EXISTS (SELECT 1 FROM "Products")
                );

                SELECT setval(
                    pg_get_serial_sequence('"Units"', 'Id'),
                    COALESCE((SELECT MAX("Id") FROM "Units"), 1),
                    EXISTS (SELECT 1 FROM "Units")
                );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Не видаляємо дані, щоб не втратити наявні записи.
        }
    }
}
