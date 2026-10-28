
using System.ComponentModel.DataAnnotations;

namespace FridgeChef.Api.Features.ShoppingItems;

public record CreateShoppingItemRequest(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name,

    [Range(0.01, 1000)]
    double Quantity,

    [Required]
    [StringLength(30, MinimumLength = 1)]
    string Unit,

    [Required]
    [StringLength(50, MinimumLength = 1)]
    string Category
);

public record UpdateShoppingItemRequest(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name,

    [Range(0.01, 1000)]
    double Quantity,

    [Required]
    [StringLength(30, MinimumLength = 1)]
    string Unit,

    [Required]
    [StringLength(50, MinimumLength = 1)]
    string Category,

    bool IsPurchased,

    bool IsInFridge
);

public record ShoppingItemResponse(
    int Id,
    string Name,
    double Quantity,
    string Unit,
    string Category,
    bool IsPurchased,
    bool IsInFridge
);
