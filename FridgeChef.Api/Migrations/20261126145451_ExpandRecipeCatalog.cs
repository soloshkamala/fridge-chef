using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeChef.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExpandRecipeCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
    INSERT INTO "Products"
        ("Name", "NameEn", "CategoryId", "IsPantryStaple")
    SELECT v.uk, v.en, c."Id", FALSE
    FROM (VALUES
        ('Хліб', 'Bread', 'Бакалія'),
        ('Багет', 'Baguette', 'Бакалія'),
        ('Какао', 'Cocoa powder', 'Бакалія'),
        ('Шоколад', 'Chocolate', 'Бакалія'),
        ('Мед', 'Honey', 'Бакалія'),
        ('Печиво', 'Cookies', 'Бакалія'),
        ('Сухі дріжджі', 'Dry yeast', 'Бакалія'),
        ('Розпушувач', 'Baking powder', 'Бакалія'),
        ('Харчова сода', 'Baking soda', 'Бакалія'),
        ('Ванільний цукор', 'Vanilla sugar', 'Бакалія'),
        ('Цукрова пудра', 'Powdered sugar', 'Бакалія'),
        ('Консервований горошок', 'Canned peas', 'Бакалія'),
        ('Вершковий сир', 'Cream cheese', 'Молочні продукти'),
        ('Морозиво', 'Ice cream', 'Молочні продукти'),
        ('Зелена цибуля', 'Green onion', 'Овочі'),
        ('Кріп', 'Dill', 'Овочі'),
        ('Петрушка', 'Parsley', 'Овочі'),
        ('Солоні огірки', 'Pickles', 'Овочі'),
        ('Оливки', 'Olives', 'Овочі'),
        ('М''ята', 'Mint', 'Овочі'),
        ('Оцет', 'Vinegar', 'Олії та соуси'),
        ('Тахіні', 'Tahini', 'Олії та соуси'),
        ('Кумин (зіра)', 'Cumin', 'Спеції та приправи'),
        ('Мускатний горіх', 'Nutmeg', 'Спеції та приправи'),
        ('Кунжут', 'Sesame seeds', 'Спеції та приправи')
    ) AS v(uk, en, category)
    JOIN "Categories" c ON c."Name" = v.category;

    INSERT INTO "ProductUnits" ("ProductId", "UnitId")
    SELECT p."Id", u."Id"
    FROM (VALUES
        ('Хліб', ARRAY['шт', 'г', 'кг']),
        ('Багет', ARRAY['шт', 'г', 'кг']),
        ('Какао', ARRAY['г', 'кг']),
        ('Шоколад', ARRAY['г', 'кг']),
        ('Мед', ARRAY['г', 'кг']),
        ('Печиво', ARRAY['г', 'кг']),
        ('Сухі дріжджі', ARRAY['г', 'кг']),
        ('Розпушувач', ARRAY['г', 'кг']),
        ('Харчова сода', ARRAY['г', 'кг']),
        ('Ванільний цукор', ARRAY['г', 'кг']),
        ('Цукрова пудра', ARRAY['г', 'кг']),
        ('Консервований горошок', ARRAY['г', 'кг']),
        ('Вершковий сир', ARRAY['г', 'кг']),
        ('Морозиво', ARRAY['г', 'кг']),
        ('Зелена цибуля', ARRAY['г', 'кг']),
        ('Кріп', ARRAY['г', 'кг']),
        ('Петрушка', ARRAY['г', 'кг']),
        ('Солоні огірки', ARRAY['шт', 'г', 'кг']),
        ('Оливки', ARRAY['г', 'кг']),
        ('М''ята', ARRAY['г', 'кг']),
        ('Оцет', ARRAY['мл', 'л']),
        ('Тахіні', ARRAY['г', 'кг']),
        ('Кумин (зіра)', ARRAY['г', 'кг']),
        ('Мускатний горіх', ARRAY['г', 'кг']),
        ('Кунжут', ARRAY['г', 'кг'])
    ) AS v(product, units)
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = ANY(v.units);

    INSERT INTO "RecipeCategories" ("Name", "NameEn")
    VALUES
        ('Випічка', 'Baking'),
        ('Десерти', 'Desserts'),
        ('Закуски', 'Appetizers'),
        ('Напої', 'Drinks');
    """);
            migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.quantity, u."Id"
    FROM (VALUES
        ('Паста з куркою', 'Молоко', 150, 'мл'),
        ('Паста з куркою', 'Соняшникова олія', 1, 'ст. л.'),
        ('Паста з куркою', 'Сіль', 1, 'ч. л.'),
        ('Паста з куркою', 'Чорний перець', 1, 'дрібка'),
        ('Паста з куркою', 'Вода', 2, 'л'),

        ('Томатний суп', 'Соняшникова олія', 1, 'ст. л.'),
        ('Томатний суп', 'Сіль', 0.5, 'ч. л.'),
        ('Томатний суп', 'Чорний перець', 1, 'дрібка'),

        ('Омлет із сиром', 'Масло', 10, 'г'),
        ('Омлет із сиром', 'Сіль', 1, 'дрібка'),
        ('Омлет із сиром', 'Чорний перець', 1, 'дрібка'),

        ('Салат із помідорами та огірками', 'Оливкова олія', 1, 'ст. л.'),
        ('Салат із помідорами та огірками', 'Сіль', 0.25, 'ч. л.'),
        ('Салат із помідорами та огірками', 'Чорний перець', 1, 'дрібка'),

        ('Гречка з куркою', 'Вода', 400, 'мл'),
        ('Гречка з куркою', 'Соняшникова олія', 1, 'ст. л.'),
        ('Гречка з куркою', 'Сіль', 0.5, 'ч. л.'),
        ('Гречка з куркою', 'Чорний перець', 1, 'дрібка')
    ) AS v(recipe, product, quantity, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);
            migrationBuilder.Sql("""
