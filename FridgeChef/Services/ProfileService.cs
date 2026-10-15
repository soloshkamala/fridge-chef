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
                ErrorMessage = "Не вдалося завантажити дані профілю та збережені рецепти.";
                IsLoading = false;
                return;
            }

            User = new UserProfile("Name Surname", "oddbody@gmail.com", "Користувач",
                null, new DateOnly(2025, 3, 10), 2, 1);

            if (!SimulateEmpty)
            {
                SavedRecipes.Add(new RecipeSummary("1", "Паста з куркою", "https://picsum.photos/300/200?1", 25, 440));
                SavedRecipes.Add(new RecipeSummary("2", "Авокадо тост", "https://picsum.photos/300/200?2", 15, 320));
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