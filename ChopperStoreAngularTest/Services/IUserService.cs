using ChopperStoreAngularTest.Models;
using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByGoogleIdAsync(string googleId);
    Task<User> CreateUserAsync(User user);
}

public class UserService : IUserService
{
    private readonly ChopperStoreContext _context;

    public UserService(ChopperStoreContext context)
    {
        _context = context;
    }

    // Obtener usuario por email
    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.users
            .Where(u => u.email == email)
            .Select(u => new User
            {
                Id = u.Id,
                GoogleId = u.GoogleId ?? "",
                name = u.name ?? "",
                lastname = u.lastname ?? "",
                email = u.email ?? "",
                phone = u.phone ?? "",
                username = u.username ?? "",
                password = u.password ?? "",
                isAdmin = u.isAdmin,
                isBlocked = u.isBlocked
            })
            .FirstOrDefaultAsync();
    }

    // Obtener usuario por Google ID
    public async Task<User> GetUserByGoogleIdAsync(string googleId)
    {
        return await _context.users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
    }

    // Crear usuario
    public async Task<User> CreateUserAsync(User user)
    {
        _context.users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}