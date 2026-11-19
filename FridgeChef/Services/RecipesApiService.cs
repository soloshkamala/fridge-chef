
using System.Net.Http.Json;
using FridgeChef.Models.Recipes;

namespace FridgeChef.Services
{
    public class RecipesApiService
    {
        private readonly HttpClient httpClient;

        private const string ApiUrl = "http://localhost:5039/api/recipes";

        public RecipesApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }


        public async Task<List<RecommendedRecipeResponse>> GetRecommendedRecipesAsync()
        {
            var response = await httpClient.GetFromJsonAsync<List<RecommendedRecipeResponse>>(
                $"{ApiUrl}/recommended");

            return response ?? new List<RecommendedRecipeResponse>();
        }

        public async Task<RecipeResponse?> GetRecipeByIdAsync(int id)
        {
            var response = await httpClient.GetAsync($"{ApiUrl}/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<RecipeResponse>();
        }
    }
}
