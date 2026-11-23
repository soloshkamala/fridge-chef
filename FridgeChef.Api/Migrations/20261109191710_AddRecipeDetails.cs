
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FridgeChef.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Recipes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Servings",
                table: "Recipes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "RecipeCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Сніданки" },
                    { 2, "Супи" },
                    { 3, "Салати" },
                    { 4, "Основні страви" }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Calories", "CategoryId", "CookingTime", "Description", "ImageUrl", "Instructions", "Name", "Servings" },
                values: new object[,]
                {
                    { 1, 450, 4, 35, "Паста з курячим філе у ніжному сирному соусі.", "", "1. Закип'ятіть воду, посоліть та відваріть макарони відповідно до інструкції на упаковці.\n2. Наріжте куряче філе невеликими шматочками.\n3. Розігрійте пательню з невеликою кількістю олії та обсмажте курку до повної готовності.\n4. Додайте молоко та натертий сир. Перемішуйте на невеликому вогні, поки сир не розплавиться.\n5. Додайте готові макарони до соусу та добре перемішайте.\n6. Прогрійте все разом ще хвилину та подавайте.", "Паста з куркою", 2 },
                    { 2, 180, 2, 40, "Ароматний овочевий суп із помідорами, морквою та цибулею.", "", "1. Помийте помідори, моркву та цибулю.\n2. Наріжте цибулю невеликими шматочками, а моркву натріть на тертці.\n3. Обсмажте цибулю та моркву в каструлі з невеликою кількістю олії протягом кількох хвилин.\n4. Додайте нарізані помідори та тушкуйте овочі приблизно 5–7 хвилин.\n5. Влийте воду, доведіть до кипіння та варіть на невеликому вогні приблизно 15 хвилин.\n6. За бажанням подрібніть суп блендером. Додайте сіль і спеції за смаком.", "Томатний суп", 2 },
                    { 3, 320, 1, 15, "Ніжний омлет із яйцями, молоком та розплавленим сиром.", "", "1. Розбийте яйця в глибоку миску.\n2. Додайте молоко, сіль і перець. Збийте виделкою до однорідності.\n3. Натріть сир на тертці.\n4. Розігрійте пательню з невеликою кількістю масла або олії.\n5. Вилийте яєчну суміш на пательню та готуйте на невеликому вогні.\n6. Коли омлет почне застигати, додайте сир.\n7. Накрийте кришкою та готуйте до повної готовності яєць.", "Омлет із сиром", 1 },
                    { 4, 120, 3, 10, "Легкий свіжий салат із сезонних овочів.", "", "1. Ретельно помийте помідори та огірки.\n2. Наріжте помідори невеликими часточками.\n3. Наріжте огірки кружальцями або півкружальцями.\n4. Перекладіть овочі в салатницю та обережно перемішайте.\n5. Додайте сіль, перець та заправку за смаком.\n6. Перемішайте салат ще раз і подавайте.", "Салат із помідорами та огірками", 2 },
                    { 5, 390, 4, 40, "Поживна гречка з обсмаженим курячим філе та цибулею.", "", "1. Промийте гречку під проточною водою.\n2. Перекладіть її в каструлю, залийте водою та варіть до готовності відповідно до інструкції на упаковці.\n3. Наріжте куряче філе невеликими шматочками.\n4. Очистьте та наріжте цибулю.\n5. Розігрійте пательню з невеликою кількістю олії та обсмажте цибулю.\n6. Додайте куряче філе, посоліть та готуйте до повної готовності.\n7. Подавайте курку з готовою гречкою.", "Гречка з куркою", 2 }
                });

            // Додаємо інгредієнти, знаходячи продукти та одиниці
            // вимірювання за назвами, а не за фіксованими ID.
            migrationBuilder.Sql("""
                DO $$
                DECLARE
                    inserted_count integer;
                BEGIN
                    WITH ingredient_data
                        ("Id", "ProductName", "Quantity", "UnitShortName", "RecipeId")
                    AS (
                        VALUES
                            (1, 'Макарони', 200, 'г', 1),
                            (2, 'Куряче філе', 250, 'г', 1),
                            (3, 'Сир', 50, 'г', 1),

                            (4, 'Помідори', 400, 'г', 2),
                            (5, 'Морква', 100, 'г', 2),
                            (6, 'Цибуля', 50, 'г', 2),
                            (7, 'Вода', 500, 'мл', 2),

                            (8, 'Яйця', 3, 'шт', 3),
                            (9, 'Сир', 50, 'г', 3),
                            (10, 'Молоко', 50, 'мл', 3),

                            (11, 'Помідори', 200, 'г', 4),
                            (12, 'Огірки', 200, 'г', 4),

                            (13, 'Гречка', 200, 'г', 5),
                            (14, 'Куряче філе', 250, 'г', 5),
                            (15, 'Цибуля', 50, 'г', 5)
                    )
                    INSERT INTO "RecipeIngredients"
                        ("Id", "ProductId", "Quantity", "RecipeId", "UnitId")
                    SELECT
                        i."Id",
                        p."Id",
                        i."Quantity",
                        i."RecipeId",
                        u."Id"
                    FROM ingredient_data i
                    INNER JOIN "Products" p
                        ON LOWER(TRIM(p."Name")) = LOWER(TRIM(i."ProductName"))
                    INNER JOIN "Units" u
                        ON LOWER(TRIM(u."ShortName")) = LOWER(TRIM(i."UnitShortName"))
                    INNER JOIN "Recipes" r
                        ON r."Id" = i."RecipeId";

                    GET DIAGNOSTICS inserted_count = ROW_COUNT;

                    IF inserted_count <> 15 THEN
                        RAISE EXCEPTION
                            'Expected 15 recipe ingredients, inserted %',
                            inserted_count;
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql("""
                SELECT setval(
                    pg_get_serial_sequence('"RecipeCategories"', 'Id'),
                    COALESCE(MAX("Id"), 1),
                    MAX("Id") IS NOT NULL
                ) FROM "RecipeCategories";

                SELECT setval(
                    pg_get_serial_sequence('"Recipes"', 'Id'),
                    COALESCE(MAX("Id"), 1),
                    MAX("Id") IS NOT NULL
                ) FROM "Recipes";

                SELECT setval(
                    pg_get_serial_sequence('"RecipeIngredients"', 'Id'),
                    COALESCE(MAX("Id"), 1),
                    MAX("Id") IS NOT NULL
                ) FROM "RecipeIngredients";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RecipeIngredients",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RecipeCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RecipeCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RecipeCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RecipeCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Servings",
                table: "Recipes");
        }
    }
}
