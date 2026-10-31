
namespace FridgeChef.Api.Features.Users;

public class UserStore
{
    private readonly List<User> users = new();
    private readonly object sync = new();
    private int nextId = 1;

    public List<User> GetAll()
    {
        lock (sync)
            return users.Select(Copy).ToList();
    }

    public User? GetById(int id)
    {
        lock (sync)
        {
            var user = users.FirstOrDefault(x => x.Id == id);
            return user is null ? null : Copy(user);
        }
    }

    public User? GetByEmail(string email)
    {
        lock (sync)
        {
            var user = users.FirstOrDefault(x =>
                string.Equals(x.Email, email,
                    StringComparison.OrdinalIgnoreCase));

            return user is null ? null : Copy(user);
        }
    }

    public bool TryAdd(User user, out User? created)
    {
        lock (sync)
        {
            if (EmailExists(user.Email))
            {
                created = null;
                return false;
            }

            var saved = Copy(user);
            saved.Id = nextId++;

            users.Add(saved);
            created = Copy(saved);
            return true;
        }
    }

    public UserUpdateResult TryReplace(User user)
    {
        lock (sync)
        {
            var index = users.FindIndex(x => x.Id == user.Id);

            if (index == -1)
                return UserUpdateResult.NotFound;

            if (EmailExists(user.Email, user.Id))
                return UserUpdateResult.Conflict;

            users[index] = Copy(user);
            return UserUpdateResult.Success;
        }
    }

    public bool Delete(int id)
    {
        lock (sync)
        {
            return users.RemoveAll(x => x.Id == id) > 0;
        }
    }

    private bool EmailExists(string email, int? excludeId = null)
    {
        return users.Any(x =>
            x.Id != excludeId &&
            string.Equals(x.Email, email,
                StringComparison.OrdinalIgnoreCase));
    }

    private static User Copy(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        PasswordHash = user.PasswordHash,
        DisplayName = user.DisplayName
    };
}

public enum UserUpdateResult
{
    Success,
    NotFound,
    Conflict
}
