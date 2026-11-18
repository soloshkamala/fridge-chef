
namespace FridgeChef.Api.Features.Products.Models;

public class ProductResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = "";
}
