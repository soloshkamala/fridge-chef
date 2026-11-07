
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
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<FridgeItem> FridgeItems => Set<FridgeItem>();
    public DbSet<ShoppingItem> ShoppingItems => Set<ShoppingItem>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<RecipeCategory> RecipeCategories => Set<RecipeCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
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

        // Унікальні поля
        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Diet>()
            .HasIndex(x => x.Name)
            .IsUnique();

        modelBuilder.Entity<Product>()
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

        // Один профіль на одного користувача
        modelBuilder.Entity<UserProfile>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        modelBuilder.Entity<NutritionProfile>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        
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
    }
}
