
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

    public DbSet<ProductUnit> ProductUnits => Set<ProductUnit>();
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
    public DbSet<UserRecipeCollection> UserRecipeCollections
    => Set<UserRecipeCollection>();
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
        // USER RECIPE COLLECTIONS
        // =====================================

        modelBuilder.Entity<UserRecipeCollection>()
            .HasKey(x => new { x.UserId, x.RecipeId });

        modelBuilder.Entity<UserRecipeCollection>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRecipeCollection>()
            .HasOne(x => x.Recipe)
            .WithMany()
            .HasForeignKey(x => x.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<ProductUnit>()
            .HasKey(x => new { x.ProductId, x.UnitId });

        modelBuilder.Entity<ProductUnit>()
            .HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductUnit>()
            .HasOne(x => x.Unit)
            .WithMany()
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);


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
    }
}
