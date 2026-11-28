using FridgeChef.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace FridgeChef.Services
{
    public class ProfileService
    {
        private readonly ProfileApiService _profileApi;

        public ProfileService(ProfileApiService profileApi)
        {
            _profileApi = profileApi;
        }

        public bool IsLoading { get; private set; } = true;
        public string? ErrorMessage { get; private set; }

        public UserProfile? User { get; private set; }

        public int? ProfileId { get; private set; }

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

            try
            {
                var profile = await _profileApi.GetMyProfileAsync();

                if (profile is not null)
                {
                    ProfileId = profile.Id;

                    User = new UserProfile(
                        profile.Name,
                        profile.Email,
                        "Користувач",
                        null,
                        null,
                        0,
                        0
                    );
                }
                else
                {
                    User = null;
                    ProfileId = null;
                }
            }
            catch (Exception)
            {
                ErrorMessage =
                    "Не вдалося завантажити дані профілю.";

                User = null;
                ProfileId = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void SetSavedRecipes(IReadOnlyList<RecipeSummary> recipes)
        {
            SavedRecipes.Clear();
            SavedRecipes.AddRange(recipes);

            if (User is not null)
            {
                User = User with { SavedCount = recipes.Count };
            }
        }

        public void SetMicrosoftProfile(string name, string email, string role)
        {
            if (User is null)
            {
                User = new UserProfile(
                    name,
                    email,
                    role,
                    null,
                    null,
                    SavedRecipes.Count,
                    0
                );

                return;
            }

            User = User with
            {
                Name = name,
                Email = email,
                Badge = role
            };
        }

        public async Task SaveProfileAsync(ProfileForm form)
        {
            var request = new ProfileApiRequest(
                form.Name ?? "",
                form.Email ?? "",
                form.Diets,
                form.FavoriteIngredients ?? ""
            );

            ProfileApiDto profile;

            if (ProfileId is null)
            {
                profile = await _profileApi.CreateProfileAsync(request);
            }
            else
            {
                profile = await _profileApi.UpdateProfileAsync(
                    ProfileId.Value,
                    request
                );
            }

            ProfileId = profile.Id;

            User = new UserProfile(
                profile.Name,
                profile.Email,
                User?.Badge ?? "Користувач",
                User?.AvatarUrl,
                User?.JoinedAt,
                SavedRecipes.Count,
                User?.FavoriteCatalogs ?? 0
            );
        }

        public async Task SaveSettingsAsync(
            IJSRuntime js,
            ProfileForm form)
        {
            var json = JsonSerializer.Serialize(form);

            await js.InvokeVoidAsync(
                "localStorage.setItem",
                "fridgechef-profile-settings",
                json
            );
        }

        public async Task<ProfileForm?> LoadSettingsAsync(IJSRuntime js)
        {
            var json = await js.InvokeAsync<string?>(
                "localStorage.getItem",
                "fridgechef-profile-settings"
            );

            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<ProfileForm>(json);
        }
    }
}