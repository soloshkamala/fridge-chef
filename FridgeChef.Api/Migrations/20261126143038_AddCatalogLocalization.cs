using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FridgeChef.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {









            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Units",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortNameEn",
                table: "Units",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionsEn",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "RecipeCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPantryStaple",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Categories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Products" AS p
                SET "NameEn" = v.en
                FROM (VALUES
                    ('Борошно', 'Flour'),
                    ('Булгур', 'Bulgur'),
                    ('Вівсянка', 'Oats'),
                    ('Гречка', 'Buckwheat'),
                    ('Квасоля', 'Beans'),
                    ('Крохмаль', 'Starch'),
                    ('Кускус', 'Couscous'),
                    ('Макарони', 'Pasta'),
                    ('Манка', 'Semolina'),
                    ('Нут', 'Chickpeas'),
                    ('Панірувальні сухарі', 'Breadcrumbs'),
                    ('Пшоно', 'Millet'),
                    ('Рис', 'Rice'),
                    ('Сочевиця', 'Lentils'),
                    ('Цукор', 'Sugar'),
                    ('Бекон', 'Bacon'),
                    ('Індичка', 'Turkey'),
                    ('Ковбаса', 'Sausage'),
                    ('Куряче філе', 'Chicken breast'),
                    ('Курячі крильця', 'Chicken wings'),
                    ('Курячі стегна', 'Chicken thighs'),
                    ('Печінка', 'Liver'),
                    ('Сардельки', 'Thick sausages'),
                    ('Свинина', 'Pork'),
                    ('Сосиски', 'Frankfurters'),
                    ('Фарш', 'Minced meat'),
                    ('Шинка', 'Ham'),
                    ('Яловичина', 'Beef'),
                    ('Вершки', 'Cream'),
                    ('Згущене молоко', 'Condensed milk'),
                    ('Йогурт', 'Yogurt'),
                    ('Кефір', 'Kefir'),
                    ('Кисломолочний сир', 'Cottage cheese'),
                    ('Масло', 'Butter'),
                    ('Молоко', 'Milk'),
                    ('Моцарела', 'Mozzarella'),
                    ('Пармезан', 'Parmesan'),
                    ('Плавлений сир', 'Processed cheese'),
                    ('Ряжанка', 'Fermented baked milk'),
                    ('Сир', 'Cheese'),
                    ('Сметана', 'Sour cream'),
                    ('Твердий сир', 'Hard cheese'),
                    ('Фета', 'Feta'),
                    ('Вода', 'Water'),
                    ('Сік', 'Juice'),
                    ('Баклажани', 'Eggplants'),
                    ('Броколі', 'Broccoli'),
                    ('Буряк', 'Beetroot'),
                    ('Гарбуз', 'Pumpkin'),
                    ('Кабачки', 'Zucchini'),
                    ('Капуста', 'Cabbage'),
                    ('Картопля', 'Potatoes'),
                    ('Морква', 'Carrots'),
                    ('Огірки', 'Cucumbers'),
                    ('Перець', 'Bell pepper'),
                    ('Печериці', 'Button mushrooms'),
                    ('Помідори', 'Tomatoes'),
                    ('Салат', 'Lettuce'),
                    ('Цвітна капуста', 'Cauliflower'),
                    ('Цибуля', 'Onion'),
                    ('Часник', 'Garlic'),
                    ('Шпинат', 'Spinach'),
                    ('Гірчиця', 'Mustard'),
                    ('Кетчуп', 'Ketchup'),
                    ('Майонез', 'Mayonnaise'),
                    ('Оливкова олія', 'Olive oil'),
                    ('Соєвий соус', 'Soy sauce'),
                    ('Соняшникова олія', 'Sunflower oil'),
                    ('Томатна паста', 'Tomato paste'),
                    ('Кальмари', 'Squid'),
                    ('Креветки', 'Shrimp'),
                    ('Лосось', 'Salmon'),
                    ('Мідії', 'Mussels'),
                    ('Оселедець', 'Herring'),
                    ('Скумбрія', 'Mackerel'),
                    ('Тунець', 'Tuna'),
                    ('Хек', 'Hake'),
                    ('Базилік', 'Basil'),
                    ('Кориця', 'Cinnamon'),
                    ('Куркума', 'Turmeric'),
                    ('Лавровий лист', 'Bay leaf'),
                    ('Орегано', 'Oregano'),
                    ('Паприка', 'Paprika'),
                    ('Сіль', 'Salt'),
                    ('Чорний перець', 'Black pepper'),
                    ('Авокадо', 'Avocado'),
                    ('Ананас', 'Pineapple'),
                    ('Апельсини', 'Oranges'),
                    ('Банани', 'Bananas'),
                    ('Виноград', 'Grapes'),
                    ('Вишня', 'Sour cherries'),
                    ('Груші', 'Pears'),
                    ('Ківі', 'Kiwi'),
                    ('Лимони', 'Lemons'),
                    ('Малина', 'Raspberries'),
                    ('Манго', 'Mango'),
                    ('Персики', 'Peaches'),
                    ('Полуниця', 'Strawberries'),
                    ('Чорниця', 'Blueberries'),
                    ('Яблука', 'Apples'),
                    ('Яйця', 'Eggs')
                ) AS v(uk, en)
                WHERE p."Name" = v.uk;

                UPDATE "Categories" AS c
                SET "NameEn" = v.en
                FROM (VALUES
                    ('Бакалія', 'Pantry goods'),
                    ('М''ясо та ковбаси', 'Meat and sausages'),
                    ('Молочні продукти', 'Dairy products'),
                    ('Напої', 'Drinks'),
                    ('Овочі', 'Vegetables'),
                    ('Олії та соуси', 'Oils and sauces'),
                    ('Риба та морепродукти', 'Fish and seafood'),
                    ('Спеції та приправи', 'Spices and seasonings'),
                    ('Фрукти', 'Fruit'),
                    ('Яйця', 'Eggs'),
                    ('Інше', 'Other'),
                    ('Інгредієнти', 'Ingredients')
                ) AS v(uk, en)
                WHERE c."Name" = v.uk;

                UPDATE "Units" AS u
                SET "NameEn" = v.en,
                    "ShortNameEn" = v.short_en
                FROM (VALUES
                    ('шт', 'Pieces', 'pcs'),
                    ('мл', 'Milliliters', 'ml'),
                    ('л', 'Liters', 'l'),
                    ('г', 'Grams', 'g'),
                    ('кг', 'Kilograms', 'kg')
                ) AS v(short_uk, en, short_en)
                WHERE u."ShortName" = v.short_uk;

                UPDATE "RecipeCategories" AS c
                SET "NameEn" = v.en
                FROM (VALUES
                    ('Сніданки', 'Breakfasts'),
                    ('Супи', 'Soups'),
                    ('Салати', 'Salads'),
                    ('Основні страви', 'Main dishes')
                ) AS v(uk, en)
                WHERE c."Name" = v.uk;

                UPDATE "Products"
                SET "IsPantryStaple" = TRUE
                WHERE "Name" IN (
                    'Вода',
                    'Сіль',
                    'Чорний перець',
                    'Цукор',
                    'Соняшникова олія'
                );

                INSERT INTO "Units"
                    ("Name", "ShortName", "NameEn", "ShortNameEn")
                VALUES
                    ('Чайна ложка', 'ч. л.', 'Teaspoon', 'tsp'),
                    ('Столова ложка', 'ст. л.', 'Tablespoon', 'tbsp'),
                    ('Дрібка', 'дрібка', 'Pinch', 'pinch'),
                    ('Зубчик', 'зубч.', 'Clove', 'clove');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "Units"
                WHERE "ShortName" IN (
                    'ч. л.',
                    'ст. л.',
                    'дрібка',
                    'зубч.'
                );
                """);

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ShortNameEn",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "InstructionsEn",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "RecipeCategories");

            migrationBuilder.DropColumn(
                name: "IsPantryStaple",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Categories");


        }
    }
}
