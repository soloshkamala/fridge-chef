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

            migrationBuilder.InsertData(
                table: "RecipeIngredients",
                columns: new[] { "Id", "ProductId", "Quantity", "RecipeId", "UnitId" },
                values: new object[,]
                {
                    { 1, 24, 200m, 1, 4 },
                    { 2, 8, 250m, 1, 4 },
                    { 3, 2, 50m, 1, 4 },
                    { 4, 11, 400m, 2, 4 },
                    { 5, 14, 100m, 2, 4 },
                    { 6, 15, 50m, 2, 4 },
                    { 7, 27, 500m, 2, 2 },
                    { 8, 21, 3m, 3, 1 },
                    { 9, 2, 50m, 3, 4 },
                    { 10, 1, 50m, 3, 2 },
                    { 11, 11, 200m, 4, 4 },
                    { 12, 12, 200m, 4, 4 },
                    { 13, 23, 200m, 5, 4 },
                    { 14, 8, 250m, 5, 4 },
                    { 15, 15, 50m, 5, 4 }
                });

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
