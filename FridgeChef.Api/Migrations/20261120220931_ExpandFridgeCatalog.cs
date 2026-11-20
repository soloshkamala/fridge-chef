using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeChef.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExpandFridgeCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductUnits",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUnits", x => new { x.ProductId, x.UnitId });
                    table.ForeignKey(
                        name: "FK_ProductUnits_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductUnits_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductUnits_UnitId",
                table: "ProductUnits",
                column: "UnitId");


            migrationBuilder.Sql("""
    INSERT INTO "Categories" ("Name")
    SELECT v."Name"
    FROM (VALUES
        ('Риба та морепродукти'),
        ('Олії та соуси'),
        ('Спеції та приправи')
    ) AS v("Name")
    WHERE NOT EXISTS (
        SELECT 1 FROM "Categories" c
        WHERE LOWER(TRIM(c."Name")) = LOWER(TRIM(v."Name"))
    );

    INSERT INTO "Products" ("Name", "CategoryId")
    SELECT v."Name", c."Id"
    FROM (VALUES
        ('Вершки', 'Молочні продукти'),
        ('Ряжанка', 'Молочні продукти'),
        ('Кисломолочний сир', 'Молочні продукти'),
        ('Моцарела', 'Молочні продукти'),
        ('Фета', 'Молочні продукти'),
        ('Пармезан', 'Молочні продукти'),
        ('Твердий сир', 'Молочні продукти'),
        ('Плавлений сир', 'Молочні продукти'),
        ('Згущене молоко', 'Молочні продукти'),

        ('Свинина', 'М''ясо та ковбаси'),
        ('Індичка', 'М''ясо та ковбаси'),
        ('Курячі стегна', 'М''ясо та ковбаси'),
        ('Курячі крильця', 'М''ясо та ковбаси'),
        ('Бекон', 'М''ясо та ковбаси'),
        ('Шинка', 'М''ясо та ковбаси'),
        ('Сосиски', 'М''ясо та ковбаси'),
        ('Сардельки', 'М''ясо та ковбаси'),
        ('Печінка', 'М''ясо та ковбаси'),

        ('Лосось', 'Риба та морепродукти'),
        ('Тунець', 'Риба та морепродукти'),
        ('Хек', 'Риба та морепродукти'),
        ('Скумбрія', 'Риба та морепродукти'),
        ('Оселедець', 'Риба та морепродукти'),
        ('Креветки', 'Риба та морепродукти'),
        ('Кальмари', 'Риба та морепродукти'),
        ('Мідії', 'Риба та морепродукти'),

        ('Капуста', 'Овочі'),
        ('Броколі', 'Овочі'),
        ('Цвітна капуста', 'Овочі'),
        ('Кабачки', 'Овочі'),
        ('Баклажани', 'Овочі'),
        ('Буряк', 'Овочі'),
        ('Часник', 'Овочі'),
        ('Шпинат', 'Овочі'),
        ('Салат', 'Овочі'),
        ('Печериці', 'Овочі'),
        ('Гарбуз', 'Овочі'),

        ('Груші', 'Фрукти'),
        ('Персики', 'Фрукти'),
        ('Ківі', 'Фрукти'),
        ('Виноград', 'Фрукти'),
        ('Полуниця', 'Фрукти'),
        ('Малина', 'Фрукти'),
        ('Чорниця', 'Фрукти'),
        ('Вишня', 'Фрукти'),
        ('Ананас', 'Фрукти'),
        ('Манго', 'Фрукти'),
        ('Авокадо', 'Фрукти'),

        ('Вівсянка', 'Бакалія'),
        ('Булгур', 'Бакалія'),
        ('Кускус', 'Бакалія'),
        ('Манка', 'Бакалія'),
        ('Пшоно', 'Бакалія'),
        ('Квасоля', 'Бакалія'),
        ('Сочевиця', 'Бакалія'),
        ('Нут', 'Бакалія'),
        ('Крохмаль', 'Бакалія'),
        ('Панірувальні сухарі', 'Бакалія'),

        ('Соняшникова олія', 'Олії та соуси'),
        ('Оливкова олія', 'Олії та соуси'),
        ('Майонез', 'Олії та соуси'),
        ('Кетчуп', 'Олії та соуси'),
        ('Гірчиця', 'Олії та соуси'),
        ('Соєвий соус', 'Олії та соуси'),
        ('Томатна паста', 'Олії та соуси'),

        ('Сіль', 'Спеції та приправи'),
        ('Чорний перець', 'Спеції та приправи'),
        ('Паприка', 'Спеції та приправи'),
        ('Кориця', 'Спеції та приправи'),
        ('Куркума', 'Спеції та приправи'),
        ('Базилік', 'Спеції та приправи'),
        ('Орегано', 'Спеції та приправи'),
        ('Лавровий лист', 'Спеції та приправи')
    ) AS v("Name", "CategoryName")
    INNER JOIN "Categories" c
        ON LOWER(TRIM(c."Name")) = LOWER(TRIM(v."CategoryName"))
    WHERE NOT EXISTS (
        SELECT 1 FROM "Products" p
        WHERE LOWER(TRIM(p."Name")) = LOWER(TRIM(v."Name"))
    );

    WITH Allowed AS (
        SELECT p."Id" AS "ProductId",
               CASE
                   WHEN p."Name" IN (
                       'Молоко', 'Кефір', 'Вода', 'Сік',
                       'Вершки', 'Ряжанка', 'Згущене молоко',
                       'Соняшникова олія', 'Оливкова олія',
                       'Соєвий соус'
                   ) THEN ARRAY['мл', 'л']

                   WHEN p."Name" = 'Яйця'
                   THEN ARRAY['шт']

                   WHEN p."Name" IN (
                       'Помідори', 'Огірки', 'Картопля',
                       'Морква', 'Цибуля', 'Перець',
                       'Яблука', 'Банани', 'Апельсини',
                       'Лимони', 'Груші', 'Персики',
                       'Ківі', 'Ананас', 'Манго',
                       'Авокадо', 'Кабачки', 'Баклажани',
                       'Буряк', 'Часник', 'Гарбуз',
                       'Капуста', 'Цвітна капуста'
                   ) THEN ARRAY['шт', 'г', 'кг']

                   WHEN p."Name" IN (
                       'Йогурт', 'Сметана',
                       'Майонез', 'Кетчуп', 'Гірчиця',
                       'Томатна паста'
                   ) THEN ARRAY['г', 'кг', 'мл']

                   ELSE ARRAY['г', 'кг']
               END AS "AllowedUnits"
        FROM "Products" p
    )
    INSERT INTO "ProductUnits" ("ProductId", "UnitId")
    SELECT a."ProductId", u."Id"
    FROM Allowed a
    CROSS JOIN LATERAL unnest(a."AllowedUnits") AS x("ShortName")
    INNER JOIN "Units" u ON u."ShortName" = x."ShortName"
    ON CONFLICT ("ProductId", "UnitId") DO NOTHING;

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
    """);

        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductUnits");
        }
    }
}
