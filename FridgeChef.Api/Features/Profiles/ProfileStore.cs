namespace FridgeChef.Api.Features.Profiles;

public class ProfileStore
{
    private readonly object _lock = new();
    private readonly List<ProfileDto> _profiles = new();
    private int _nextId = 1;

    public IReadOnlyList<ProfileDto> GetAll()
    {
        lock (_lock)
    {
        return _profiles.ToList();
    }

    }

    public ProfileDto? GetById(int id)
    {
         lock (_lock)
    {
        return _profiles.FirstOrDefault(p => p.Id == id);
    }

    }

    public ProfileDto Add(CreateProfileRequest request)
{
    lock (_lock)
    {
        var profile = new ProfileDto(
            _nextId++,
            request.Name,
            request.Email,
            request.Diets,
            request.FavoriteIngredients
        );

        _profiles.Add(profile);
        return profile;
    }
}


    public ProfileDto? TryAdd(CreateProfileRequest request)
{
    lock (_lock)
    {
        if (_profiles.Any(p =>
            string.Equals(
                p.Email,
                request.Email,
                StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        return Add(request);
    }
}



    public string TryUpdate(int id, UpdateProfileRequest request)
{
    lock (_lock)
    {
        var index = _profiles.FindIndex(p => p.Id == id);

        if (index == -1)
            return "NotFound";

        if (_profiles.Any(p =>
            p.Id != id &&
            string.Equals(
                p.Email,
                request.Email,
                StringComparison.OrdinalIgnoreCase)))
        {
            return "Conflict";
        }

        _profiles[index] = new ProfileDto(
            id,
            request.Name,
            request.Email,
            request.Diets,
            request.FavoriteIngredients
        );

        return "Updated";
    }
}

    public bool Delete(int id)
{
    lock (_lock)
    {
        return _profiles.RemoveAll(p => p.Id == id) > 0;
    }
}
}