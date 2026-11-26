
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


        modelBuilder.Entity<Category>()
            .Property(x => x.NameEn)
            .HasMaxLength(50);

        modelBuilder.Entity<Product>()
            .Property(x => x.NameEn)
            .HasMaxLength(100);

        modelBuilder.Entity<Unit>()
            .Property(x => x.NameEn)
            .HasMaxLength(50);

        modelBuilder.Entity<Unit>()
            .Property(x => x.ShortNameEn)
            .HasMaxLength(20);

        modelBuilder.Entity<Product>()
            .Property(x => x.IsPantryStaple)
            .HasDefaultValue(false);

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


    }
}
