
using FridgeChef.Models;

namespace FridgeChef.Services
{
    public class ProfileService
    {
        public bool IsLoading { get; private set; } = true;
        public string? ErrorMessage { get; set; }

        public bool SimulateError { get; set; } = false;
        public bool SimulateEmpty { get; set; } = false;

        public UserProfile? User { get; private set; }

        public List<RecipeSummary> SavedRecipes { get; } = new();

        public ProfileViewState State =>
            IsLoading ? ProfileViewState.Loading :
            ErrorMessage != null ? ProfileViewState.Error :
            SavedRecipes.Count == 0 ? ProfileViewState.Empty :
            ProfileViewState.Data;

        public async Task LoadAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            SavedRecipes.Clear();

            
            await Task.Delay(1500);

            if (SimulateError)
            {
                ErrorMessage =
                    "Не вдалося завантажити дані профілю та збережені рецепти.";

                IsLoading = false;
                return;
            }

            User = new UserProfile(
                "Name Surname",
                "oddbody@gmail.com",
                "Користувач",
                null,
                new DateOnly(2025, 3, 10),
                4,
                1
            );

            if (!SimulateEmpty)
            {
                SavedRecipes.Add(new RecipeSummary(
                    "1",
                    "Паста з куркою",
                    "https://images.unsplash.com/photo-1473093295043-cdd812d0e601?w=600",
                    25,
                    440
                ));

                SavedRecipes.Add(new RecipeSummary(
                    "2",
                    "Авокадо тост",
                    "https://images.unsplash.com/photo-1525351484163-7529414344d8?w=600",
                    15,
                    320
                ));

                SavedRecipes.Add(new RecipeSummary(
                    "3",
                    "Томатний суп",
                    "https://images.unsplash.com/photo-1547592180-85f173990554?w=600",
                    30,
                    230
                ));

                SavedRecipes.Add(new RecipeSummary(
                    "4",
                    "Салат з тунцем",
                    "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=600",
                    20,
                    300
                ));
            }

            IsLoading = false;
        }

        public void SaveProfile(string name, string email)
        {
            if (User is null)
                return;

            User = User with
            {
                Name = name,
                Email = email
            };
        }
    }
}