INSERT INTO "Products" ("Name", "NameEn", "CategoryId", "IsPantryStaple")
SELECT 'Волоські горіхи', 'Walnuts', "Id", FALSE FROM "Categories" WHERE "Name" = 'Бакалія';
INSERT INTO "ProductUnits" ("ProductId", "UnitId")
SELECT p."Id", u."Id" FROM "Products" p CROSS JOIN "Units" u WHERE p."Name" = 'Волоські горіхи' AND u."ShortName" IN ('г','кг');
INSERT INTO "Recipes" ("Name", "NameEn", "Description", "DescriptionEn", "CategoryId", "CookingTime", "Calories", "Servings", "Instructions", "InstructionsEn", "ImageUrl")
SELECT v.uk,v.en,v.d,v.de,c."Id",v.t,v.k,v.s,v.i,v.ie,v.image
FROM (VALUES
('Сирники','Cottage cheese pancakes','Ніжні сирники зі сметаною.','Tender cottage cheese pancakes with sour cream.',25,450,2,'1. Розімніть сир виделкою.
2. Додайте яйце та цукор, перемішайте.
3. Вмішайте 30 г борошна, решту залиште для формування.
4. Розділіть масу на 6 частин і сформуйте кружечки завтовшки 2 см.
5. Розігрійте олію на середньому вогні.
6. Смажте по 3–4 хвилини з кожного боку до готовності всередині.
7. Подавайте зі сметаною, ягодами, пудрою та м’ятою.','1. Mash the cottage cheese with a fork.
2. Mix in the egg and sugar.
3. Stir in 30 g flour and reserve the rest for shaping.
4. Divide into six portions and shape into 2 cm thick rounds.
5. Heat oil over medium heat.
6. Fry for 3–4 minutes per side until cooked through.
7. Serve with sour cream, berries, powdered sugar and mint.','/images/recipes/syrnyky.webp'),
('Млинці','Crepes','Тонкі млинці на молоці.','Thin milk crepes.',30,570,2,'1. Збийте яйця з цукром і сіллю.
2. Влийте половину молока та поступово всипте борошно, розмішуючи вінчиком.
3. Додайте решту молока та половину олії.
4. Залиште тісто на 5 хвилин.
5. Розігрійте пательню на середньому вогні та змастіть рештою олії.
6. Наливайте тонкий шар тіста, смажте 1–2 хвилини, перевертайте і смажте ще 30 секунд.
7. Складіть млинці трикутниками та подайте з медом, ягодами й м’ятою.','1. Whisk eggs with sugar and salt.
2. Add half the milk and gradually whisk in flour.
3. Stir in remaining milk and half the oil.
4. Rest the batter for 5 minutes.
5. Heat a pan over medium heat and grease with remaining oil.
6. Pour a thin layer of batter, cook for 1–2 minutes, flip and cook for 30 seconds.
7. Fold into triangles and serve with honey, berries and mint.','/images/recipes/pancakes.webp'),
('Вівсянка з бананом','Banana oatmeal','Вівсяна каша з бананом і корицею.','Oatmeal with banana and cinnamon.',15,520,1,'1. Налийте молоко в невелику каструлю.
2. Доведіть до слабкого кипіння.
3. Всипте вівсянку та перемішайте.
4. Варіть 5–10 хвилин на слабкому вогні, періодично помішуючи.
5. Очистьте банан і наріжте кружальцями.
6. Перекладіть кашу в миску, додайте банан, корицю, мед і подрібнені горіхи.','1. Pour milk into a small saucepan.
2. Bring to a gentle boil.
3. Stir in oats.
4. Simmer for 5–10 minutes, stirring occasionally.
5. Peel and slice the banana.
6. Transfer to a bowl and top with banana, cinnamon, honey and chopped walnuts.','/images/recipes/banana-oatmeal.webp'),
('Яєчня з помідорами','Fried eggs with tomatoes','Яйця зі свіжими помідорами.','Eggs with fresh tomatoes.',15,230,1,'1. Помийте помідори та петрушку.
2. Наріжте помідори часточками.
3. Розігрійте олію на середньому вогні.
4. Обсмажте помідори 3–4 хвилини.
5. Розбийте яйця між помідорами, додайте сіль і перець.
6. Зменште вогонь, накрийте кришкою і готуйте 4–6 хвилин до застигання білків і жовтків.
7. Посипте подрібненою петрушкою.','1. Wash tomatoes and parsley.
2. Cut tomatoes into wedges.
3. Heat oil over medium heat.
4. Fry tomatoes for 3–4 minutes.
5. Crack eggs between the tomatoes and season with salt and pepper.
6. Reduce heat, cover and cook for 4–6 minutes until whites and yolks are set.
7. Sprinkle with chopped parsley.','/images/recipes/fried-eggs-tomatoes.webp'),
('Оладки','Kefir pancakes','Пухкі оладки на кефірі.','Fluffy kefir pancakes.',25,590,2,'1. Змішайте кефір кімнатної температури з яйцем, цукром і сіллю.
2. Окремо змішайте борошно та соду.
3. Поступово додайте суху суміш до кефіру, перемішайте до густого тіста.
4. Залиште на 5 хвилин.
5. Розігрійте олію на середньому вогні.
6. Викладайте тісто столовою ложкою, залишаючи проміжки.
7. Смажте по 2–3 хвилини з кожного боку до готовності середини.
8. Подавайте з йогуртом, ягодами, пудрою та м’ятою.','1. Mix room-temperature kefir with egg, sugar and salt.
2. Combine flour and baking soda separately.
3. Gradually stir dry ingredients into kefir to form a thick batter.
4. Rest for 5 minutes.
5. Heat oil over medium heat.
6. Spoon batter into the pan, leaving space between pancakes.
7. Fry for 2–3 minutes per side until cooked through.
8. Serve with yogurt, berries, powdered sugar and mint.','/images/recipes/kefir-pancakes.webp'),
('Тости з авокадо','Avocado toast','Хрусткі тости з авокадо та лимоном.','Crisp toast with avocado and lemon.',15,410,1,'1. Підсушіть хліб на сухій пательні по 1–2 хвилини з кожного боку.
2. Розріжте авокадо, видаліть кісточку та шкірку.
3. Розімніть половину м’якоті з лимонним соком, сіллю та перцем, решту наріжте скибками.
4. Доведіть воду до слабкого кипіння.
5. Розбийте яйце в чашку, обережно опустіть у воду та варіть 4–5 хвилин або довше до бажаної готовності; для повної готовності жовток має застигнути.
6. Намастіть хліб авокадо, викладіть скибки та яйце.
7. Посипте паприкою та петрушкою.','1. Toast bread in a dry pan for 1–2 minutes per side.
2. Halve avocado and remove pit and skin.
3. Mash half with lemon juice, salt and pepper; slice the rest.
4. Bring water to a gentle simmer.
5. Crack egg into a cup, slide into water and poach for 4–5 minutes or longer to desired doneness; for fully cooked eggs the yolk should be firm.
6. Spread avocado on toast and add slices and egg.
7. Sprinkle with paprika and parsley.','/images/recipes/avocado-toast.webp')
) AS v(uk,en,d,de,t,k,s,i,ie,image)
JOIN "RecipeCategories" c ON c."Name"='Сніданки';
INSERT INTO "RecipeIngredients" ("RecipeId","ProductId","Quantity","UnitId")
SELECT r."Id",p."Id",v.q,u."Id" FROM (VALUES
('Сирники','Кисломолочний сир',300,'г'),('Сирники','Яйця',1,'шт'),('Сирники','Цукор',1,'ст. л.'),('Сирники','Борошно',40,'г'),('Сирники','Соняшникова олія',1,'ст. л.'),('Сирники','Сметана',40,'г'),
('Млинці','Яйця',2,'шт'),('Млинці','Молоко',300,'мл'),('Млинці','Цукор',1,'ст. л.'),('Млинці','Сіль',1,'дрібка'),('Млинці','Борошно',150,'г'),('Млинці','Соняшникова олія',2,'ст. л.'),
('Вівсянка з бананом','Молоко',200,'мл'),('Вівсянка з бананом','Вівсянка',50,'г'),('Вівсянка з бананом','Банани',1,'шт'),('Вівсянка з бананом','Кориця',0.25,'ч. л.'),
('Яєчня з помідорами','Помідори',150,'г'),('Яєчня з помідорами','Яйця',2,'шт'),('Яєчня з помідорами','Соняшникова олія',1,'ч. л.'),('Яєчня з помідорами','Сіль',1,'дрібка'),('Яєчня з помідорами','Чорний перець',1,'дрібка'),
('Оладки','Кефір',250,'мл'),('Оладки','Яйця',1,'шт'),('Оладки','Цукор',1,'ст. л.'),('Оладки','Сіль',1,'дрібка'),('Оладки','Борошно',180,'г'),('Оладки','Харчова сода',0.5,'ч. л.'),('Оладки','Соняшникова олія',2,'ст. л.'),
('Тости з авокадо','Хліб',60,'г'),('Тости з авокадо','Авокадо',100,'г'),('Тости з авокадо','Лимони',0.25,'шт'),('Тости з авокадо','Сіль',1,'дрібка'),('Тости з авокадо','Чорний перець',1,'дрібка')
,
('Сирники','Полуниця',40,'г'),
('Сирники','Малина',20,'г'),
('Сирники','Чорниця',20,'г'),
('Сирники','М''ята',2,'г'),
('Млинці','Полуниця',40,'г'),
('Млинці','Малина',20,'г'),
('Млинці','Чорниця',20,'г'),
('Млинці','М''ята',2,'г'),
('Оладки','Полуниця',40,'г'),
('Оладки','Малина',20,'г'),
('Оладки','Чорниця',20,'г'),
('Оладки','М''ята',2,'г'),
('Сирники','Цукрова пудра',5,'г'),
('Млинці','Мед',1,'ст. л.'),
('Оладки','Цукрова пудра',5,'г'),
('Оладки','Йогурт',50,'г'),
('Вівсянка з бананом','Мед',1,'ч. л.'),
('Вівсянка з бананом','Волоські горіхи',15,'г'),
('Яєчня з помідорами','Петрушка',5,'г'),
('Тости з авокадо','Яйця',1,'шт'),
('Тости з авокадо','Вода',1,'л'),
('Тости з авокадо','Паприка',1,'дрібка'),
('Тости з авокадо','Петрушка',5,'г')
) AS v(recipe,product,q,unit)
JOIN "Recipes" r ON r."Name"=v.recipe JOIN "Products" p ON p."Name"=v.product JOIN "Units" u ON u."ShortName"=v.unit;
""");

            migrationBuilder.Sql("""
    INSERT INTO "Recipes"
    ("Name", "NameEn", "Description", "DescriptionEn",
    "CategoryId", "CookingTime", "Calories", "Servings",
    "Instructions", "InstructionsEn", "ImageUrl")
    SELECT v.uk, v.en, v.d, v.de, c."Id",
           v.t, v.k, v.s, v.i, v.ie, ''
    FROM (VALUES
    (
        'Український борщ',
        'Ukrainian borscht',
        'Традиційний український борщ із яловичиною, буряком, капустою та сметаною.',
        'Traditional Ukrainian borscht with beef, beetroot, cabbage and sour cream.',
        100, 380, 4,
        '1. Промийте яловичину, покладіть у каструлю та залийте водою.
    2. Доведіть до кипіння, зніміть піну та варіть на слабкому вогні приблизно 45 хвилин.
    3. Очистьте картоплю, буряк, моркву, цибулю та часник.
    4. Наріжте картоплю кубиками, капусту тонкими смужками, цибулю дрібними кубиками. Натріть моркву та буряк.
    5. Додайте картоплю до бульйону та варіть 10 хвилин.
    6. Розігрійте олію на пательні. Обсмажте цибулю та моркву протягом 5 хвилин.
    7. Додайте буряк, томатну пасту, оцет і цукор. Тушкуйте 10 хвилин.
    8. Додайте капусту та овочеву засмажку до каструлі.
    9. Додайте сіль, перець і лавровий лист. Варіть ще 15 хвилин до м’якості овочів.
    10. Додайте подрібнений часник, вимкніть нагрівання та залиште борщ під кришкою на 10 хвилин.
    11. Видаліть лавровий лист. Подавайте зі сметаною та подрібненим кропом.',
        '1. Rinse the beef, place it in a pot and cover with water.
    2. Bring to a boil, skim off the foam and simmer for approximately 45 minutes.
    3. Peel the potatoes, beetroot, carrots, onion and garlic.
    4. Dice the potatoes and onion, shred the cabbage and grate the carrots and beetroot.
    5. Add the potatoes to the broth and cook for 10 minutes.
    6. Heat the oil in a frying pan. Fry the onion and carrots for 5 minutes.
    7. Add the beetroot, tomato paste, vinegar and sugar. Cook for 10 minutes.
    8. Add the cabbage and fried vegetables to the pot.
    9. Add salt, pepper and bay leaf. Simmer for another 15 minutes until the vegetables are tender.
    10. Add minced garlic, turn off the heat and leave covered for 10 minutes.
    11. Remove the bay leaf. Serve with sour cream and chopped dill.'
    ),
    (
        'Курячий суп',
        'Chicken soup',
        'Легкий домашній суп із курячим філе, картоплею, морквою та макаронами.',
        'Light homemade soup with chicken breast, potatoes, carrots and pasta.',
        45, 310, 4,
        '1. Промийте картоплю та моркву, очистьте їх разом із цибулею.
    2. Наріжте куряче філе невеликими шматочками.
    3. Покладіть курку в каструлю, залийте водою та доведіть до кипіння.
    4. Зніміть піну та варіть на слабкому вогні 15 хвилин.
    5. Наріжте картоплю кубиками, моркву кружальцями, а цибулю дрібними шматочками.
    6. Розігрійте олію на пательні та обсмажте цибулю з морквою протягом 5 хвилин.
    7. Додайте картоплю та обсмажені овочі до бульйону. Варіть 10 хвилин.
    8. Додайте макарони, сіль, перець і лавровий лист.
    9. Варіть до готовності макаронів і курки. Видаліть лавровий лист.
    10. Додайте подрібнену петрушку та подавайте гарячим.',
        '1. Wash and peel the potatoes and carrots, then peel the onion.
    2. Cut the chicken breast into small pieces.
    3. Place the chicken in a pot, cover with water and bring to a boil.
    4. Skim off the foam and simmer for 15 minutes.
    5. Dice the potatoes, slice the carrots and finely chop the onion.
    6. Heat the oil in a frying pan and fry the onion and carrots for 5 minutes.
    7. Add the potatoes and fried vegetables to the broth. Cook for 10 minutes.
    8. Add the pasta, salt, pepper and bay leaf.
    9. Cook until the pasta and chicken are fully cooked. Remove the bay leaf.
    10. Add chopped parsley and serve hot.'
    ),
    (
        'Грибний крем-суп',
        'Cream of mushroom soup',
        'Ніжний крем-суп із печериць, картоплі та вершків із насиченим грибним ароматом.',
        'Smooth mushroom soup with potatoes, cream and a rich mushroom flavour.',
        40, 320, 3,
        '1. Очистьте печериці та наріжте їх пластинками.
    2. Очистьте картоплю та цибулю. Наріжте картоплю кубиками, а цибулю дрібними шматочками.
    3. Розтопіть вершкове масло в каструлі на середньому вогні.
    4. Додайте цибулю та обсмажуйте 3–4 хвилини до прозорості.
    5. Додайте печериці та готуйте 7–8 хвилин, періодично перемішуючи.
    6. Додайте картоплю та воду. Доведіть до кипіння.
    7. Варіть на слабкому вогні 15–20 хвилин до м’якості картоплі.
    8. Зніміть каструлю з вогню та обережно подрібніть суп занурювальним блендером.
    9. Влийте вершки, додайте сіль і чорний перець.
    10. Прогрійте суп 2–3 хвилини, не доводячи до активного кипіння.
    11. Розлийте по тарілках і прикрасьте подрібненою петрушкою.',
        '1. Clean the mushrooms and cut them into slices.
    2. Peel the potatoes and onion. Dice the potatoes and finely chop the onion.
    3. Melt the butter in a pot over medium heat.
    4. Add the onion and cook for 3–4 minutes until translucent.
    5. Add the mushrooms and cook for 7–8 minutes, stirring occasionally.
    6. Add the potatoes and water. Bring to a boil.
    7. Simmer for 15–20 minutes until the potatoes are tender.
    8. Remove from the heat and carefully blend with an immersion blender.
    9. Pour in the cream and add salt and black pepper.
    10. Heat for 2–3 minutes without bringing to a rolling boil.
    11. Serve in bowls and garnish with chopped parsley.'
    ),
    (
        'Гарбузовий крем-суп',
        'Creamy pumpkin soup',
        'Оксамитовий суп із гарбуза, моркви та вершків із нотками мускатного горіха.',
        'Velvety pumpkin soup with carrots, cream and a hint of nutmeg.',
        40, 270, 3,
        '1. Очистьте гарбуз від шкірки та насіння. Наріжте м’якоть кубиками.
    2. Очистьте моркву, цибулю та часник. Наріжте овочі невеликими шматочками.
    3. Розігрійте соняшникову олію в каструлі на середньому вогні.
    4. Обсмажте цибулю та моркву протягом 5 хвилин.
    5. Додайте подрібнений часник і готуйте ще 30 секунд.
    6. Додайте гарбуз та воду. Доведіть до кипіння.
    7. Зменште вогонь і варіть 20 хвилин до м’якості овочів.
    8. Зніміть каструлю з вогню та обережно подрібніть овочі блендером до однорідності.
    9. Додайте вершки, сіль, чорний перець і мускатний горіх.
    10. Перемішайте та прогрійте на слабкому вогні 2–3 хвилини.
    11. Подавайте суп теплим.',
        '1. Peel and deseed the pumpkin. Cut the flesh into cubes.
    2. Peel the carrots, onion and garlic. Cut the vegetables into small pieces.
    3. Heat sunflower oil in a pot over medium heat.
    4. Fry the onion and carrots for 5 minutes.
    5. Add minced garlic and cook for another 30 seconds.
    6. Add the pumpkin and water. Bring to a boil.
    7. Reduce the heat and simmer for 20 minutes until the vegetables are tender.
    8. Remove from the heat and carefully blend until smooth.
    9. Add cream, salt, black pepper and nutmeg.
    10. Stir and heat gently for 2–3 minutes.
    11. Serve warm.'
    ),
    (
        'Сочевичний суп',
        'Lentil soup',
        'Поживний овочевий суп із сочевицею, томатами та ароматними спеціями.',
        'Hearty vegetable soup with lentils, tomatoes and aromatic spices.',
        45, 330, 4,
        '1. Ретельно промийте сочевицю під проточною водою.
    2. Очистьте цибулю, моркву, картоплю та часник.
    3. Наріжте цибулю дрібними кубиками, моркву кружальцями, картоплю та помідори кубиками.
    4. Розігрійте олію в каструлі на середньому вогні.
    5. Обсмажте цибулю та моркву протягом 5 хвилин.
    6. Додайте подрібнений часник, паприку та кумин. Перемішуйте 30 секунд.
    7. Додайте помідори й томатну пасту. Тушкуйте 3 хвилини.
    8. Додайте сочевицю, картоплю та воду.
    9. Доведіть до кипіння, зменште вогонь і варіть 25–30 хвилин до м’якості сочевиці.
    10. Додайте сіль, чорний перець і вичавлений лимонний сік.
    11. Перемішайте та подавайте з подрібненою петрушкою.',
        '1. Rinse the lentils thoroughly under running water.
    2. Peel the onion, carrots, potatoes and garlic.
    3. Finely dice the onion, slice the carrots and dice the potatoes and tomatoes.
    4. Heat the oil in a pot over medium heat.
    5. Fry the onion and carrots for 5 minutes.
    6. Add minced garlic, paprika and cumin. Stir for 30 seconds.
    7. Add the tomatoes and tomato paste. Cook for 3 minutes.
    8. Add the lentils, potatoes and water.
    9. Bring to a boil, reduce the heat and simmer for 25–30 minutes until the lentils are tender.
    10. Add salt, black pepper and freshly squeezed lemon juice.
    11. Stir and serve with chopped parsley.'
    )
    ) AS v(uk, en, d, de, t, k, s, i, ie)
    JOIN "RecipeCategories" c ON c."Name" = 'Супи';
    """);

            migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.q, u."Id"
    FROM (VALUES
        ('Український борщ', 'Яловичина', 400, 'г'),
        ('Український борщ', 'Вода', 1.5, 'л'),
        ('Український борщ', 'Буряк', 300, 'г'),
        ('Український борщ', 'Картопля', 300, 'г'),
        ('Український борщ', 'Капуста', 250, 'г'),
        ('Український борщ', 'Морква', 100, 'г'),
        ('Український борщ', 'Цибуля', 100, 'г'),
        ('Український борщ', 'Часник', 3, 'зубч.'),
        ('Український борщ', 'Томатна паста', 2, 'ст. л.'),
        ('Український борщ', 'Соняшникова олія', 2, 'ст. л.'),
        ('Український борщ', 'Оцет', 1, 'ст. л.'),
        ('Український борщ', 'Цукор', 1, 'ч. л.'),
        ('Український борщ', 'Сіль', 1.5, 'ч. л.'),
        ('Український борщ', 'Чорний перець', 0.25, 'ч. л.'),
        ('Український борщ', 'Лавровий лист', 2, 'шт'),
        ('Український борщ', 'Сметана', 80, 'г'),
        ('Український борщ', 'Кріп', 15, 'г'),

        ('Курячий суп', 'Куряче філе', 400, 'г'),
        ('Курячий суп', 'Вода', 1.5, 'л'),
        ('Курячий суп', 'Картопля', 300, 'г'),
        ('Курячий суп', 'Морква', 100, 'г'),
        ('Курячий суп', 'Цибуля', 80, 'г'),
        ('Курячий суп', 'Макарони', 100, 'г'),
        ('Курячий суп', 'Соняшникова олія', 1, 'ст. л.'),
        ('Курячий суп', 'Сіль', 1, 'ч. л.'),
        ('Курячий суп', 'Чорний перець', 2, 'дрібка'),
        ('Курячий суп', 'Лавровий лист', 1, 'шт'),
        ('Курячий суп', 'Петрушка', 15, 'г'),

        ('Грибний крем-суп', 'Печериці', 400, 'г'),
        ('Грибний крем-суп', 'Картопля', 250, 'г'),
        ('Грибний крем-суп', 'Цибуля', 100, 'г'),
        ('Грибний крем-суп', 'Масло', 30, 'г'),
        ('Грибний крем-суп', 'Вода', 500, 'мл'),
        ('Грибний крем-суп', 'Вершки', 200, 'мл'),
        ('Грибний крем-суп', 'Сіль', 0.75, 'ч. л.'),
        ('Грибний крем-суп', 'Чорний перець', 2, 'дрібка'),
        ('Грибний крем-суп', 'Петрушка', 10, 'г'),

        ('Гарбузовий крем-суп', 'Гарбуз', 600, 'г'),
        ('Гарбузовий крем-суп', 'Морква', 150, 'г'),
        ('Гарбузовий крем-суп', 'Цибуля', 100, 'г'),
        ('Гарбузовий крем-суп', 'Часник', 2, 'зубч.'),
        ('Гарбузовий крем-суп', 'Соняшникова олія', 1, 'ст. л.'),
        ('Гарбузовий крем-суп', 'Вода', 500, 'мл'),
        ('Гарбузовий крем-суп', 'Вершки', 150, 'мл'),
        ('Гарбузовий крем-суп', 'Сіль', 0.75, 'ч. л.'),
        ('Гарбузовий крем-суп', 'Чорний перець', 1, 'дрібка'),
        ('Гарбузовий крем-суп', 'Мускатний горіх', 1, 'дрібка'),

        ('Сочевичний суп', 'Сочевиця', 250, 'г'),
        ('Сочевичний суп', 'Картопля', 200, 'г'),
        ('Сочевичний суп', 'Помідори', 200, 'г'),
        ('Сочевичний суп', 'Морква', 100, 'г'),
        ('Сочевичний суп', 'Цибуля', 100, 'г'),
        ('Сочевичний суп', 'Часник', 2, 'зубч.'),
        ('Сочевичний суп', 'Томатна паста', 1, 'ст. л.'),
        ('Сочевичний суп', 'Соняшникова олія', 2, 'ст. л.'),
        ('Сочевичний суп', 'Вода', 1.2, 'л'),
        ('Сочевичний суп', 'Паприка', 1, 'ч. л.'),
        ('Сочевичний суп', 'Кумин (зіра)', 0.5, 'ч. л.'),
        ('Сочевичний суп', 'Сіль', 1, 'ч. л.'),
        ('Сочевичний суп', 'Чорний перець', 2, 'дрібка'),
        ('Сочевичний суп', 'Лимони', 0.5, 'шт'),
        ('Сочевичний суп', 'Петрушка', 15, 'г')
    ) AS v(recipe, product, q, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);
            migrationBuilder.Sql("""
    UPDATE "Recipes"
    SET "ImageUrl" = '/images/recipes/tomato-soup.webp'
    WHERE "Name" = 'Томатний суп';

    UPDATE "Recipes"
    SET "ImageUrl" = '/images/recipes/borscht.webp'
    WHERE "Name" = 'Український борщ';

    UPDATE "Recipes"
    SET "ImageUrl" = '/images/recipes/chicken-soup.webp'
    WHERE "Name" = 'Курячий суп';

    UPDATE "Recipes"
    SET "ImageUrl" = '/images/recipes/mushroom-soup.webp'
    WHERE "Name" = 'Грибний крем-суп';

    UPDATE "Recipes"
    SET "ImageUrl" = '/images/recipes/pumpkin-soup.webp'
    WHERE "Name" = 'Гарбузовий крем-суп';

    UPDATE "Recipes"
    SET "ImageUrl" = '/images/recipes/lentil-soup.webp'
    WHERE "Name" = 'Сочевичний суп';
    """);
            migrationBuilder.Sql("""
    INSERT INTO "Recipes"
    ("Name", "NameEn", "Description", "DescriptionEn",
    "CategoryId", "CookingTime", "Calories", "Servings",
    "Instructions", "InstructionsEn", "ImageUrl")
    SELECT v.uk, v.en, v.d, v.de, c."Id",
           v.t, v.k, v.s, v.i, v.ie, v.img
    FROM (VALUES
    (
        'Грецький салат',
        'Greek salad',
        'Свіжий овочевий салат із фетою, оливками та ароматною оливковою заправкою.',
        'Fresh vegetable salad with feta, olives and aromatic olive oil dressing.',
        15, 290, 2,
        '1. Ретельно помийте помідори, огірки, перець і листя салату.
    2. Наріжте помідори великими часточками, а огірки товстими півкружальцями.
    3. Видаліть насіння з перцю та наріжте його тонкими смужками.
    4. Очистьте цибулю та наріжте тонкими півкільцями.
    5. Наріжте фету кубиками приблизно 2 см.
    6. Порвіть листя салату руками та перекладіть у велику миску.
    7. Додайте нарізані овочі, оливки та фету.
    8. В окремій мисці змішайте оливкову олію, лимонний сік, орегано, сіль і чорний перець.
    9. Полийте салат заправкою та обережно перемішайте, щоб не пошкодити фету.
    10. Подавайте одразу після приготування.',
        '1. Thoroughly wash the tomatoes, cucumbers, bell pepper and lettuce.
    2. Cut the tomatoes into large wedges and the cucumbers into thick half-moons.
    3. Remove the seeds from the bell pepper and cut it into thin strips.
    4. Peel the onion and slice it into thin half-rings.
    5. Cut the feta into approximately 2 cm cubes.
    6. Tear the lettuce by hand and place it in a large bowl.
    7. Add the chopped vegetables, olives and feta.
    8. In a separate bowl, combine olive oil, lemon juice, oregano, salt and black pepper.
    9. Pour the dressing over the salad and toss gently to avoid breaking the feta.
    10. Serve immediately.',
        '/images/recipes/greek-salad.webp'
    ),
    (
        'Цезар',
        'Caesar salad',
        'Хрусткий салат із соковитим курячим філе, пармезаном, золотистими грінками та вершковою заправкою.',
        'Crisp salad with juicy chicken breast, Parmesan, golden croutons and creamy dressing.',
        35, 540, 2,
        '1. Промийте листя салату та ретельно обсушіть паперовим рушником.
    2. Наріжте хліб кубиками розміром приблизно 1,5 см.
    3. Змішайте хліб із половиною оливкової олії та половиною подрібненого часнику.
    4. Викладіть хліб на деко та запікайте при 180 °C протягом 8–10 хвилин до золотистої скоринки.
    5. Обсушіть куряче філе, приправте частиною солі та чорного перцю.
    6. Розігрійте решту оливкової олії на пательні. Обсмажуйте філе на середньому вогні по 6–8 хвилин з кожного боку до внутрішньої температури 74 °C.
    7. Залиште курку на 5 хвилин, після чого наріжте скибочками.
    8. Для заправки змішайте майонез, гірчицю, лимонний сік, решту часнику, солі та перцю.
    9. Порвіть листя салату руками та перемішайте із заправкою.
    10. Додайте куряче філе, грінки та тонкі скибочки пармезану.
    11. Подавайте одразу, поки грінки залишаються хрусткими.',
        '1. Wash the lettuce and dry it thoroughly with a paper towel.
    2. Cut the bread into approximately 1.5 cm cubes.
    3. Toss the bread with half the olive oil and half the minced garlic.
    4. Spread the bread on a baking tray and bake at 180 °C for 8–10 minutes until golden.
    5. Pat the chicken breast dry and season with some of the salt and black pepper.
    6. Heat the remaining olive oil in a frying pan. Cook the chicken over medium heat for 6–8 minutes per side until its internal temperature reaches 74 °C.
    7. Rest the chicken for 5 minutes, then slice it.
    8. For the dressing, combine mayonnaise, mustard, lemon juice and the remaining garlic, salt and pepper.
    9. Tear the lettuce by hand and toss with the dressing.
    10. Add the chicken, croutons and Parmesan shavings.
    11. Serve immediately while the croutons are still crisp.',
        '/images/recipes/caesar-salad.webp'
    ),
    (
        'Олів''є',
        'Olivier salad',
        'Традиційний ситний салат із картоплею, шинкою, яйцями, зеленим горошком і майонезом.',
        'Traditional hearty salad with potatoes, ham, eggs, green peas and mayonnaise.',
        60, 420, 4,
        '1. Ретельно помийте картоплю та моркву, не очищаючи від шкірки.
    2. Покладіть овочі в каструлю, залийте частиною води та доведіть до кипіння.
    3. Варіть моркву приблизно 20 хвилин, а картоплю 25–30 хвилин до м’якості.
    4. В окремій каструлі залийте яйця рештою води та варіть 10 хвилин після закипання.
    5. Злийте воду та залиште овочі й яйця охолоджуватися.
    6. Очистьте картоплю, моркву та яйця. Наріжте однаковими невеликими кубиками.
    7. Наріжте шинку та солоні огірки кубиками приблизно такого самого розміру.
    8. Злийте рідину з консервованого горошку.
    9. З’єднайте всі нарізані інгредієнти та горошок у великій мисці.
    10. Додайте майонез, сіль і чорний перець. Обережно перемішайте.
    11. Поставте салат у холодильник на 15 хвилин.
    12. Перед подаванням прикрасьте петрушкою.',
        '1. Thoroughly wash the potatoes and carrots without peeling them.
    2. Place the vegetables in a pot, cover with some of the water and bring to a boil.
    3. Cook the carrots for approximately 20 minutes and the potatoes for 25–30 minutes until tender.
    4. In a separate pot, cover the eggs with the remaining water and boil for 10 minutes.
    5. Drain the water and allow the vegetables and eggs to cool.
    6. Peel the potatoes, carrots and eggs. Cut them into evenly sized small cubes.
    7. Dice the ham and pickles into similarly sized cubes.
    8. Drain the canned peas.
    9. Combine all the chopped ingredients and peas in a large bowl.
    10. Add mayonnaise, salt and black pepper. Mix gently.
    11. Refrigerate the salad for 15 minutes.
    12. Garnish with parsley before serving.',
        '/images/recipes/olivier-salad.webp'
    ),
    (
        'Вінегрет',
        'Beetroot vinaigrette salad',
        'Яскравий овочевий салат із буряком, картоплею, морквою, солоними огірками та зеленим горошком.',
        'Colourful vegetable salad with beetroot, potatoes, carrots, pickles and green peas.',
        75, 230, 4,
        '1. Ретельно помийте буряк, картоплю та моркву.
    2. Покладіть буряк в окрему каструлю, залийте частиною води та варіть 45–60 хвилин до м’якості.
    3. В іншій каструлі залийте картоплю та моркву рештою води. Варіть 20–30 хвилин.
    4. Перевірте готовність овочів ножем. Він повинен легко входити в м’якоть.
    5. Злийте воду та повністю охолодіть овочі.
    6. Очистьте буряк, картоплю та моркву. Наріжте невеликими кубиками.
    7. Наріжте солоні огірки кубиками, а цибулю дрібними шматочками.
    8. Злийте рідину з консервованого горошку.
    9. Змішайте буряк із половиною соняшникової олії в окремій мисці.
    10. Додайте решту овочів, горошок, оцет, сіль, перець та решту олії.
    11. Обережно перемішайте та додайте подрібнену петрушку.
    12. Подавайте охолодженим.',
        '1. Thoroughly wash the beetroot, potatoes and carrots.
    2. Place the beetroot in a separate pot, cover with some of the water and boil for 45–60 minutes until tender.
    3. In another pot, cover the potatoes and carrots with the remaining water. Cook for 20–30 minutes.
    4. Check the vegetables with a knife. It should slide easily into the flesh.
    5. Drain the water and allow the vegetables to cool completely.
    6. Peel the beetroot, potatoes and carrots. Cut them into small cubes.
    7. Dice the pickles and finely chop the onion.
    8. Drain the canned peas.
    9. Toss the beetroot with half the sunflower oil in a separate bowl.
    10. Add the remaining vegetables, peas, vinegar, salt, pepper and remaining oil.
    11. Mix gently and add chopped parsley.
    12. Serve chilled.',
        '/images/recipes/vinaigrette.webp'
    ),
    (
        'Салат із тунцем',
        'Tuna salad',
        'Легкий білковий салат із тунцем, свіжими овочами, кукурудзою та лимонною заправкою.',
        'Light protein-rich salad with tuna, fresh vegetables, sweetcorn and lemon dressing.',
        25, 330, 2,
        '1. Помийте помідори, огірки, петрушку та листя салату.
    2. Ретельно обсушіть листя салату та порвіть його руками.
    3. Наріжте помідори часточками, а огірки півкружальцями.
    4. Очистьте цибулю та наріжте тонкими півкільцями.
    5. Злийте рідину з консервованого тунця та розділіть його виделкою на невеликі шматочки.
    6. Злийте рідину з консервованої кукурудзи.
    7. Викладіть листя салату в миску та додайте овочі, кукурудзу й тунець.
    8. Змішайте оливкову олію, лимонний сік, сіль і чорний перець.
    9. Полийте салат заправкою та обережно перемішайте.
    10. Посипте подрібненою петрушкою та подавайте.',
        '1. Wash the tomatoes, cucumbers, parsley and lettuce.
    2. Dry the lettuce thoroughly and tear it by hand.
    3. Cut the tomatoes into wedges and the cucumbers into half-moons.
    4. Peel the onion and slice it into thin half-rings.
    5. Drain the canned tuna and break it into small chunks with a fork.
    6. Drain the canned sweetcorn.
    7. Place the lettuce in a bowl and add the vegetables, sweetcorn and tuna.
    8. Combine olive oil, lemon juice, salt and black pepper.
    9. Pour the dressing over the salad and toss gently.
    10. Sprinkle with chopped parsley and serve.',
        '/images/recipes/tuna-salad.webp'
    ),
    (
        'Салат із капусти та моркви',
        'Cabbage and carrot salad',
        'Свіжий хрусткий салат із капусти та моркви з легкою лимонною заправкою.',
        'Fresh and crunchy cabbage and carrot salad with a light lemon dressing.',
        15, 140, 2,
        '1. Видаліть верхні пошкоджені листки капусти та промийте її.
    2. Нашаткуйте капусту тонкими довгими смужками.
    3. Перекладіть капусту в миску, додайте сіль і злегка помніть руками протягом 1–2 хвилин.
    4. Помийте та очистьте моркву.
    5. Натріть моркву на великій тертці та додайте до капусти.
    6. Помийте петрушку, обсушіть і дрібно наріжте.
    7. Змішайте соняшникову олію, лимонний сік, цукор і чорний перець.
    8. Додайте заправку та петрушку до овочів. Ретельно перемішайте.
    9. Залиште салат на 5 хвилин, щоб овочі просочилися заправкою.
    10. Подавайте свіжим або охолодженим.',
        '1. Remove the damaged outer cabbage leaves and wash the cabbage.
    2. Shred the cabbage into long, thin strips.
    3. Place the cabbage in a bowl, add salt and gently massage it for 1–2 minutes.
    4. Wash and peel the carrot.
    5. Coarsely grate the carrot and add it to the cabbage.
    6. Wash, dry and finely chop the parsley.
    7. Combine sunflower oil, lemon juice, sugar and black pepper.
    8. Add the dressing and parsley to the vegetables. Mix thoroughly.
    9. Let the salad rest for 5 minutes to absorb the dressing.
    10. Serve fresh or chilled.',
        '/images/recipes/cabbage-carrot-salad.webp'
    )
    ) AS v(uk, en, d, de, t, k, s, i, ie, img)
    JOIN "RecipeCategories" c ON c."Name" = 'Салати';
    """);
            migrationBuilder.Sql("""
    INSERT INTO "Products"
        ("Name", "NameEn", "CategoryId", "IsPantryStaple")
    VALUES
    (
        'Консервована кукурудза',
        'Canned sweetcorn',
        (SELECT "Id" FROM "Categories"
         WHERE "Name" = 'Бакалія'),
        false
    ),
    (
        'Консервований тунець',
        'Canned tuna',
        (SELECT "Id" FROM "Categories"
         WHERE "Name" = 'Риба та морепродукти'),
        false
    );

    INSERT INTO "ProductUnits" ("ProductId", "UnitId")
    SELECT p."Id", u."Id"
    FROM "Products" p
    CROSS JOIN "Units" u
    WHERE p."Name" IN
        ('Консервована кукурудза', 'Консервований тунець')
    AND u."ShortName" IN ('г', 'кг');
    """); migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.q, u."Id"
    FROM (VALUES
        ('Грецький салат', 'Помідори', 250, 'г'),
        ('Грецький салат', 'Огірки', 200, 'г'),
        ('Грецький салат', 'Перець', 100, 'г'),
        ('Грецький салат', 'Цибуля', 50, 'г'),
        ('Грецький салат', 'Салат', 80, 'г'),
        ('Грецький салат', 'Фета', 120, 'г'),
        ('Грецький салат', 'Оливки', 60, 'г'),
        ('Грецький салат', 'Оливкова олія', 1.5, 'ст. л.'),
        ('Грецький салат', 'Лимони', 0.5, 'шт'),
        ('Грецький салат', 'Орегано', 0.5, 'ч. л.'),
        ('Грецький салат', 'Сіль', 1, 'дрібка'),
        ('Грецький салат', 'Чорний перець', 1, 'дрібка'),

        ('Цезар', 'Куряче філе', 250, 'г'),
        ('Цезар', 'Салат', 200, 'г'),
        ('Цезар', 'Хліб', 100, 'г'),
        ('Цезар', 'Пармезан', 50, 'г'),
        ('Цезар', 'Оливкова олія', 2, 'ст. л.'),
        ('Цезар', 'Майонез', 50, 'г'),
        ('Цезар', 'Гірчиця', 1, 'ч. л.'),
        ('Цезар', 'Часник', 2, 'зубч.'),
        ('Цезар', 'Лимони', 0.5, 'шт'),
        ('Цезар', 'Сіль', 0.5, 'ч. л.'),
        ('Цезар', 'Чорний перець', 2, 'дрібка'),

        ('Олів''є', 'Картопля', 350, 'г'),
        ('Олів''є', 'Морква', 150, 'г'),
        ('Олів''є', 'Яйця', 3, 'шт'),
        ('Олів''є', 'Шинка', 250, 'г'),
        ('Олів''є', 'Солоні огірки', 150, 'г'),
        ('Олів''є', 'Консервований горошок', 150, 'г'),
        ('Олів''є', 'Майонез', 120, 'г'),
        ('Олів''є', 'Вода', 2, 'л'),
        ('Олів''є', 'Сіль', 0.5, 'ч. л.'),
        ('Олів''є', 'Чорний перець', 2, 'дрібка'),
        ('Олів''є', 'Петрушка', 10, 'г'),

        ('Вінегрет', 'Буряк', 350, 'г'),
        ('Вінегрет', 'Картопля', 300, 'г'),
        ('Вінегрет', 'Морква', 150, 'г'),
        ('Вінегрет', 'Солоні огірки', 150, 'г'),
        ('Вінегрет', 'Цибуля', 60, 'г'),
        ('Вінегрет', 'Консервований горошок', 150, 'г'),
        ('Вінегрет', 'Соняшникова олія', 3, 'ст. л.'),
        ('Вінегрет', 'Оцет', 1, 'ст. л.'),
        ('Вінегрет', 'Вода', 2.5, 'л'),
        ('Вінегрет', 'Сіль', 0.5, 'ч. л.'),
        ('Вінегрет', 'Чорний перець', 2, 'дрібка'),
        ('Вінегрет', 'Петрушка', 10, 'г'),

        ('Салат із тунцем', 'Консервований тунець', 200, 'г'),
        ('Салат із тунцем', 'Помідори', 200, 'г'),
        ('Салат із тунцем', 'Огірки', 150, 'г'),
        ('Салат із тунцем', 'Салат', 100, 'г'),
        ('Салат із тунцем', 'Цибуля', 40, 'г'),
        ('Салат із тунцем', 'Консервована кукурудза', 100, 'г'),
        ('Салат із тунцем', 'Оливкова олія', 2, 'ст. л.'),
        ('Салат із тунцем', 'Лимони', 0.5, 'шт'),
        ('Салат із тунцем', 'Сіль', 1, 'дрібка'),
        ('Салат із тунцем', 'Чорний перець', 1, 'дрібка'),
        ('Салат із тунцем', 'Петрушка', 10, 'г'),

        ('Салат із капусти та моркви', 'Капуста', 300, 'г'),
        ('Салат із капусти та моркви', 'Морква', 150, 'г'),
        ('Салат із капусти та моркви', 'Соняшникова олія', 1.5, 'ст. л.'),
        ('Салат із капусти та моркви', 'Лимони', 0.5, 'шт'),
        ('Салат із капусти та моркви', 'Цукор', 0.5, 'ч. л.'),
        ('Салат із капусти та моркви', 'Сіль', 0.25, 'ч. л.'),
        ('Салат із капусти та моркви', 'Чорний перець', 1, 'дрібка'),
        ('Салат із капусти та моркви', 'Петрушка', 10, 'г')
    ) AS v(recipe, product, q, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);

            migrationBuilder.Sql("""
    INSERT INTO "Recipes"
    ("Name", "NameEn", "Description", "DescriptionEn",
    "CategoryId", "CookingTime", "Calories", "Servings",
    "Instructions", "InstructionsEn", "ImageUrl")
    SELECT v.uk, v.en, v.d, v.de, c."Id",
           v.t, v.k, v.s, v.i, v.ie, v.img
    FROM (VALUES
    (
        'Запечений лосось',
        'Baked salmon',
        'Соковите філе лосося, запечене з лимоном, часником та ароматними травами.',
        'Juicy salmon fillet baked with lemon, garlic and aromatic herbs.',
        30, 460, 2,
        '1. Розігрійте духовку до 190 °C.
    2. Обсушіть філе лосося паперовим рушником та перевірте, чи немає кісток.
    3. Очистьте часник і дрібно подрібніть його.
    4. Помийте лимон. З половини лимона вичавіть сік, решту наріжте тонкими кружальцями.
    5. Змішайте оливкову олію, лимонний сік, часник, сіль і чорний перець.
    6. Викладіть лосось у форму для запікання шкіркою донизу.
    7. Полийте рибу маринадом і викладіть зверху кружальця лимона.
    8. Запікайте 15–20 хвилин до внутрішньої температури 63 °C.
    9. Дістаньте рибу з духовки та залиште на 3 хвилини.
    10. Посипте подрібненим кропом і подавайте теплою.',
        '1. Preheat the oven to 190 °C.
    2. Pat the salmon fillets dry with a paper towel and check for bones.
    3. Peel and finely mince the garlic.
    4. Wash the lemon. Squeeze the juice from one half and thinly slice the other half.
    5. Combine olive oil, lemon juice, garlic, salt and black pepper.
    6. Place the salmon skin-side down in a baking dish.
    7. Pour the marinade over the fish and arrange the lemon slices on top.
    8. Bake for 15–20 minutes until the internal temperature reaches 63 °C.
    9. Remove from the oven and rest for 3 minutes.
    10. Sprinkle with chopped dill and serve warm.',
        '/images/recipes/baked-salmon.webp'
    ),
    (
        'Плов',
        'Rice pilaf with pork',
        'Ситний ароматний плов зі свининою, рисом, морквою, часником і спеціями.',
        'Hearty aromatic rice pilaf with pork, carrots, garlic and spices.',
        65, 610, 4,
        '1. Промийте рис кілька разів, доки вода не стане майже прозорою.
    2. Наріжте свинину кубиками розміром приблизно 3 см.
    3. Очистьте цибулю та моркву. Наріжте цибулю кубиками, а моркву довгими смужками.
    4. Розігрійте соняшникову олію в казані або глибокій каструлі.
    5. Обсмажуйте свинину на середньо-сильному вогні 7–8 хвилин до золотистої скоринки.
    6. Додайте цибулю та моркву. Готуйте ще 5–7 хвилин.
    7. Додайте кумин, паприку, сіль, чорний перець і половину води. Тушкуйте 15 хвилин.
    8. Викладіть рис рівномірним шаром поверх м’яса та овочів, не перемішуючи.
    9. Додайте очищені зубчики часнику та обережно влийте решту гарячої води.
    10. Накрийте кришкою та готуйте на слабкому вогні 20–25 хвилин, доки рис не вбере воду, а м’ясо не стане готовим.
    11. Вимкніть нагрівання та залиште плов під кришкою на 10 хвилин.
    12. Обережно перемішайте та подавайте гарячим.',
        '1. Rinse the rice several times until the water is nearly clear.
    2. Cut the pork into approximately 3 cm cubes.
    3. Peel the onion and carrots. Dice the onion and cut the carrots into long strips.
    4. Heat sunflower oil in a heavy-bottomed pot.
    5. Fry the pork over medium-high heat for 7–8 minutes until golden.
    6. Add the onion and carrots. Cook for another 5–7 minutes.
    7. Add cumin, paprika, salt, black pepper and half the water. Simmer for 15 minutes.
    8. Spread the rice evenly over the meat and vegetables without stirring.
    9. Add the peeled garlic cloves and carefully pour in the remaining hot water.
    10. Cover and cook over low heat for 20–25 minutes until the rice absorbs the water and the meat is cooked through.
    11. Turn off the heat and leave covered for 10 minutes.
    12. Stir gently and serve hot.',
        '/images/recipes/pilaf.webp'
    ),
    (
        'Котлети',
        'Homemade meat patties',
        'Соковиті домашні котлети з м’ясного фаршу із золотистою хрусткою скоринкою.',
        'Juicy homemade minced meat patties with a crispy golden crust.',
        40, 410, 4,
        '1. Покладіть хліб у миску, залийте молоком і залиште на 5 хвилин.
    2. Очистьте цибулю та часник. Дрібно наріжте або натріть їх.
    3. Перекладіть фарш у глибоку миску.
    4. Додайте розмочений хліб разом із молоком, цибулю, часник, яйце, сіль і чорний перець.
    5. Ретельно вимішуйте фарш протягом 3–4 хвилин до однорідності.
    6. Сформуйте 8 однакових котлет.
    7. Обваляйте кожну котлету в панірувальних сухарях.
    8. Розігрійте соняшникову олію на пательні на середньому вогні.
    9. Обсмажуйте котлети по 4–5 хвилин з кожного боку до золотистої скоринки.
    10. Зменште вогонь, накрийте кришкою та готуйте ще 5–8 хвилин до внутрішньої температури щонайменше 74 °C.
    11. Викладіть котлети на паперовий рушник, щоб прибрати надлишок олії.
    12. Подавайте теплими.',
        '1. Place the bread in a bowl, cover with milk and leave for 5 minutes.
    2. Peel the onion and garlic. Finely chop or grate them.
    3. Place the minced meat in a large bowl.
    4. Add the soaked bread with the milk, onion, garlic, egg, salt and black pepper.
    5. Mix thoroughly for 3–4 minutes until evenly combined.
    6. Shape the mixture into 8 equal patties.
    7. Coat each patty with breadcrumbs.
    8. Heat sunflower oil in a frying pan over medium heat.
    9. Fry the patties for 4–5 minutes per side until golden.
    10. Reduce the heat, cover and cook for another 5–8 minutes until the internal temperature reaches at least 74 °C.
    11. Transfer the patties to a paper towel to remove excess oil.
    12. Serve warm.',
        '/images/recipes/meat-patties.webp'
    ),
    (
        'Картопля з грибами',
        'Fried potatoes with mushrooms',
        'Золотиста смажена картопля з печерицями, цибулею та свіжою зеленню.',
        'Golden fried potatoes with mushrooms, onion and fresh herbs.',
        40, 350, 3,
        '1. Помийте та очистьте картоплю. Наріжте її тонкими брусочками.
    2. Ретельно обсушіть картоплю паперовим рушником.
    3. Очистьте печериці та наріжте пластинками.
    4. Очистьте цибулю та наріжте півкільцями.
    5. Розігрійте половину соняшникової олії на великій пательні.
    6. Обсмажуйте картоплю на середньо-сильному вогні 15–20 хвилин, періодично перевертаючи.
    7. На іншій пательні розігрійте решту олії та обсмажте цибулю протягом 3 хвилин.
    8. Додайте печериці та готуйте 7–8 хвилин до випаровування зайвої рідини.
    9. Перекладіть гриби з цибулею до картоплі.
    10. Додайте вершкове масло, сіль і чорний перець. Готуйте ще 5 хвилин до м’якості картоплі.
    11. Посипте подрібненою петрушкою та подавайте гарячою.',
        '1. Wash and peel the potatoes. Cut them into thin sticks.
    2. Pat the potatoes thoroughly dry with a paper towel.
    3. Clean the mushrooms and cut them into slices.
    4. Peel the onion and slice it into half-rings.
    5. Heat half the sunflower oil in a large frying pan.
    6. Fry the potatoes over medium-high heat for 15–20 minutes, turning occasionally.
    7. Heat the remaining oil in another pan and fry the onion for 3 minutes.
    8. Add the mushrooms and cook for 7–8 minutes until excess moisture evaporates.
    9. Transfer the mushrooms and onion to the potatoes.
    10. Add butter, salt and black pepper. Cook for another 5 minutes until the potatoes are tender.
    11. Sprinkle with chopped parsley and serve hot.',
        '/images/recipes/potatoes-mushrooms.webp'
    ),
    (
        'Паста болоньєзе',
        'Pasta Bolognese',
        'Паста з насиченим томатно-м’ясним соусом, овочами та пармезаном.',
        'Pasta with a rich tomato and meat sauce, vegetables and Parmesan.',
        60, 650, 4,
        '1. Очистьте цибулю, моркву та часник.
    2. Дрібно наріжте цибулю й часник, а моркву натріть на тертці.
    3. Помийте помідори та наріжте невеликими кубиками.
    4. Розігрійте оливкову олію в глибокій пательні.
    5. Обсмажуйте цибулю та моркву на середньому вогні 5 хвилин.
    6. Додайте часник і фарш. Готуйте 7–10 хвилин, розбиваючи грудочки лопаткою.
    7. Додайте помідори, томатну пасту, орегано, базилік, чорний перець і половину солі.
    8. Влийте 200 мл води, перемішайте та тушкуйте на слабкому вогні 25–30 хвилин до загустіння соусу.
    9. Закип’ятіть решту води, додайте решту солі та відваріть макарони відповідно до інструкції на упаковці.
    10. Злийте воду з макаронів і розкладіть їх по тарілках.
    11. Викладіть зверху гарячий м’ясний соус.
    12. Посипте натертим пармезаном і подавайте.',
        '1. Peel the onion, carrot and garlic.
    2. Finely chop the onion and garlic, and grate the carrot.
    3. Wash the tomatoes and cut them into small cubes.
    4. Heat olive oil in a deep frying pan.
    5. Fry the onion and carrot over medium heat for 5 minutes.
    6. Add garlic and minced meat. Cook for 7–10 minutes, breaking up the lumps with a spatula.
    7. Add tomatoes, tomato paste, oregano, basil, black pepper and half the salt.
    8. Pour in 200 ml of water, stir and simmer over low heat for 25–30 minutes until the sauce thickens.
    9. Bring the remaining water to a boil, add the remaining salt and cook the pasta according to the package instructions.
    10. Drain the pasta and divide it between plates.
    11. Spoon the hot meat sauce over the pasta.
    12. Sprinkle with grated Parmesan and serve.',
        '/images/recipes/pasta-bolognese.webp'
    ),
    (
        'Курка в соєвому соусі',
        'Chicken in soy sauce',
        'Ніжні шматочки курячого філе в солодко-солоному соєвому соусі з медом, часником і кунжутом.',
        'Tender chicken pieces in a sweet and savoury soy sauce with honey, garlic and sesame.',
        35, 390, 2,
        '1. Обсушіть куряче філе паперовим рушником і наріжте невеликими кубиками.
    2. Очистьте та подрібніть часник.
    3. У мисці змішайте соєвий соус, мед, часник і чорний перець.
    4. Додайте курку до маринаду, перемішайте та залиште в холодильнику на 10 хвилин.
    5. Розігрійте соняшникову олію на пательні на середньо-сильному вогні.
    6. Вийміть курку з маринаду та обсмажуйте 6–8 хвилин, періодично перемішуючи.
    7. Змішайте крохмаль із холодною водою до повного розчинення.
    8. Додайте маринад і крохмальну суміш до курки.
    9. Доведіть соус до кипіння та готуйте 3–5 хвилин до загустіння і внутрішньої температури курки 74 °C.
    10. Посипте кунжутом і подрібненою зеленою цибулею.
    11. Подавайте гарячою.',
        '1. Pat the chicken breast dry and cut it into small cubes.
    2. Peel and mince the garlic.
    3. Combine soy sauce, honey, garlic and black pepper in a bowl.
    4. Add the chicken, mix and refrigerate for 10 minutes to marinate.
    5. Heat sunflower oil in a frying pan over medium-high heat.
    6. Remove the chicken from the marinade and fry for 6–8 minutes, stirring occasionally.
    7. Mix the starch with cold water until fully dissolved.
    8. Add the marinade and starch mixture to the chicken.
    9. Bring the sauce to a boil and cook for 3–5 minutes until thickened and the chicken reaches an internal temperature of 74 °C.
    10. Sprinkle with sesame seeds and chopped green onion.
    11. Serve hot.',
        '/images/recipes/soy-sauce-chicken.webp'
    ),
    (
        'Фарширований перець',
        'Stuffed bell peppers',
        'Солодкий перець із начинкою з м’ясного фаршу та рису, тушкований у томатному соусі.',
        'Sweet bell peppers filled with minced meat and rice, simmered in tomato sauce.',
        80, 440, 4,
        '1. Промийте рис під проточною водою.
    2. Залийте рис 200 мл води та варіть 8–10 хвилин до напівготовності. Залиште охолоджуватися.
    3. Помийте перець, зріжте верхівки та видаліть насіння.
    4. Очистьте цибулю, моркву та часник. Цибулю й часник подрібніть, моркву натріть.
    5. Розігрійте олію на пательні та обсмажте цибулю з морквою протягом 5 хвилин.
    6. Змішайте фарш, рис, половину обсмажених овочів, часник, половину солі та чорного перцю.
    7. Наповніть перці начинкою, залишаючи трохи вільного місця для рису.
    8. Поставте фаршировані перці вертикально в глибоку каструлю.
    9. Змішайте решту обсмажених овочів, томатну пасту, сметану, решту води, солі та перцю.
    10. Залийте перці соусом і доведіть до кипіння.
    11. Накрийте кришкою та тушкуйте на слабкому вогні 40–45 хвилин до м’якості перцю та внутрішньої температури начинки щонайменше 74 °C.
    12. Посипте подрібненою петрушкою та подавайте разом із соусом.',
        '1. Rinse the rice under running water.
    2. Cover the rice with 200 ml of water and cook for 8–10 minutes until partially cooked. Allow to cool.
    3. Wash the bell peppers, cut off the tops and remove the seeds.
    4. Peel the onion, carrot and garlic. Chop the onion and garlic, and grate the carrot.
    5. Heat the oil in a frying pan and fry the onion and carrot for 5 minutes.
    6. Combine minced meat, rice, half the fried vegetables, garlic, half the salt and black pepper.
    7. Fill the peppers with the mixture, leaving a little room for the rice to expand.
    8. Arrange the stuffed peppers upright in a deep pot.
    9. Combine the remaining fried vegetables, tomato paste, sour cream, remaining water, salt and pepper.
    10. Pour the sauce over the peppers and bring to a boil.
    11. Cover and simmer over low heat for 40–45 minutes until the peppers are tender and the filling reaches at least 74 °C.
    12. Sprinkle with chopped parsley and serve with the sauce.',
        '/images/recipes/stuffed-peppers.webp'
    ),
    (
        'Запечений хек',
        'Baked hake with vegetables',
        'Ніжне філе хека, запечене з картоплею, морквою, цибулею та лимоном.',
        'Tender hake fillets baked with potatoes, carrots, onion and lemon.',
        55, 370, 3,
        '1. Розігрійте духовку до 190 °C.
    2. Якщо риба заморожена, заздалегідь розморозьте її в холодильнику.
    3. Обсушіть філе хека паперовим рушником і перевірте на наявність кісток.
    4. Очистьте картоплю, моркву та цибулю.
    5. Наріжте картоплю й моркву тонкими кружальцями, а цибулю півкільцями.
    6. Змішайте овочі з половиною оливкової олії, солі та чорного перцю.
    7. Викладіть овочі у форму для запікання та готуйте 20 хвилин.
    8. Змішайте решту олії, лимонний сік, паприку, сіль і чорний перець.
    9. Змастіть рибу отриманою сумішшю та викладіть поверх овочів.
    10. Запікайте ще 15–20 хвилин до м’якості овочів та внутрішньої температури риби 63 °C.
    11. Посипте подрібненим кропом і подавайте гарячим.',
        '1. Preheat the oven to 190 °C.
    2. If the fish is frozen, thaw it in the refrigerator beforehand.
    3. Pat the hake fillets dry and check for bones.
    4. Peel the potatoes, carrots and onion.
    5. Thinly slice the potatoes and carrots, and cut the onion into half-rings.
    6. Toss the vegetables with half the olive oil, salt and black pepper.
    7. Place the vegetables in a baking dish and bake for 20 minutes.
    8. Combine the remaining oil, lemon juice, paprika, salt and black pepper.
    9. Coat the fish with the mixture and place it on top of the vegetables.
    10. Bake for another 15–20 minutes until the vegetables are tender and the fish reaches an internal temperature of 63 °C.
    11. Sprinkle with chopped dill and serve hot.',
        '/images/recipes/baked-hake.webp'
    )
    ) AS v(uk, en, d, de, t, k, s, i, ie, img)
    JOIN "RecipeCategories" c ON c."Name" = 'Основні страви';
    """);
            migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.q, u."Id"
    FROM (VALUES
        ('Запечений лосось', 'Лосось', 400, 'г'),
        ('Запечений лосось', 'Лимони', 1, 'шт'),
        ('Запечений лосось', 'Часник', 2, 'зубч.'),
        ('Запечений лосось', 'Оливкова олія', 2, 'ст. л.'),
        ('Запечений лосось', 'Кріп', 10, 'г'),
        ('Запечений лосось', 'Сіль', 0.5, 'ч. л.'),
        ('Запечений лосось', 'Чорний перець', 2, 'дрібка'),

        ('Плов', 'Свинина', 500, 'г'),
        ('Плов', 'Рис', 300, 'г'),
        ('Плов', 'Морква', 250, 'г'),
        ('Плов', 'Цибуля', 150, 'г'),
        ('Плов', 'Часник', 5, 'зубч.'),
        ('Плов', 'Соняшникова олія', 3, 'ст. л.'),
        ('Плов', 'Вода', 600, 'мл'),
        ('Плов', 'Кумин (зіра)', 1, 'ч. л.'),
        ('Плов', 'Паприка', 1, 'ч. л.'),
        ('Плов', 'Сіль', 1.5, 'ч. л.'),
        ('Плов', 'Чорний перець', 0.5, 'ч. л.'),

        ('Котлети', 'Фарш', 500, 'г'),
        ('Котлети', 'Хліб', 80, 'г'),
        ('Котлети', 'Молоко', 100, 'мл'),
        ('Котлети', 'Цибуля', 100, 'г'),
        ('Котлети', 'Часник', 2, 'зубч.'),
        ('Котлети', 'Яйця', 1, 'шт'),
        ('Котлети', 'Панірувальні сухарі', 60, 'г'),
        ('Котлети', 'Соняшникова олія', 3, 'ст. л.'),
        ('Котлети', 'Сіль', 1, 'ч. л.'),
        ('Котлети', 'Чорний перець', 0.5, 'ч. л.'),

        ('Картопля з грибами', 'Картопля', 700, 'г'),
        ('Картопля з грибами', 'Печериці', 350, 'г'),
        ('Картопля з грибами', 'Цибуля', 150, 'г'),
        ('Картопля з грибами', 'Соняшникова олія', 3, 'ст. л.'),
        ('Картопля з грибами', 'Масло', 20, 'г'),
        ('Картопля з грибами', 'Петрушка', 15, 'г'),
        ('Картопля з грибами', 'Сіль', 0.75, 'ч. л.'),
        ('Картопля з грибами', 'Чорний перець', 2, 'дрібка'),

        ('Паста болоньєзе', 'Макарони', 350, 'г'),
        ('Паста болоньєзе', 'Фарш', 400, 'г'),
        ('Паста болоньєзе', 'Помідори', 500, 'г'),
        ('Паста болоньєзе', 'Томатна паста', 2, 'ст. л.'),
        ('Паста болоньєзе', 'Цибуля', 100, 'г'),
        ('Паста болоньєзе', 'Морква', 100, 'г'),
        ('Паста болоньєзе', 'Часник', 3, 'зубч.'),
        ('Паста болоньєзе', 'Оливкова олія', 2, 'ст. л.'),
        ('Паста болоньєзе', 'Пармезан', 60, 'г'),
        ('Паста болоньєзе', 'Орегано', 1, 'ч. л.'),
        ('Паста болоньєзе', 'Базилік', 1, 'ч. л.'),
        ('Паста болоньєзе', 'Вода', 3.2, 'л'),
        ('Паста болоньєзе', 'Сіль', 2, 'ч. л.'),
        ('Паста болоньєзе', 'Чорний перець', 0.5, 'ч. л.'),

        ('Курка в соєвому соусі', 'Куряче філе', 400, 'г'),
        ('Курка в соєвому соусі', 'Соєвий соус', 4, 'ст. л.'),
        ('Курка в соєвому соусі', 'Мед', 2, 'ст. л.'),
        ('Курка в соєвому соусі', 'Часник', 3, 'зубч.'),
        ('Курка в соєвому соусі', 'Соняшникова олія', 1, 'ст. л.'),
        ('Курка в соєвому соусі', 'Крохмаль', 1, 'ст. л.'),
        ('Курка в соєвому соусі', 'Вода', 80, 'мл'),
        ('Курка в соєвому соусі', 'Кунжут', 1, 'ст. л.'),
        ('Курка в соєвому соусі', 'Зелена цибуля', 20, 'г'),
        ('Курка в соєвому соусі', 'Чорний перець', 2, 'дрібка'),

        ('Фарширований перець', 'Перець', 4, 'шт'),
        ('Фарширований перець', 'Фарш', 500, 'г'),
        ('Фарширований перець', 'Рис', 120, 'г'),
        ('Фарширований перець', 'Цибуля', 150, 'г'),
        ('Фарширований перець', 'Морква', 100, 'г'),
        ('Фарширований перець', 'Часник', 2, 'зубч.'),
        ('Фарширований перець', 'Томатна паста', 3, 'ст. л.'),
        ('Фарширований перець', 'Сметана', 100, 'г'),
        ('Фарширований перець', 'Соняшникова олія', 2, 'ст. л.'),
        ('Фарширований перець', 'Вода', 700, 'мл'),
        ('Фарширований перець', 'Сіль', 1, 'ч. л.'),
        ('Фарширований перець', 'Чорний перець', 0.5, 'ч. л.'),
        ('Фарширований перець', 'Петрушка', 15, 'г'),

        ('Запечений хек', 'Хек', 600, 'г'),
        ('Запечений хек', 'Картопля', 500, 'г'),
        ('Запечений хек', 'Морква', 150, 'г'),
        ('Запечений хек', 'Цибуля', 100, 'г'),
        ('Запечений хек', 'Лимони', 0.5, 'шт'),
        ('Запечений хек', 'Оливкова олія', 3, 'ст. л.'),
        ('Запечений хек', 'Паприка', 1, 'ч. л.'),
        ('Запечений хек', 'Кріп', 15, 'г'),
        ('Запечений хек', 'Сіль', 0.75, 'ч. л.'),
        ('Запечений хек', 'Чорний перець', 2, 'дрібка')
    ) AS v(recipe, product, q, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);
            migrationBuilder.Sql("""
    INSERT INTO "Recipes"
    ("Name", "NameEn", "Description", "DescriptionEn",
    "CategoryId", "CookingTime", "Calories", "Servings",
    "Instructions", "InstructionsEn", "ImageUrl")
    SELECT v.uk, v.en, v.d, v.de, c."Id",
           v.t, v.k, v.s, v.i, v.ie, v.img
    FROM (VALUES
    (
        'Піца Маргарита',
        'Margherita pizza',
        'Класична італійська піца на тонкому дріжджовому тісті з томатним соусом, моцарелою та свіжим базиліком.',
        'Classic Italian pizza with thin yeast dough, tomato sauce, mozzarella and fresh basil.',
        100, 480, 3,
        '1. Підігрійте воду до температури 35–38 °C.
    2. Додайте сухі дріжджі та цукор, перемішайте й залиште на 10 хвилин.
    3. Просійте борошно в глибоку миску, додайте половину солі та дріжджову суміш.
    4. Додайте половину оливкової олії та вимішуйте тісто 8–10 хвилин до еластичності.
    5. Накрийте миску та залиште в теплому місці на 45–60 хвилин, доки тісто не збільшиться приблизно вдвічі.
    6. Подрібніть помідори та часник. Розігрійте решту олії на пательні.
    7. Додайте часник, помідори, томатну пасту, орегано, решту солі та чорний перець. Тушкуйте 10–15 хвилин до загустіння.
    8. Розігрійте духовку до 230 °C.
    9. Розтягніть тісто руками у круглу основу діаметром приблизно 30 см і викладіть на пергамент.
    10. Рівномірно розподіліть томатний соус, залишаючи вільними краї. Викладіть шматочки моцарели.
    11. Випікайте 10–15 хвилин до золотистих країв і розплавлення сиру.
    12. Прикрасьте свіжим базиліком, наріжте та подавайте гарячою.',
        '1. Warm the water to 35–38 °C.
    2. Add dry yeast and sugar, stir and leave for 10 minutes.
    3. Sift the flour into a large bowl, add half the salt and the yeast mixture.
    4. Add half the olive oil and knead for 8–10 minutes until elastic.
    5. Cover the bowl and leave in a warm place for 45–60 minutes until approximately doubled in size.
    6. Chop the tomatoes and garlic. Heat the remaining oil in a frying pan.
    7. Add garlic, tomatoes, tomato paste, oregano, remaining salt and black pepper. Simmer for 10–15 minutes until thickened.
    8. Preheat the oven to 230 °C.
    9. Stretch the dough into a round base approximately 30 cm wide and place it on baking parchment.
    10. Spread the tomato sauce evenly, leaving the edges uncovered. Arrange the mozzarella pieces on top.
    11. Bake for 10–15 minutes until the edges are golden and the cheese has melted.
    12. Garnish with fresh basil, slice and serve hot.',
        '/images/recipes/pizza-margherita.webp'
    ),
    (
        'Піца з шинкою та грибами',
        'Ham and mushroom pizza',
        'Ароматна домашня піца з томатним соусом, шинкою, печерицями та розплавленою моцарелою.',
        'Homemade pizza with tomato sauce, ham, mushrooms and melted mozzarella.',
        110, 590, 3,
        '1. Підігрійте воду до 35–38 °C, додайте сухі дріжджі та цукор. Залиште на 10 хвилин.
    2. Просійте борошно, додайте половину солі, дріжджову суміш та половину оливкової олії.
    3. Вимішуйте тісто 8–10 хвилин, доки воно не стане гладким та еластичним.
    4. Накрийте тісто та залиште в теплому місці на 45–60 хвилин.
    5. Помийте помідори, подрібніть їх разом із часником.
    6. Розігрійте решту олії на пательні. Додайте помідори, часник, томатну пасту, орегано, решту солі та чорний перець. Тушкуйте 10–15 хвилин.
    7. Очистьте печериці та наріжте тонкими пластинками. Обсмажте їх на сухій пательні 5–7 хвилин до випаровування зайвої рідини.
    8. Наріжте шинку тонкими смужками, а моцарелу невеликими шматочками.
    9. Розігрійте духовку до 230 °C.
    10. Розтягніть тісто у круглу основу діаметром приблизно 30 см і викладіть на пергамент.
    11. Змастіть основу томатним соусом, розподіліть моцарелу, шинку та гриби.
    12. Випікайте 12–15 хвилин до золотистої скоринки та розплавлення сиру.',
        '1. Warm the water to 35–38 °C, add dry yeast and sugar. Leave for 10 minutes.
    2. Sift the flour and add half the salt, yeast mixture and half the olive oil.
    3. Knead for 8–10 minutes until the dough is smooth and elastic.
    4. Cover the dough and leave in a warm place for 45–60 minutes.
    5. Wash the tomatoes and chop them together with the garlic.
    6. Heat the remaining oil in a frying pan. Add tomatoes, garlic, tomato paste, oregano, remaining salt and black pepper. Simmer for 10–15 minutes.
    7. Clean the mushrooms and slice them thinly. Fry in a dry pan for 5–7 minutes until excess moisture evaporates.
    8. Cut the ham into thin strips and the mozzarella into small pieces.
    9. Preheat the oven to 230 °C.
    10. Stretch the dough into a round base approximately 30 cm wide and place it on baking parchment.
    11. Spread the tomato sauce over the base and distribute the mozzarella, ham and mushrooms.
    12. Bake for 12–15 minutes until the crust is golden and the cheese has melted.',
        '/images/recipes/pizza-ham-mushrooms.webp'
    ),
    (
        'Часникові булочки',
        'Garlic bread rolls',
        'М’які пухкі дріжджові булочки з ароматною часниковою заправкою та свіжим кропом.',
        'Soft and fluffy yeast rolls with aromatic garlic dressing and fresh dill.',
        130, 240, 8,
        '1. Підігрійте молоко до 35–38 °C.
    2. Додайте сухі дріжджі та цукор, перемішайте й залиште на 10 хвилин.
    3. Просійте борошно у велику миску та додайте половину солі.
    4. Додайте дріжджову суміш, яйце та половину розтопленого, але не гарячого вершкового масла.
    5. Вимішуйте тісто 8–10 хвилин до гладкої еластичної консистенції.
    6. Накрийте миску та залиште в теплому місці на 45–60 хвилин.
    7. Розділіть тісто на 8 однакових частин і сформуйте круглі булочки.
    8. Викладіть булочки у форму, застелену пергаментом, залишаючи між ними невеликі проміжки.
    9. Накрийте та залиште ще на 20 хвилин. Розігрійте духовку до 180 °C.
    10. Випікайте 20–25 хвилин до золотистої скоринки.
    11. Подрібніть часник і кріп. Змішайте їх із рештою розтопленого масла, соняшниковою олією та рештою солі.
    12. Змастіть гарячі булочки часниковою сумішшю та залиште на 5 хвилин перед подаванням.',
        '1. Warm the milk to 35–38 °C.
    2. Add dry yeast and sugar, stir and leave for 10 minutes.
    3. Sift the flour into a large bowl and add half the salt.
    4. Add the yeast mixture, egg and half the melted but not hot butter.
    5. Knead for 8–10 minutes until smooth and elastic.
    6. Cover the bowl and leave in a warm place for 45–60 minutes.
    7. Divide the dough into 8 equal portions and shape them into round rolls.
    8. Place the rolls in a baking dish lined with parchment, leaving small gaps between them.
    9. Cover and leave for another 20 minutes. Preheat the oven to 180 °C.
    10. Bake for 20–25 minutes until golden.
    11. Mince the garlic and chop the dill. Combine with the remaining melted butter, sunflower oil and remaining salt.
    12. Brush the hot rolls with the garlic mixture and leave for 5 minutes before serving.',
        '/images/recipes/garlic-rolls.webp'
    ),
    (
        'Сирні мафіни',
        'Savoury cheese muffins',
        'Ніжні несолодкі мафіни з твердим сиром, кефіром та зеленою цибулею.',
        'Tender savoury muffins with hard cheese, kefir and green onion.',
        40, 190, 8,
        '1. Розігрійте духовку до 180 °C.
    2. Підготуйте форму для 8 мафінів і змастіть заглиблення частиною соняшникової олії.
    3. Натріть твердий сир на великій тертці.
    4. Помийте зелену цибулю, обсушіть і дрібно наріжте.
    5. У глибокій мисці збийте яйця з кефіром та рештою соняшникової олії.
    6. В окремій мисці змішайте просіяне борошно, розпушувач, сіль і чорний перець.
    7. Поступово додайте сухі інгредієнти до рідких та перемішайте до зникнення сухого борошна.
    8. Додайте натертий сир і зелену цибулю. Обережно перемішайте лопаткою.
    9. Розподіліть тісто між формочками, заповнюючи їх приблизно на три чверті.
    10. Випікайте 20–25 хвилин до золотистого кольору. Перевірте готовність дерев’яною шпажкою.
    11. Залиште мафіни у формі на 5 хвилин, після чого обережно дістаньте.
    12. Подавайте теплими або повністю охолодженими.',
        '1. Preheat the oven to 180 °C.
    2. Prepare an 8-cup muffin tin and grease the cups with some of the sunflower oil.
    3. Coarsely grate the hard cheese.
    4. Wash, dry and finely chop the green onion.
    5. In a large bowl, whisk the eggs with kefir and the remaining sunflower oil.
    6. In a separate bowl, combine sifted flour, baking powder, salt and black pepper.
    7. Gradually add the dry ingredients to the wet ingredients and mix until no dry flour remains.
    8. Add the grated cheese and green onion. Fold gently with a spatula.
    9. Divide the batter between the muffin cups, filling each approximately three-quarters full.
    10. Bake for 20–25 minutes until golden. Check doneness with a wooden skewer.
    11. Leave the muffins in the tin for 5 minutes, then carefully remove them.
    12. Serve warm or completely cooled.',
        '/images/recipes/cheese-muffins.webp'
    ),
    (
        'Фокача',
        'Italian focaccia',
        'Пухкий італійський хліб із хрусткою скоринкою, оливковою олією, помідорами, оливками та орегано.',
        'Fluffy Italian bread with a crispy crust, olive oil, tomatoes, olives and oregano.',
        150, 290, 6,
        '1. Підігрійте воду до 35–38 °C.
    2. Додайте сухі дріжджі та цукор. Перемішайте й залиште на 10 хвилин.
    3. Просійте борошно у велику миску, додайте більшу частину солі та дріжджову суміш.
    4. Додайте половину оливкової олії та вимішуйте тісто 8–10 хвилин. Тісто має залишатися м’яким і трохи липким.
    5. Накрийте миску та залиште тісто в теплому місці на 60 хвилин.
    6. Змастіть форму для запікання частиною оливкової олії.
    7. Перекладіть тісто у форму та обережно розтягніть руками по всій поверхні.
    8. Накрийте форму та залиште тісто ще на 25–30 хвилин.
    9. Помийте помідори та наріжте невеликими часточками. Злийте рідину з оливок.
    10. Зробіть пальцями невеликі заглиблення в тісті. Розподіліть помідори та оливки, полийте рештою олії, посипте орегано й рештою солі.
    11. Випікайте в розігрітій до 220 °C духовці 20–25 хвилин до золотистої скоринки.
    12. Залиште фокачу охолоджуватися на решітці протягом 10 хвилин, після чого наріжте.',
        '1. Warm the water to 35–38 °C.
    2. Add dry yeast and sugar. Stir and leave for 10 minutes.
    3. Sift the flour into a large bowl, add most of the salt and the yeast mixture.
    4. Add half the olive oil and knead for 8–10 minutes. The dough should remain soft and slightly sticky.
    5. Cover the bowl and leave the dough in a warm place for 60 minutes.
    6. Grease a baking dish with some of the olive oil.
    7. Transfer the dough to the dish and gently stretch it across the surface.
    8. Cover the dish and leave the dough for another 25–30 minutes.
    9. Wash the tomatoes and cut them into small wedges. Drain the olives.
    10. Make small dimples in the dough with your fingers. Arrange the tomatoes and olives, drizzle with the remaining oil and sprinkle with oregano and remaining salt.
    11. Bake in a preheated oven at 220 °C for 20–25 minutes until golden.
    12. Cool the focaccia on a wire rack for 10 minutes before slicing.',
        '/images/recipes/focaccia.webp'
    )
    ) AS v(uk, en, d, de, t, k, s, i, ie, img)
    JOIN "RecipeCategories" c ON c."Name" = 'Випічка';
    """);
            migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.q, u."Id"
    FROM (VALUES
        ('Піца Маргарита', 'Борошно', 250, 'г'),
        ('Піца Маргарита', 'Вода', 150, 'мл'),
        ('Піца Маргарита', 'Сухі дріжджі', 5, 'г'),
        ('Піца Маргарита', 'Цукор', 1, 'ч. л.'),
        ('Піца Маргарита', 'Сіль', 1, 'ч. л.'),
        ('Піца Маргарита', 'Оливкова олія', 2, 'ст. л.'),
        ('Піца Маргарита', 'Помідори', 200, 'г'),
        ('Піца Маргарита', 'Томатна паста', 1, 'ст. л.'),
        ('Піца Маргарита', 'Часник', 2, 'зубч.'),
        ('Піца Маргарита', 'Моцарела', 150, 'г'),
        ('Піца Маргарита', 'Базилік', 10, 'г'),
        ('Піца Маргарита', 'Орегано', 0.5, 'ч. л.'),
        ('Піца Маргарита', 'Чорний перець', 1, 'дрібка'),

        ('Піца з шинкою та грибами', 'Борошно', 250, 'г'),
        ('Піца з шинкою та грибами', 'Вода', 150, 'мл'),
        ('Піца з шинкою та грибами', 'Сухі дріжджі', 5, 'г'),
        ('Піца з шинкою та грибами', 'Цукор', 1, 'ч. л.'),
        ('Піца з шинкою та грибами', 'Сіль', 1, 'ч. л.'),
        ('Піца з шинкою та грибами', 'Оливкова олія', 2, 'ст. л.'),
        ('Піца з шинкою та грибами', 'Помідори', 200, 'г'),
        ('Піца з шинкою та грибами', 'Томатна паста', 1, 'ст. л.'),
        ('Піца з шинкою та грибами', 'Часник', 2, 'зубч.'),
        ('Піца з шинкою та грибами', 'Орегано', 0.5, 'ч. л.'),
        ('Піца з шинкою та грибами', 'Чорний перець', 1, 'дрібка'),
        ('Піца з шинкою та грибами', 'Моцарела', 180, 'г'),
        ('Піца з шинкою та грибами', 'Шинка', 150, 'г'),
        ('Піца з шинкою та грибами', 'Печериці', 150, 'г'),

        ('Часникові булочки', 'Борошно', 400, 'г'),
        ('Часникові булочки', 'Молоко', 200, 'мл'),
        ('Часникові булочки', 'Сухі дріжджі', 7, 'г'),
        ('Часникові булочки', 'Цукор', 1, 'ст. л.'),
        ('Часникові булочки', 'Сіль', 1, 'ч. л.'),
        ('Часникові булочки', 'Яйця', 1, 'шт'),
        ('Часникові булочки', 'Масло', 50, 'г'),
        ('Часникові булочки', 'Соняшникова олія', 1, 'ст. л.'),
        ('Часникові булочки', 'Часник', 4, 'зубч.'),
        ('Часникові булочки', 'Кріп', 20, 'г'),

        ('Сирні мафіни', 'Борошно', 200, 'г'),
        ('Сирні мафіни', 'Твердий сир', 150, 'г'),
        ('Сирні мафіни', 'Кефір', 200, 'мл'),
        ('Сирні мафіни', 'Яйця', 2, 'шт'),
        ('Сирні мафіни', 'Соняшникова олія', 3, 'ст. л.'),
        ('Сирні мафіни', 'Розпушувач', 2, 'ч. л.'),
        ('Сирні мафіни', 'Зелена цибуля', 30, 'г'),
        ('Сирні мафіни', 'Сіль', 0.5, 'ч. л.'),
        ('Сирні мафіни', 'Чорний перець', 2, 'дрібка'),

        ('Фокача', 'Борошно', 350, 'г'),
        ('Фокача', 'Вода', 250, 'мл'),
        ('Фокача', 'Сухі дріжджі', 7, 'г'),
        ('Фокача', 'Цукор', 1, 'ч. л.'),
        ('Фокача', 'Сіль', 1.5, 'ч. л.'),
        ('Фокача', 'Оливкова олія', 4, 'ст. л.'),
        ('Фокача', 'Помідори', 150, 'г'),
        ('Фокача', 'Оливки', 80, 'г'),
        ('Фокача', 'Орегано', 1, 'ч. л.')
    ) AS v(recipe, product, q, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);
            migrationBuilder.Sql("""
    INSERT INTO "Recipes"
    ("Name", "NameEn", "Description", "DescriptionEn",
    "CategoryId", "CookingTime", "Calories", "Servings",
    "Instructions", "InstructionsEn", "ImageUrl")
    SELECT v.uk, v.en, v.d, v.de, c."Id",
           v.t, v.k, v.s, v.i, v.ie, v.img
    FROM (VALUES
    (
        'Чізкейк',
        'Cheesecake',
        'Ніжний вершковий чізкейк на хрусткій основі з печива, прикрашений свіжою полуницею.',
        'Creamy cheesecake with a crunchy biscuit base, topped with fresh strawberries.',
        420, 490, 8,
        '1. Дістаньте вершковий сир, яйця та вершки з холодильника за 30 хвилин до приготування.
    2. Розігрійте духовку до 170 °C. Застеліть дно роз''ємної форми діаметром 20 см пергаментом.
    3. Подрібніть печиво блендером до дрібної крихти.
    4. Розтопіть вершкове масло та змішайте його з крихтою печива.
    5. Викладіть суміш на дно форми, щільно утрамбуйте та випікайте 10 хвилин. Залиште охолоджуватися.
    6. Зменште температуру духовки до 150 °C.
    7. Змішайте вершковий сир із цукром та ванільним цукром до однорідності.
    8. По одному додайте яйця, обережно перемішуючи після кожного. Влийте вершки, додайте крохмаль і лимонний сік.
    9. Вилийте сирну масу на основу та злегка постукайте формою об стіл, щоб видалити великі бульбашки повітря.
    10. Випікайте 60–75 хвилин. Краї мають застигнути, а середина залишатися трохи рухомою.
    11. Вимкніть духовку, прочиніть дверцята та залиште чізкейк усередині на 30 хвилин. Охолодіть до кімнатної температури та поставте в холодильник щонайменше на 4 години.
    12. Помийте й обсушіть полуницю. Прикрасьте охолоджений чізкейк ягодами та наріжте порційними шматочками.',
        '1. Remove the cream cheese, eggs and cream from the refrigerator 30 minutes before cooking.
    2. Preheat the oven to 170 °C. Line the bottom of a 20 cm springform pan with baking parchment.
    3. Crush the biscuits in a blender until fine crumbs form.
    4. Melt the butter and combine it with the biscuit crumbs.
    5. Press the mixture firmly into the bottom of the pan and bake for 10 minutes. Allow to cool.
    6. Reduce the oven temperature to 150 °C.
    7. Mix the cream cheese with sugar and vanilla sugar until smooth.
    8. Add the eggs one at a time, mixing gently after each addition. Add cream, starch and lemon juice.
    9. Pour the filling over the base and gently tap the pan against the counter to remove large air bubbles.
    10. Bake for 60–75 minutes. The edges should be set while the centre remains slightly wobbly.
    11. Turn off the oven, open the door slightly and leave the cheesecake inside for 30 minutes. Cool to room temperature and refrigerate for at least 4 hours.
    12. Wash and dry the strawberries. Decorate the chilled cheesecake with berries and slice into portions.',
        '/images/recipes/cheesecake.webp'
    ),
    (
        'Шоколадний торт',
        'Chocolate cake',
        'Насичений шоколадний торт із м’якими бісквітними коржами та ніжним шоколадним кремом.',
        'Rich chocolate cake with soft sponge layers and smooth chocolate ganache.',
        150, 540, 10,
        '1. Розігрійте духовку до 175 °C. Застеліть дно форми діаметром 20 см пергаментом.
    2. Просійте борошно, какао, розпушувач і харчову соду. Додайте сіль.
    3. В окремій мисці збийте яйця із цукром та ванільним цукром протягом 3–4 хвилин.
    4. Додайте молоко та соняшникову олію, перемішайте до однорідності.
    5. Поступово додайте сухі інгредієнти та перемішайте вінчиком.
    6. Влийте гарячу воду й обережно перемішайте. Тісто має бути досить рідким.
    7. Перелийте тісто у форму та випікайте 40–50 хвилин. Перевірте готовність дерев’яною шпажкою.
    8. Залиште бісквіт у формі на 10 хвилин, потім перекладіть на решітку та повністю охолодіть.
    9. Подрібніть шоколад. Нагрійте вершки майже до кипіння та залийте ними шоколад.
    10. Залиште на 2 хвилини, перемішайте до гладкої консистенції та охолодіть до загустіння.
    11. Розріжте бісквіт горизонтально на два коржі. Змастіть нижній корж частиною шоколадного крему.
    12. Накрийте другим коржем, покрийте верх і боки рештою крему та поставте в холодильник на 30 хвилин.',
        '1. Preheat the oven to 175 °C. Line the bottom of a 20 cm cake pan with baking parchment.
    2. Sift together flour, cocoa powder, baking powder and baking soda. Add salt.
    3. In a separate bowl, beat the eggs with sugar and vanilla sugar for 3–4 minutes.
    4. Add milk and sunflower oil and mix until smooth.
    5. Gradually add the dry ingredients and whisk until combined.
    6. Pour in the hot water and mix gently. The batter should be fairly thin.
    7. Pour the batter into the pan and bake for 40–50 minutes. Check doneness with a wooden skewer.
    8. Leave the sponge in the pan for 10 minutes, then transfer to a wire rack and cool completely.
    9. Chop the chocolate. Heat the cream until almost boiling and pour it over the chocolate.
    10. Leave for 2 minutes, stir until smooth and cool until thickened.
    11. Slice the sponge horizontally into two layers. Spread some chocolate ganache over the bottom layer.
    12. Cover with the second layer, coat the top and sides with the remaining ganache and refrigerate for 30 minutes.',
        '/images/recipes/chocolate-cake.webp'
    ),
    (
        'Яблучна шарлотка',
        'Apple charlotte',
        'Пухкий домашній яблучний пиріг із ніжним бісквітом, корицею та цукровою пудрою.',
        'Fluffy homemade apple cake with tender sponge, cinnamon and powdered sugar.',
        60, 260, 8,
        '1. Розігрійте духовку до 180 °C.
    2. Застеліть дно форми діаметром 22 см пергаментом і змастіть боки вершковим маслом.
    3. Помийте яблука, видаліть серцевини та наріжте тонкими скибочками.
    4. Змішайте яблука з корицею та лимонним соком.
    5. Розбийте яйця у глибоку миску. Додайте цукор, ванільний цукор і дрібку солі.
    6. Збивайте міксером 5–7 хвилин до утворення світлої пишної маси.
    7. Просійте борошно разом із розпушувачем.
    8. Поступово додайте борошно до яєчної маси, обережно перемішуючи лопаткою знизу догори.
    9. Викладіть яблука у форму та рівномірно залийте тістом.
    10. Випікайте 35–40 хвилин до золотистого кольору. Не відкривайте духовку протягом перших 25 хвилин.
    11. Перевірте готовність шпажкою, дістаньте пиріг із духовки та залиште охолоджуватися.
    12. Посипте цукровою пудрою та наріжте порційними шматочками.',
        '1. Preheat the oven to 180 °C.
    2. Line the bottom of a 22 cm cake pan with baking parchment and grease the sides with butter.
    3. Wash the apples, remove the cores and cut them into thin slices.
    4. Toss the apples with cinnamon and lemon juice.
    5. Crack the eggs into a large bowl. Add sugar, vanilla sugar and a pinch of salt.
    6. Beat with a mixer for 5–7 minutes until pale and fluffy.
    7. Sift the flour together with the baking powder.
    8. Gradually add the flour to the egg mixture, gently folding with a spatula.
    9. Arrange the apples in the pan and pour the batter evenly over them.
    10. Bake for 35–40 minutes until golden. Do not open the oven during the first 25 minutes.
    11. Check doneness with a skewer, remove from the oven and allow to cool.
    12. Dust with powdered sugar and slice into portions.',
        '/images/recipes/apple-charlotte.webp'
    ),
    (
        'Медовик',
        'Honey cake',
        'Традиційний багатошаровий медовий торт із тонкими коржами та ніжним сметанним кремом.',
        'Traditional layered honey cake with thin honey sponge layers and smooth sour cream frosting.',
        600, 520, 10,
        '1. Підготуйте водяну баню, наливши воду в каструлю та довівши її до слабкого кипіння.
    2. У жаростійкій мисці змішайте мед, вершкове масло та цукор.
    3. Поставте миску на водяну баню та перемішуйте до розчинення цукру й масла.
    4. Додайте харчову соду. Перемішуйте 1–2 хвилини, доки суміш не збільшиться в об’ємі.
    5. Зніміть миску з водяної бані та залиште суміш охолоджуватися на 5–10 хвилин.
    6. По одному додайте яйця, швидко перемішуючи вінчиком.
    7. Поступово додайте просіяне борошно та замісіть м’яке тісто. Залиште трохи борошна для розкачування.
    8. Розділіть тісто на 8 частин. Розкачайте кожну на пергаменті у тонкий круг діаметром приблизно 20 см.
    9. Випікайте кожен корж при 180 °C протягом 5–7 хвилин до золотистого кольору. Обріжте краї за шаблоном і збережіть обрізки.
    10. Змішайте густу охолоджену сметану із цукровою пудрою та ванільним цукром до однорідності.
    11. Почергово викладайте коржі, змащуючи кожен кремом. Покрийте кремом верх і боки торта, а обрізки подрібніть та використайте для обсипання.
    12. Поставте торт у холодильник щонайменше на 8 годин, щоб коржі просочилися та стали м’якими.',
        '1. Prepare a double boiler by pouring water into a saucepan and bringing it to a gentle simmer.
    2. Combine honey, butter and sugar in a heatproof bowl.
    3. Place the bowl over the simmering water and stir until the sugar and butter dissolve.
    4. Add baking soda. Stir for 1–2 minutes until the mixture increases in volume.
    5. Remove the bowl from the double boiler and allow the mixture to cool for 5–10 minutes.
    6. Add the eggs one at a time, whisking quickly.
    7. Gradually add sifted flour and knead into a soft dough. Reserve some flour for rolling.
    8. Divide the dough into 8 portions. Roll each portion on baking parchment into a thin circle approximately 20 cm wide.
    9. Bake each layer at 180 °C for 5–7 minutes until golden. Trim the edges using a template and reserve the trimmings.
    10. Mix thick chilled sour cream with powdered sugar and vanilla sugar until smooth.
    11. Stack the layers, spreading frosting between each one. Cover the top and sides with frosting, then crush the trimmings and sprinkle the crumbs over the cake.
    12. Refrigerate for at least 8 hours so the layers absorb the frosting and become soft.',
        '/images/recipes/honey-cake.webp'
    ),
    (
        'Брауні',
        'Chocolate brownies',
        'Насичений шоколадний десерт із тонкою хрусткою скоринкою, вологою серединою та волоськими горіхами.',
        'Rich chocolate brownies with a thin crispy crust, fudgy centre and walnuts.',
        60, 330, 9,
        '1. Розігрійте духовку до 175 °C.
    2. Застеліть квадратну форму розміром 20 на 20 см пергаментом.
    3. Поламайте шоколад на невеликі шматочки та покладіть у жаростійку миску разом із вершковим маслом.
    4. Розтопіть шоколад і масло в мікрохвильовій печі короткими інтервалами по 20 секунд, перемішуючи після кожного нагрівання.
    5. Залиште шоколадну суміш охолоджуватися протягом 5 хвилин.
    6. В окремій мисці збийте яйця із цукром та ванільним цукром протягом 2–3 хвилин.
    7. Додайте шоколадну суміш до яєчної маси та перемішайте.
    8. Просійте борошно, какао та сіль. Додайте до рідкої суміші й обережно перемішайте лопаткою.
    9. Подрібніть волоські горіхи та додайте до тіста.
    10. Перекладіть тісто у форму, розрівняйте поверхню та випікайте 22–28 хвилин.
    11. Перевірте готовність шпажкою: на ній мають залишатися вологі крихти, але не сире тісто.
    12. Охолодіть брауні у формі та наріжте на 9 квадратних шматочків.',
        '1. Preheat the oven to 175 °C.
    2. Line a 20 by 20 cm square baking pan with parchment.
    3. Break the chocolate into small pieces and place in a heatproof bowl with the butter.
    4. Melt the chocolate and butter in the microwave in short 20-second intervals, stirring after each interval.
    5. Allow the chocolate mixture to cool for 5 minutes.
    6. In a separate bowl, beat the eggs with sugar and vanilla sugar for 2–3 minutes.
    7. Add the chocolate mixture to the eggs and stir until combined.
    8. Sift together flour, cocoa powder and salt. Add to the wet mixture and gently fold with a spatula.
    9. Chop the walnuts and fold them into the batter.
    10. Transfer the batter to the pan, smooth the surface and bake for 22–28 minutes.
    11. Check with a skewer: it should have moist crumbs attached but no raw batter.
    12. Cool the brownies in the pan and cut into 9 squares.',
        '/images/recipes/brownies.webp'
    ),
    (
        'Бананові мафіни',
        'Banana muffins',
        'М’які ароматні мафіни зі стиглими бананами, корицею та шматочками шоколаду.',
        'Soft and fragrant muffins with ripe bananas, cinnamon and chocolate chunks.',
        40, 240, 10,
        '1. Розігрійте духовку до 180 °C.
    2. Підготуйте форму для 10 мафінів і викладіть у заглиблення паперові формочки.
    3. Очистьте стиглі банани та розімніть виделкою до консистенції пюре.
    4. Розтопіть вершкове масло та залиште його трохи охолодитися.
    5. В окремій мисці змішайте яйця, цукор, ванільний цукор, молоко та розтоплене масло.
    6. Додайте бананове пюре та перемішайте до однорідності.
    7. Просійте борошно, розпушувач, харчову соду, корицю та сіль.
    8. Додайте сухі інгредієнти до бананової суміші та обережно перемішайте до зникнення сухого борошна.
    9. Наріжте шоколад невеликими шматочками. Більшу частину додайте до тіста.
    10. Розподіліть тісто між формочками, заповнюючи їх на три чверті. Посипте рештою шоколаду.
    11. Випікайте 20–25 хвилин до золотистого кольору. Перевірте готовність дерев’яною шпажкою.
    12. Залиште мафіни у формі на 5 хвилин, після чого перекладіть на решітку та охолодіть.',
        '1. Preheat the oven to 180 °C.
    2. Prepare a 10-cup muffin tin and line the cups with paper liners.
    3. Peel the ripe bananas and mash them with a fork until smooth.
    4. Melt the butter and allow it to cool slightly.
    5. In a separate bowl, combine eggs, sugar, vanilla sugar, milk and melted butter.
    6. Add the mashed bananas and mix until combined.
    7. Sift together flour, baking powder, baking soda, cinnamon and salt.
    8. Add the dry ingredients to the banana mixture and gently stir until no dry flour remains.
    9. Chop the chocolate into small pieces. Fold most of it into the batter.
    10. Divide the batter between the muffin cups, filling each three-quarters full. Sprinkle with the remaining chocolate.
    11. Bake for 20–25 minutes until golden. Check doneness with a wooden skewer.
    12. Leave the muffins in the tin for 5 minutes, then transfer to a wire rack and allow to cool.',
        '/images/recipes/banana-muffins.webp'
    )
    ) AS v(uk, en, d, de, t, k, s, i, ie, img)
    JOIN "RecipeCategories" c ON c."Name" = 'Десерти';
    """);
            migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.q, u."Id"
    FROM (VALUES
        ('Чізкейк', 'Печиво', 200, 'г'),
        ('Чізкейк', 'Масло', 80, 'г'),
        ('Чізкейк', 'Вершковий сир', 600, 'г'),
        ('Чізкейк', 'Вершки', 150, 'мл'),
        ('Чізкейк', 'Яйця', 3, 'шт'),
        ('Чізкейк', 'Цукор', 150, 'г'),
        ('Чізкейк', 'Ванільний цукор', 10, 'г'),
        ('Чізкейк', 'Крохмаль', 20, 'г'),
        ('Чізкейк', 'Лимони', 0.5, 'шт'),
        ('Чізкейк', 'Полуниця', 150, 'г'),

        ('Шоколадний торт', 'Борошно', 250, 'г'),
        ('Шоколадний торт', 'Какао', 70, 'г'),
        ('Шоколадний торт', 'Цукор', 250, 'г'),
        ('Шоколадний торт', 'Яйця', 3, 'шт'),
        ('Шоколадний торт', 'Молоко', 200, 'мл'),
        ('Шоколадний торт', 'Соняшникова олія', 100, 'мл'),
        ('Шоколадний торт', 'Вода', 150, 'мл'),
        ('Шоколадний торт', 'Розпушувач', 1.5, 'ч. л.'),
        ('Шоколадний торт', 'Харчова сода', 0.5, 'ч. л.'),
        ('Шоколадний торт', 'Ванільний цукор', 10, 'г'),
        ('Шоколадний торт', 'Сіль', 1, 'дрібка'),
        ('Шоколадний торт', 'Шоколад', 250, 'г'),
        ('Шоколадний торт', 'Вершки', 250, 'мл'),

        ('Яблучна шарлотка', 'Яблука', 500, 'г'),
        ('Яблучна шарлотка', 'Борошно', 180, 'г'),
        ('Яблучна шарлотка', 'Яйця', 4, 'шт'),
        ('Яблучна шарлотка', 'Цукор', 150, 'г'),
        ('Яблучна шарлотка', 'Ванільний цукор', 10, 'г'),
        ('Яблучна шарлотка', 'Розпушувач', 1, 'ч. л.'),
        ('Яблучна шарлотка', 'Кориця', 1, 'ч. л.'),
        ('Яблучна шарлотка', 'Лимони', 0.5, 'шт'),
        ('Яблучна шарлотка', 'Масло', 10, 'г'),
        ('Яблучна шарлотка', 'Сіль', 1, 'дрібка'),
        ('Яблучна шарлотка', 'Цукрова пудра', 20, 'г'),

        ('Медовик', 'Борошно', 450, 'г'),
        ('Медовик', 'Мед', 100, 'г'),
        ('Медовик', 'Масло', 100, 'г'),
        ('Медовик', 'Цукор', 150, 'г'),
        ('Медовик', 'Яйця', 2, 'шт'),
        ('Медовик', 'Харчова сода', 1, 'ч. л.'),
        ('Медовик', 'Сметана', 800, 'г'),
        ('Медовик', 'Цукрова пудра', 150, 'г'),
        ('Медовик', 'Ванільний цукор', 10, 'г'),
        ('Медовик', 'Вода', 500, 'мл'),

        ('Брауні', 'Шоколад', 180, 'г'),
        ('Брауні', 'Масло', 120, 'г'),
        ('Брауні', 'Яйця', 3, 'шт'),
        ('Брауні', 'Цукор', 150, 'г'),
        ('Брауні', 'Борошно', 90, 'г'),
        ('Брауні', 'Какао', 25, 'г'),
        ('Брауні', 'Ванільний цукор', 10, 'г'),
        ('Брауні', 'Сіль', 1, 'дрібка'),
        ('Брауні', 'Волоські горіхи', 60, 'г'),

        ('Бананові мафіни', 'Банани', 3, 'шт'),
        ('Бананові мафіни', 'Борошно', 250, 'г'),
        ('Бананові мафіни', 'Яйця', 2, 'шт'),
        ('Бананові мафіни', 'Масло', 80, 'г'),
        ('Бананові мафіни', 'Молоко', 80, 'мл'),
        ('Бананові мафіни', 'Цукор', 100, 'г'),
        ('Бананові мафіни', 'Ванільний цукор', 10, 'г'),
        ('Бананові мафіни', 'Розпушувач', 1.5, 'ч. л.'),
        ('Бананові мафіни', 'Харчова сода', 0.5, 'ч. л.'),
        ('Бананові мафіни', 'Кориця', 1, 'ч. л.'),
        ('Бананові мафіни', 'Сіль', 1, 'дрібка'),
        ('Бананові мафіни', 'Шоколад', 100, 'г')
    ) AS v(recipe, product, q, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);

            migrationBuilder.Sql("""
    INSERT INTO "Recipes"
    ("Name", "NameEn", "Description", "DescriptionEn",
    "CategoryId", "CookingTime", "Calories", "Servings",
    "Instructions", "InstructionsEn", "ImageUrl")
    SELECT v.uk, v.en, v.d, v.de, c."Id",
           v.t, v.k, v.s, v.i, v.ie, v.img
    FROM (VALUES
    (
        'Брускети з помідорами',
        'Tomato bruschetta',
        'Хрусткі скибочки багета зі свіжими помідорами, часником, базиліком та оливковою олією.',
        'Crispy baguette slices topped with fresh tomatoes, garlic, basil and olive oil.',
        20, 260, 3,
        '1. Розігрійте духовку до 200 °C.
    2. Наріжте багет скибочками завтовшки приблизно 1,5–2 см.
    3. Викладіть скибочки на деко та змастіть половиною оливкової олії.
    4. Запікайте 5–7 хвилин до золотистої хрусткої скоринки.
    5. Помийте помідори, видаліть тверді частини та наріжте м’якоть невеликими кубиками.
    6. Помийте базилік, обсушіть і дрібно наріжте.
    7. Очистьте часник. Один зубчик подрібніть, а другим натріть теплі скибочки багета.
    8. Змішайте помідори, подрібнений часник, базилік, решту оливкової олії, сіль і чорний перець.
    9. Залиште начинку на 5 хвилин, щоб поєдналися аромати.
    10. Викладіть помідорову суміш на підсмажений багет безпосередньо перед подаванням.',
        '1. Preheat the oven to 200 °C.
    2. Cut the baguette into slices approximately 1.5–2 cm thick.
    3. Arrange the slices on a baking tray and brush with half the olive oil.
    4. Bake for 5–7 minutes until golden and crispy.
    5. Wash the tomatoes, remove the tough cores and dice the flesh.
    6. Wash, dry and finely chop the basil.
    7. Peel the garlic. Mince one clove and rub the warm baguette slices with the other.
    8. Combine the tomatoes, minced garlic, basil, remaining olive oil, salt and black pepper.
    9. Let the topping rest for 5 minutes to allow the flavours to combine.
    10. Spoon the tomato mixture onto the toasted baguette just before serving.',
        '/images/recipes/tomato-bruschetta.webp'
    ),
    (
        'Фаршировані яйця',
        'Deviled eggs',
        'Ніжні варені яйця з кремовою начинкою із жовтків, майонезу, гірчиці та зелені.',
        'Hard-boiled eggs filled with a creamy mixture of yolks, mayonnaise, mustard and herbs.',
        30, 190, 3,
        '1. Покладіть яйця в каструлю та залийте холодною водою.
    2. Доведіть воду до кипіння на середньому вогні.
    3. Варіть яйця 10–11 хвилин після закипання.
    4. Злийте гарячу воду та охолодіть яйця під холодною проточною водою.
    5. Очистьте яйця від шкаралупи та розріжте кожне вздовж навпіл.
    6. Обережно вийміть жовтки та перекладіть їх в окрему миску.
    7. Розімніть жовтки виделкою, додайте майонез, гірчицю, сіль і чорний перець.
    8. Ретельно перемішайте до однорідної кремової консистенції.
    9. Наповніть половинки білків начинкою за допомогою ложки або кондитерського мішка.
    10. Посипте паприкою та подрібненим кропом.
    11. Поставте в холодильник на 5–10 хвилин і подавайте охолодженими.',
        '1. Place the eggs in a saucepan and cover with cold water.
    2. Bring the water to a boil over medium heat.
    3. Boil the eggs for 10–11 minutes.
    4. Drain the hot water and cool the eggs under cold running water.
    5. Peel the eggs and cut each one in half lengthwise.
    6. Carefully remove the yolks and transfer them to a separate bowl.
    7. Mash the yolks with a fork and add mayonnaise, mustard, salt and black pepper.
    8. Mix thoroughly until smooth and creamy.
    9. Fill the egg white halves using a spoon or piping bag.
    10. Sprinkle with paprika and chopped dill.
    11. Refrigerate for 5–10 minutes and serve chilled.',
        '/images/recipes/deviled-eggs.webp'
    ),
    (
        'Хумус',
        'Hummus',
        'Кремова близькосхідна закуска з нуту, тахіні, лимонного соку, часнику та оливкової олії.',
        'Creamy Middle Eastern dip made with chickpeas, tahini, lemon juice, garlic and olive oil.',
        660, 280, 4,
        '1. Переберіть сухий нут і ретельно промийте під проточною водою.
    2. Залийте нут одним літром холодної води та залиште в холодильнику на 8–10 годин.
    3. Злийте воду після замочування та повторно промийте нут.
    4. Перекладіть нут у каструлю, залийте одним літром свіжої води та доведіть до кипіння.
    5. Зменште вогонь і варіть 60–90 хвилин, доки нут не стане дуже м’яким.
    6. Злийте воду, залишивши приблизно 100 мл відвару для регулювання консистенції.
    7. Очистьте часник і вичавіть сік із лимона.
    8. Покладіть варений нут, тахіні, часник, лимонний сік, кумин, сіль і половину оливкової олії в чашу блендера.
    9. Подрібнюйте 2–3 хвилини, поступово додаючи відвар нуту до отримання гладкої кремової консистенції.
    10. Перекладіть хумус у неглибоку миску та зробіть ложкою невелике заглиблення.
    11. Полийте рештою оливкової олії, посипте паприкою та подрібненою петрушкою.
    12. Подавайте охолодженим або кімнатної температури.',
        '1. Sort the dried chickpeas and rinse thoroughly under running water.
    2. Cover the chickpeas with one litre of cold water and refrigerate for 8–10 hours.
    3. Drain the soaking water and rinse the chickpeas again.
    4. Transfer the chickpeas to a saucepan, cover with one litre of fresh water and bring to a boil.
    5. Reduce the heat and simmer for 60–90 minutes until the chickpeas are very soft.
    6. Drain, reserving approximately 100 ml of cooking liquid to adjust the consistency.
    7. Peel the garlic and squeeze the juice from the lemon.
    8. Place the cooked chickpeas, tahini, garlic, lemon juice, cumin, salt and half the olive oil in a blender.
    9. Blend for 2–3 minutes, gradually adding the reserved cooking liquid until smooth and creamy.
    10. Transfer the hummus to a shallow bowl and make a small indentation with a spoon.
    11. Drizzle with the remaining olive oil and sprinkle with paprika and chopped parsley.
    12. Serve chilled or at room temperature.',
        '/images/recipes/hummus.webp'
    ),
    (
        'Сирна закуска з часником',
        'Garlic cheese spread',
        'Ніжна сирна закуска з часником, майонезом та свіжим кропом, подана на хрустких скибочках багета.',
        'Creamy garlic cheese spread with mayonnaise and fresh dill, served on crispy baguette slices.',
        20, 320, 4,
        '1. Розігрійте духовку до 190 °C.
    2. Наріжте багет скибочками завтовшки приблизно 1,5 см.
    3. Викладіть скибочки на деко та підсушіть у духовці протягом 5–7 хвилин.
    4. Натріть твердий сир на дрібній тертці.
    5. Очистьте часник і пропустіть його через прес або дрібно подрібніть ножем.
    6. Помийте кріп, обсушіть і дрібно наріжте.
    7. З’єднайте натертий сир, вершковий сир, майонез і часник у мисці.
    8. Додайте чорний перець та більшу частину кропу.
    9. Ретельно перемішайте до утворення густої однорідної маси.
    10. Нанесіть сирну суміш на охолоджені скибочки багета.
    11. Прикрасьте рештою кропу та подавайте.',
        '1. Preheat the oven to 190 °C.
    2. Cut the baguette into slices approximately 1.5 cm thick.
    3. Arrange the slices on a baking tray and toast for 5–7 minutes.
    4. Finely grate the hard cheese.
    5. Peel the garlic and crush it with a garlic press or finely mince it.
    6. Wash, dry and finely chop the dill.
    7. Combine the grated cheese, cream cheese, mayonnaise and garlic in a bowl.
    8. Add black pepper and most of the dill.
    9. Mix thoroughly until a thick, uniform spread forms.
    10. Spread the cheese mixture over the cooled baguette slices.
    11. Garnish with the remaining dill and serve.',
        '/images/recipes/garlic-cheese-spread.webp'
    ),
    (
        'Рулетики з баклажанів',
        'Eggplant rolls',
        'Ніжні рулетики із запечених баклажанів із вершково-сирною начинкою, часником, волоськими горіхами та зеленню.',
        'Tender roasted eggplant rolls filled with cream cheese, garlic, walnuts and fresh herbs.',
        45, 290, 4,
        '1. Розігрійте духовку до 200 °C.
    2. Помийте баклажани, зріжте плодоніжки та наріжте вздовж пластинками завтовшки 4–5 мм.
    3. Посипте пластинки половиною солі та залиште на 10 хвилин.
    4. Промокніть поверхню баклажанів паперовим рушником, щоб прибрати зайву вологу.
    5. Застеліть деко пергаментом, викладіть баклажани одним шаром та змастіть оливковою олією з обох боків.
    6. Запікайте 15–20 хвилин до м’якості, перевернувши пластинки приблизно посередині приготування.
    7. Дістаньте баклажани з духовки та залиште охолоджуватися.
    8. Очистьте й подрібніть часник. Дрібно наріжте волоські горіхи та петрушку.
    9. Змішайте вершковий сир, сметану, часник, горіхи, більшу частину петрушки, решту солі та чорний перець.
    10. Викладіть приблизно одну столову ложку начинки на край кожної пластинки баклажана.
    11. Обережно загорніть пластинки в рулетики та викладіть на тарілку швом донизу.
    12. Прикрасьте рештою петрушки та подавайте охолодженими.',
        '1. Preheat the oven to 200 °C.
    2. Wash the eggplants, remove the stems and slice lengthwise into 4–5 mm thick strips.
    3. Sprinkle the slices with half the salt and leave for 10 minutes.
    4. Pat the eggplant slices dry with a paper towel to remove excess moisture.
    5. Line a baking tray with parchment, arrange the eggplant in a single layer and brush both sides with olive oil.
    6. Bake for 15–20 minutes until tender, turning the slices halfway through cooking.
    7. Remove the eggplant from the oven and allow to cool.
    8. Peel and mince the garlic. Finely chop the walnuts and parsley.
    9. Combine cream cheese, sour cream, garlic, walnuts, most of the parsley, remaining salt and black pepper.
    10. Place approximately one tablespoon of filling at the end of each eggplant slice.
    11. Carefully roll up the slices and arrange them seam-side down on a serving plate.
    12. Garnish with the remaining parsley and serve chilled.',
        '/images/recipes/eggplant-rolls.webp'
    )
    ) AS v(uk, en, d, de, t, k, s, i, ie, img)
    JOIN "RecipeCategories" c ON c."Name" = 'Закуски';
    """);
            migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.q, u."Id"
    FROM (VALUES
        ('Брускети з помідорами', 'Багет', 180, 'г'),
        ('Брускети з помідорами', 'Помідори', 300, 'г'),
        ('Брускети з помідорами', 'Часник', 2, 'зубч.'),
        ('Брускети з помідорами', 'Базилік', 15, 'г'),
        ('Брускети з помідорами', 'Оливкова олія', 2, 'ст. л.'),
        ('Брускети з помідорами', 'Сіль', 0.25, 'ч. л.'),
        ('Брускети з помідорами', 'Чорний перець', 2, 'дрібка'),

        ('Фаршировані яйця', 'Яйця', 6, 'шт'),
        ('Фаршировані яйця', 'Майонез', 30, 'г'),
        ('Фаршировані яйця', 'Гірчиця', 1, 'ч. л.'),
        ('Фаршировані яйця', 'Вода', 1, 'л'),
        ('Фаршировані яйця', 'Сіль', 1, 'дрібка'),
        ('Фаршировані яйця', 'Чорний перець', 1, 'дрібка'),
        ('Фаршировані яйця', 'Паприка', 0.5, 'ч. л.'),
        ('Фаршировані яйця', 'Кріп', 10, 'г'),

        ('Хумус', 'Нут', 200, 'г'),
        ('Хумус', 'Тахіні', 40, 'г'),
        ('Хумус', 'Лимони', 1, 'шт'),
        ('Хумус', 'Часник', 2, 'зубч.'),
        ('Хумус', 'Оливкова олія', 2, 'ст. л.'),
        ('Хумус', 'Кумин (зіра)', 0.5, 'ч. л.'),
        ('Хумус', 'Сіль', 0.75, 'ч. л.'),
        ('Хумус', 'Вода', 2, 'л'),
        ('Хумус', 'Паприка', 0.5, 'ч. л.'),
        ('Хумус', 'Петрушка', 10, 'г'),

        ('Сирна закуска з часником', 'Твердий сир', 150, 'г'),
        ('Сирна закуска з часником', 'Вершковий сир', 80, 'г'),
        ('Сирна закуска з часником', 'Майонез', 30, 'г'),
        ('Сирна закуска з часником', 'Часник', 2, 'зубч.'),
        ('Сирна закуска з часником', 'Кріп', 15, 'г'),
        ('Сирна закуска з часником', 'Багет', 160, 'г'),
        ('Сирна закуска з часником', 'Чорний перець', 1, 'дрібка'),

        ('Рулетики з баклажанів', 'Баклажани', 500, 'г'),
        ('Рулетики з баклажанів', 'Вершковий сир', 200, 'г'),
        ('Рулетики з баклажанів', 'Сметана', 40, 'г'),
        ('Рулетики з баклажанів', 'Волоські горіхи', 50, 'г'),
        ('Рулетики з баклажанів', 'Часник', 2, 'зубч.'),
        ('Рулетики з баклажанів', 'Оливкова олія', 2, 'ст. л.'),
        ('Рулетики з баклажанів', 'Петрушка', 15, 'г'),
        ('Рулетики з баклажанів', 'Сіль', 0.5, 'ч. л.'),
        ('Рулетики з баклажанів', 'Чорний перець', 2, 'дрібка')
    ) AS v(recipe, product, q, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);
            migrationBuilder.Sql("""
    INSERT INTO "Recipes"
    ("Name", "NameEn", "Description", "DescriptionEn",
    "CategoryId", "CookingTime", "Calories", "Servings",
    "Instructions", "InstructionsEn", "ImageUrl")
    SELECT v.uk, v.en, v.d, v.de, c."Id",
           v.t, v.k, v.s, v.i, v.ie, v.img
    FROM (VALUES
    (
        'Банановий смузі',
        'Banana smoothie',
        'Ніжний поживний смузі зі стиглими бананами, молоком, вівсянкою, медом та корицею.',
        'Creamy and nutritious smoothie with ripe bananas, milk, oats, honey and cinnamon.',
        15, 290, 2,
        '1. Очистьте стиглі банани та наріжте невеликими кружальцями.
    2. Відміряйте необхідну кількість вівсянки та перекладіть її в чашу блендера.
    3. Додайте половину молока та залиште на 5 хвилин, щоб вівсянка трохи розм’якла.
    4. Додайте нарізані банани, мед і решту молока.
    5. Збивайте на високій швидкості протягом 1–2 хвилин до однорідної кремової консистенції.
    6. Додайте більшу частину кориці та збивайте ще 10 секунд.
    7. Розлийте смузі у дві склянки.
    8. Посипте рештою кориці та подавайте одразу після приготування.',
        '1. Peel the ripe bananas and cut them into small slices.
    2. Measure the oats and transfer them to a blender.
    3. Add half the milk and leave for 5 minutes to soften the oats.
    4. Add the sliced bananas, honey and remaining milk.
    5. Blend on high speed for 1–2 minutes until smooth and creamy.
    6. Add most of the cinnamon and blend for another 10 seconds.
    7. Pour the smoothie into two glasses.
    8. Sprinkle with the remaining cinnamon and serve immediately.',
        '/images/recipes/banana-smoothie.webp'
    ),
    (
        'Полуничний молочний коктейль',
        'Strawberry milkshake',
        'Освіжаючий молочний коктейль зі свіжою полуницею, вершковим морозивом та ванільним ароматом.',
        'Refreshing milkshake with fresh strawberries, creamy ice cream and vanilla flavour.',
        10, 310, 2,
        '1. Ретельно помийте полуницю під прохолодною проточною водою.
    2. Видаліть зелені плодоніжки та розріжте великі ягоди навпіл.
    3. Відкладіть дві ягоди для прикрашання готового напою.
    4. Перекладіть решту полуниці в чашу блендера.
    5. Додайте охолоджене молоко, вершкове морозиво та ванільний цукор.
    6. Збивайте протягом 45–60 секунд до однорідної консистенції та утворення легкої піни.
    7. Розлийте коктейль у дві високі склянки.
    8. Прикрасьте кожну склянку полуницею та подавайте одразу, поки коктейль холодний.',
        '1. Thoroughly wash the strawberries under cool running water.
    2. Remove the green stems and cut large berries in half.
    3. Reserve two strawberries for garnishing the finished drink.
    4. Transfer the remaining strawberries to a blender.
    5. Add chilled milk, creamy ice cream and vanilla sugar.
    6. Blend for 45–60 seconds until smooth and slightly frothy.
    7. Pour the milkshake into two tall glasses.
    8. Garnish each glass with a strawberry and serve immediately while cold.',
        '/images/recipes/strawberry-milkshake.webp'
    ),
    (
        'Домашній лимонад',
        'Homemade lemonade',
        'Освіжаючий домашній напій із натуральним лимонним соком, свіжою м’ятою та легкою солодкістю.',
        'Refreshing homemade drink with natural lemon juice, fresh mint and a touch of sweetness.',
        40, 80, 4,
        '1. Ретельно помийте лимони та листя м’яти.
    2. Налийте 200 мл води в невелику каструлю та додайте цукор.
    3. Нагрівайте на середньому вогні, постійно перемішуючи, доки цукор повністю не розчиниться.
    4. Зніміть цукровий сироп із вогню та залиште охолоджуватися.
    5. Розріжте два лимони навпіл і вичавіть із них сік.
    6. Процідіть лимонний сік через ситечко, щоб видалити кісточки.
    7. Наріжте третій лимон тонкими кружальцями.
    8. Злегка розімніть листя м’яти ложкою, щоб воно виділило аромат.
    9. У великому глечику змішайте лимонний сік, охолоджений сироп і решту холодної води.
    10. Додайте кружальця лимона та м’яту, ретельно перемішайте.
    11. Поставте лимонад у холодильник на 20 хвилин.
    12. Розлийте напій у склянки та подавайте охолодженим.',
        '1. Thoroughly wash the lemons and mint leaves.
    2. Pour 200 ml of water into a small saucepan and add the sugar.
    3. Heat over medium heat, stirring constantly until the sugar dissolves completely.
    4. Remove the sugar syrup from the heat and allow it to cool.
    5. Cut two lemons in half and squeeze out their juice.
    6. Strain the lemon juice through a sieve to remove the seeds.
    7. Cut the third lemon into thin slices.
    8. Gently crush the mint leaves with a spoon to release their aroma.
    9. In a large pitcher, combine lemon juice, cooled syrup and the remaining cold water.
    10. Add the lemon slices and mint, and stir thoroughly.
    11. Refrigerate the lemonade for 20 minutes.
    12. Pour into glasses and serve chilled.',
        '/images/recipes/homemade-lemonade.webp'
    ),
    (
        'Ягідний смузі',
        'Berry smoothie',
        'Яскравий освіжаючий смузі з полуницею, малиною, чорницею, бананом та натуральним йогуртом.',
        'Refreshing and colourful smoothie with strawberries, raspberries, blueberries, banana and natural yogurt.',
        10, 210, 2,
        '1. Ретельно переберіть полуницю, малину та чорницю.
    2. Обережно промийте ягоди прохолодною водою та видаліть плодоніжки полуниці.
    3. Відкладіть кілька ягід для прикрашання готового напою.
    4. Очистьте банан та наріжте невеликими шматочками.
    5. Перекладіть ягоди й банан у чашу блендера.
    6. Додайте натуральний йогурт, охолоджене молоко та мед.
    7. Збивайте на високій швидкості протягом 1–2 хвилин до однорідної консистенції.
    8. Розлийте смузі у дві склянки.
    9. Прикрасьте відкладеними ягодами та свіжою м’ятою.
    10. Подавайте одразу після приготування.',
        '1. Carefully sort the strawberries, raspberries and blueberries.
    2. Gently rinse the berries with cool water and remove the strawberry stems.
    3. Reserve a few berries for garnishing the finished drink.
    4. Peel the banana and cut it into small pieces.
    5. Transfer the berries and banana to a blender.
    6. Add natural yogurt, chilled milk and honey.
    7. Blend on high speed for 1–2 minutes until smooth.
    8. Pour the smoothie into two glasses.
    9. Garnish with the reserved berries and fresh mint.
    10. Serve immediately after preparation.',
        '/images/recipes/berry-smoothie.webp'
    )
    ) AS v(uk, en, d, de, t, k, s, i, ie, img)
    JOIN "RecipeCategories" c ON c."Name" = 'Напої';
    """);
            migrationBuilder.Sql("""
    INSERT INTO "RecipeIngredients"
        ("RecipeId", "ProductId", "Quantity", "UnitId")
    SELECT r."Id", p."Id", v.q, u."Id"
    FROM (VALUES
        ('Банановий смузі', 'Банани', 2, 'шт'),
        ('Банановий смузі', 'Молоко', 300, 'мл'),
        ('Банановий смузі', 'Вівсянка', 40, 'г'),
        ('Банановий смузі', 'Мед', 1, 'ст. л.'),
        ('Банановий смузі', 'Кориця', 0.5, 'ч. л.'),

        ('Полуничний молочний коктейль', 'Полуниця', 200, 'г'),
        ('Полуничний молочний коктейль', 'Молоко', 300, 'мл'),
        ('Полуничний молочний коктейль', 'Морозиво', 200, 'г'),
        ('Полуничний молочний коктейль', 'Ванільний цукор', 10, 'г'),

        ('Домашній лимонад', 'Лимони', 3, 'шт'),
        ('Домашній лимонад', 'Вода', 1, 'л'),
        ('Домашній лимонад', 'Цукор', 70, 'г'),
        ('Домашній лимонад', 'М''ята', 15, 'г'),

        ('Ягідний смузі', 'Полуниця', 100, 'г'),
        ('Ягідний смузі', 'Малина', 80, 'г'),
        ('Ягідний смузі', 'Чорниця', 80, 'г'),
        ('Ягідний смузі', 'Банани', 1, 'шт'),
        ('Ягідний смузі', 'Йогурт', 150, 'г'),
        ('Ягідний смузі', 'Молоко', 100, 'мл'),
        ('Ягідний смузі', 'Мед', 1, 'ст. л.'),
        ('Ягідний смузі', 'М''ята', 5, 'г')
    ) AS v(recipe, product, q, unit)
    JOIN "Recipes" r ON r."Name" = v.recipe
    JOIN "Products" p ON p."Name" = v.product
    JOIN "Units" u ON u."ShortName" = v.unit;
    """);
            migrationBuilder.Sql("""
    UPDATE "Recipes" AS r
    SET
        "NameEn" = v.en,
        "DescriptionEn" = v.de,
        "Instructions" = v.i,
        "InstructionsEn" = v.ie,
        "ImageUrl" = v.img
    FROM (VALUES
    (
        'Паста з куркою',
        'Chicken pasta',
        'Pasta with chicken breast in a creamy cheese sauce.',
        '1. Налийте 2 л води в каструлю та доведіть до кипіння.
    2. Додайте половину солі та відваріть макарони відповідно до інструкції на упаковці.
    3. Обсушіть куряче філе паперовим рушником і наріжте кубиками приблизно 2 см.
    4. Натріть сир на дрібній тертці.
    5. Розігрійте соняшникову олію на пательні на середньому вогні.
    6. Додайте курку, решту солі та чорний перець.
    7. Обсмажуйте 8–10 хвилин, періодично перемішуючи, до внутрішньої температури 74 °C.
    8. Зменште вогонь, влийте молоко та додайте натертий сир.
    9. Перемішуйте соус протягом 3–4 хвилин, доки сир не розплавиться.
    10. Злийте воду з макаронів і додайте їх до курки із соусом.
    11. Ретельно перемішайте, прогрійте ще 1–2 хвилини та подавайте гарячою.',
        '1. Pour 2 litres of water into a saucepan and bring to a boil.
    2. Add half the salt and cook the pasta according to the package instructions.
    3. Pat the chicken breast dry and cut it into approximately 2 cm cubes.
    4. Finely grate the cheese.
    5. Heat sunflower oil in a frying pan over medium heat.
    6. Add the chicken, remaining salt and black pepper.
    7. Fry for 8–10 minutes, stirring occasionally, until the internal temperature reaches 74 °C.
    8. Reduce the heat, pour in the milk and add the grated cheese.
    9. Stir the sauce for 3–4 minutes until the cheese melts.
    10. Drain the pasta and add it to the chicken and sauce.
    11. Mix thoroughly, heat for another 1–2 minutes and serve hot.',
        '/images/recipes/chicken-pasta.webp'
    ),
    (
        'Томатний суп',
        'Tomato soup',
        'Aromatic vegetable soup with tomatoes, carrots and onion.',
        '1. Ретельно помийте помідори та моркву.
    2. Очистьте цибулю й моркву.
    3. Наріжте цибулю невеликими кубиками, а моркву натріть на великій тертці.
    4. Наріжте помідори невеликими шматочками.
    5. Розігрійте соняшникову олію в каструлі з товстим дном.
    6. Обсмажуйте цибулю на середньому вогні протягом 3 хвилин.
    7. Додайте моркву та готуйте ще 4–5 хвилин.
    8. Додайте помідори та тушкуйте 5–7 хвилин, періодично перемішуючи.
    9. Влийте 500 мл води, додайте сіль і чорний перець.
    10. Доведіть до кипіння та варіть на слабкому вогні 15 хвилин до м’якості овочів.
    11. Зніміть каструлю з вогню та обережно подрібніть суп занурювальним блендером.
    12. Розлийте суп у тарілки та подавайте теплим.',
        '1. Thoroughly wash the tomatoes and carrots.
    2. Peel the onion and carrots.
    3. Dice the onion and coarsely grate the carrots.
    4. Cut the tomatoes into small pieces.
    5. Heat sunflower oil in a heavy-bottomed saucepan.
    6. Fry the onion over medium heat for 3 minutes.
    7. Add the carrots and cook for another 4–5 minutes.
    8. Add the tomatoes and simmer for 5–7 minutes, stirring occasionally.
    9. Pour in 500 ml of water and add salt and black pepper.
    10. Bring to a boil and simmer over low heat for 15 minutes until the vegetables are tender.
    11. Remove the saucepan from the heat and carefully blend the soup using an immersion blender.
    12. Divide the soup between bowls and serve warm.',
        '/images/recipes/tomato-soup.webp'
    ),
    (
        'Омлет із сиром',
        'Cheese omelette',
        'Tender omelette with eggs, milk and melted cheese.',
        '1. Розбийте яйця в глибоку миску.
    2. Додайте молоко, сіль і чорний перець.
    3. Збийте суміш виделкою або вінчиком до однорідності.
    4. Натріть сир на великій тертці.
    5. Розігрійте пательню на середньо-слабкому вогні та розтопіть вершкове масло.
    6. Вилийте яєчну суміш на пательню та рівномірно розподіліть її по поверхні.
    7. Готуйте на слабкому вогні протягом 3–4 хвилин.
    8. Коли краї омлету почнуть застигати, рівномірно посипте його натертим сиром.
    9. Накрийте пательню кришкою та готуйте ще 2–3 хвилини, доки яйця повністю не застигнуть, а сир не розплавиться.
    10. Обережно перекладіть омлет на тарілку та подавайте гарячим.',
        '1. Crack the eggs into a large bowl.
    2. Add milk, salt and black pepper.
    3. Beat the mixture with a fork or whisk until smooth.
    4. Coarsely grate the cheese.
    5. Heat a frying pan over medium-low heat and melt the butter.
    6. Pour the egg mixture into the pan and spread it evenly.
    7. Cook over low heat for 3–4 minutes.
    8. When the edges begin to set, sprinkle the grated cheese evenly over the omelette.
    9. Cover the pan and cook for another 2–3 minutes until the eggs are fully set and the cheese has melted.
    10. Carefully transfer the omelette to a plate and serve hot.',
        '/images/recipes/cheese-omelette.webp'
    ),
    (
        'Салат із помідорами та огірками',
        'Tomato and cucumber salad',
        'Light and refreshing salad with seasonal vegetables.',
        '1. Ретельно помийте помідори та огірки під проточною водою.
    2. Обсушіть овочі паперовим рушником.
    3. Видаліть тверді частини помідорів та наріжте їх невеликими часточками.
    4. Зріжте кінчики огірків і наріжте огірки півкружальцями.
    5. Перекладіть нарізані овочі у глибоку салатницю.
    6. В окремій невеликій мисці змішайте оливкову олію, сіль і чорний перець.
    7. Полийте овочі підготовленою заправкою.
    8. Обережно перемішайте салат, щоб помідори зберегли форму.
    9. Розкладіть салат на дві порції та подавайте одразу після приготування.',
        '1. Thoroughly wash the tomatoes and cucumbers under running water.
    2. Pat the vegetables dry with a paper towel.
    3. Remove the tough tomato cores and cut the tomatoes into small wedges.
    4. Trim the cucumber ends and slice the cucumbers into half-moons.
    5. Transfer the chopped vegetables to a large salad bowl.
    6. In a separate small bowl, combine olive oil, salt and black pepper.
    7. Pour the prepared dressing over the vegetables.
    8. Toss gently to keep the tomato wedges intact.
    9. Divide the salad into two portions and serve immediately.',
        '/images/recipes/tomato-cucumber-salad.webp'
    ),
    (
        'Гречка з куркою',
        'Buckwheat with chicken',
        'Nutritious buckwheat with pan-fried chicken breast and onion.',
        '1. Переберіть гречку та ретельно промийте її під проточною водою.
    2. Перекладіть гречку в каструлю та залийте 400 мл води.
    3. Додайте половину солі та доведіть до кипіння.
    4. Зменште вогонь, накрийте каструлю кришкою та варіть 15–20 хвилин до повного поглинання води.
    5. Обсушіть куряче філе паперовим рушником і наріжте невеликими кубиками.
    6. Очистьте цибулю та дрібно наріжте.
    7. Розігрійте соняшникову олію на пательні на середньому вогні.
    8. Обсмажуйте цибулю 3–4 хвилини до м’якості.
    9. Додайте куряче філе, решту солі та чорний перець.
    10. Обсмажуйте курку 8–12 хвилин, періодично перемішуючи, до внутрішньої температури 74 °C.
    11. Зніміть готову гречку з вогню та залиште під кришкою на 5 хвилин.
    12. Розкладіть гречку по тарілках, додайте курку з цибулею та подавайте гарячою.',
        '1. Sort the buckwheat and rinse it thoroughly under running water.
    2. Transfer the buckwheat to a saucepan and add 400 ml of water.
    3. Add half the salt and bring to a boil.
    4. Reduce the heat, cover and cook for 15–20 minutes until all the water is absorbed.
    5. Pat the chicken breast dry and cut it into small cubes.
    6. Peel and finely chop the onion.
    7. Heat sunflower oil in a frying pan over medium heat.
    8. Fry the onion for 3–4 minutes until softened.
    9. Add the chicken breast, remaining salt and black pepper.
    10. Fry the chicken for 8–12 minutes, stirring occasionally, until the internal temperature reaches 74 °C.
    11. Remove the cooked buckwheat from the heat and leave covered for 5 minutes.
    12. Divide the buckwheat between plates, add the chicken and onion, and serve hot.',
        '/images/recipes/buckwheat-chicken.webp'
    )
    ) AS v(uk, en, de, i, ie, img)
    WHERE r."Name" = v.uk;
    """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM "UserRecipeCollections" WHERE "RecipeId" IN (SELECT "Id" FROM "Recipes" WHERE "Name" IN ('Сирники', 'Млинці', 'Вівсянка з бананом', 'Яєчня з помідорами', 'Оладки', 'Тости з авокадо', 'Український борщ', 'Курячий суп', 'Грибний крем-суп', 'Гарбузовий крем-суп', 'Сочевичний суп', 'Грецький салат', 'Цезар', 'Олів''є', 'Вінегрет', 'Салат із тунцем', 'Салат із капусти та моркви', 'Запечений лосось', 'Плов', 'Котлети', 'Картопля з грибами', 'Паста болоньєзе', 'Курка в соєвому соусі', 'Фарширований перець', 'Запечений хек', 'Піца Маргарита', 'Піца з шинкою та грибами', 'Часникові булочки', 'Сирні мафіни', 'Фокача', 'Чізкейк', 'Шоколадний торт', 'Яблучна шарлотка', 'Медовик', 'Брауні', 'Бананові мафіни', 'Брускети з помідорами', 'Фаршировані яйця', 'Хумус', 'Сирна закуска з часником', 'Рулетики з баклажанів', 'Банановий смузі', 'Полуничний молочний коктейль', 'Домашній лимонад', 'Ягідний смузі'))) THEN
        RAISE EXCEPTION 'Rollback blocked: new recipes are in user collections';
    END IF;
    IF EXISTS (SELECT 1 FROM "FridgeItems" WHERE "ProductId" IN (SELECT "Id" FROM "Products" WHERE "Name" IN ('Хліб', 'Багет', 'Какао', 'Шоколад', 'Мед', 'Печиво', 'Сухі дріжджі', 'Розпушувач', 'Харчова сода', 'Ванільний цукор', 'Цукрова пудра', 'Консервований горошок', 'Вершковий сир', 'Морозиво', 'Зелена цибуля', 'Кріп', 'Петрушка', 'Солоні огірки', 'Оливки', 'М''ята', 'Оцет', 'Тахіні', 'Кумин (зіра)', 'Мускатний горіх', 'Кунжут', 'Волоські горіхи', 'Консервована кукурудза', 'Консервований тунець'))) THEN
        RAISE EXCEPTION 'Rollback blocked: new products are used in FridgeItems';
    END IF;
    IF EXISTS (SELECT 1 FROM "ShoppingItems" WHERE "ProductId" IN (SELECT "Id" FROM "Products" WHERE "Name" IN ('Хліб', 'Багет', 'Какао', 'Шоколад', 'Мед', 'Печиво', 'Сухі дріжджі', 'Розпушувач', 'Харчова сода', 'Ванільний цукор', 'Цукрова пудра', 'Консервований горошок', 'Вершковий сир', 'Морозиво', 'Зелена цибуля', 'Кріп', 'Петрушка', 'Солоні огірки', 'Оливки', 'М''ята', 'Оцет', 'Тахіні', 'Кумин (зіра)', 'Мускатний горіх', 'Кунжут', 'Волоські горіхи', 'Консервована кукурудза', 'Консервований тунець'))) THEN
        RAISE EXCEPTION 'Rollback blocked: new products are used in ShoppingItems';
    END IF;
    IF EXISTS (SELECT 1 FROM "UserFavoriteProducts" WHERE "ProductId" IN (SELECT "Id" FROM "Products" WHERE "Name" IN ('Хліб', 'Багет', 'Какао', 'Шоколад', 'Мед', 'Печиво', 'Сухі дріжджі', 'Розпушувач', 'Харчова сода', 'Ванільний цукор', 'Цукрова пудра', 'Консервований горошок', 'Вершковий сир', 'Морозиво', 'Зелена цибуля', 'Кріп', 'Петрушка', 'Солоні огірки', 'Оливки', 'М''ята', 'Оцет', 'Тахіні', 'Кумин (зіра)', 'Мускатний горіх', 'Кунжут', 'Волоські горіхи', 'Консервована кукурудза', 'Консервований тунець'))) THEN
        RAISE EXCEPTION 'Rollback blocked: new products are used in UserFavoriteProducts';
    END IF;
END $$;
DELETE FROM "RecipeIngredients" WHERE "RecipeId" IN (SELECT "Id" FROM "Recipes" WHERE "Name" IN ('Сирники', 'Млинці', 'Вівсянка з бананом', 'Яєчня з помідорами', 'Оладки', 'Тости з авокадо', 'Український борщ', 'Курячий суп', 'Грибний крем-суп', 'Гарбузовий крем-суп', 'Сочевичний суп', 'Грецький салат', 'Цезар', 'Олів''є', 'Вінегрет', 'Салат із тунцем', 'Салат із капусти та моркви', 'Запечений лосось', 'Плов', 'Котлети', 'Картопля з грибами', 'Паста болоньєзе', 'Курка в соєвому соусі', 'Фарширований перець', 'Запечений хек', 'Піца Маргарита', 'Піца з шинкою та грибами', 'Часникові булочки', 'Сирні мафіни', 'Фокача', 'Чізкейк', 'Шоколадний торт', 'Яблучна шарлотка', 'Медовик', 'Брауні', 'Бананові мафіни', 'Брускети з помідорами', 'Фаршировані яйця', 'Хумус', 'Сирна закуска з часником', 'Рулетики з баклажанів', 'Банановий смузі', 'Полуничний молочний коктейль', 'Домашній лимонад', 'Ягідний смузі'));
DELETE FROM "Recipes" WHERE "Name" IN ('Сирники', 'Млинці', 'Вівсянка з бананом', 'Яєчня з помідорами', 'Оладки', 'Тости з авокадо', 'Український борщ', 'Курячий суп', 'Грибний крем-суп', 'Гарбузовий крем-суп', 'Сочевичний суп', 'Грецький салат', 'Цезар', 'Олів''є', 'Вінегрет', 'Салат із тунцем', 'Салат із капусти та моркви', 'Запечений лосось', 'Плов', 'Котлети', 'Картопля з грибами', 'Паста болоньєзе', 'Курка в соєвому соусі', 'Фарширований перець', 'Запечений хек', 'Піца Маргарита', 'Піца з шинкою та грибами', 'Часникові булочки', 'Сирні мафіни', 'Фокача', 'Чізкейк', 'Шоколадний торт', 'Яблучна шарлотка', 'Медовик', 'Брауні', 'Бананові мафіни', 'Брускети з помідорами', 'Фаршировані яйця', 'Хумус', 'Сирна закуска з часником', 'Рулетики з баклажанів', 'Банановий смузі', 'Полуничний молочний коктейль', 'Домашній лимонад', 'Ягідний смузі');
DELETE FROM "RecipeIngredients" ri USING "Recipes" r, "Products" p, "Units" u, (VALUES
('Паста з куркою', 'Молоко', 150, 'мл'),
('Паста з куркою', 'Соняшникова олія', 1, 'ст. л.'),
('Паста з куркою', 'Сіль', 1, 'ч. л.'),
('Паста з куркою', 'Чорний перець', 1, 'дрібка'),
('Паста з куркою', 'Вода', 2, 'л'),
('Томатний суп', 'Соняшникова олія', 1, 'ст. л.'),
('Томатний суп', 'Сіль', 0.5, 'ч. л.'),
('Томатний суп', 'Чорний перець', 1, 'дрібка'),
('Омлет із сиром', 'Масло', 10, 'г'),
('Омлет із сиром', 'Сіль', 1, 'дрібка'),
('Омлет із сиром', 'Чорний перець', 1, 'дрібка'),
('Салат із помідорами та огірками', 'Оливкова олія', 1, 'ст. л.'),
('Салат із помідорами та огірками', 'Сіль', 0.25, 'ч. л.'),
('Салат із помідорами та огірками', 'Чорний перець', 1, 'дрібка'),
('Гречка з куркою', 'Вода', 400, 'мл'),
('Гречка з куркою', 'Соняшникова олія', 1, 'ст. л.'),
('Гречка з куркою', 'Сіль', 0.5, 'ч. л.'),
('Гречка з куркою', 'Чорний перець', 1, 'дрібка')
) AS v(recipe, product, quantity, unit)
WHERE ri."RecipeId" = r."Id" AND ri."ProductId" = p."Id" AND ri."UnitId" = u."Id" AND ri."Quantity" = v.quantity AND r."Name" = v.recipe AND p."Name" = v.product AND u."ShortName" = v.unit;
UPDATE "Recipes" SET "NameEn" = NULL, "DescriptionEn" = NULL, "InstructionsEn" = NULL, "ImageUrl" = '', "Instructions" = '1. Закип''ятіть воду, посоліть та відваріть макарони відповідно до інструкції на упаковці.
2. Наріжте куряче філе невеликими шматочками.
3. Розігрійте пательню з невеликою кількістю олії та обсмажте курку до повної готовності.
4. Додайте молоко та натертий сир. Перемішуйте на невеликому вогні, поки сир не розплавиться.
5. Додайте готові макарони до соусу та добре перемішайте.
6. Прогрійте все разом ще хвилину та подавайте.' WHERE "Name" = 'Паста з куркою';
UPDATE "Recipes" SET "NameEn" = NULL, "DescriptionEn" = NULL, "InstructionsEn" = NULL, "ImageUrl" = '', "Instructions" = '1. Помийте помідори, моркву та цибулю.
2. Наріжте цибулю невеликими шматочками, а моркву натріть на тертці.
3. Обсмажте цибулю та моркву в каструлі з невеликою кількістю олії протягом кількох хвилин.
4. Додайте нарізані помідори та тушкуйте овочі приблизно 5–7 хвилин.
5. Влийте воду, доведіть до кипіння та варіть на невеликому вогні приблизно 15 хвилин.
6. За бажанням подрібніть суп блендером. Додайте сіль і спеції за смаком.' WHERE "Name" = 'Томатний суп';
UPDATE "Recipes" SET "NameEn" = NULL, "DescriptionEn" = NULL, "InstructionsEn" = NULL, "ImageUrl" = '', "Instructions" = '1. Розбийте яйця в глибоку миску.
2. Додайте молоко, сіль і перець. Збийте виделкою до однорідності.
3. Натріть сир на тертці.
4. Розігрійте пательню з невеликою кількістю масла або олії.
5. Вилийте яєчну суміш на пательню та готуйте на невеликому вогні.
6. Коли омлет почне застигати, додайте сир.
7. Накрийте кришкою та готуйте до повної готовності яєць.' WHERE "Name" = 'Омлет із сиром';
UPDATE "Recipes" SET "NameEn" = NULL, "DescriptionEn" = NULL, "InstructionsEn" = NULL, "ImageUrl" = '', "Instructions" = '1. Ретельно помийте помідори та огірки.
2. Наріжте помідори невеликими часточками.
3. Наріжте огірки кружальцями або півкружальцями.
4. Перекладіть овочі в салатницю та обережно перемішайте.
5. Додайте сіль, перець та заправку за смаком.
6. Перемішайте салат ще раз і подавайте.' WHERE "Name" = 'Салат із помідорами та огірками';
UPDATE "Recipes" SET "NameEn" = NULL, "DescriptionEn" = NULL, "InstructionsEn" = NULL, "ImageUrl" = '', "Instructions" = '1. Промийте гречку під проточною водою.
2. Перекладіть її в каструлю, залийте водою та варіть до готовності відповідно до інструкції на упаковці.
3. Наріжте куряче філе невеликими шматочками.
4. Очистьте та наріжте цибулю.
5. Розігрійте пательню з невеликою кількістю олії та обсмажте цибулю.
6. Додайте куряче філе, посоліть та готуйте до повної готовності.
7. Подавайте курку з готовою гречкою.' WHERE "Name" = 'Гречка з куркою';
DELETE FROM "ProductUnits" WHERE "ProductId" IN (SELECT "Id" FROM "Products" WHERE "Name" IN ('Хліб', 'Багет', 'Какао', 'Шоколад', 'Мед', 'Печиво', 'Сухі дріжджі', 'Розпушувач', 'Харчова сода', 'Ванільний цукор', 'Цукрова пудра', 'Консервований горошок', 'Вершковий сир', 'Морозиво', 'Зелена цибуля', 'Кріп', 'Петрушка', 'Солоні огірки', 'Оливки', 'М''ята', 'Оцет', 'Тахіні', 'Кумин (зіра)', 'Мускатний горіх', 'Кунжут', 'Волоські горіхи', 'Консервована кукурудза', 'Консервований тунець'));
DELETE FROM "Products" WHERE "Name" IN ('Хліб', 'Багет', 'Какао', 'Шоколад', 'Мед', 'Печиво', 'Сухі дріжджі', 'Розпушувач', 'Харчова сода', 'Ванільний цукор', 'Цукрова пудра', 'Консервований горошок', 'Вершковий сир', 'Морозиво', 'Зелена цибуля', 'Кріп', 'Петрушка', 'Солоні огірки', 'Оливки', 'М''ята', 'Оцет', 'Тахіні', 'Кумин (зіра)', 'Мускатний горіх', 'Кунжут', 'Волоські горіхи', 'Консервована кукурудза', 'Консервований тунець');
DELETE FROM "RecipeCategories" WHERE "Name" IN ('Випічка', 'Десерти', 'Закуски', 'Напої');
""");
        }
    }
}

