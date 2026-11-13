
using FridgeChef.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FridgeChef.Api.Data;

public class FridgeChefDbContext : DbContext
{
    public FridgeChefDbContext(
        DbContextOptions<FridgeChefDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Diet> Diets => Set<Diet>();
    public DbSet<UserDiet> UserDiets => Set<UserDiet>();
    public DbSet<UserFavoriteProduct> UserFavoriteProducts => Set<UserFavoriteProduct>();
    public DbSet<NutritionProfile> NutritionProfiles => Set<NutritionProfile>();

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Unit> Units => Set<Unit>();

    public DbSet<FridgeItem> FridgeItems => Set<FridgeItem>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<RecipeCategory> RecipeCategories => Set<RecipeCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================
        // УНІКАЛЬНІ ІНДЕКСИ
        // =====================================

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(x => new { x.EntraTenantId, x.EntraObjectId })
            .IsUnique();

        modelBuilder.Entity<Diet>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Unit>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Unit>()
            .HasIndex(x => x.ShortName)
            .IsUnique();

        modelBuilder.Entity<RecipeCategory>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<UserProfile>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        modelBuilder.Entity<NutritionProfile>()
            .HasIndex(x => x.UserId)
            .IsUnique();


        // =====================================
        // ОБМЕЖЕННЯ ПОЛІВ
        // =====================================

        modelBuilder.Entity<Category>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Unit>()
            .Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<Unit>()
            .Property(x => x.ShortName)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<ShoppingItem>()
            .Property(x => x.Quantity)
            .HasPrecision(10, 2);


        // =====================================
        // USER PROFILE
        // =====================================

        modelBuilder.Entity<UserProfile>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<NutritionProfile>()
            .HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<NutritionProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // =====================================
        // USER FAVORITE PRODUCTS
        // =====================================

        modelBuilder.Entity<UserFavoriteProduct>()
            .HasKey(x => new { x.UserId, x.ProductId });

        modelBuilder.Entity<UserFavoriteProduct>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserFavoriteProduct>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================
        // USER DIETS
        // =====================================

        modelBuilder.Entity<UserDiet>()
            .HasKey(x => new { x.UserId, x.DietId });

        modelBuilder.Entity<UserDiet>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserDiet>()
            .HasOne(x => x.Diet)
            .WithMany()
            .HasForeignKey(x => x.DietId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================
        // PRODUCTS AND CATEGORIES
        // =====================================

        modelBuilder.Entity<Product>()
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================
        // FRIDGE ITEMS
        // =====================================

        modelBuilder.Entity<FridgeItem>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FridgeItem>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FridgeItem>()
            .HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================
        // SHOPPING ITEMS
        // =====================================

        modelBuilder.Entity<ShoppingItem>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingItem>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShoppingItem>()
            .HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);


        // =====================================
        // RECIPES
        // =====================================

        modelBuilder.Entity<Recipe>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Recipes)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(x => x.Recipe)
            .WithMany(x => x.Ingredients)
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // =====================================
        // ПОЧАТКОВІ КАТЕГОРІЇ
        // =====================================

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 2, Name = "Молочні продукти" },
            new Category { Id = 3, Name = "М'ясо та ковбаси" },
            new Category { Id = 4, Name = "Овочі" },
            new Category { Id = 5, Name = "Фрукти" },
            new Category { Id = 6, Name = "Яйця" },
            new Category { Id = 7, Name = "Бакалія" },
            new Category { Id = 8, Name = "Напої" }
        );

        // =====================================
        // ПОЧАТКОВІ ПРОДУКТИ
        // =====================================

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Молоко", CategoryId = 2 },
            new Product { Id = 2, Name = "Сир", CategoryId = 2 },
            new Product { Id = 3, Name = "Масло", CategoryId = 2 },
            new Product { Id = 4, Name = "Сметана", CategoryId = 2 },
            new Product { Id = 5, Name = "Йогурт", CategoryId = 2 },
            new Product { Id = 6, Name = "Кефір", CategoryId = 2 },

            new Product { Id = 7, Name = "Ковбаса", CategoryId = 3 },
            new Product { Id = 8, Name = "Куряче філе", CategoryId = 3 },
            new Product { Id = 9, Name = "Яловичина", CategoryId = 3 },
            new Product { Id = 10, Name = "Фарш", CategoryId = 3 },

            new Product { Id = 11, Name = "Помідори", CategoryId = 4 },
            new Product { Id = 12, Name = "Огірки", CategoryId = 4 },
            new Product { Id = 13, Name = "Картопля", CategoryId = 4 },
            new Product { Id = 14, Name = "Морква", CategoryId = 4 },
            new Product { Id = 15, Name = "Цибуля", CategoryId = 4 },
            new Product { Id = 16, Name = "Перець", CategoryId = 4 },

            new Product { Id = 17, Name = "Яблука", CategoryId = 5 },
            new Product { Id = 18, Name = "Банани", CategoryId = 5 },
            new Product { Id = 19, Name = "Апельсини", CategoryId = 5 },
            new Product { Id = 20, Name = "Лимони", CategoryId = 5 },

            new Product { Id = 21, Name = "Яйця", CategoryId = 6 },

            new Product { Id = 22, Name = "Рис", CategoryId = 7 },
            new Product { Id = 23, Name = "Гречка", CategoryId = 7 },
            new Product { Id = 24, Name = "Макарони", CategoryId = 7 },
            new Product { Id = 25, Name = "Борошно", CategoryId = 7 },
            new Product { Id = 26, Name = "Цукор", CategoryId = 7 },

            new Product { Id = 27, Name = "Вода", CategoryId = 8 },
            new Product { Id = 28, Name = "Сік", CategoryId = 8 }
        );

        // =====================================
        // ПОЧАТКОВІ ОДИНИЦІ ВИМІРЮВАННЯ
        // =====================================

        modelBuilder.Entity<Unit>().HasData(
            new Unit { Id = 1, Name = "Штуки", ShortName = "шт" },
            new Unit { Id = 2, Name = "Мілілітри", ShortName = "мл" },
            new Unit { Id = 3, Name = "Літри", ShortName = "л" },
            new Unit { Id = 4, Name = "Грами", ShortName = "г" },
            new Unit { Id = 5, Name = "Кілограми", ShortName = "кг" }
        );

        // =====================================
        // ПОЧАТКОВІ КАТЕГОРІЇ РЕЦЕПТІВ
        // =====================================

        modelBuilder.Entity<RecipeCategory>().HasData(
            new RecipeCategory { Id = 1, Name = "Сніданки" },
            new RecipeCategory { Id = 2, Name = "Супи" },
            new RecipeCategory { Id = 3, Name = "Салати" },
            new RecipeCategory { Id = 4, Name = "Основні страви" }
        );

        // =====================================
        // ПОЧАТКОВІ РЕЦЕПТИ
        // =====================================

        modelBuilder.Entity<Recipe>().HasData(
            new Recipe
            {
                Id = 1,
                Name = "Паста з куркою",
                CategoryId = 4,
                Description = "Паста з курячим філе у ніжному сирному соусі.",
                ImageUrl = "",
                CookingTime = 35,
                Calories = 450,
                Servings = 2,
                Instructions = "1. Закип'ятіть воду, посоліть та відваріть макарони відповідно до інструкції на упаковці.\n" +
                               "2. Наріжте куряче філе невеликими шматочками.\n" +
                               "3. Розігрійте пательню з невеликою кількістю олії та обсмажте курку до повної готовності.\n" +
                               "4. Додайте молоко та натертий сир. Перемішуйте на невеликому вогні, поки сир не розплавиться.\n" +
                               "5. Додайте готові макарони до соусу та добре перемішайте.\n" +
                               "6. Прогрійте все разом ще хвилину та подавайте."
            },
            new Recipe
            {
                Id = 2,
                Name = "Томатний суп",
                CategoryId = 2,
                Description = "Ароматний овочевий суп із помідорами, морквою та цибулею.",
                ImageUrl = "",
                CookingTime = 40,
                Calories = 180,
                Servings = 2,
                Instructions = "1. Помийте помідори, моркву та цибулю.\n" +
                               "2. Наріжте цибулю невеликими шматочками, а моркву натріть на тертці.\n" +
                               "3. Обсмажте цибулю та моркву в каструлі з невеликою кількістю олії протягом кількох хвилин.\n" +
                               "4. Додайте нарізані помідори та тушкуйте овочі приблизно 5–7 хвилин.\n" +
                               "5. Влийте воду, доведіть до кипіння та варіть на невеликому вогні приблизно 15 хвилин.\n" +
                               "6. За бажанням подрібніть суп блендером. Додайте сіль і спеції за смаком."
            },
            new Recipe
            {
                Id = 3,
                Name = "Омлет із сиром",
                CategoryId = 1,
                Description = "Ніжний омлет із яйцями, молоком та розплавленим сиром.",
                ImageUrl = "",
                CookingTime = 15,
                Calories = 320,
                Servings = 1,
                Instructions = "1. Розбийте яйця в глибоку миску.\n" +
                               "2. Додайте молоко, сіль і перець. Збийте виделкою до однорідності.\n" +
                               "3. Натріть сир на тертці.\n" +
                               "4. Розігрійте пательню з невеликою кількістю масла або олії.\n" +
                               "5. Вилийте яєчну суміш на пательню та готуйте на невеликому вогні.\n" +
                               "6. Коли омлет почне застигати, додайте сир.\n" +
                               "7. Накрийте кришкою та готуйте до повної готовності яєць."
            },
            new Recipe
            {
                Id = 4,
                Name = "Салат із помідорами та огірками",
                CategoryId = 3,
                Description = "Легкий свіжий салат із сезонних овочів.",
                ImageUrl = "",
                CookingTime = 10,
                Calories = 120,
                Servings = 2,
                Instructions = "1. Ретельно помийте помідори та огірки.\n" +
                               "2. Наріжте помідори невеликими часточками.\n" +
                               "3. Наріжте огірки кружальцями або півкружальцями.\n" +
                               "4. Перекладіть овочі в салатницю та обережно перемішайте.\n" +
                               "5. Додайте сіль, перець та заправку за смаком.\n" +
                               "6. Перемішайте салат ще раз і подавайте."
            },
            new Recipe
            {
                Id = 5,
                Name = "Гречка з куркою",
                CategoryId = 4,
                Description = "Поживна гречка з обсмаженим курячим філе та цибулею.",
                ImageUrl = "",
                CookingTime = 40,
                Calories = 390,
                Servings = 2,
                Instructions = "1. Промийте гречку під проточною водою.\n" +
                               "2. Перекладіть її в каструлю, залийте водою та варіть до готовності відповідно до інструкції на упаковці.\n" +
                               "3. Наріжте куряче філе невеликими шматочками.\n" +
                               "4. Очистьте та наріжте цибулю.\n" +
                               "5. Розігрійте пательню з невеликою кількістю олії та обсмажте цибулю.\n" +
                               "6. Додайте куряче філе, посоліть та готуйте до повної готовності.\n" +
                               "7. Подавайте курку з готовою гречкою."
            }
        );

        // =====================================
        // ІНГРЕДІЄНТИ РЕЦЕПТІВ
        // =====================================

        modelBuilder.Entity<RecipeIngredient>().HasData(
    
            new RecipeIngredient { Id = 1, RecipeId = 1, ProductId = 24, UnitId = 4, Quantity = 200 },
            new RecipeIngredient { Id = 2, RecipeId = 1, ProductId = 8, UnitId = 4, Quantity = 250 },
            new RecipeIngredient { Id = 3, RecipeId = 1, ProductId = 2, UnitId = 4, Quantity = 50 },

  
            new RecipeIngredient { Id = 4, RecipeId = 2, ProductId = 11, UnitId = 4, Quantity = 400 },
            new RecipeIngredient { Id = 5, RecipeId = 2, ProductId = 14, UnitId = 4, Quantity = 100 },
            new RecipeIngredient { Id = 6, RecipeId = 2, ProductId = 15, UnitId = 4, Quantity = 50 },
            new RecipeIngredient { Id = 7, RecipeId = 2, ProductId = 27, UnitId = 2, Quantity = 500 },


            new RecipeIngredient { Id = 8, RecipeId = 3, ProductId = 21, UnitId = 1, Quantity = 3 },
            new RecipeIngredient { Id = 9, RecipeId = 3, ProductId = 2, UnitId = 4, Quantity = 50 },
            new RecipeIngredient { Id = 10, RecipeId = 3, ProductId = 1, UnitId = 2, Quantity = 50 },


            new RecipeIngredient { Id = 11, RecipeId = 4, ProductId = 11, UnitId = 4, Quantity = 200 },
            new RecipeIngredient { Id = 12, RecipeId = 4, ProductId = 12, UnitId = 4, Quantity = 200 },


            new RecipeIngredient { Id = 13, RecipeId = 5, ProductId = 23, UnitId = 4, Quantity = 200 },
            new RecipeIngredient { Id = 14, RecipeId = 5, ProductId = 8, UnitId = 4, Quantity = 250 },
            new RecipeIngredient { Id = 15, RecipeId = 5, ProductId = 15, UnitId = 4, Quantity = 50 }
        );
    }
}
